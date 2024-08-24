#ifndef CUSTOM_CURVE_TERRAIN_LIGHTING_INCLUDED
#define CUSTOM_CURVE_TERRAIN_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Header.hlsl"

void InitializeInputData(Varyings input, out InputData inputData)
{
    inputData = (InputData)0;

    inputData.positionCS = input.PositionCS;
    inputData.normalWS = half3(0, 1, 0);
    inputData.viewDirectionWS = half3(0, 0, 1);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.ShadowCoords;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(input.PositionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = input.LightingFog.a;
    inputData.vertexLighting = input.LightingFog.rgb;
    //inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, input.NormalWS.xyz);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.PositionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
    inputData.positionWS = input.PositionWS;

    #if defined(DEBUG_DISPLAY)
    inputData.uv = input.UV01;
    #endif
}

void InitializeSurfaceData(half3 albedo, half alpha, out SurfaceData surfaceData)
{
    surfaceData = (SurfaceData)0;

    surfaceData.albedo = albedo;
    surfaceData.alpha = alpha;
    surfaceData.emission = half3(0, 0, 0);
    surfaceData.metallic = 0;
    surfaceData.occlusion = 0;
    surfaceData.smoothness = 1;
    surfaceData.specular = half3(0, 0, 0);
    surfaceData.clearCoatMask = 0;
    surfaceData.clearCoatSmoothness = 1;
    surfaceData.normalTS = half3(0, 0, 1);
}

half4 UniversalTerrainLit(InputData inputData, SurfaceData surfaceData)
{
    #if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, debugColor))
    {
        return debugColor;
    }
    #endif

    #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    half3 lighting = inputData.vertexLighting * MainLightRealtimeShadow(inputData.shadowCoord);
    #else
    half3 lighting = inputData.vertexLighting;
    #endif
    half4 color = half4(surfaceData.albedo, surfaceData.alpha);

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_GLOBAL_ILLUMINATION))
    {
        lighting += inputData.bakedGI;
    }

    color.rgb *= lighting;

    return color;
}

half4 UniversalTerrainLit(InputData inputData, half3 albedo, half alpha)
{
    SurfaceData surfaceData;
    InitializeSurfaceData(albedo, alpha, surfaceData);

    return UniversalTerrainLit(inputData, surfaceData);
}

#endif