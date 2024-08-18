#ifndef CUSTOM_VERTEX_CURVE_INCLUDED
#define CUSTOM_VERTEX_CURVE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#define VERTEX_CURVE_UNIFORM\
    float _CurveFactor;\
    float _CurveOffset;\
    float _CurveStrength;

inline void CalcVertexCurve(float factor, float offset, float strength, VertexPositionInputs vertPosInput, out VertexPositionInputs curvedVertexInput)
{
    float3 curvedPosWS = vertPosInput.positionWS;
    float distance = 0;
    
    #if defined(_CURVE_TYPE_CAMERA_FORWARD)
    distance = pow(vertPosInput.positionVS.z - offset, factor);
    #elif defined(_CURVE_TYPE_CAMERA_DISTANCE)
    distance = pow(abs(length(vertPosInput.positionWS - _WorldSpaceCameraPos) - offset), factor);
    #elif defined(_CURVE_TYPE_WORLD_FORWARD)
    distance = pow(vertPosInput.positionWS.z - _WorldSpaceCameraPos.z + offset, factor);
    #endif

    curvedPosWS.y -= distance * strength;

    curvedVertexInput = GetVertexPositionInputs(TransformWorldToObject(curvedPosWS).xyz);
}

#endif