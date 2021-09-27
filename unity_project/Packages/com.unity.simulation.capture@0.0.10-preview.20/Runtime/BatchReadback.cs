#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Unity.Simulation
{
    public class BatchReadback
    {
        static BatchReadback _instance;

        public static BatchReadback Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BatchReadback();
                    Manager.Instance.ShutdownNotification += _instance.FlushRequestsPool;
                }
                return _instance;
            }
        }

        public void Shutdown()
        {
            Flush();
            _instance = null;
        }

        public int BatchSize = 50;

        Queue<ReadbackRequest> _requestsBatch = new Queue<ReadbackRequest>();
        Queue<ReadbackRequest> _requestsPool  = new Queue<ReadbackRequest>();

        /// <summary>
        /// Queue a rendertexture for readback. The readback will happen after the number of requests reaches the batchsize.
        /// </summary>
        /// <param name="request">The request associated with this batch readback instance.</param>
        /// <param name="channel">Which channel this readback is for.</param>
        /// <param name="renderTexture">Render texture to readback.</param>
        public void QueueReadback(AsyncRequest<CaptureCamera.CaptureState> request, CaptureCamera.Channel channel, RenderTexture renderTexture)
        {
            Debug.Assert(request.data.GetFunctor(channel) != null, $"QueueReadback request has no completion function for {channel} channel");

            var rbr = GetReadBackRequestFromPool(request, channel, renderTexture);
            _requestsBatch.Enqueue(rbr);
            if (_requestsBatch.Count == BatchSize)
                Flush();
        }

        void Flush()
        {
            if (GraphicsUtilities.SupportsAsyncReadback())
                ProcessBatchAsync();
            else
                ProcessBatch();
        }

        ReadbackRequest GetReadBackRequestFromPool(AsyncRequest<CaptureCamera.CaptureState> request, CaptureCamera.Channel channel, RenderTexture renderTexture)
        {
            ReadbackRequest rbr;
            if (_requestsPool.Count > 0)
            {
                rbr = _requestsPool.Dequeue();
            }
            else
            {
                rbr = new ReadbackRequest();
            }

            rbr.request  = request;
            rbr.channel  = channel;
            rbr.callback = request.data.SetFunctor(channel, null);

            if (rbr.renderTexture == null ||
                rbr.renderTexture.width != renderTexture.width ||
                rbr.renderTexture.height != renderTexture.height ||
                !rbr.renderTexture.CompareFormat(renderTexture.graphicsFormat))
                rbr.renderTexture = new RenderTexture(renderTexture);

            Graphics.Blit(renderTexture, rbr.renderTexture);

            return rbr;
        }

        void ProcessBatchAsync()
        {
            while (_requestsBatch.Count > 0)
            {
                var request = _requestsBatch.Dequeue();
                AsyncGPUReadback.Request(request.renderTexture, 0, (asyncRequest) =>
                {
                    if (asyncRequest.hasError)
                    {
                        Log.E("Async GPUReadbackRequest failed!");
                    }
                    else
                    {
                        request.InvokeCallback(asyncRequest.GetData<byte>().ToArray());
                    }
                    _requestsPool.Enqueue(request);
                });
            }
        }

        void ProcessBatch()
        {
            while(_requestsBatch.Count > 0)
            {
                var request = _requestsBatch.Dequeue();

                request.InvokeCallback(GraphicsUtilities.GetPixelsSlow(request.renderTexture));
                _requestsPool.Enqueue(request);
            }
        }
        
        void FlushRequestsPool()
        {
            while(_requestsBatch.Count > 0)
                ProcessBatch();
        }
    }

    public class ReadbackRequest
    {
        /// <summary>
        /// The request associated with this batched readback instance.
        /// </summary>
        public AsyncRequest<CaptureCamera.CaptureState> request;

        /// <summary>
        /// Channel this readback request is for.
        /// </summary>
        public CaptureCamera.Channel channel;

        /// <summary>
        /// Render texture for which the readback is requested
        /// </summary>
        public RenderTexture renderTexture;
        
        /// <summary>
        /// Texture to which the readback data is to be copied to.
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// Completion callback function.
        /// </summary>
        public Func<AsyncRequest<CaptureCamera.CaptureState>, AsyncRequest.Result> callback;

        /// <summary>
        /// Invoke the request functor upon completion of the readback request.
        /// </summary>
        /// <param name="data"></param>
        public void InvokeCallback(byte[] data)
        {
            Debug.Assert(data != null && data.Length != 0);
            Debug.Assert(callback != null);
            request.data.SetBuffer(channel, data);
            request.Enqueue(callback);
            request.Execute();
        }
    }
}
#endif // UNITY_2019_3_OR_NEWER
