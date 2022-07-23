// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#if CREST_HDRP

namespace Crest
{
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;

    public class UnderwaterEffectPassHDRP : CustomPass
    {
        const string k_Name = "Underwater Effect";
        const string k_ShaderPath = "Hidden/Crest/Underwater/Underwater Effect HDRP";

        PropertyWrapperMaterial _underwaterEffectMaterial;
        RTHandle _colorTexture;
        RTHandle _depthTexture;
        RenderTargetIdentifier _depthTarget;
        bool _firstRender = true;
        UnderwaterRenderer.UnderwaterSphericalHarmonicsData _sphericalHarmonicsData = new UnderwaterRenderer.UnderwaterSphericalHarmonicsData();

        Material _depthValuesMaterial;
        MaterialPropertyBlock _depthValuesMaterialPropertyBlock;

        static GameObject s_GameObject;
        static UnderwaterRenderer s_UnderwaterRenderer;

        static ShaderTagId[] s_ForwardShaderTags;

        public static void Enable(UnderwaterRenderer underwaterRenderer)
        {
            CustomPassHelpers.CreateOrUpdate<UnderwaterEffectPassHDRP>(ref s_GameObject, k_Name, CustomPassInjectionPoint.BeforePostProcess);
            s_UnderwaterRenderer = underwaterRenderer;
        }

        public static void Disable()
        {
            // It should be safe to rely on this reference for this reference to fail.
            if (s_GameObject != null)
            {
                s_GameObject.SetActive(false);
            }
        }

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            if (s_ForwardShaderTags == null)
            {
                // Taken from:
                // https://github.com/Unity-Technologies/Graphics/blob/778ddac6207ade1689999b95380cd835b0669f2d/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/DrawRenderersCustomPass.cs#L136-L142
                s_ForwardShaderTags = new ShaderTagId[]
                {
                    HDShaderPassNames.s_ForwardName,            // HD Lit shader
                    HDShaderPassNames.s_ForwardOnlyName,        // HD Unlit shader
                    HDShaderPassNames.s_SRPDefaultUnlitName,    // Cross SRP Unlit shader
                };
            }

            if (_depthValuesMaterialPropertyBlock == null)
            {
                _depthValuesMaterialPropertyBlock = new MaterialPropertyBlock();
            }

            if (_underwaterEffectMaterial?.material == null)
            {
                _underwaterEffectMaterial = new PropertyWrapperMaterial(k_ShaderPath);
            }

            // TODO: Use a temporary RT if possible.
            _colorTexture = RTHandles.Alloc
            (
                Vector2.one,
                TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.B10G11R11_UFloatPack32,
                useDynamicScale: true,
                wrapMode: TextureWrapMode.Clamp,
                name: "Crest Camera Color Texture"
            );

            SetUpVolumes();
        }

        protected override void Cleanup()
        {
            if (_underwaterEffectMaterial?.material != null)
            {
                CoreUtils.Destroy(_underwaterEffectMaterial.material);
            }

            _colorTexture?.Release();

            CleanUpVolumes();
        }

        void SetUpVolumes()
        {
            if (s_UnderwaterRenderer._mode != UnderwaterRenderer.Mode.VolumeFlyThrough)
            {
                CleanUpVolumes();
                return;
            }

            if (_depthTexture == null)
            {
                _depthTexture = RTHandles.Alloc
                (
                    Vector2.one,
                    TextureXR.slices,
                    dimension: TextureXR.dimension,
                    depthBufferBits: DepthBits.Depth32,
                    colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm, // This appears to be used for depth.
                    useDynamicScale: true,
                    wrapMode: TextureWrapMode.Clamp,
                    name: "Crest Camera Depth Texture"
                );

                _depthTarget = new RenderTargetIdentifier(_depthTexture, 0, CubemapFace.Unknown, -1);
            }

            if (_depthValuesMaterial == null)
            {
                _depthValuesMaterial = CoreUtils.CreateEngineMaterial("Hidden/HDRP/DepthValues");
            }
        }

        void CleanUpVolumes()
        {
            _depthTexture?.Release();
            _depthTexture = null;

            if (_depthValuesMaterial != null)
            {
                CoreUtils.Destroy(_depthValuesMaterial);
            }
        }

