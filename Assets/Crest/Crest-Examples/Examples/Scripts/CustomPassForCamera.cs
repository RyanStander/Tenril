// Crest Ocean System

// Copyright 2022 Wave Harmonic Ltd

namespace Crest.Examples
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering;
#if CREST_HDRP
    using UnityEngine.Rendering.HighDefinition;
#endif
#if CREST_URP
    using UnityEngine.Rendering.Universal;
#endif

    [ExecuteAlways]
    public abstract class CustomPassForCameraBase : MonoBehaviour
    {
        // Use int to support other RPs. We could use a custom enum to map to each RP in the future.
        protected abstract int Event { get; }

        [SerializeField, RenderPipeline(Crest.RenderPipeline.HighDefinition), DecoratedField]
        float _priority;

        int _currentEvent;
        CommandBuffer _buffer;
        List<Camera> _cameras = new List<Camera>();

        void OnEnable()
        {
#if CREST_URP
            if (RenderPipelineHelper.IsUniversal)
            {
                _customPassURP = new CustomPassURP(this);
                RenderPipelineManager.beginCameraRendering -= EnqueuePass;
                RenderPipelineManager.beginCameraRendering += EnqueuePass;
                return;
            }
#endif

#if CREST_HDRP
            if (RenderPipelineHelper.IsHighDefinition)
            {
                if (!TryGetComponent(out _volume))
                {
                    _volume = gameObject.AddComponent<CustomPassVolume>();
                    _volume.injectionPoint = (CustomPassInjectionPoint)Event;
                    _volume.isGlobal = true;
                    _volume.priority = _priority;
                    _customPassHDRP = new CustomPassHDRP()
                    {
                        name = "CustomPassForCamera",
                        targetColorBuffer = CustomPass.TargetBuffer.None,
                        targetDepthBuffer = CustomPass.TargetBuffer.None,
                    };
                    _volume.customPasses.Add(_customPassHDRP);
                    _volume.hideFlags = HideFlags.DontSave;
                }
                else
                {
                    _customPassHDRP = (CustomPassHDRP)_volume.customPasses.Find(x => x is CustomPassHDRP);
                    _volume.enabled = true;
                }

                _customPassHDRP._manager = this;
            }
#endif

            if (_buffer == null)
            {
                _buffer = new CommandBuffer()
                {
                    name = "Execute Command Buffer",
                };
            }

            _currentEvent = Event;

            Camera.onPreRender -= OnBeforeRender;
            Camera.onPreRender += OnBeforeRender;
        }

        void OnDisable()
        {
#if CREST_URP
            if (RenderPipelineHelper.IsUniversal)
            {
                RenderPipelineManager.beginCameraRendering -= EnqueuePass;
                return;
            }
#endif

#if CREST_HDRP
            if (RenderPipelineHelper.IsHighDefinition)
            {
                _volume.enabled = false;
                return;
            }
#endif

            Camera.onPreRender -= OnBeforeRender;
            CleanCameras();
        }

        void CleanCameras()
        {
            foreach (var camera in _cameras)
            {
                // This can happen on recompile. Thankfully, command buffers will be removed for us.
                if (camera == null)
                {
                    continue;
                }

                foreach (CameraEvent @event in System.Enum.GetValues(typeof(CameraEvent)))
                {
                    camera.RemoveCommandBuffer(@event, _buffer);
                }

                Clear(_buffer, camera);
                Graphics.ExecuteCommandBuffer(_buffer);
            }

            _cameras.Clear();
        }

        void OnBeforeRender(Camera camera)
        {
            if (!_cameras.Contains(camera))
            {
                if (_currentEvent != Event)
                {
                    CleanCameras();
                    _currentEvent = Event;
                }

                _cameras.Add(camera);
                camera.AddCommandBuffer((CameraEvent)Event, _buffer);
            }

            // Buffer will be shared by multiple cameras and multiple Execute calls. Clear once for each camera.
            _buffer.Clear();

            // Only execute for main camera and editor only cameras.
            if (Camera.main != camera && camera.cameraType != CameraType.SceneView && !camera.name.StartsWith("Preview"))
            {
                Clear(_buffer, camera);
                return;
            }

            Execute(_buffer, camera, XRHelpers.GetRenderTextureDescriptor(camera));
        }

#if CREST_URP
        CustomPassURP _customPassURP;

        internal class CustomPassURP : ScriptableRenderPass
        {
            CustomPassForCameraBase _manager;
            bool _isClearPass;

            public CustomPassURP(CustomPassForCameraBase instance)
            {
                _manager = instance;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                // This will execute in edit mode always, but if an event callback is not set to "Editor And Runtime"
                // then the entire thing will no longer work in play mode. I believe this is because it will execute
                // lots of nothing which either triggers a bug or Unity ignores it as an optimisation.
                if (!Application.isPlaying)
                {
                    return;
                }

                var cameraData = renderingData.cameraData;
                var renderer = cameraData.renderer;

                var buffer = CommandBufferPool.Get("Custom Pass");

                if (_isClearPass)
                {
                    _manager.Clear(buffer, cameraData.camera);
                }
                else
                {
                    _manager.Execute(buffer, cameraData.camera, cameraData.cameraTargetDescriptor);
                }

                // We have to restore the render target as not every pass will do so.
                buffer.SetRenderTarget(renderer.cameraColorTarget, renderer.cameraDepthTarget);

                context.ExecuteCommandBuffer(buffer);
                CommandBufferPool.Release(buffer);
            }
        }

        void EnqueuePass(ScriptableRenderContext context, Camera camera)
        {
            _customPassURP.renderPassEvent = (RenderPassEvent)Event;

            // There is no need to clear the command buffer as we are using on from a pool.

            // Only execute for main camera and editor only cameras.
            if (Camera.main != camera && camera.cameraType != CameraType.SceneView && !camera.name.StartsWith("Preview"))
            {
                return;
            }

            // Enqueue the pass. This happens every frame.
            camera.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(_customPassURP);
        }
#endif // CREST_URP

#if CREST_HDRP
        CustomPassVolume _volume;
        CustomPassHDRP _customPassHDRP;

        internal class CustomPassHDRP : CustomPass
        {
            internal CustomPassForCameraBase _manager;
            bool _isClearPass;

            protected override void Execute(CustomPassContext context)
            {
                // This will execute in edit mode always, but if an event callback is not set to "Editor And Runtime"
                // then the entire thing will no longer work in play mode. I believe this is because it will execute
                // lots of nothing which either triggers a bug or Unity ignores it as an optimisation.
                if (!Application.isPlaying)
                {
                    return;
                }

                if (_manager == null)
                {
                    return;
                }

                var buffer = context.cmd;
                var camera = context.hdCamera.camera;

                if (Camera.main != camera && camera.cameraType != CameraType.SceneView && !camera.name.StartsWith("Preview"))
                {
                    return;
                }

                if (_isClearPass)
                {
                    _manager.Clear(buffer, camera);
                }
                else
                {
                    _manager.Execute(buffer, camera, new RenderTextureDescriptor(100, 100));
                }
            }
        }
#endif // CREST_HDRP

        protected abstract void Execute(CommandBuffer buffer, Camera camera, RenderTextureDescriptor descriptor);
        protected abstract void Clear(CommandBuffer buffer, Camera camera);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomPassForCameraBase), true), CanEditMultipleObjects]
    public class CustomPassForCameraBaseEditor : Editor
    {

    }
