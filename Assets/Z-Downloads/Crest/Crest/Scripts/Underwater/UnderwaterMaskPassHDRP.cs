// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#if CREST_HDRP

namespace Crest
{
    using UnityEngine;
    using UnityEngine.Rendering.HighDefinition;
    using UnityEngine.Rendering;
    using UnityEngine.Experimental.Rendering;

    internal class UnderwaterMaskPassHDRP : CustomPass
    {
        const string k_Name = "Underwater Mask";
        const string k_ShaderPath = "Hidden/Crest/Underwater/Ocean Mask HDRP";
        internal const string k_ShaderPathWaterVolumeGeometry = "Hidden/Crest/Water Volume Geometry HDRP";

        Material _oceanMaskMaterial;
        RTHandle _maskTexture;
        RTHandle _depthTexture;
        RTHandle _volumeFrontFaceRT;
        RTHandle _volumeBackFaceRT;
        RenderTargetIdentifier _maskTarget;
        RenderTargetIdentifier _depthTarget;
        internal RenderTargetIdentifier _volumeFrontFaceTarget;
        internal RenderTargetIdentifier _volumeBackFaceTarget;
        Plane[] _cameraFrustumPlanes;

        static GameObject s_GameObject;
        static UnderwaterRenderer s_UnderwaterRenderer;

        public static void Enable(UnderwaterRenderer underwaterRenderer)
        {
            CustomPassHelpers.CreateOrUpdate<UnderwaterMaskPassHDRP>(ref s_GameObject, k_Name, CustomPassInjectionPoint.BeforeRendering);
            s_UnderwaterRenderer = underwaterRenderer;
        }

        public static void Disable()
        {
            // It should be safe to rely on this reference for this reference to fail.
            if (s_GameObject != null)
            {
                s_GameObject.SetActive(false);
            }

            UnderwaterRenderer.DisableOceanMaskKeywords();
        }

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            _oceanMaskMaterial = CoreUtils.CreateEngineMaterial(k_ShaderPath);

            _maskTexture = RTHandles.Alloc
            (
                scaleFactor: Vector2.one,
                slices: TextureXR.slices,
                dimension: TextureXR.dimension,
                colorFormat: GraphicsFormat.R16_SFloat,
                enableRandomWrite: true,
                useDynamicScale: true,
                name: "Crest Ocean Mask"
            );

            _maskTarget = new RenderTargetIdentifier
            (
                _maskTexture,
                mipLevel: 0,
                CubemapFace.Unknown,
                depthSlice: -1 // Bind all XR slices.
            );

            _depthTexture = RTHandles.Alloc
            (
                scaleFactor: Vector2.one,
                slices: TextureXR.slices,
                dimension: TextureXR.dimension,
                depthBufferBits: DepthBits.Depth24,
                colorFormat: GraphicsFormat.R8_UNorm, // This appears to be used for depth.
                enableRandomWrite: false,
                useDynamicScale: true,
                name: "Crest Ocean Mask Depth"
            );

            _depthTarget = new RenderTargetIdentifier
            (
                _depthTexture,
                mipLevel: 0,
                CubemapFace.Unknown,
                depthSlice: -1 // Bind all XR slices.
            );

            if (s_UnderwaterRenderer != null)
            {
                s_UnderwaterRenderer._volumeMaterial = CoreUtils.CreateEngineMaterial(k_ShaderPathWaterVolumeGeometry);
                SetUpVolumeTextures();
                s_UnderwaterRenderer.SetUpFixMaskArtefactsShader();
            }
        }
        protected override void Cleanup()
        {
            CoreUtils.Destroy(_oceanMaskMaterial);
            _maskTexture.Release();
            _depthTexture.Release();

            if (s_UnderwaterRenderer == null)
            {
                return;
            }

            CleanUpVolumeTextures();

            if (s_UnderwaterRenderer._volumeMaterial != null)
            {
                CoreUtils.Destroy(s_UnderwaterRenderer._volumeMaterial);
            }
        }

        void SetUpVolumeTextures()
        {
            if (s_UnderwaterRenderer._mode == UnderwaterRenderer.Mode.FullScreen)
            {
                CleanUpVolumeTextures();
                return;
            }

            if (_volumeFrontFaceRT == null)
            {
                _volumeFrontFaceRT = RTHandles.Alloc
                (
                    scaleFactor: Vector2.one,
                    slices: TextureXR.slices,
                    dimension: TextureXR.dimension,
                    depthBufferBits: DepthBits.Depth24,
                    colorFormat: GraphicsFormat.R8_UNorm, // This appears to be used for depth.
                    enableRandomWrite: false,
                    useDynamicScale: true,
                    name: "_CrestVolumeFrontFaceTexture"
                );

                _volumeFrontFaceTarget = new RenderTargetIdentifier
                (
                    _volumeFrontFaceRT,
                    mipLevel: 0,
                    CubemapFace.Unknown,
                    depthSlice: -1 // Bind all XR slices.
                );
            }

            if (s_UnderwaterRenderer._mode == UnderwaterRenderer.Mode.Volume || s_UnderwaterRenderer._mode == UnderwaterRenderer.Mode.VolumeFlyThrough)
            {
                if (_volumeBackFaceRT == null)
                {
                    _volumeBackFaceRT = RTHandles.Alloc
                    (
                        scaleFactor: Vector2.one,
                        slices: TextureXR.slices,
                        dimension: TextureXR.dimension,
                        depthBufferBits: DepthBits.Depth24,
                        colorFormat: GraphicsFormat.R8_UNorm, // This appears to be used for depth.
                        enableRandomWrite: false,
                        useDynamicScale: true,
                        name: "_CrestVolumeBackFaceTexture"
                    );

                    _volumeBackFaceTarget = new RenderTargetIdentifier
                    (
                        _volumeBackFaceRT,
                        mipLevel: 0,
                        CubemapFace.Unknown,
                        depthSlice: -1 // Bind all XR slices.
                    );
                }
            }
            else
            {
                _volumeBackFaceRT?.Release();
                _volumeBackFaceRT = null;
            }
        }

