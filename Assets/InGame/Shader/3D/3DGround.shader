Shader "Custom/3DGround"
{
    Properties
    {
        [SingleLineTexture]_ModelNormalTex("Model NormalMap", 2D) = "bump" {}
        [KeywordEnum(Planar, Triplanar)] _Mapping("MappingMode", Int) = 0
        _PlanarWeight ("Weight", Range(1, 64)) = 1
        _PlanarScaleOffset ("Planar Mapping Scale Offset", Vector) = (1, 1, 0, 0)
        [Space(10)]
        
        [Header(Planar)]
        [SingleLineTexture]_MainTex ("Texture", 2D) = "white" {}
        [SingleLineTexture]_NormalTex ("NormalMap", 2D) = "bump" {}
        [Space(10)]
        
        [Header(TriPlanar)]
        [SingleLineTexture]_TriTexX ("TriplanarTex X", 2D) = "white" {}
        [SingleLineTexture]_TriNormalTexX ("Tri NormalTex X", 2D) = "bump" {}
        [SingleLineTexture]_TriTexY ("TriplanarTex Y", 2D) = "white" {}
        [SingleLineTexture]_TriNormalTexY ("Tri NormalTex Y", 2D) = "bump" {}
        [SingleLineTexture]_TriTexZ ("TriplanarTex Z", 2D) = "white" {}
        [SingleLineTexture]_TriNormalTexZ ("Tri NormalTex Z", 2D) = "bump" {}
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
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #pragma multi_compile _MAPPING_PLANAR _MAPPING_TRIPLANAR
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 tangentWS : TANGENT;
                float3 bitangentWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                half fogFactor : TEXCOORD5;
                float3 vertexLight : TEXCOORD6;
                float3 normalBias : TEXCOORD7;

                #if defined(_MAPPING_PLANAR)
                float2 planarUv : TEXCOORD8;
                #else
                float2 triplanarUvX : TEXCOORD8;
                float2 triplanarUvY : TEXCOORD9;
                float2 triplanarUvZ : TEXCOORD10;
                #endif


                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalTex);
            SAMPLER(sampler_NormalTex);

            TEXTURE2D(_ModelNormalTex);
            SAMPLER(sampler_ModelNormalTex);

            TEXTURE2D(_TriTexX);
            SAMPLER(sampler_TriTexX);
            TEXTURE2D(_TriNormalTexX);
            SAMPLER(sampler_TriNormalTexX);
            TEXTURE2D(_TriTexY);
            SAMPLER(sampler_TriTexY);
            TEXTURE2D(_TriNormalTexY);
            SAMPLER(sampler_TriNormalTexY);
            TEXTURE2D(_TriTexZ);
            SAMPLER(sampler_TriTexZ);
            TEXTURE2D(_TriNormalTexZ);
            SAMPLER(sampler_TriNormalTexZ);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _PlanarScaleOffset;

            float _PlanarWeight;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                float3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                output.positionHCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.shadowCoord = GetShadowCoord(vertexInput);

                output.normalBias = pow(abs(output.normalWS), _PlanarWeight);
                output.normalBias = output.normalBias / (output.normalBias.x + output.normalBias.y + output.normalBias.z);

                #if defined(_MAPPING_PLANAR)

                output.planarUv = output.positionWS.xz * _PlanarScaleOffset.xy + _PlanarScaleOffset.zw;

                #else

                output.triplanarUvX = output.positionWS.yz * _PlanarScaleOffset.xy + _PlanarScaleOffset.zw;
                output.triplanarUvY = output.positionWS.xz * _PlanarScaleOffset.xy + _PlanarScaleOffset.zw;
                output.triplanarUvZ = output.positionWS.xy * _PlanarScaleOffset.xy + _PlanarScaleOffset.zw;
                
                #endif 

                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float3 normalBias = input.normalBias;
                float3 modelNormal = UnpackNormal(SAMPLE_TEXTURE2D(_ModelNormalTex, sampler_ModelNormalTex, input.uv));
                modelNormal = input.tangentWS * modelNormal.x + input.bitangentWS * modelNormal.y + input.normalWS * modelNormal.z;
                
            #if defined(_MAPPING_PLANAR)
                
                input.planarUv = frac(input.planarUv);
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.planarUv);
                
                float3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, input.planarUv));
                float3 normalMapNormalWS = input.tangentWS * normalMap.x + input.bitangentWS * normalMap.y + input.normalWS * normalMap.z;
                

            #elif defined(_MAPPING_TRIPLANAR)
                
                // input.triplanarUvX = frac(input.triplanarUvX);
                // input.triplanarUvY = frac(input.triplanarUvY);
                // input.triplanarUvZ = frac(input.triplanarUvZ);

                half4 colX = SAMPLE_TEXTURE2D(_TriTexX, sampler_TriTexX, input.triplanarUvX);
                half4 colY = SAMPLE_TEXTURE2D(_TriTexY, sampler_TriTexY, input.triplanarUvY);
                half4 colZ = SAMPLE_TEXTURE2D(_TriTexZ, sampler_TriTexZ, input.triplanarUvZ);
                
                half4 col = colX * normalBias.x + colY * normalBias.y + colZ * normalBias.z;
                
                float3 normalX = UnpackNormal(SAMPLE_TEXTURE2D(_TriNormalTexX, sampler_TriNormalTexX, input.triplanarUvX));
                float3 normalY = UnpackNormal(SAMPLE_TEXTURE2D(_TriNormalTexY, sampler_TriNormalTexY, input.triplanarUvY));
                float3 normalZ = UnpackNormal(SAMPLE_TEXTURE2D(_TriNormalTexZ, sampler_TriNormalTexZ, input.triplanarUvZ));

                float3 normalMap = normalX * normalBias.x + normalY * normalBias.y + normalZ * normalBias.z;
                float3 normalMapNormalWS = input.tangentWS * normalMap.x + input.bitangentWS * normalMap.y + input.normalWS * normalMap.z;
            #endif
                
                normalMapNormalWS = normalize(normalMapNormalWS + modelNormal);

                SurfaceData surfaceData = (SurfaceData)0;
                
                
                surfaceData.albedo = col.rgb;
                surfaceData.alpha = col.a;
                surfaceData.emission = 0;
                surfaceData.metallic = 0.1;
                surfaceData.occlusion = 0.5;
                surfaceData.smoothness = 0.1;
                surfaceData.specular = 0.5;
                surfaceData.normalTS = modelNormal;

                InputData inputData = (InputData)0;
                inputData.fogCoord = input.fogFactor;
                inputData.shadowCoord = input.shadowCoord;
                inputData.shadowMask = 0;
                inputData.vertexLighting = input.vertexLight;
                inputData.bakedGI = 0.5;
                inputData.normalWS = normalMapNormalWS;
                inputData.positionCS = input.positionHCS;
                inputData.tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);

                col = UniversalFragmentPBR(inputData, surfaceData);
                
                return col;
            }
            ENDHLSL
        }
        
         Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster


            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };

            VertexOutput vertShadowCaster(VertexInput v) {

                VertexOutput o = (VertexOutput)0;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }

            float4 fragShadowCaster(VertexOutput i) : SV_TARGET {
                return 0;
            }


            ENDHLSL

        }
    }
}
