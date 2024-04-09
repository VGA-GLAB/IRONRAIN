#ifndef CUSTOM_GLITCH_INCLUDED
#define CUSTOM_GLITCH_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Random.hlsl"

float2 Glitch(float2 xy, float flameRate, float frequency, float strength)
{
    float seed1 = floor(frac(perlinNoise(_SinTime.xy) * 10) / (1 / flameRate)) * (1 / flameRate);
    float seed2 = floor(frac(perlinNoise(_SinTime.xy) * 5) / (1 / flameRate)) * (1 / flameRate);

    float noiseX = (2.0 * rand(seed1) - 1.0F) * strength;

    frequency = step(rand(seed2), frequency);
    noiseX *= frequency;

    float noiseY = 2.0 * rand(seed1) - 0.5;

    float glitchLine1 = step(frac(xy.y) - noiseY, rand(noiseY));
    float glitchLine2 = step(frac(xy.y) + noiseY, noiseY);
    float glitch = saturate(glitchLine1 - glitchLine2);

    xy.x = lerp(xy.x, xy.x + noiseX, glitch);

    return xy;
}

#endif