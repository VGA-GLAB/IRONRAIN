Shader "Custom/2DMonitor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OpenEyesAmount ("OpenEyesAmount", Range(0, 1)) = 0
        _OpenEyesOffset ("OpenEyesOffset", Range(-1, 1)) = 0
        _CloseEyesColor ("CloseEyesColor", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                half fogFactor : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            
            float4 _MainTex_ST;
            float _OpenEyesAmount;
            float _OpenEyesOffset;
            half4 _CloseEyesColor;

            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fogFactor = ComputeFogFactor(o.positionCS.z);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv);

                col.rgb = MixFog(col.rgb, i.fogFactor);

                float v = abs(i.uv.y * 2 + _OpenEyesOffset - 1);
                
                col = lerp(col, _CloseEyesColor, step(_OpenEyesAmount * 2, v));
                
                return col;
            }
            ENDHLSL
        }
    }
}