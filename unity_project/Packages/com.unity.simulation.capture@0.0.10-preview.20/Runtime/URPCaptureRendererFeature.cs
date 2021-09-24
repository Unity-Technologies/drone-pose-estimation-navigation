#if URP_ENABLED
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unity.Simulation
{
    public class URPCaptureRendererFeature : ScriptableRendererFeature
    {
        public List<ScriptableRenderPass> renderPasses = new List<ScriptableRenderPass>();

        public override void Create()
        {}

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (Application.isPlaying)
            {
                foreach (var pass in renderPasses)
                    renderer.EnqueuePass(pass);
            }
        }
    }
}
#endif // URP_ENABLED
