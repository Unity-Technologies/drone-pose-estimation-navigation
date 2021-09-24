Shader "usim/BlitCopyMotionHDRP"
{
    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "BlitCopyMotionHDRP"

            HLSLPROGRAM

            #pragma multi_compile HDRP_DISABLED HDRP_ENABLED
            #pragma only_renderers d3d11 vulkan metal
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment FullScreenPass

#if HDRP_ENABLED
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/MotionBlurCommon.hlsl"

            float4 FullScreenPass(Varyings varyings) : SV_Target
            {
                float2 motionVector = DecodeMotionVectorFromPacked(LOAD_TEXTURE2D_X(_CameraMotionVectorsTexture, varyings.positionCS.xy));
                return float4(motionVector, 0, 1);
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