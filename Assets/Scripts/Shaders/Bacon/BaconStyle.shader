// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BaconStyle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (0, 0, 0, 0)
        _ShadowValue("Shadow Value", Range(0,1)) = 0.5
        //_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0)
        _Steps ("Steps", Range(-1,2)) = 0.25
        _StepsStart ("Steps Start", Range(-1,2)) = 0.8
        _EmissionTex ("Emission Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)
        _SpecularIntensity("Specular Intensity", Range(0,1)) = 0.5
        _SpecularPower("Specular Power", Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            float _Steps;
            float _StepsStart;
            float4 _MainColor;
            
            float _ShadowValue;
            float4 _EmissionColor;
            float _SpecularIntensity;
            float _SpecularPower;

            uniform float3 _MainLightDirection;
            uniform half4  _BaconMainLightColor;
            uniform float _MainLightIntensity;
            uniform float4 _ShadowColor;

            uniform int _PointLightNumber;
            uniform float4 _PointLightColor[64];
            uniform float _PointLightIntensity[64];
            uniform float3 _PointLightPosition[64];
            uniform float _PointLightRadius[64];

            uniform int _SpotLightNumber;
            uniform float4 _SpotLightColor[32];
            uniform float _SpotLightIntensity[32];
            uniform float3 _SpotLightPosition[32];
            uniform float _SpotLightRadius[32];
            uniform float _SpotLightAngle[32];
            uniform float3 _SpotLightDir[32];


            float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
                return minNew + (v-minOld) * (maxNew - minNew) / (maxOld-minOld);
            }

            float getFrag (float3 dirLight, float3 viewDir, float3 dirNormal, float4 intensity){
                float dotProduct = 1 - dot(dirLight, dirNormal);

                float _step = 1 - _Steps;
                float currentStep = _StepsStart;
                float finalCol = 0;

                finalCol += (intensity * step(currentStep, dotProduct)) * 0.25;
                currentStep += _step;
                finalCol += (intensity * step(currentStep, dotProduct)) * 0.25;
                currentStep += _step;
                finalCol += (intensity * step(currentStep, dotProduct)) * 0.25;
                currentStep += _step;
                finalCol += (intensity * step(currentStep, dotProduct)) * 0.25;
                return finalCol;
            }

            float getPointFrag (float3 posLight, float3 fragPos, float radius, float3 viewDir, float3 dirNormal, float4 intensity){
                float3 vec3 = fragPos - posLight;
                float dis = length(vec3);
                if(dis > radius){
                    return 0;
                }
                float ratio = 1 - (dis * dis)/(radius * radius);
                float temp = 0;
                intensity *= (step(0.1, ratio) * 0.5) + (step(0.3, ratio) * 0.25) + (step(0.5, ratio) * 0.1);
                return getFrag(vec3, viewDir, dirNormal, intensity * ratio);
            }

            float getSpotFrag (float3 posLight, float3 fragPos, float radius, float3 viewDir, float3 dirNormal, float4 intensity, float angle, float3 dirLight){
                float3 vec3 = fragPos - posLight;
                float dis = length(vec3);
                if(dis > radius){
                    return 0;
                }
                vec3 = normalize(vec3);
                float dot3 = dot(dirLight, vec3);
                dot3 -= angle;
                dot3 = saturate(dot3);
                dot3 = remap(dot3, 0, 1 - angle, 0, 1);

                float ratio = dot3 * (1 - (dis / radius));
                float temp = 0;
                intensity *= (step(0.1, ratio) * 0.55) + (step(0.3, ratio) * 0.3) + (step(0.5, ratio) * 0.15);
                return getFrag(dirLight, viewDir, dirNormal, intensity * ratio);
            }

            float getSpecular(float3 lightDir, float3 viewDir, float3 dirNormal, float intensity, float power){
                float3 V = viewDir;
                float3 R = reflect(normalize(lightDir), normalize(dirNormal));
                return intensity * pow( saturate( dot( R, V ) ), power);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                //half3 viewDir : POSITION1;
                half3 viewDir : NORMAL2;
                half3 normal : NORMAL;
                half3 normalLocal : NORMAL1;
                //float2 depth : TEXCOORD2;
                float3 worldPos : POSITION2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _EmissionTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalLocal = v.normal;
                float3 worldVert = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldVert;
                o.normal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.viewDir = _WorldSpaceCameraPos - worldVert;  
                //UNITY_TRANSFER_FOG(o,o.vertex);
                //UNITY_TRANSFER_DEPTH(o.depth);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(float3(i.viewDir));
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                //UNITY_OUTPUT_DEPTH(i.depth);
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                float4 tex = tex2D(_MainTex, i.uv) * _MainColor;
                float3 normal = float3(i.normal);
                float mainBInt = getFrag(_MainLightDirection, viewDir, normal, _MainLightIntensity);
                float4 mainBColor = float4(_BaconMainLightColor) * mainBInt;
                float4 mixedPColor = float4(0,0,0,1);
                float mixedPInt = 0;
                float specular = getSpecular(_MainLightDirection, viewDir, normal, _SpecularIntensity, _SpecularPower);
                for(int j = 0; j < _PointLightNumber; j++){
                    float current = getPointFrag(_PointLightPosition[j], i.worldPos, _PointLightRadius[j], viewDir, normal, _PointLightIntensity[j]);
                    mixedPInt += current;
                    mixedPColor += _PointLightColor[j] * current;
                }
                for(int j = 0; j < _SpotLightNumber; j++){
                    float current = getSpotFrag(_SpotLightPosition[j], i.worldPos, _SpotLightRadius[j], viewDir, normal, _SpotLightIntensity[j], _SpotLightAngle[j], _SpotLightDir[j]);
                    mixedPInt += current;
                    mixedPColor += _SpotLightColor[j] * current;
                    //float3 test = (normalize(i.worldPos - _SpotLightPosition[j]) + float3(1,1,1)) /2;
                    //float3 test = abs(normalize(i.worldPos - _SpotLightPosition[j]));
                    // float dotkk = dot(normalize(i.worldPos - _SpotLightPosition[j]), _SpotLightDir[j]);
                    // return float4(dotkk, dotkk, dotkk, 1);
                    // float3 test = abs(_SpotLightDir[j]);
                    // return float4(test.x, test.y, test.z, 1);
                }
                float sum = mixedPInt + mainBInt;
                if(sum > 0){
                    tex *= (mixedPColor * (mixedPInt / sum)) + (mainBColor * (mainBInt / sum));
                }else{
                    tex = 0;
                }
                float shadow = saturate(1 - sum);
                tex += _ShadowColor * shadow * _ShadowValue;
                //UNITY_APPLY_FOG(i.fogCoord, tex);
                // float outlineT = (saturate(dot(viewDir, i.normal))) * 2;
                // outlineT = step(0.1, outlineT);
                // float4 outline = float4(outlineT, outlineT, outlineT, 1.0);
                //return saturate(tex * outline + (tex2D(_EmissionTex, i.uv) * _EmissionColor));
                return saturate(tex + (tex2D(_EmissionTex, i.uv) * _EmissionColor) + specular);
            }
            ENDCG
        }
    }
}