        protected override void Execute(CustomPassContext context)
        {
            if (!s_UnderwaterRenderer.IsActive)
            {
                return;
            }

            var camera = context.hdCamera.camera;

            // Only support main camera, scene camera and preview camera.
            if (!ReferenceEquals(s_UnderwaterRenderer._camera, camera))
            {
#if UNITY_EDITOR
                if (!s_UnderwaterRenderer.IsActiveForEditorCamera(camera))
#endif
                {
                    return;
                }
            }

            if (!Helpers.MaskIncludesLayer(camera.cullingMask, OceanRenderer.Instance.Layer))
            {
                return;
            }

#if UNITY_EDITOR
            if (!UnderwaterRenderer.IsFogEnabledForEditorCamera(camera))
            {
                return;
            }
#endif

            XRHelpers.Update(camera);
            XRHelpers.UpdatePassIndex(ref UnderwaterRenderer.s_xrPassIndex);

            SetUpVolumes();

            UnderwaterRenderer.UpdatePostProcessMaterial(
                s_UnderwaterRenderer._mode,
                camera,
                _underwaterEffectMaterial,
                _sphericalHarmonicsData,
                s_UnderwaterRenderer._meniscus,
                _firstRender || s_UnderwaterRenderer._copyOceanMaterialParamsEachFrame,
                s_UnderwaterRenderer._debug._viewOceanMask,
                s_UnderwaterRenderer._debug._viewStencil,
                s_UnderwaterRenderer._filterOceanData,
                ref s_UnderwaterRenderer._currentOceanMaterial,
                s_UnderwaterRenderer.EnableShaderAPI
            );

            var isMSAA = Helpers.IsMSAAEnabled(camera);

            // Create a separate stencil buffer context by copying the depth texture.
            if (s_UnderwaterRenderer.UseStencilBufferOnEffect)
            {
                if (isMSAA)
                {
                    CoreUtils.SetRenderTarget(context.cmd, _depthTexture);
                    var rtScale = context.cameraDepthBuffer.rtHandleProperties.rtHandleScale / DynamicResolutionHandler.instance.GetCurrentScale();
                    _depthValuesMaterialPropertyBlock.SetTexture("_DepthTextureMS", context.cameraDepthBuffer);
                    _depthValuesMaterialPropertyBlock.SetVector("_BlitScaleBias", rtScale);
                    // Copy depth then clear stencil.
                    CoreUtils.DrawFullScreen(context.cmd, _depthValuesMaterial, _depthValuesMaterialPropertyBlock, shaderPassId: 0);
                    Helpers.Blit(context.cmd, _depthTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.ClearStencil);
                }
                else
                {
                    // Copy depth then clear stencil.
                    context.cmd.CopyTexture(context.cameraDepthBuffer, _depthTarget);
                    CoreUtils.SetRenderTarget(context.cmd, _depthTexture);
                    Helpers.Blit(context.cmd, _depthTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.ClearStencil, context.propertyBlock);
                }
            }

            // Copy color buffer.
            HDUtils.BlitCameraTexture(context.cmd, context.cameraColorBuffer, _colorTexture);
            context.propertyBlock.SetTexture(UnderwaterRenderer.ShaderIDs.s_CrestCameraColorTexture, _colorTexture);

            if (s_UnderwaterRenderer.UseStencilBufferOnEffect)
            {
                CoreUtils.SetRenderTarget(context.cmd, context.cameraColorBuffer, _depthTexture, ClearFlag.None);
            }
            else
            {
                if (isMSAA)
                {
                    // Not setting the depth buffer for MSAA is less broken than setting it.
                    CoreUtils.SetRenderTarget(context.cmd, context.cameraColorBuffer, ClearFlag.None);
                }
                else
                {
                    CoreUtils.SetRenderTarget(context.cmd, context.cameraColorBuffer, context.cameraDepthBuffer, ClearFlag.None);
                }
            }

            s_UnderwaterRenderer.ExecuteEffect(context.cmd, _underwaterEffectMaterial.material, context.propertyBlock);

            // Render transparent objects (using layer mask) a second time after the underwater effect.
            // Taken and modified from:
            // https://github.com/Unity-Technologies/Graphics/blob/778ddac6207ade1689999b95380cd835b0669f2d/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/DrawRenderersCustomPass.cs#L200-L212
            if (s_UnderwaterRenderer.EnableShaderAPI)
            {
                var renderConfig = context.hdCamera.frameSettings.IsEnabled(FrameSettingsField.Shadowmask) ? HDUtils.GetBakedLightingWithShadowMaskRenderConfig() : HDUtils.GetBakedLightingRenderConfig();
#pragma warning disable 0618
                var result = new RendererListDesc(s_ForwardShaderTags, context.cullingResults, context.hdCamera.camera)
#pragma warning restore 0618
                {
                    rendererConfiguration = renderConfig,
                    renderQueueRange = GetRenderQueueRange(RenderQueueType.AllTransparent),
                    sortingCriteria = SortingCriteria.CommonTransparent,
                    excludeObjectMotionVectors = false,
                    layerMask = s_UnderwaterRenderer._transparentObjectLayers,
                };

                context.cmd.EnableShaderKeyword("CREST_UNDERWATER_OBJECTS_PASS");
#pragma warning disable 0618
                CoreUtils.DrawRendererList(context.renderContext, context.cmd, RendererList.Create(result));
#pragma warning restore 0618
                context.cmd.DisableShaderKeyword("CREST_UNDERWATER_OBJECTS_PASS");
            }

            _firstRender = false;
        }
    }
}

#endif // CREST_HDRP
