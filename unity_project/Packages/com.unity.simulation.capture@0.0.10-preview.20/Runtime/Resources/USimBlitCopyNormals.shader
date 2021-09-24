Shader "usim/BlitCopyNormals"
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

            sampler2D _CameraDepthNormalsTexture;
            float4 _CameraDepthNormalsTexture_ST;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord.xy, _CameraDepthNormalsTexture);
                return o;
            }

#if CHANNELS1
            float frag(v2f i) : COLOR
            {
                float3 normal;
                float  depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
                return normal.x;
            }
#endif
#if CHANNELS2
            float2 frag(v2f i) : COLOR
            {
                float3 normal;
                float  depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
                return normal.xy;
            }
#endif
#if CHANNELS3
            float3 frag(v2f i) : COLOR
            {
                float3 normal;
                float  depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
                return normal.xyz;
            }
#endif
#if CHANNELS4
            float4 frag(v2f i) : COLOR
            {
                float3 normal;
                float  depth;
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
                return float4(normal, 1);
            }
#endif
            ENDCG
        }
    }
}