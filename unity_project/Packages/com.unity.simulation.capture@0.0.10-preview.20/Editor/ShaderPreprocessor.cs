using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Simulation
{
    class ShaderPreprocessor : IPreprocessShaders
    {
        string[] HDRPShaders = new string[]
        {
            "usim/BlitCopyDepthHDRP",
            "usim/BlitCopyMotionHDRP",
            "usim/BlitCopyNormalsHDRP"
        };

        string[] URPShaders = new string[]
        {

        };

        public int callbackOrder { get { return 0; } }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
#if !HDRP_ENABLED
            foreach (var sn in HDRPShaders)
                if (string.Compare(sn, shader.name) == 0)
                {
                    data.Clear();
                    break;
                }
#endif
#if !URP_ENABLED
            foreach (var sn in URPShaders)
                if (string.Compare(sn, shader.name) == 0)
                {
                    data.Clear();
                    break;
                }
#endif
        }
    }
}