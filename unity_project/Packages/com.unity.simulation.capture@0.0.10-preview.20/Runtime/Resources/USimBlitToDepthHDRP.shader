Shader "usim/BlitCopyDepthHDRP"
{
    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "BlitCopyDepthHDRP"
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM

            #pragma multi_compile HDRP_DISABLED HDRP_ENABLED
            #pragma only_renderers d3d11 vulkan metal
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment FullScreenPass

#if HDRP_ENABLED
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

            float4 FullScreenPass(Varyings varyings) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
                float depth = LoadCameraDepth(varyings.positionCS.xy);
                depth = Linear01Depth(depth, _ZBufferParams);
                return float4(depth, depth, depth, 1);
            }
#else
            /// Dummy Implementation for non HDRP_ENABLED variants

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f Vert(appdata v)
            {
                v2f o;
                o.uv     = float2(0, 0);
                o.vertex = float4(0, 0, 0, 0);
                return o;
            }

            float4 FullScreenPass(v2f i) : SV_Target
            {
                return float4(0, 0, 0, 1);
            }
#endif
            ENDHLSL
        }
    }
    Fallback Off
}