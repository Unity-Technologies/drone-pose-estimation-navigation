using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
 
namespace Unity.Simulation
{
    /// <summary>
    /// Enum for overriding the internal check to determine if the image should be flipped.
    /// Can be used on a per channel basis, or for all channels.
    /// </summary>
    [Flags]
    public enum ForceFlip
    {
        None,
        Color  = (1 << 0),
        Depth  = (1 << 1),
        Normal = (1 << 2),
        Motion = (1 << 3),
        All    = Color | Depth | Normal | Motion,
    }

    /// <summary>
    /// Capture class for cameras. Supports Color, Depth, MotionVectors.
    /// Captures supported channels to file system and notifies Manager.
    /// </summary>
    public static class CaptureCamera
    {
        /// <summary>
        /// Invoked when a request has completed reading back data for a channel.
        /// </summary>
        public delegate void ReadbackCompletionDelegate(AsyncRequest<CaptureState> request, Channel channel, RenderTexture rt, byte[] data);

        /// <summary>
        /// Invoked when a request has completed readback of all channels.
        /// </summary>
        public delegate AsyncRequest.Result RequestCompletionDelegate(AsyncRequest<CaptureState> request);

        /// <summary>
        /// Enumeration for the supported channels.
        /// </summary>
        public enum Channel
        {
            /// <summary>
            /// Enumeration value specifying the color channel.
            /// </summary>
            Color,
            /// <summary>
            /// Enumeration value specifying the depth channel.
            /// </summary>
            Depth,
            /// <summary>
            /// Enumeration value specifying the normals channel.
            /// </summary>
            Normal,
            /// <summary>
            /// Enumeration value specifying the motion vectors channel.
            /// </summary>
            Motion,
            /// <summary>
            /// Enumeration value specifying the max number of channels.
            /// </summary>
            Max
        }

        /// <summary>
        /// Capture state when asynchronously capturing a camera render.
        /// </summary>
        public struct CaptureState
        {
            /// <summary>
            /// The camera associated with this capture request.
            /// </summary>
            public Camera camera;

            /// <summary>
            /// The frame number this request was issued on.
            /// </summary>
            public int frame;

            /// <summary>
            /// While in flight, references the source buffer to read from.
            /// When completed, references the captured data in Array form.
            /// </summary>
            public object colorBuffer;

            /// <summary>
            /// While in flight, references the source buffer to read from.
            /// When completed, references the captured data in Array form.
            /// </summary>
            public object depthBuffer;

            /// <summary>
            /// While in flight, references the source buffer to read from.
            /// When completed, references the captured data in Array form.
            /// </summary>
            public object normalBuffer;

            /// <summary>
            /// While in flight, references the source buffer to read from.
            /// When completed, references the captured data in Array form.
            /// </summary>
            public object motionBuffer;

            /// <summary>
            /// Helper method to set the buffer for a specified channel.
            /// </summary>
            /// <param name="channel">Enumeration value for which channel you are specifying.</param>
            /// <param name="buffer">Object specifying the buffer you are setting for the specified channel.</param>
            public void SetBuffer(Channel channel, object buffer)
            {
                switch (channel)
                {
                    case Channel.Color:  colorBuffer  = buffer; break;
                    case Channel.Depth:  depthBuffer  = buffer; break;
                    case Channel.Normal: normalBuffer = buffer; break;
                    case Channel.Motion: motionBuffer = buffer; break;
                    default: throw new ArgumentException("CaptureState.SetBuffer invalid channel.");
                }
            }

            /// <summary>
            /// Completion function for handling capture results. Invoked once the capture data is ready.
            /// The handler is responsible for persisting the data. Once invoked, the data is discarded.
            /// </summary>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> colorFunctor;

            /// <summary>
            /// Completion function for handling capture results. Invoked once the capture data is ready.
            /// The handler is responsible for persisting the data. Once invoked, the data is discarded.
            /// </summary>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> depthFunctor;

            /// <summary>
            /// Completion function for handling capture results. Invoked once the capture data is ready.
            /// The handler is responsible for persisting the data. Once invoked, the data is discarded.
            /// </summary>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> normalFunctor;

            /// <summary>
            /// Completion function for handling capture results. Invoked once the capture data is ready.
            /// The handler is responsible for persisting the data. Once invoked, the data is discarded.
            /// </summary>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> motionFunctor;

