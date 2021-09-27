Shader "usim/BlitCopyMotion"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM

            #pragma multi_compile CHANNELS1 CHANNELS2 CHANNELS3 CHANNELS4
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraMotionVectorsTexture;
            float4 _CameraMotionVectorsTexture_ST;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _CameraMotionVectorsTexture);
                return o;
            }

#if CHANNELS1
            float2 frag(v2f i) : COLOR
            {
                float2 motion = tex2D(_CameraMotionVectorsTexture, i.uv).rg;
                return motion.x;
            }
#endif
#if CHANNELS2
            float2 frag(v2f i) : COLOR
            {
                float2 motion = tex2D(_CameraMotionVectorsTexture, i.uv).rg;
                return motion;
            }
#endif
#if CHANNELS3
            float3 frag(v2f i) : COLOR
            {
                float2 motion = tex2D(_CameraMotionVectorsTexture, i.uv).rg;
                return float3(motion, 0);
            }
#endif
#if CHANNELS4
            float4 frag(v2f i) : COLOR
            {
                float2 motion = tex2D(_CameraMotionVectorsTexture, i.uv).rg;
                return float4(motion, 0, 1);
            }
#endif
            ENDCG
        }
    }
}