#ifndef CUSTOM_CURVE_TERRAIN_HEADER_INCLUDED
#define CUSTOM_CURVE_TERRAIN_HEADER_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "../VertexCurve.hlsl"

// Attribute Define
#define ATTRIBUTES_POSITION_OS_ON

#define ATTRIBUTES_UV0_ON

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define ATTRIBUTES_UV1_ON
#endif

#define ATTRIBUTES_NORMAL_OS_ON

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define ATTRIBUTES_COLOR_ON
#endif

// Varyings Define
#define VARYINGS_TEX_COORD0_ON

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_DECLARE_LIGHTMAP_OR_SH_ON
#endif

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_COLOR_ON
#endif

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_LIGHTING_FOG_ON
#endif

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_SHADOW_COORDS_ON
#endif

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_NORMAL_WS_ON
#endif

#if defined(CUSTOM_CURVE_TERRAIN_FORWARD_PASS)
#define VARYINGS_POSITION_WS_ON
#endif

#define VARYINGS_POSITION_CS_ON

struct Attributes
{
    #if defined(ATTRIBUTES_POSITION_OS_ON)
    float4 PositionOS : POSITION;
    #endif
    #if defined(ATTRIBUTES_UV0_ON)
    float2 UV0 : TEXCOORD0;
    #endif
    #if defined(ATTRIBUTES_UV1_ON)
    float2 UV1 : TEXCOORD1;
    #endif
    #if defined(ATTRIBUTES_NORMAL_OS_ON)
    float3 NormalOS : NORMAL;
    #endif
    #if defined(ATTRIBUTES_COLOR_ON)
    half4 Color : COLOR;
    #endif
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    #if defined(VARYINGS_TEX_COORD0_ON)
    float2 UV01 : TEXCOORD0;
    #endif
    #if defined(VARYINGS_DECLARE_LIGHTMAP_OR_SH_ON)
    DECLARE_LIGHTMAP_OR_SH(staticLightMapUV, vertexSH, 1);
    #endif
    #if defined(VARYINGS_COLOR_ON)
    half4 Color : TEXCOORD2;
    #endif
    #if defined(VARYINGS_LIGHTING_FOG_ON)
    half4 LightingFog : TEXCOORD3;
    #endif
    #if defined(VARYINGS_SHADOW_COORDS_ON)
    float4 ShadowCoords : TEXCOORD4;
    #endif
    #if defined(VARYINGS_NORMAL_WS_ON)
    half4 NormalWS : TEXCOORD5;
    #endif
    #if defined(VARYINGS_POSITION_WS_ON)
    float3 PositionWS : TEXCOORD6;
    #endif
    #if defined(VARYINGS_POSITION_CS_ON)
    float4 PositionCS : SV_POSITION;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

CBUFFER_START(UnityPerMaterial)

float4 _MainTex_ST;

VERTEX_CURVE_UNIFORM

CBUFFER_END

#endif