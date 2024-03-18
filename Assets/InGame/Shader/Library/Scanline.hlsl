#ifndef CUSTOM_SCANLINE_LIBRARY_INCLUDED
#define CUSTOM_SCANLINE_LIBRARY_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float StepScanline(float target, float speed, float distance, float width)
{
    target = target + _Time.x * speed;
    target = (sin(target * TWO_PI * distance) + 1) / 2;

    return step(target, width);
}

float Scanline(float target, float speed, float distance, float width)
{
    target = target + _Time.x * speed;
    target = (sin(target * TWO_PI * distance) + 1) / 2;

    return pow(target, width);
}

#endif