            /// <summary>
            /// Helper method to set the completion functor for a specified channel.
            /// </summary>
            /// <param name="channel">Enumeration value for which channel you are specifying.</param>
            /// <param name="functor">Completion functor for handling the captured data when available.</param>
            /// <returns>The previous completion functor.</returns>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> SetFunctor(Channel channel, Func<AsyncRequest<CaptureState>, AsyncRequest.Result> functor)
            {
                Func<AsyncRequest<CaptureState>, AsyncRequest.Result> previous = null;
                switch (channel)
                {
                    case Channel.Color:  previous = colorFunctor;  colorFunctor  = functor; break;
                    case Channel.Depth:  previous = depthFunctor;  depthFunctor  = functor; break;
                    case Channel.Normal: previous = normalFunctor; normalFunctor = functor; break;
                    case Channel.Motion: previous = motionFunctor; motionFunctor = functor; break;
                    default: throw new ArgumentException("CaptureState.SetFunctor invalid channel.");
                }
                return previous;
            }

            /// <summary>
            /// Action to populate the command buffer to readback a color target.
            /// </summary>
            public Action<CommandBuffer, RenderTargetIdentifier> colorTrigger;

            /// <summary>
            /// Action to populate the command buffer to readback a depth target.
            /// </summary>
            public Action<CommandBuffer, RenderTargetIdentifier> depthTrigger;

            /// <summary>
            /// Action to populate the command buffer to readback a normal target.
            /// </summary>
            public Action<CommandBuffer, RenderTargetIdentifier> normalTrigger;

            /// <summary>
            /// Action to populate the command buffer to readback a motion target.
            /// </summary>
            public Action<CommandBuffer, RenderTargetIdentifier> motionTrigger;

            /// <summary>
            /// Help method to set the command buffer to use, for the specified channel.
            /// </summary>
            /// <param name="channel">Enumeration value for which channel you are specifying.</param>
            /// <param name="action">Action to populate the command buffer.</param>
            public Action<CommandBuffer, RenderTargetIdentifier> SetTrigger(Channel channel, Action<CommandBuffer, RenderTargetIdentifier> action)
            {
                Action<CommandBuffer, RenderTargetIdentifier> previous = null;
                switch (channel)
                {
                    case Channel.Color:  previous = this.colorTrigger;  this.colorTrigger  = action; break;
                    case Channel.Depth:  previous = this.depthTrigger;  this.depthTrigger  = action; break;
                    case Channel.Normal: previous = this.normalTrigger; this.normalTrigger = action; break;
                    case Channel.Motion: previous = this.motionTrigger; this.motionTrigger = action; break;
                    default: throw new InvalidOperationException();
                }
                return previous;
            }

            /// <summary>
            /// Property to determine if the capture request has completed.
            /// </summary>
            /// <returns>true if completed, false otherwise.</returns>
            public bool completed { get { return colorFunctor == null && depthFunctor == null && normalFunctor == null && motionFunctor == null; }}

            /// <summary>
            /// Help method to get the completion functor for the specified channel.
            /// </summary>
            /// <param name="channel">Enumeration value for which channel you are specifying.</param>
            public Func<AsyncRequest<CaptureState>, AsyncRequest.Result> GetFunctor(Channel channel)
            {
                switch (channel)
                {
                    case Channel.Color:  return colorFunctor;
                    case Channel.Depth:  return depthFunctor;
                    case Channel.Normal: return normalFunctor;
                    case Channel.Motion: return motionFunctor;
                    default: throw new ArgumentException("CaptureState.SetFunctor invalid channel.");
                }
            } 
        }

        [RuntimeInitializeOnLoadMethod]
        static void Notifications()
        {
            Log.I("Resolution Height: " + Screen.currentResolution.height + " Width: " + Screen.currentResolution.width);
            Manager.Instance.StartNotification += () =>
            {
                SetupMaterials();
            };

            Manager.Instance.ShutdownNotification += () =>
            {
                CommandBuffer cb;
                while (_commandBufferPool.TryDequeue(out cb))
                    cb.Dispose();
            };
        }

        /// <summary>
        /// Support for Scriptable Render Pipeline.
        /// SRP works a little differently, this abstraction allows for custom capture options when using SRP.
        /// The default implementation queues camera captures, and dispatches on RenderPipelineManager.endFrameRendering.
        /// You can provide your own implementation of QueueCameraRequest, and dispatch that when appropriate.
        /// </summary>
        public static SRPSupport SRPSupport;

