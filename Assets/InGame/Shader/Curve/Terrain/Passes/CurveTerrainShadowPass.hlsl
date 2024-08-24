#ifndef CUSTOM_CURVE_TERRAIN_SHADOW_CASTER_PASS_INCLUDED
#define CUSTOM_CURVE_TERRAIN_SHADOW_CASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "../Header.hlsl"
#include "../../VertexCurve.hlsl"

float3 _LightDirection;
float3 _LightPosition;

void TerrainInstancing(inout float4 positionOS, inout float3 normal, inout float2 uv)
{
    #ifdef UNITY_INSTANCING_ENABLED
    float2 patchVertex = positionOS.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    float height = UnpackHeightmap(_TerrainHeightmapTexture.Load(int3(sampleCoords, 0)));

    positionOS.xz = sampleCoords * _TerrainHeightmapScale.xz;
    positionOS.y = height * _TerrainHeightmapScale.y;

    #ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
    normal = float3(0, 1, 0);
    #else
    normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb * 2 - 1;
    #endif
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;
    #endif
}

Varyings CurveTerrainShadowCasterVertex(Attributes input)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    TerrainInstancing(input.PositionOS, input.NormalOS, input.UV0);

    VertexPositionInputs curvedPositionInput;
    CalcVertexCurve(_CurveFactor, _CurveOffset, _CurveStrength, _CurveHeightOffset,GetVertexPositionInputs(input.PositionOS), curvedPositionInput);
    
    float3 positionWS = curvedPositionInput.positionWS;
    float3 normalWS = TransformObjectToWorldNormal(input.NormalOS);

    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
    #else
    float3 lightDirectionWS = _LightDirection;
    #endif

    float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

    #if UNITY_REVERSED_Z
    clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
    #else
    clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
    #endif

    o.PositionCS = clipPos;

    o.UV01 = input.UV0;

    return o;
}

half4 TerrainLitShadowCasterFragment(Varyings input) : SV_Target
{
    #ifdef _ALPHATEST_ON
    ClipHoles(IN.texcoord);
    #endif
    return 0;
}

#endif