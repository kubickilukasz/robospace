#ifndef BACON_FUNC
#define BACON_FUNC

float remap(float v, float minOld, float maxOld, float minNew, float maxNew)
{
    return minNew + (v - minOld) * (maxNew - minNew) / (maxOld - minOld);
}

float getFrag(float3 dirLight, float3 viewDir, float3 dirNormal, float4 intensity, float _StepsStart, float _Steps)
{
    float dotProduct = 1 - dot(dirLight, dirNormal);
    float _step = 1 - _Steps;
    float currentStep = 1;
    float finalCol = 0;
    finalCol += (intensity * step(_StepsStart, dotProduct)) * 0.25;
    finalCol += (intensity * step(_StepsStart += _step, dotProduct)) * 0.25;
    finalCol += (intensity * step(_StepsStart += _step, dotProduct)) * 0.25;
    finalCol += (intensity * step(_StepsStart += _step, dotProduct)) * 0.25;
    return finalCol;
}

float getPointFrag(float3 posLight, float3 fragPos, float radius, float3 viewDir, float3 dirNormal, float4 intensity, float _StepsStart, float _Steps)
{
    float3 vec3 = fragPos - posLight;
    float dis = length(vec3);
    if (dis > radius)
    {
        return 0;
    }
    float ratio = 1 - (dis * dis) / (radius * radius);
    float temp = 0;
    intensity *= (step(0.1, ratio) * 0.5) + (step(0.3, ratio) * 0.25) + (step(0.5, ratio) * 0.1);
    return getFrag(vec3, viewDir, dirNormal, intensity * ratio, _StepsStart, _Steps);
}


uniform float3 _MainLightDirection;
uniform float4 _MainLightColor;
uniform float _MainLightIntensity;

uniform int _PointLightNumner;

uniform float4 _PointLightColor[64];
uniform float _PointLightIntensity[64];
uniform float3 _PointLightPosition[64];
uniform float _PointLightRadius[64];


void getBacon_float(float4 baseColor, float3 _viewDir, float3 _normal, float3 worldPos, float _StepsStart, float _Steps, out float4 _color)
{
    _color = float4(0, 0, 0, 0);
    float4 tex = baseColor;
    float3 viewDir = normalize(float3(_viewDir));
    float3 normal = float3(_normal);
    float mainBInt = getFrag(_MainLightDirection, viewDir, normal, _MainLightIntensity, _StepsStart, _Steps);
    float4 mainBColor = float4(_MainLightColor) * mainBInt;
    float4 mixedPColor = float4(0, 0, 0, 1);
    float mixedPInt = 0;
    for (int j = 0; j < _PointLightNumner; j++)
    {
        float current = getPointFrag(_PointLightPosition[j], worldPos, _PointLightRadius[j], viewDir, normal, _PointLightIntensity[j], _StepsStart, _Steps);
        mixedPInt += current;
        mixedPColor += _PointLightColor[j] * current;
    }
    float sum = mixedPInt + mainBInt;
    if (sum > 0)
    {
        tex *= (mixedPColor * mixedPInt / sum) + (mainBColor * mainBInt / sum);
    }
    else
    {
        return;
    }
    
    float outlineT = (saturate(dot(viewDir, normal))) * 2;
    outlineT = step(0.1, outlineT);
    float4 outline = float4(outlineT, outlineT, outlineT, 1.0);
    _color = saturate(tex * outline);
    return;
    
}

#endif