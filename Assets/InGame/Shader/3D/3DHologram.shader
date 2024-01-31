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
        
        [Header(Glitch)]
        _FrameRate ("FrameRate", Range(0.1, 30)) = 15
        _Frequency ("Frequency", Range(0, 1)) = 0.1
        _GlitchStrength ("Strengh", Range(0, 1)) = 0.1
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
            Cull Off
            ZWrite On
            ZTest Less
            
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

            float _FrameRate;
            float _Frequency;
            float _GlitchStrength;
            
            CBUFFER_END

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            
            float perlinNoise(float2 st)
            {
                float2 p = floor(st);
                float2 f = frac(st);
                float2 u = f * f * (3.0 - 2.0 * f);

                float v00 = rand(p + float2(0, 0));
                float v10 = rand(p + float2(1, 0));
                float v01 = rand(p + float2(0, 1));
                float v11 = rand(p + float2(1, 1));

                return lerp(lerp(dot(v00, f - float2(0, 0)), dot(v10, f - float2(1, 0)), u.x),
                            lerp(dot(v01, f - float2(0, 1)), dot(v11, f - float2(1, 1)), u.x),
                            u.y) + 0.5f;
            }


            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
                float3 positionVS = TransformWorldToView(output.positionWS);


                // vertex Glitch
                float seed1 = floor(frac(perlinNoise(_SinTime.xy) * 10) / (1 / _FrameRate)) * (1 / _FrameRate);
                float seed2 = floor(frac(perlinNoise(_SinTime.xy) * 5) / (1 / _FrameRate)) * (1 / _FrameRate);
                
                float noiseX = (2.0 * rand(seed1) - 1.0F) * _GlitchStrength;

                float frequency = step(rand(seed2), _Frequency);
                noiseX *= frequency;

                float noiseY = 2.0 * rand(seed1) - 0.5;

                float glitchLine1 = step(frac(positionVS.y) - noiseY, rand(noiseY));
                float glitchLine2 = step(frac(positionVS.y) + noiseY, noiseY);
                float glitch = saturate(glitchLine1 - glitchLine2);

                positionVS.x = lerp(positionVS.x, positionVS.x + noiseX, glitch);

                output.positionHCS = TransformWViewToHClip(positionVS);
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
                
                col *= pow(1 - abs(NdotV), _RimPower);

                // Scanline
                float height = input.positionWS.y + _Time.x * _ScanSpeed;

                height = (sin(height * TWO_PI * _ScanDistance) + 1) / 2;
                
                col *= step(height, _ScanWidth);

                clip(col.a - 0.01);
                
                col.rgb *= col.a;
                
                return col;
            }
            ENDHLSL
        }
    }
}
