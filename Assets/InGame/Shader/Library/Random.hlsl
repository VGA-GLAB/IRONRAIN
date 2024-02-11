#ifndef CUSTOM_RANDOM_INCLUDED
#define CUSTOM_RANDOM_INCLUDED

float rand(float2 co)
{
    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

            
float perlinNoise(float2 st)
{
    float2 p = floor(st);
    float2 f = frac(st);
    float2 u = f * f * (3.0 - 2.0 * f);

    float v00 = rand(p + float2(0, 0));
    float v10 = rand(p + float2(1, 0));
    float v01 = rand(p + float2(0, 1));
    float v11 = rand(p + float2(1, 1));

    return lerp(lerp(dot(v00, f - float2(0, 0)), dot(v10, f - float2(1, 0)), u.x),
                lerp(dot(v01, f - float2(0, 1)), dot(v11, f - float2(1, 1)), u.x),
                u.y) + 0.5f;
}

#endif