        void CleanUpVolumeTextures()
        {
            _volumeFrontFaceRT?.Release();
            _volumeFrontFaceRT = null;
            _volumeBackFaceRT?.Release();
            _volumeBackFaceRT = null;
        }

        protected override void Execute(CustomPassContext context)
        {
            // Null check can be removed once post-processing is removed.
            if (s_UnderwaterRenderer != null && !s_UnderwaterRenderer.IsActive)
            {
                UnderwaterRenderer.DisableOceanMaskKeywords();
                return;
            }

            var camera = context.hdCamera.camera;
            var commandBuffer = context.cmd;

            // HDRP PP compatibility.
            if (s_UnderwaterRenderer == null)
            {
                if (!ReferenceEquals(camera, OceanRenderer.Instance.ViewCamera) || camera.cameraType != CameraType.Game)
                {
                    return;
                }
            }
            // Only support main camera, scene camera and preview camera.
            else if (!ReferenceEquals(s_UnderwaterRenderer._camera, camera))
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

            if (_cameraFrustumPlanes == null)
            {
                _cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            }

            // This property is either on the UnderwaterRenderer or UnderwaterPostProcessHDRP.
            var debugDisableOceanMask = false;
            if (s_UnderwaterRenderer != null)
            {
                debugDisableOceanMask = s_UnderwaterRenderer._debug._disableOceanMask;
            }
            else if (UnderwaterPostProcessHDRP.Instance != null)
            {
                debugDisableOceanMask = UnderwaterPostProcessHDRP.Instance._disableOceanMask.value;
            }

            var farPlaneMultiplier = 1.0f;
            if (s_UnderwaterRenderer != null)
            {
                farPlaneMultiplier = s_UnderwaterRenderer._farPlaneMultiplier;
            }
            else if (UnderwaterPostProcessHDRP.Instance != null)
            {
                farPlaneMultiplier = UnderwaterPostProcessHDRP.Instance._farPlaneMultiplier.value;
            }

            if (s_UnderwaterRenderer != null)
            {
                s_UnderwaterRenderer.SetUpVolume(_oceanMaskMaterial);

                // Populate water volume before mask so we can use the stencil.
                if (s_UnderwaterRenderer._mode != UnderwaterRenderer.Mode.FullScreen && s_UnderwaterRenderer._volumeGeometry != null)
                {
                    SetUpVolumeTextures();
                    s_UnderwaterRenderer.PopulateVolume(commandBuffer, _volumeFrontFaceTarget, _volumeBackFaceTarget, null, _volumeFrontFaceRT.rtHandleProperties.currentViewportSize);
                    // Copy only the stencil by copying everything and clearing depth.
                    commandBuffer.CopyTexture(s_UnderwaterRenderer._mode == UnderwaterRenderer.Mode.Portal ? _volumeFrontFaceTarget : _volumeBackFaceTarget, _depthTarget);
                    Helpers.Blit(commandBuffer, _depthTarget, Helpers.UtilityMaterial, (int)Helpers.UtilityPass.ClearDepth);
                }

                s_UnderwaterRenderer.SetUpMask(commandBuffer, _maskTarget, _depthTarget);
                // For dynamic scaling to work.
                CoreUtils.SetViewport(commandBuffer, _maskTexture);
            }
            else
            {
                CoreUtils.SetRenderTarget(commandBuffer, _maskTexture, _depthTexture);
                CoreUtils.ClearRenderTarget(commandBuffer, ClearFlag.All, Color.black);
                commandBuffer.SetGlobalTexture(UnderwaterRenderer.ShaderIDs.s_CrestOceanMaskTexture, _maskTexture);
                commandBuffer.SetGlobalTexture(UnderwaterRenderer.ShaderIDs.s_CrestOceanMaskDepthTexture, _depthTexture);
            }

            UnderwaterRenderer.PopulateOceanMask(
                commandBuffer,
                camera,
                OceanRenderer.Instance.Tiles,
                _cameraFrustumPlanes,
                _oceanMaskMaterial,
                farPlaneMultiplier,
                s_UnderwaterRenderer != null ? s_UnderwaterRenderer.EnableShaderAPI : false,
                debugDisableOceanMask
            );

            if (s_UnderwaterRenderer != null)
            {
                var size = _maskTexture.GetScaledSize(_maskTexture.rtHandleProperties.currentViewportSize);
                var descriptor = new RenderTextureDescriptor(size.x, size.y);
                s_UnderwaterRenderer.FixMaskArtefacts(commandBuffer, descriptor, _maskTarget);
            }
        }
    }
}

#endif // CREST_HDRP