        /// <summary>
        /// Property for determining whether or not a scriptable render pipeline is enabled or not.
        /// Will only ever return true on Unity versions 2019.3 or later.
        /// </summary>
        public static bool scriptableRenderPipeline
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                return SRPSupport != null && SRPSupport.UsingCustomRenderPipeline();
#else
                return false;
#endif
            }
        }

        static ConcurrentQueue<CommandBuffer> _commandBufferPool = new ConcurrentQueue<CommandBuffer>();

        static Vector2 _invertVec2d = new Vector2(1, -1);

        static CameraEvent[] _cameraEvents = new CameraEvent[]
        {
            CameraEvent.AfterEverything,
            CameraEvent.AfterDepthTexture,
            CameraEvent.AfterDepthTexture,
            CameraEvent.BeforeImageEffects,
        };

        static Material[] _depthMaterials;
        static Material[] _normalMaterials;
        static Material[] _motionMaterials;

        /// <summary>
        /// Captures a camera render and writes out the color channel to a file.
        /// </summary>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="colorFormat"> The color pixel format to capture in. </param>
        /// <param name="colorPath"> The location of the file to write out. </param>
        /// <param name="colorImageFormat"> The image format to write the data out in. </param>
        /// <returns>AsyncRequest&lt;CaptureState&gt;</returns>
        public static AsyncRequest<CaptureState> CaptureColorToFile(Camera camera, GraphicsFormat colorFormat, string colorPath, CaptureImageEncoder.ImageFormat colorImageFormat = CaptureImageEncoder.ImageFormat.Jpg)
        {
            return CaptureColorAndDepthToFile(camera, colorFormat, colorPath, colorImageFormat);
        }

        /// <summary>
        /// Captures a camera render and writes out the depth channel to a file.
        /// </summary>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="depthFormat"> The pixel format to capture in. </param>
        /// <param name="depthPath"> The location of the file to write out. </param>
        /// <param name="depthImageFormat"> The image format to write the data out in. </param>
        /// <returns>AsyncRequest&lt;CaptureState&gt;</returns>
        public static AsyncRequest<CaptureState> CaptureDepthToFile(Camera camera, GraphicsFormat depthFormat, string depthPath, CaptureImageEncoder.ImageFormat depthImageFormat = CaptureImageEncoder.ImageFormat.Raw)
        {
            return CaptureColorAndDepthToFile(camera, depthFormat: depthFormat, depthPath: depthPath, depthImageFormat: depthImageFormat);
        }

        /// <summary>
        /// Captures a camera render and writes out the color and depth channels to a file.
        /// </summary>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="colorFormat"> The pixel format to capture in. </param>
        /// <param name="colorPath"> The location of the file to write out. </param>
        /// <param name="colorImageFormat"> The image format to write the data out in. </param>
        /// <param name="depthFormat"> The pixel format to capture in. </param>
        /// <param name="depthPath"> The location of the file to write out. </param>
        /// <param name="depthImageFormat"> The image format to write the data out in. </param>
        /// <returns>AsyncRequest&lt;CaptureState&gt;</returns>
        public static AsyncRequest<CaptureState> CaptureColorAndDepthToFile
        (
            Camera camera,
            GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_UNorm, 
            string colorPath = null, 
            CaptureImageEncoder.ImageFormat colorImageFormat = CaptureImageEncoder.ImageFormat.Jpg,
            GraphicsFormat depthFormat = GraphicsFormat.R16_UNorm, 
            string depthPath = null,
            CaptureImageEncoder.ImageFormat depthImageFormat = CaptureImageEncoder.ImageFormat.Raw
        )
        {
            Debug.Assert(camera != null, "CaptureColorAndDepthToFile camera cannot be null");

            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> colorFunctor = null;
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> depthFunctor = null;

            var width  = camera.pixelWidth;
            var height = camera.pixelHeight;

            if (colorPath != null)
            {
                colorFunctor = (AsyncRequest<CaptureState> r) =>
                {
                    colorPath = CaptureImageEncoder.EnforceFileExtension(colorPath, colorImageFormat);
                    var result = FileProducer.Write(colorPath, CaptureImageEncoder.EncodeArray(r.data.colorBuffer as Array, width, height, colorFormat, colorImageFormat));
                    return result ? AsyncRequest.Result.Completed : AsyncRequest.Result.Error;
                };
            }

            if (depthPath != null)
            {
                depthFunctor = (AsyncRequest<CaptureState> r) =>
                {
                    depthPath = CaptureImageEncoder.EnforceFileExtension(depthPath, depthImageFormat);
                    var result = FileProducer.Write(depthPath, CaptureImageEncoder.EncodeArray(r.data.depthBuffer as Array, width, height, depthFormat, depthImageFormat));
                    return result ? AsyncRequest.Result.Completed : AsyncRequest.Result.Error;
                };
            }

            return Capture(camera, colorFunctor, colorFormat, depthFunctor, depthFormat, forceFlip: ForceFlip.None);
        }

        /// <summary>
        /// Main Capture entrypoint. 
        /// </summary>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="colorFunctor"> Completion functor for the color channel. </param>
        /// <param name="colorFormat"> The pixel format to capture in. </param>
        /// <param name="depthFunctor"> Completion functor for the depth channel. </param>
        /// <param name="depthFormat"> The pixel format to capture in. </param>
        /// <param name="motionFunctor"> Completion functor for the motion vectors channel. </param>
        /// <param name="motionFormat"> The pixel format to capture in. </param>
        /// <param name="flipY"> Whether or not to flip the image vertically. </param>
        /// <returns>AsyncRequest&lt;CaptureState&gt;</returns>
        [Obsolete("Capture with boolean flipY has been deprecated. Use the version with ForceFlipY instead.")]
        public static AsyncRequest<CaptureState> Capture
        (
            Camera camera,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> colorFunctor = null,
            GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_UNorm, 
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> depthFunctor = null,
            GraphicsFormat depthFormat = GraphicsFormat.R16_UNorm,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> motionFunctor = null,
            GraphicsFormat motionFormat = GraphicsFormat.R16_UNorm,
            bool flipY = false
        )
        {
            return Capture(camera, colorFunctor, colorFormat, depthFunctor, depthFormat, motionFunctor, motionFormat, forceFlip: flipY ? ForceFlip.All : ForceFlip.None);
        }

        /// <summary>
        /// Main Capture entrypoint. 
        /// </summary>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="colorFunctor"> Completion functor for the color channel. </param>
        /// <param name="colorFormat"> The pixel format to capture in. </param>
        /// <param name="depthFunctor"> Completion functor for the depth channel. </param>
        /// <param name="depthFormat"> The pixel format to capture in. </param>
        /// <param name="motionFunctor"> Completion functor for the motion vectors channel. </param>
        /// <param name="motionFormat"> The pixel format to capture in. </param>
        /// <param name="forceFlipY"> Override one ore more channels to force flip either true or false. </param>
        /// <param name="readWrite"> Specify the desired color space conversion. If Default, then will be set to sRGB for SRP Color channel. </param>
        /// <returns>AsyncRequest&lt;CaptureState&gt;</returns>
        public static AsyncRequest<CaptureState> Capture
        (
            Camera camera,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> colorFunctor = null,
            GraphicsFormat colorFormat = GraphicsFormat.R8G8B8A8_UNorm, 
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> depthFunctor = null,
            GraphicsFormat depthFormat = GraphicsFormat.R8G8B8A8_UNorm,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> normalFunctor = null,
            GraphicsFormat normalFormat = GraphicsFormat.R8G8B8A8_UNorm,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> motionFunctor = null,
            GraphicsFormat motionFormat = GraphicsFormat.R8G8B8A8_UNorm,
            ForceFlip forceFlip = ForceFlip.None,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default
        )
        {
            var request = Manager.Instance.CreateRequest<AsyncRequest<CaptureState>>();

            request.data.camera = camera;
            request.data.frame  = Time.frameCount;

            if (colorFunctor != null)
                SetupCaptureRequest(request, Channel.Color,  camera, colorFormat,  colorFunctor,  forceFlip, readWrite);
            if (depthFunctor != null)
                SetupCaptureRequest(request, Channel.Depth,  camera, depthFormat,  depthFunctor,  forceFlip, readWrite);
            if (normalFunctor != null)
                SetupCaptureRequest(request, Channel.Normal, camera, normalFormat, normalFunctor, forceFlip, readWrite);
            if (motionFunctor != null)
                SetupCaptureRequest(request, Channel.Motion, camera, motionFormat, motionFunctor, forceFlip, readWrite);

#if UNITY_EDITOR
            ValidateRequestParameters(request);
#endif

#if UNITY_2019_3_OR_NEWER
            if (scriptableRenderPipeline)
                SRPSupport?.QueueCameraRequest(camera, request);
#endif
            return request;
        }

        /// <summary>
        /// Setup a capture request for a channel. Once completed, the functor will be called with the channel data, in the format requested.
        /// </summary>
        /// <param name="request"> AsyncRequest to enqueue readbacks to. When all are completed, the request is marked completed. </param>
        /// <param name="channel"> The channel to capture data from (color, depth etc.) </param>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="format"> The graphics format you want the data to be in. </param>
        /// <param name="functor"> The completion functor to call with the data. </param>
        /// <param name="forceFlipY"> Flags allowing you to force flipY for arbitrary channels. </param>
        /// <param name="readWrite"> Specify the desired color space conversion. If Default, then will be set to sRGB for SRP Color channel. </param>
        public static void SetupCaptureRequest
        (
            AsyncRequest<CaptureState> request,
            Channel channel,
            Camera camera,
            GraphicsFormat format,
            Func<AsyncRequest<CaptureState>, AsyncRequest.Result> functor,
            ForceFlip forceFlipY,
            RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default
        )
        {
            request.data.SetFunctor(channel, functor);

            Debug.Assert(request.data.camera == camera, "Capture: Camera must match the camera in request.data.camera");
            Debug.Assert(GraphicsUtilities.SupportsRenderTextureFormat(format), $"Capture: GraphicsFormat {format} not supported for {channel} channel");

            var material = SelectShaderVariantForChannel(channel, format);

            if (scriptableRenderPipeline)
                request.data.SetTrigger(channel, (cb, rtid) => SetupCaptureRequestCommandBufferForChannel(request, channel, camera, cb, rtid, material, format, forceFlipY, readWrite, HandleReadbackCompletion));
            else
                SetupCaptureRequestCommandBufferForChannel(request, channel, camera, null, default, material, format, forceFlipY, readWrite, HandleReadbackCompletion);
        }

        /// <summary>
        /// Setup a CommandBuffer for capturing from a channel.
        /// </summary>
        /// <param name="request"> AsyncRequest to enqueue readbacks to. When all are completed, the request is marked completed. </param>
        /// <param name="channel"> The channel to capture data from (color, depth etc.) </param>
        /// <param name="camera"> The Camera to capture data from. </param>
        /// <param name="commandBuffer"> The command buffer to populate. If null, a command buffer will be allocated from a pool. </param>
        /// <param name="rtid"> The render target identifier to capture. If default(RenderTargetIdentifier) then readback will occur from either the Camera.targetTexture or the back buffer. </param>
        /// <param name="material"> Material to use when blitting. If null, then no blit will occur. Use by depth to copy from depth buffer to color buffer, but could be useful elsewhere as well. </param>
        /// <param name="format"> The graphics format you want the data to be in. </param>
        /// <param name="forceFlipY"> Flags allowing you to force flipY for arbitrary channels. </param>
        /// <param name="readWrite"> Specify the desired color space conversion. If Default, then will be set to sRGB for SRP Color channel. </param>
        /// <param name="completion"> The completion functor to call with the data. </param>
        public static void SetupCaptureRequestCommandBufferForChannel(AsyncRequest<CaptureState> request, Channel channel, Camera camera, CommandBuffer commandBuffer, RenderTargetIdentifier rtid, Material material, GraphicsFormat format, ForceFlip forceFlipY, RenderTextureReadWrite readWrite, ReadbackCompletionDelegate completion)
        {
            Debug.Assert(ThreadUtility.IsMainThread());

            // Some pipelines need to add commands to their own command buffers before calling us.
            // In these cases, the command buffer is passed in, otherwise we just create our own.
            bool usePassedInCommandBuffer  = commandBuffer != null;
            bool usePassedInRenderTargetId = !Equals(rtid, default(RenderTargetIdentifier));

            // If we were not passed in a command buffer, then we can use one from our pool.
            if (!usePassedInCommandBuffer)
            {
                if (!_commandBufferPool.TryDequeue(out commandBuffer))
                    commandBuffer = new CommandBuffer();
                commandBuffer.name = $"cb.{channel.ToString()}";
            }

            if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.BUILTIN)
            {
                camera.AddCommandBuffer(_cameraEvents[(int)channel], commandBuffer);
                commandBuffer.SetRenderTarget(null as RenderTexture);
            }

            var flipY = ShouldFlipY(camera, channel, forceFlipY, usePassedInRenderTargetId);
            var rtf   = GraphicsFormatUtility.GetRenderTextureFormat(format);

            // If the passed in readWrite is default, then we assume sRGB if we are using SRP and this is a color channel.
            readWrite = readWrite == RenderTextureReadWrite.Default && scriptableRenderPipeline && channel == Channel.Color ? RenderTextureReadWrite.sRGB : readWrite;
            
            RenderTexture rt = camera.targetTexture, rt1 = null, rt2 = null, rt3 = null, rt4 = null;

            // If we're doing a blit from NULL ("backbuffer") into destination, we'll need to blit once
            // into a RT, (since we can't sample backbuffer in a shader), and inverting (sourceScale)
            // is ignored when blitting from the NULL target, due to taking the GrabPass path.
            if (channel == Channel.Color && rt == null && !usePassedInRenderTargetId)
            {
                rt1 = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight, 0, rtf, readWrite);
                commandBuffer.Blit(null, rt1);
                rtid = rt = rt1;
            }

            if (material != null)
            {
                var src = usePassedInRenderTargetId ? rtid : rt; usePassedInRenderTargetId = false;
                rt2 = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight, 0, rtf, readWrite);
                commandBuffer.Blit(src, rt2, material);
                rtid = rt = rt2;
            }

            // Currently there is no Blit variant that takes a material and a scale, so the flipY always
            // needs to be separate from any other blits that might occur with a material.
            if (flipY)
            {
                var src = usePassedInRenderTargetId ? rtid : rt; usePassedInRenderTargetId = false;
                rt3 = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight, 0, rtf, readWrite);
                commandBuffer.Blit(src, rt3, _invertVec2d, Vector2.up);
                rtid = rt = rt3;
            }

            // Finally, if the pixel format doesn't match the requested format, we copy the texture so that
            // the color format is corrected (which is done automatically). This really only happens when
            // we are capturing from a camera with a RT, and we don't need to flip or use a material.
            if (usePassedInRenderTargetId || !rt.CompareFormat(format))
            {
                var src = usePassedInRenderTargetId ? rtid : rt; usePassedInRenderTargetId = false;
                rt4 = RenderTexture.GetTemporary(camera.pixelWidth, camera.pixelHeight, 0, rtf, readWrite);
                commandBuffer.Blit(src, rt4);
                rtid = rt = rt4;
            }

