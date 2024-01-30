Shader "Custom/3DHologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [HDR] _HoloColor ("Color", Color) = (0, 1, 0, 1)
        
        [Header(Rim)]
        _RimPower ("Power", Range(0, 3)) = 1
        [Header(ScanLine)]
        _ScanDistance ("Distance", Float) = 1
        _ScanWidth("Width", Range(0, 1)) = 0.2
        _ScanSpeed("Scroll Speed", Float) = 2.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D_X(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            
            float4 _MainTex_ST;
            half4 _HoloColor;
            
            float _RimPower;

            float _ScanDistance;
            float _ScanWidth;
            float _ScanSpeed;
            
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = UnityStereoTransformScreenSpaceTex(input.uv);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                half4 col = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, input.uv) * _HoloColor;

                // // Rim
                float3 normal = input.normalWS;
                float3 view = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float NdotV = dot(normal, view);
                
                col *= pow(1 - saturate(NdotV), _RimPower);

                // Scanline
                float height = input.positionWS.y + _Time.x * _ScanSpeed;

                height = (sin(height * TWO_PI * _ScanDistance) + 1) / 2;
                
                col *= step(height, _ScanWidth);

                col.rgb *= col.a;
                
                return col;
            }
            ENDHLSL
        }
    }
}
