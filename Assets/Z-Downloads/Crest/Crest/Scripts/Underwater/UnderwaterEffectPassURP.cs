// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#if CREST_URP

namespace Crest
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    internal class UnderwaterEffectPassURP : ScriptableRenderPass
    {
        const string SHADER_UNDERWATER_EFFECT = "Hidden/Crest/Underwater/Underwater Effect URP";
        static readonly int sp_TemporaryColor = Shader.PropertyToID("_TemporaryColor");
        static readonly int sp_CameraForward = Shader.PropertyToID("_CameraForward");

        readonly PropertyWrapperMaterial _underwaterEffectMaterial;
        RenderTargetIdentifier _colorTarget;
        RenderTargetIdentifier _depthTarget;
        RenderTargetIdentifier _temporaryColorTarget = new RenderTargetIdentifier(sp_TemporaryColor, 0, CubemapFace.Unknown, -1);
        bool _firstRender = true;
        Camera _camera;

        static UnderwaterEffectPassURP s_instance;
        static RenderObjectsWithoutFogPass s_ApplyFogToTransparentObjects;
        UnderwaterRenderer _underwaterRenderer;

        public UnderwaterEffectPassURP()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            _underwaterEffectMaterial = new PropertyWrapperMaterial(SHADER_UNDERWATER_EFFECT);
        }

        ~UnderwaterEffectPassURP()
        {
            CoreUtils.Destroy(_underwaterEffectMaterial.material);
        }

        public static void Enable(UnderwaterRenderer underwaterRenderer)
        {
            if (s_instance == null)
            {
                s_instance = new UnderwaterEffectPassURP();
                s_ApplyFogToTransparentObjects = new RenderObjectsWithoutFogPass();
            }

            s_instance._underwaterRenderer = underwaterRenderer;

            RenderPipelineManager.beginCameraRendering -= EnqueuePass;
            RenderPipelineManager.beginCameraRendering += EnqueuePass;
        }

        public static void Disable()
        {
            RenderPipelineManager.beginCameraRendering -= EnqueuePass;
        }

        static void EnqueuePass(ScriptableRenderContext context, Camera camera)
        {
            if (!s_instance._underwaterRenderer.IsActive)
            {
                return;
            }

            // Only support main camera, scene camera and preview camera.
            if (!ReferenceEquals(s_instance._underwaterRenderer._camera, camera))
            {
#if UNITY_EDITOR
                if (!s_instance._underwaterRenderer.IsActiveForEditorCamera(camera))
#endif
                {
                    return;
                }
            }

            if (!Helpers.MaskIncludesLayer(camera.cullingMask, OceanRenderer.Instance.Layer))
            {
                return;
            }

            // Enqueue the pass. This happens every frame.
            var renderer = camera.GetUniversalAdditionalCameraData().scriptableRenderer;
            renderer.EnqueuePass(s_instance);
            if (s_instance._underwaterRenderer.EnableShaderAPI)
            {
                renderer.EnqueuePass(s_ApplyFogToTransparentObjects);
            }
        }

        // Called before Configure.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _colorTarget = renderingData.cameraData.renderer.cameraColorTarget;
            _depthTarget = renderingData.cameraData.renderer.cameraDepthTarget;
            _camera = renderingData.cameraData.camera;
        }

        // Called before Execute.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraDescriptor)
        {
            // Calling ConfigureTarget is recommended by Unity, but that means it can only use it once? Also Blit breaks
            // XR SPI. Using SetRenderTarget and custom Blit instead.
            {
                var descriptor = cameraDescriptor;
                descriptor.msaaSamples = 1;
                cmd.GetTemporaryRT(sp_TemporaryColor, descriptor);
            }

            if (_underwaterRenderer.UseStencilBufferOnEffect)
            {
                var descriptor = cameraDescriptor;
                descriptor.colorFormat = RenderTextureFormat.Depth;
                descriptor.depthBufferBits = 24;
                descriptor.SetMSAASamples(_camera);
                descriptor.bindMS = descriptor.msaaSamples > 1;

                cmd.GetTemporaryRT(UnderwaterRenderer.ShaderIDs.s_CrestWaterVolumeStencil, descriptor);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera;

            // Ensure legacy underwater fog is disabled.
            if (_firstRender)
            {
                OceanRenderer.Instance.OceanMaterial.DisableKeyword("_OLD_UNDERWATER");
            }

#if UNITY_EDITOR
            if (!UnderwaterRenderer.IsFogEnabledForEditorCamera(camera))
            {
                return;
            }
#endif

            CommandBuffer commandBuffer = CommandBufferPool.Get("Underwater Effect");

            UnderwaterRenderer.UpdatePostProcessMaterial(
                _underwaterRenderer._mode,
                camera,
                _underwaterEffectMaterial,
                _underwaterRenderer._sphericalHarmonicsData,
                _underwaterRenderer._meniscus,
                _firstRender || _underwaterRenderer._copyOceanMaterialParamsEachFrame,
                _underwaterRenderer._debug._viewOceanMask,
                _underwaterRenderer._debug._viewStencil,
                _underwaterRenderer._filterOceanData,
                ref _underwaterRenderer._currentOceanMaterial,
                UnderwaterRenderer.Instance.EnableShaderAPI
            );

            // Required for XR SPI as forward vector in matrix is incorrect.
            _underwaterEffectMaterial.material.SetVector(sp_CameraForward, camera.transform.forward);

            // Create a separate stencil buffer context by copying the depth texture.
            if (_underwaterRenderer.UseStencilBufferOnEffect)
            {
                if (Helpers.IsMSAAEnabled(camera) || camera.cameraType == CameraType.SceneView)
                {
                    commandBuffer.SetRenderTarget(_underwaterRenderer._depthStencilTarget);
                    Helpers.Blit(commandBuffer, _underwaterRenderer._depthStencilTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.CopyDepth);
                }
                else
                {
                    // Copy depth then clear stencil. Things to note:
                    // - Does not work with MSAA. Source is null.
                    // - Does not work with scene camera due to possible Unity bug. Source is RenderTextureFormat.ARGB32 instead of RenderTextureFormat.Depth.
                    commandBuffer.CopyTexture(_depthTarget, _underwaterRenderer._depthStencilTarget);
                    commandBuffer.SetRenderTarget(_underwaterRenderer._depthStencilTarget);
                    Helpers.Blit(commandBuffer, _underwaterRenderer._depthStencilTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.ClearStencil);
                }
            }

            // Copy color buffer.
            if (Helpers.IsMSAAEnabled(camera))
            {
                Helpers.Blit(commandBuffer, _temporaryColorTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.CopyColor);
            }
            else
            {
                commandBuffer.CopyTexture(_colorTarget, _temporaryColorTarget);
            }

            commandBuffer.SetGlobalTexture(UnderwaterRenderer.ShaderIDs.s_CrestCameraColorTexture, _temporaryColorTarget);

            if (_underwaterRenderer.UseStencilBufferOnEffect)
            {
                commandBuffer.SetRenderTarget(_colorTarget, _underwaterRenderer._depthStencilTarget);
            }
            else
            {
                // Determined the following from testing. Likely a Unity bug. Also probably why we should use
                // ConfigureTarget as recommended by Unity... but can only configure one target per ScriptableRenderPass.
                if (Helpers.IsMSAAEnabled(camera))
                {
                    if (renderingData.cameraData.xrRendering)
                    {
                        // XR MSAA needed depth target set.
                        commandBuffer.SetRenderTarget(_colorTarget, _depthTarget);
                    }
                    else
                    {
                        // MSAA did not like depth target being set.
                        commandBuffer.SetRenderTarget(_colorTarget);
                    }
                }
                else
                {
#if UNITY_EDITOR
                    if (camera.cameraType == CameraType.SceneView)
                    {
                        // If executing before transparents, scene view needed this. Works for other events too.
                        commandBuffer.SetRenderTarget(_colorTarget);
                    }
                    else
#endif
                    {
                        // No MSAA needed depth target set. Setting depth is necessary for depth to be bound as a target
                        // which is needed for volumes.
                        commandBuffer.SetRenderTarget(_colorTarget,  _depthTarget);
                    }
                }
            }

            _underwaterRenderer.ExecuteEffect(commandBuffer, _underwaterEffectMaterial.material);

            context.ExecuteCommandBuffer(commandBuffer);

            commandBuffer.ReleaseTemporaryRT(sp_TemporaryColor);

            if (_underwaterRenderer.UseStencilBufferOnEffect)
            {
                commandBuffer.ReleaseTemporaryRT(UnderwaterRenderer.ShaderIDs.s_CrestWaterVolumeStencil);
            }

            CommandBufferPool.Release(commandBuffer);

            _firstRender = false;
        }

        class RenderObjectsWithoutFogPass : ScriptableRenderPass
        {
            FilteringSettings m_FilteringSettings;

            static readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
            };

            public RenderObjectsWithoutFogPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                m_FilteringSettings = new FilteringSettings(RenderQueueRange.transparent, 0);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var drawingSettings = CreateDrawingSettings
                (
                    m_ShaderTagIdList,
                    ref renderingData,
                    SortingCriteria.CommonTransparent
                );

                m_FilteringSettings.layerMask = s_instance._underwaterRenderer._transparentObjectLayers;

                var buffer = CommandBufferPool.Get();

                // Disable Unity's fog keywords as there is no option to ignore fog for the Shader Graph.
                if (RenderSettings.fog)
                {
                    switch (RenderSettings.fogMode)
                    {
                        case FogMode.Exponential:
                            buffer.DisableShaderKeyword("FOG_EXP");
                            break;
                        case FogMode.Linear:
                            buffer.DisableShaderKeyword("FOG_LINEAR");
                            break;
                        case FogMode.ExponentialSquared:
                            buffer.DisableShaderKeyword("FOG_EXP2");
                            break;
                    }
                }

                buffer.EnableShaderKeyword("CREST_UNDERWATER_OBJECTS_PASS");
                // If we want anything to apply to DrawRenderers, it has to be executed before:
                // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.DrawRenderers.html
                context.ExecuteCommandBuffer(buffer);
                buffer.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);

                // Revert fog keywords.
                if (RenderSettings.fog)
                {
                    switch (RenderSettings.fogMode)
                    {
                        case FogMode.Exponential:
                            buffer.EnableShaderKeyword("FOG_EXP");
                            break;
                        case FogMode.Linear:
                            buffer.EnableShaderKeyword("FOG_LINEAR");
                            break;
                        case FogMode.ExponentialSquared:
                            buffer.EnableShaderKeyword("FOG_EXP2");
                            break;
                    }
                }

                buffer.DisableShaderKeyword("CREST_UNDERWATER_OBJECTS_PASS");
                context.ExecuteCommandBuffer(buffer);

                CommandBufferPool.Release(buffer);
            }
        }
    }
}

#endif // CREST_URP
