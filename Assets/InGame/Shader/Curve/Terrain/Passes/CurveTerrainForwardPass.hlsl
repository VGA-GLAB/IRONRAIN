#ifndef CUSTOM_CURVE_TERRAIN_FORWARD_PASS_INCLUDED
#define CUSTOM_CURVE_TERRAIN_FORWARD_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "../Header.hlsl"
#include "../Lighting.hlsl"
#include "../../VertexCurve.hlsl"

Varyings CurveTerrainForwardVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.UV01 = TRANSFORM_TEX(input.UV0, _MainTex);
    OUTPUT_LIGHTMAP_UV(input.UV1, unity_LightmapST, output.staticLightmapUV);
    const VertexPositionInputs vertexInput = GetVertexPositionInputs(input.PositionOS.xyz);

    VertexPositionInputs curvedVertexInput;
    
    CalcVertexCurve(_CurveFactor, _CurveOffset, _CurveStrength, _CurveHeightOffset, vertexInput, curvedVertexInput);
    
    output.Color = input.Color;
    output.PositionCS = curvedVertexInput.positionCS;

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.ShadowCoords = GetShadowCoord(vertexInput);
    #endif

    // Vertex Lighting
    half3 normalWS = input.NormalOS;
    OUTPUT_SH(normalWS, output.vertexSH);
    Light mainLight = GetMainLight();
    half3 attenuatedLightColor = mainLight.color * mainLight.distanceAttenuation;
    half3 diffuseColor = half3(0, 0, 0);

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_MAIN_LIGHT))
    {
        diffuseColor += LightingLambert(attenuatedLightColor, mainLight.direction, normalWS);
    }

    #if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_ADDITIONAL_LIGHTS))
    {
        int pixelLightCount = GetAdditionalLightsCount();
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, curvedVertexInput.positionWS);
            half3 attenuatedLightColor = light.color * light.distanceAttenuation;
            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, normalWS);
        }
    }
    #endif

    output.LightingFog.xyz = diffuseColor;

    // Fog factor
    output.LightingFog.w = ComputeFogFactor(output.PositionCS.z);

    output.NormalWS.xyz = normalWS;
    output.PositionWS = curvedVertexInput.positionWS;
    
    return output;
}

half4 TerrainLitForwardFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    InputData inputData;
    InitializeInputData(input, inputData);
    half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.UV01);
    half4 color = UniversalTerrainLit(inputData, tex.rgb, tex.a);

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}

#endif