#endif

    public class CustomPassForCamera : CustomPassForCameraBase
    {
        // TODO: We need a separate event for deferred as it is very different from forward. But having an event per
        // camera complicates things so will skip for now. Will probably need a dictionary.
        [SerializeField]
        CameraEvent _event;

#if CREST_URP
        [SerializeField]
        RenderPassEvent _eventUniversal;
#else
        int _eventUniversal;
#endif

#if CREST_HDRP
        [SerializeField]
        CustomPassInjectionPoint _eventHighDefinition;
#else
        int _eventHighDefinition;
#endif

        [SerializeField]
        UnityEvent<CommandBuffer, Camera, RenderTextureDescriptor> _onExecute = new UnityEvent<CommandBuffer, Camera, RenderTextureDescriptor>();

        [SerializeField]
        UnityEvent<CommandBuffer, Camera> _onClear = new UnityEvent<CommandBuffer, Camera>();

        protected override int Event
        {
            get
            {
#if CREST_URP
                if (RenderPipelineHelper.IsUniversal)
                {
                    return (int)_eventUniversal;
                }
#endif

#if CREST_HDRP
                if (RenderPipelineHelper.IsHighDefinition)
                {
                    return (int)_eventHighDefinition;
                }
#endif

                return (int)_event;
            }
        }

        protected override void Execute(CommandBuffer buffer, Camera camera, RenderTextureDescriptor descriptor)
        {
            _onExecute.Invoke(buffer, camera, descriptor);
        }

        protected override void Clear(CommandBuffer buffer, Camera camera)
        {
            _onClear.Invoke(buffer, camera);
        }
    }
}