#if UNITY_2019_3_OR_NEWER
            if (CaptureOptions.useBatchReadback)
            {
                Manager.Instance.QueueForEndOfFrame(() =>
                {
                    if (request.data.GetFunctor(channel) != null)
                    {
                        completion?.Invoke(request, channel, rt, null);

                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.BUILTIN)
                            camera.RemoveCommandBuffer(_cameraEvents[(int)channel], commandBuffer);
                        if (!usePassedInCommandBuffer)
                            RecycleCommandBuffer(commandBuffer);
                        ReleaseTemporaryTextures(rt1, rt2, rt3, rt4);
                    }
                });
            }
            else
#endif
            if (GraphicsUtilities.SupportsAsyncReadback())
            {
                commandBuffer.RequestAsyncReadback(rt, r =>
                {
                    if (request.data.GetFunctor(channel) != null)
                    {
                        completion?.Invoke(request, channel, rt, r.hasError ? null : r.GetData<byte>().ToArray());

                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.BUILTIN)
                            camera.RemoveCommandBuffer(_cameraEvents[(int)channel], commandBuffer);
                        if (!usePassedInCommandBuffer)
                            RecycleCommandBuffer(commandBuffer);
                        ReleaseTemporaryTextures(rt1, rt2, rt3, rt4);
                    }
                });
            }
            else
            {
                Manager.Instance.QueueForEndOfFrame(() =>
                {
                    if (request.data.GetFunctor(channel) != null)
                    {
                        completion?.Invoke(request, channel, rt, GraphicsUtilities.GetPixelsSlow(rt));

                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.BUILTIN)
                            camera.RemoveCommandBuffer(_cameraEvents[(int)channel], commandBuffer);
                        if (!usePassedInCommandBuffer)
                            RecycleCommandBuffer(commandBuffer);
                        ReleaseTemporaryTextures(rt1, rt2, rt3, rt4);
                    }
                });
            }

            // If our command buffer wasn't passed to us, and we are not the builtin render pipeline, we can execute.
            if (!usePassedInCommandBuffer && SRPSupport.GetCurrentPipelineRenderingType() != RenderingPipelineType.BUILTIN)
                Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        static void HandleReadbackCompletion(AsyncRequest<CaptureState> request, Channel channel, RenderTexture rt, byte[] data)
        {
#if UNITY_2019_3_OR_NEWER
            if (CaptureOptions.useBatchReadback)
                BatchReadback.Instance.QueueReadback(request, channel, rt);
            else
#endif
            {
                request.error = data == null;
                if (data != null)
                {
                    request.data.SetBuffer(channel, data);
                    request.Enqueue(request.data.GetFunctor(channel));
                    request.Execute();
                }
            }
            request.data.SetFunctor(channel, null);
        }

        /// <summary>
        /// Check if for the given rendering pipeline there is a need to flip Y during readback.
        /// </summary>
        /// <param name="camera">Camera from which the readback is being performed.</param>
        /// <param name="channel">Channel which the readback is being performed for.</param>
        /// <param name="forceFlipY">Override the default to either force flip or force not flip, or neither.</param>
        /// <param name="usePassedInRenderTargetId">When we are using a passed in rtid, then we don't need to flip.</param>
        /// <returns>A boolean indicating if the flip is required.</returns>
        public static bool ShouldFlipY(Camera camera, Channel channel, ForceFlip forceFlipY, bool usePassedInRenderTargetId)
        {
            bool shouldFlipY = false;

            if (forceFlipY != ForceFlip.None)
            {
                var mask = 1 << (int)channel;
                shouldFlipY = ((int)forceFlipY & mask) == mask ? true : false;
            }
            else
            {
                switch (channel)
                {
                    case Channel.Color:
#if !URP_ENABLED && !HDRP_ENABLED // URP and HDRP always pass in a RenderTargetIdentifier for the color render texture, which never needs flipping.
                        shouldFlipY = !usePassedInRenderTargetId && camera.targetTexture == null && SystemInfo.graphicsUVStartsAtTop;
#endif
                        break;
                    case Channel.Depth:
                        break;
                    case Channel.Normal:
                        break;
                    case Channel.Motion:
                        break;
                }
            }

            if (Log.level == Log.Level.Verbose)
            {
                var rt    = camera.targetTexture == null ? "null" : camera.targetTexture.ToString();
                var uv    = SystemInfo.graphicsUVStartsAtTop.ToString();
                var async = GraphicsUtilities.SupportsAsyncReadback();
                var ffy   = forceFlipY.ToString();
                var chan  = channel.ToString();
                var pipe  = SRPSupport.GetCurrentPipelineRenderingType().ToString();
                var gfx   = SystemInfo.graphicsDeviceType.ToString();

                Log.V($"ShouldFlipY: {shouldFlipY} <= rt({rt}) uv({uv}) async({async}) ffY({ffy}) chan({chan}) pipe({pipe}) gfx({gfx})");
            }

            return shouldFlipY;
        }

        static void RecycleCommandBuffer(CommandBuffer commandBuffer)
        {
            commandBuffer.Clear();
            _commandBufferPool.Enqueue(commandBuffer);
        }

        static void ReleaseTemporaryTextures(RenderTexture rt1, RenderTexture rt2, RenderTexture rt3, RenderTexture rt4)
        {
            if (rt1 != null)
                RenderTexture.ReleaseTemporary(rt1);
            if (rt2 != null)
                RenderTexture.ReleaseTemporary(rt2);
            if (rt3 != null)
                RenderTexture.ReleaseTemporary(rt3);
            if (rt4 != null)
                RenderTexture.ReleaseTemporary(rt4);
        }

        static void SetupMaterials()
        {
            _depthMaterials  = new Material[4];
            _normalMaterials = new Material[4];
            _motionMaterials = new Material[4];

            switch (SRPSupport.GetCurrentPipelineRenderingType())
            {
                case RenderingPipelineType.BUILTIN:
                    for (var i = 0; i < _depthMaterials.Length; ++i)
                    {
                        _depthMaterials[i] = new Material(Shader.Find("usim/BlitCopyDepth"));
                        _depthMaterials[i].EnableKeyword($"CHANNELS{i + 1}");
                    };
                    for (var i = 0; i < _normalMaterials.Length; ++i)
                    {
                        _normalMaterials[i] = new Material(Shader.Find("usim/BlitCopyNormals"));
                        _normalMaterials[i].EnableKeyword($"CHANNELS{i + 1}");
                    };
                    for (var i = 0; i < _motionMaterials.Length; ++i)
                    {
                        _motionMaterials[i] = new Material(Shader.Find("usim/BlitCopyMotion"));
                        _motionMaterials[i].EnableKeyword($"CHANNELS{i + 1}");
                    };
                    break;

                case RenderingPipelineType.URP:
                    goto case RenderingPipelineType.BUILTIN;

                case RenderingPipelineType.HDRP:
                    _depthMaterials[0] = new Material(Shader.Find("usim/BlitCopyDepthHDRP"));
                    _depthMaterials[0].EnableKeyword("HDRP_ENABLED");
                    _normalMaterials[0] = new Material(Shader.Find("usim/BlitCopyNormalsHDRP"));
                    _normalMaterials[0].EnableKeyword("HDRP_ENABLED");
                    _motionMaterials[0] = new Material(Shader.Find("usim/BlitCopyMotionHDRP"));
                    _motionMaterials[0].EnableKeyword("HDRP_ENABLED");
                    break;

                default:
                    throw new InvalidOperationException("Invalid RenderingPipelineType");
            }
        }

        public static Material SelectShaderVariantForChannel(Channel channel, GraphicsFormat format)
        {
            switch (channel)
            {
                case Channel.Color:
                    break;

                case Channel.Depth:
                    {
                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.HDRP)
                        {
                            Debug.Assert(_depthMaterials[0] != null);
                            return _depthMaterials[0];
                        }
                        else
                        {
                            var componentCount = GraphicsUtilities.GetComponentCount(format);
                            Debug.Assert(componentCount >= 1 && componentCount <= 4);
                            Debug.Assert(_depthMaterials[componentCount - 1] != null);
                            return _depthMaterials[componentCount - 1];
                        }
                    }

                case Channel.Normal:
                    {
                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.HDRP)
                        {
                            Debug.Assert(_normalMaterials[0] != null);
                            return _normalMaterials[0];
                        }
                        else
                        {
                            var componentCount = GraphicsUtilities.GetComponentCount(format);
                            Debug.Assert(componentCount >= 1 && componentCount <= 4);
                            Debug.Assert(_normalMaterials[componentCount - 1] != null);
                            return _normalMaterials[componentCount - 1];
                        }
                    }

                case Channel.Motion:
                    {
                        if (SRPSupport.GetCurrentPipelineRenderingType() == RenderingPipelineType.HDRP)
                        {
                            Debug.Assert(_motionMaterials[0] != null);
                            return _motionMaterials[0];
                        }
                        else
                        {
                            var componentCount = GraphicsUtilities.GetComponentCount(format);
                            Debug.Assert(componentCount >= 1 && componentCount <= 4);
                            Debug.Assert(_motionMaterials[componentCount - 1] != null);
                            return _motionMaterials[componentCount - 1];
                        }
                    }
            }
            return null;
        }

