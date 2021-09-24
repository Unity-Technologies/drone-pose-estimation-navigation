#if URP_ENABLED
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unity.Simulation
{
    public class URPCallbackPass : ScriptableRenderPass
    {
        public RenderPassCallbackDelegate callback;

        string            profilerTag;
        FilteringSettings filteringSettings;

        public URPCallbackPass(string profilerTag, RenderPassEvent renderPassEvent, FilteringSettings filteringSettings)
        {
            this.profilerTag       = profilerTag;
            this.renderPassEvent   = renderPassEvent;
            this.filteringSettings = filteringSettings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Application.isPlaying)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get(profilerTag);

                callback?.Invoke(context, renderingData.cameraData.camera, commandBuffer);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }
    }
}
#endif // URP_ENABLED
