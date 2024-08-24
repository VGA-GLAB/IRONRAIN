#ifndef CUSTOM_VERTEX_CURVE_INCLUDED
#define CUSTOM_VERTEX_CURVE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#define VERTEX_CURVE_UNIFORM\
    float _CurveFactor;\
    float _CurveOffset;\
    float _CurveStrength;\
    float _CurveHeightOffset;

inline void CalcVertexCurve(float factor, float offset, float strength, float heightOffset, VertexPositionInputs vertPosInput, out VertexPositionInputs curvedVertexInput)
{
    float3 curvedPosWS = vertPosInput.positionWS;
    float distance = 0;
    
    // #if defined(_CURVE_TYPE_CAMERA_FORWARD)
    // distance = pow(vertPosInput.positionVS.z - offset, factor);
    // #elif defined(_CURVE_TYPE_CAMERA_DISTANCE)
    // distance = pow(abs(length(vertPosInput.positionWS - _WorldSpaceCameraPos) - offset), factor);
    //
    // #elif defined(_CURVE_TYPE_WORLD_FORWARD)
    // distance = pow(vertPosInput.positionWS.z + offset, factor);
    // #endif

    distance = pow(length(vertPosInput.positionWS.z - _WorldSpaceCameraPos.z + offset), factor);

    curvedPosWS.y -= distance * strength;
    curvedPosWS.y += heightOffset;

    curvedVertexInput = GetVertexPositionInputs(TransformWorldToObject(curvedPosWS).xyz);
}

#endif