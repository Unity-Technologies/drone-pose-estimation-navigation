using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Rendering;

#if URP_ENABLED
using UnityEngine.Rendering.Universal;
#endif

#if HDRP_ENABLED
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Unity.Simulation
{
    /// <summary>
    /// An enum indicating the type of rendering pipeline being used.
    /// </summary>
    public enum RenderingPipelineType
    {
        URP,
        HDRP,
        BUILTIN
    }

    public delegate void RenderPassCallbackDelegate(ScriptableRenderContext context, Camera camera, CommandBuffer cb);

    public class SRPSupport
    { 
        public static SRPSupport instance { get; protected set; }

#if HDRP_ENABLED
        GameObject _customPassesVolumeGO;
        CustomPassVolume _afterPostPassVolume;
#endif

#if URP_ENABLED
        URPCaptureRendererFeature _URPRendererFeature;
        bool _addedRendererFeature = false;

        public bool URPCameraDepthEnabled
        {
            get
            {
                return SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.URP ? UniversalRenderPipeline.asset.supportsCameraDepthTexture : false;
            }
        }

        public bool URPRendererFeatureAdded
        {
            get
            {
                return SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.URP ? _URPRendererFeature != null : false;
            }
        }
#endif

        Dictionary<Camera, List<AsyncRequest<CaptureCamera.CaptureState>>> _pendingCameraRequests = new Dictionary<Camera, List<AsyncRequest<CaptureCamera.CaptureState>>>();

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (SRPSupport.UsingCustomRenderPipeline())
            {
                instance = new SRPSupport();

                if (CaptureCamera.SRPSupport == null)
                    CaptureCamera.SRPSupport = instance;

                Manager.Instance.StartNotification += () =>
                {
#if URP_ENABLED
                    instance.URPSetupCustomPasses();
#endif

#if HDRP_ENABLED
                    instance.HDRPSetupCustomPasses();
#endif
                    RenderPipelineManager.endCameraRendering += SRPSupport.instance.EndCameraRendering;
                };

                Manager.Instance.ShutdownNotification += () =>
                {
                    RenderPipelineManager.endCameraRendering -= SRPSupport.instance.EndCameraRendering;
#if URP_ENABLED
                    if (instance._addedRendererFeature && instance._URPRendererFeature != null)
                        instance.RemoveRendererFeatureIfPresent(instance._URPRendererFeature);
#endif
                };
            }
        }

        /// <summary>
        /// Returns true if using a custom render pipeline or false otherwise.
        /// </summary>
        /// <returns>bool</returns>
        public static bool UsingCustomRenderPipeline()
        {
#if UNITY_2019_3_OR_NEWER
            return GraphicsSettings.currentRenderPipeline != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// Get Current Rendering Pipeline type.
        /// </summary>
        /// <returns>RenderingPipelineType indicating type of current renering pipeline : (URP/HDRP/Built-in)</returns>
        public static RenderingPipelineType GetCurrentPipelineRenderingType()
        {
            if (UsingCustomRenderPipeline())
            {
#if URP_ENABLED
                return RenderingPipelineType.URP;
#endif
#if HDRP_ENABLED
                return RenderingPipelineType.HDRP;
#endif
            }
            return RenderingPipelineType.BUILTIN;
        }

        /// <summary>
        /// With different rendering pipelines, the moment when you need to capture a camera migh be different.
        /// This method will allow for the CaptureCamera class to operate as normal, while allowing the author
        /// of the render pipeline to decide when the work get dispatched.
        /// </summary>
        /// <param name="camera">The camera that you wish to queue a request for.</param>
        /// <param name="request">The request you are queueing for this camera.</param>
        public void QueueCameraRequest(Camera camera, AsyncRequest<CaptureCamera.CaptureState> request)
        {
            if (!instance._pendingCameraRequests.ContainsKey(camera))
                instance._pendingCameraRequests.Add(camera, new List<AsyncRequest<CaptureCamera.CaptureState>>());
            instance._pendingCameraRequests[camera].Add(request);
        }

        void EndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (Application.isPlaying && instance._pendingCameraRequests.ContainsKey(camera))
            {
                var pending = instance._pendingCameraRequests[camera];
                foreach (var r in pending)
                {
#if !URP_ENABLED && !HDRP_ENABLED
                    if (r.data.colorTrigger != null)
                    {
                        r.data.colorTrigger(null, default);
                        r.data.colorTrigger = null;
                    }
#endif
#if !URP_ENABLED
                    if (r.data.depthTrigger != null)
                    {
                        r.data.depthTrigger(null, default);
                        r.data.depthTrigger = null;
                    }
#endif
                    if (r.data.normalTrigger != null)
                    {
                        r.data.normalTrigger(null, default);
                        r.data.normalTrigger = null;
                    }
                    if (r.data.motionTrigger != null)
                    {
                        r.data.motionTrigger(null, default);
                        r.data.motionTrigger = null;
                    }
                }
                CleanupCameraPendingRequests(pending);
            }
        }

        void CleanupCameraPendingRequests(List<AsyncRequest<CaptureCamera.CaptureState>> pending)
        {
            for (int i = pending.Count - 1; i >= 0; --i)
            {
                if (pending[i].data.completed)
                    pending.RemoveAt(i);
            }
        }

#if URP_ENABLED
        public List<ScriptableRendererFeature> GetScriptableRendererFeatures()
        {
            UniversalRenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipelineAsset == null)
                return null;
            var fi = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(fi != null);
            var rendererDataList = fi.GetValue(pipelineAsset) as ScriptableRendererData[];
            Debug.Assert(rendererDataList != null);
            return rendererDataList[0].rendererFeatures;
        }

        public ScriptableRendererFeature FindRendererFeatureType(Type type)
        {
            var rendererFeatures = GetScriptableRendererFeatures();
            if (rendererFeatures == null)
                return null;
            foreach (var f in rendererFeatures)
                if (f.GetType() == type)
                    return f;
            return null;
        }

        public void AddRendererFeatureIfNotPresent(ScriptableRendererFeature feature)
        {
            var rendererFeatures = GetScriptableRendererFeatures();
            if (rendererFeatures != null && !rendererFeatures.Exists(element => element.GetType() == feature.GetType()))
                rendererFeatures.Add(feature);
        }

        public void RemoveRendererFeatureIfPresent(ScriptableRendererFeature feature)
        {
            var rendererFeatures = GetScriptableRendererFeatures();
            if (rendererFeatures != null && rendererFeatures.Exists(element => element.Equals(feature)))
                rendererFeatures.Remove(feature);
        }

        void URPSetupCustomPasses()
        {
            _URPRendererFeature = FindRendererFeatureType(typeof(URPCaptureRendererFeature)) as URPCaptureRendererFeature;
            if (_URPRendererFeature == null)
            {
                _addedRendererFeature = true;
                _URPRendererFeature = ScriptableObject.CreateInstance(typeof(URPCaptureRendererFeature)) as URPCaptureRendererFeature;
                _URPRendererFeature.name = "Simulation Capture";
                AddRendererFeatureIfNotPresent(_URPRendererFeature);
            }

            var postCallbackPass = new URPCallbackPass("Unity Simulation URP AfterPost Pass", RenderPassEvent.AfterRenderingPostProcessing, new FilteringSettings(RenderQueueRange.all, -1));
            postCallbackPass.callback = (context, camera, commandBuffer) =>
            {
                if (Application.isPlaying && instance._pendingCameraRequests.ContainsKey(camera))
                {
                    RenderTargetIdentifier rtid = default;
                    UniversalAdditionalCameraData additionalCameraData;
                    if (camera.gameObject.TryGetComponent(out additionalCameraData))
                        rtid = additionalCameraData.scriptableRenderer.cameraColorTarget;

                    var pending = instance._pendingCameraRequests[camera];
                    foreach (var r in pending)
                    {
                        if (r.data.colorTrigger != null)
                        {
                            r.data.colorTrigger(commandBuffer, rtid);
                            r.data.colorTrigger = null;
                        }
                    }

                    CleanupCameraPendingRequests(pending);
                }
            };
            _URPRendererFeature.renderPasses.Add(postCallbackPass);

            var depthCallbackPass = new URPCallbackPass("Unity Simulation URP AfterOpaque Pass", RenderPassEvent.AfterRenderingOpaques, new FilteringSettings(RenderQueueRange.opaque, -1));
            depthCallbackPass.callback = (context, camera, commandBuffer) =>
            {
                if (Application.isPlaying && instance._pendingCameraRequests.ContainsKey(camera))
                {
                    RenderTargetIdentifier rtid = default;
                    UniversalAdditionalCameraData additionalCameraData;
                    if (camera.gameObject.TryGetComponent(out additionalCameraData))
                    {
// Working around the script updater, which has an issue in the project settings screen.
// When you open the project settings screen on some platforms, it goes into an infinite
// loop trying to update this script cameraDepth -> cameraDepthTarget.
#if URP_10_0_OR_LATER
                        rtid = additionalCameraData.scriptableRenderer.cameraDepthTarget;
#else
                        rtid = additionalCameraData.scriptableRenderer.cameraDepth;
#endif
                    }

                    var pending = instance._pendingCameraRequests[camera];
                    foreach (var r in pending)
                    {
                        if (r.data.depthTrigger != null)
                        {
                            r.data.depthTrigger(commandBuffer, rtid);
                            r.data.depthTrigger = null;
                        }
                    }

                    CleanupCameraPendingRequests(pending);
                }
            };
            _URPRendererFeature.renderPasses.Add(depthCallbackPass);
        }
#endif

#if HDRP_ENABLED
        void HDRPSetupCustomPasses()
        {
            var colorCallbackPass = new HDRPCallbackPass("Capture.AfterPost.Pass");
            colorCallbackPass.callback = (context, camera, commandBuffer) =>
            {
                if (Application.isPlaying && instance._pendingCameraRequests.ContainsKey(camera))
                {
                    var pending = instance._pendingCameraRequests[camera];
                    foreach (var r in pending)
                    {
                        if (r.data.colorTrigger != null)
                        {
                            r.data.colorTrigger(commandBuffer, default);
                            r.data.colorTrigger = null;
                        }
                    }

                    CleanupCameraPendingRequests(pending);
                }
            };

            _customPassesVolumeGO = new GameObject("Capture.Volume.CustomPass");
            _afterPostPassVolume = _customPassesVolumeGO.AddComponent<CustomPassVolume>();
            _afterPostPassVolume.injectionPoint = CustomPassInjectionPoint.AfterPostProcess;
            _afterPostPassVolume.customPasses.Add(colorCallbackPass);
        }
#endif // HDRP_ENABLED
    }
}

