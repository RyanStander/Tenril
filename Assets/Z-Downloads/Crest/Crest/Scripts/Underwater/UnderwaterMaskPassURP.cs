// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#if CREST_URP

namespace Crest
{
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    internal class UnderwaterMaskPassURP : ScriptableRenderPass
    {
        const string k_ShaderPathOceanMask = "Hidden/Crest/Underwater/Ocean Mask URP";

        readonly PropertyWrapperMaterial _oceanMaskMaterial;

        static UnderwaterMaskPassURP s_instance;
        UnderwaterRenderer _underwaterRenderer;

        public UnderwaterMaskPassURP()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            _oceanMaskMaterial = new PropertyWrapperMaterial(k_ShaderPathOceanMask);
        }

        ~UnderwaterMaskPassURP()
        {
            CoreUtils.Destroy(_oceanMaskMaterial.material);
        }

        public static void Enable(UnderwaterRenderer underwaterRenderer)
        {
            if (s_instance == null)
            {
                s_instance = new UnderwaterMaskPassURP();
            }

            UnderwaterRenderer.Instance.OnEnableMask();

            s_instance._underwaterRenderer = underwaterRenderer;

            RenderPipelineManager.beginCameraRendering -= EnqueuePass;
            RenderPipelineManager.beginCameraRendering += EnqueuePass;
        }

        public static void Disable()
        {
            if (UnderwaterRenderer.Instance != null)
            {
                UnderwaterRenderer.Instance.OnDisableMask();
            }

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
            camera.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(s_instance);
        }

        // Called before Configure.
        public override void OnCameraSetup(CommandBuffer buffer, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            // Keywords and other things.
            _underwaterRenderer.SetUpVolume(_oceanMaskMaterial.material);
            _underwaterRenderer.SetUpMaskTextures(descriptor);
            if (_underwaterRenderer._mode != UnderwaterRenderer.Mode.FullScreen && _underwaterRenderer._volumeGeometry != null)
            {
                _underwaterRenderer.SetUpVolumeTextures(descriptor);
            }
        }

        // Called before Execute.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(UnderwaterRenderer.Instance._maskTarget, UnderwaterRenderer.Instance._depthTarget);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera;

            XRHelpers.Update(camera);
            XRHelpers.UpdatePassIndex(ref UnderwaterRenderer.s_xrPassIndex);

            CommandBuffer commandBuffer = CommandBufferPool.Get("Ocean Mask");


            // Populate water volume before mask so we can use the stencil.
            if (_underwaterRenderer._mode != UnderwaterRenderer.Mode.FullScreen && _underwaterRenderer._volumeGeometry != null)
            {
                _underwaterRenderer.PopulateVolume(commandBuffer, _underwaterRenderer._volumeFrontFaceTarget, _underwaterRenderer._volumeBackFaceTarget);
                // Copy only the stencil by copying everything and clearing depth.
                commandBuffer.CopyTexture(_underwaterRenderer._mode == UnderwaterRenderer.Mode.Portal ? _underwaterRenderer._volumeFrontFaceTarget : _underwaterRenderer._volumeBackFaceTarget, _underwaterRenderer._depthTarget);
                Helpers.Blit(commandBuffer, _underwaterRenderer._depthTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.ClearDepth);
            }

            _underwaterRenderer.SetUpMask(commandBuffer, _underwaterRenderer._maskTarget, _underwaterRenderer._depthTarget);
            UnderwaterRenderer.PopulateOceanMask(
                commandBuffer,
                camera,
                OceanRenderer.Instance.Tiles,
                _underwaterRenderer._cameraFrustumPlanes,
                _oceanMaskMaterial.material,
                _underwaterRenderer._farPlaneMultiplier,
                _underwaterRenderer.EnableShaderAPI,
                _underwaterRenderer._debug._disableOceanMask
            );

            UnderwaterRenderer.Instance.FixMaskArtefacts
            (
                commandBuffer,
                renderingData.cameraData.cameraTargetDescriptor,
                UnderwaterRenderer.Instance._maskTarget
            );

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }
    }
}

#endif // CREST_URP