#if UNITY_EDITOR
        public static void ValidateRequestParameters(AsyncRequest<CaptureState> request)
        {
            Debug.Assert(request != null);

            Debug.Assert(request.data.camera != null, "Capture: camera cannot be null.");
            Debug.Assert(request.data.colorFunctor != null || request.data.depthFunctor != null || request.data.normalFunctor != null || request.data.motionFunctor != null, "Capture: one functor must be valid.");

            if (request.data.depthFunctor != null)
            {
                Debug.Assert((request.data.camera.depthTextureMode & DepthTextureMode.Depth) != 0, "Capture: Depth not specified for camera");
#if URP_ENABLED
                Debug.Assert(SRPSupport.URPRendererFeatureAdded, "Capture: Unity Simulation URPCaptureRendererFeature has not been added to the scriptable renderer.");
                Debug.Assert(SRPSupport.URPCameraDepthEnabled, "Capture: Depth is not enabled in the URP render pipeline asset.");
#endif
            }

            if (request.data.normalFunctor != null)
            {
                Debug.Assert((request.data.camera.depthTextureMode & DepthTextureMode.DepthNormals) != 0, "Capture: DepthNormals not specified for camera");
            }

            if (request.data.motionFunctor != null)
            {
                Debug.Assert((request.data.camera.depthTextureMode & DepthTextureMode.MotionVectors) != 0, "Capture: Motion vectors not enabled in depthTextureMode");
                Debug.Assert(SystemInfo.supportsMotionVectors, "Capture: Motion vectors are not supported");
            }
        }
#endif // UNITY_EDITOR
    }
}
