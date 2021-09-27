#if HDRP_ENABLED
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

using System;
using System.IO;

namespace Unity.Simulation
{
    public class HDRPCallbackPass : CustomPass
    {
        public RenderPassCallbackDelegate callback;

        public HDRPCallbackPass(string name)
        {
            this.name = name;
        }

#if HDRP_10_2_2_OR_LATER
        protected override void Execute(CustomPassContext ctx)
        {
            if (Application.isPlaying)
            {
                callback?.Invoke(ctx.renderContext, ctx.hdCamera.camera, ctx.cmd);
            }
        }
#else
        protected override void Execute(ScriptableRenderContext context, CommandBuffer commandBuffer, HDCamera hdCamera, CullingResults cullingResult)
        {
            if (Application.isPlaying)
            {
                callback?.Invoke(context, hdCamera.camera, commandBuffer);
            }
        }
#endif // HDRP_10_2_2_OR_LATER
    }
}
#endif // HDRP_ENABLED
