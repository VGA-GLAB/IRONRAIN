Shader "Custom/3DGround"
{
    Properties
    {
        [SingleLineTexture]_ModelNormalTex("Model NormalMap", 2D) = "bump" {}
        [KeywordEnum(Planar, Triplanar)] _Mapping("MappingMode", Int) = 0
        [KeywordEnum(Vertex, Fragment)] _Bias("BiasCulcStage", Int) = 0
        _PlanarWeight ("Weight", Range(0, 120)) = 0.5
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

            #include "../Curve/VertexCurve.hlsl"
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            // Unity keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            #pragma multi_compile _MAPPING_PLANAR _MAPPING_TRIPLANAR
            #pragma multi_compile _BIAS_VERTEX _BIAS_FRAGMENT
            #pragma multi_compile _CURVE_TYPE_NONE _CURVE_TYPE_CAMERA_FORWARD _CURVE_TYPE_CAMERA_DISTANCE _CURVE_TYPE_WORLD_FORWARD

            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float2 lightmapUv : TEXCOORD1;

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
                
                #if defined(_BIAS_VERTEX)
                float3 normalBias : TEXCOORD7;
                #endif
                #if defined(_MAPPING_PLANAR)
                float2 planarUv : TEXCOORD8;
                #else
                float2 triplanarUvX : TEXCOORD8;
                float2 triplanarUvY : TEXCOORD9;
                float2 triplanarUvZ : TEXCOORD10;
                #endif

                DECLARE_LIGHTMAP_OR_SH(lightmapUv, vertexSH, 11);
                
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

            float _CurveFactor;
            float _CurveOffset;
            float _CurveStrength;
            CBUFFER_END

            float3 CulcBias(float3 absNormal, float weight)
            {
                float4 temp = lerp(
                    float4(1, 0, 0, absNormal.x),
                    float4(0, 1, 0, absNormal.y),
                    step(absNormal.x, absNormal.y));

                temp = lerp(temp, float4(0, 0, 1, 0), step(temp.w, absNormal.z));

                return normalize(lerp(absNormal, temp.xyz, weight));
            }

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs curvedVertexInput;
                CalcVertexCurve(_CurveFactor, _CurveOffset, _CurveStrength,
                    GetVertexPositionInputs(input.positionOS.xyz), curvedVertexInput);
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                float3 viewDirWS = GetWorldSpaceViewDir(curvedVertexInput.positionWS);
                output.vertexLight = VertexLighting(curvedVertexInput.positionWS, normalInput.normalWS);
                output.fogFactor = ComputeFogFactor(curvedVertexInput.positionCS.z);

                output.positionHCS = curvedVertexInput.positionCS;
                output.positionWS = curvedVertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;

                OUTPUT_LIGHTMAP_UV(input.lightmapUv, unity_LightmapST, output.lightmapUv);
                OUTPUT_SH(output.normalWS, output.vertexSH);

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.shadowCoord = GetShadowCoord(curvedVertexInput);

                #if defined(_BIAS_VERTEX)
                output.normalBias = pow(abs(output.normalWS), _PlanarWeight);
                output.normalBias = output.normalBias / (output.normalBias.x + output.normalBias.y + output.normalBias.z);
                #endif
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
                
                #if defined(_BIAS_VERTEX)
                float3 normalBias = input.normalBias;
                #else
                float3 normalBias = pow(abs(input.normalWS), _PlanarWeight);
                normalBias = normalBias / (normalBias.x + normalBias.y + normalBias.z);
                #endif
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
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUv);
                inputData.vertexLighting = input.vertexLight;
                inputData.bakedGI = SAMPLE_GI(input.lightmapUv, input.vertexSH, input.normalWS);
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
