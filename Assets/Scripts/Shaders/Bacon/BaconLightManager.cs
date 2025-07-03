using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class BaconLightManager : Singleton<BaconLightManager> {

    const int maxPointLights = 64;
    const int maxSpotLights = 32;

    const string mainDirectionName = "_MainLightDirection";
    const string mainColorName = "_BaconMainLightColor";
    const string shadowColorName = "_ShadowColor";
    const string mainIntensityName = "_MainLightIntensity";

    Dictionary<Type, List<BaconAdditionalLight>> additionalLights = new();

    [SerializeField] Color colorLight = Color.white;
    [SerializeField] Color colorShadow = Color.black;
    [SerializeField] float intensity = 1f;

    PropertyID mainDirectionProperty;
    PropertyID mainColorProperty;
    PropertyID shadowColorProperty;
    PropertyID mainIntensityProperty;

    PropertyID pointLightNumber;
    PropertyID pointLightIntensity;
    PropertyID pointLightRadius;
    PropertyID pointLightColor;
    PropertyID pointLightPosition;

    PropertyID spotLightNumber;
    PropertyID spotLightIntensity;
    PropertyID spotLightRadius;
    PropertyID spotLightColor;
    PropertyID spotLightPosition;
    PropertyID spotLightAngle;
    PropertyID spotLightDir;

    Vector4[] _PointLightColor = new Vector4[maxPointLights];
    float[] _PointLightIntensity = new float[maxPointLights];
    Vector4[] _PointLightPosition = new Vector4[maxPointLights];
    float[] _PointLightRadius = new float[maxPointLights];

    Vector4[] _SpotLightColor = new Vector4[maxSpotLights];
    float[] _SpotLightIntensity = new float[maxSpotLights];
    Vector4[] _SpotLightPosition = new Vector4[maxSpotLights];
    float[] _SpotLightRadius = new float[maxSpotLights];
    float[] _SpotLightAngle = new float[maxSpotLights];
    Vector4[] _SpotLightDir = new Vector4[maxSpotLights];

    bool pointLightsOnUpdate = false;
    bool wasInitProperties = false;

    void OnValidate() {
        UpdateVariables();
    }

    void Start() {
        SetupProperty();
        UpdateVariables();
    }

    void LateUpdate() {
        if (pointLightsOnUpdate) {
            UpdateAdditionalLights();
        }
    }

    public void Add<T>(T light) where T : BaconAdditionalLight {
        Type lightType = typeof(T);
        List<BaconAdditionalLight> lights = null;
        if (additionalLights.ContainsKey(lightType)) {
            lights = additionalLights[lightType];
        } else {
            lights = new();
            additionalLights.Add(lightType, lights);
        }

        if (!lights.Contains(light)) {
            lights.Add(light);
            if (light.OnUpdate) {
                pointLightsOnUpdate = true;
            }
        }
        UpdateAdditionalLights();
    }

    public void Remove<T>(T light) where T : BaconAdditionalLight {
        Type lightType = typeof(T);
        List<BaconAdditionalLight> lights = null;
        if (additionalLights.ContainsKey(lightType)) {
            lights = additionalLights[lightType];
        } else {
            return;
        }

        if (lights.Contains(light)) {
            lights.Remove(light);
            pointLightsOnUpdate = additionalLights.Any(x => x.Value.Any(p => p.OnUpdate));
        }
        UpdateAdditionalLights();
    }

    public void UpdateAdditionalLights() {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            RefreshAdditionalLights();
            SetupProperty();
        }
#endif
        if (additionalLights.TryGetValue(typeof(BaconPointLight), out List<BaconAdditionalLight> pointLights)) {
            int i = 0;
            foreach (BaconPointLight point in pointLights) {
                _PointLightColor[i] = point.color;
                _PointLightIntensity[i] = point.intensity;
                _PointLightPosition[i] = point.transform.position;
                _PointLightRadius[i] = point.radius;
                i++;
                if (i >= maxPointLights) {
                    break;
                }
            }
            i = Mathf.Min(maxPointLights, i);
            Shader.SetGlobalInt(pointLightNumber.GetId(), i);
            Shader.SetGlobalFloatArray(pointLightIntensity.GetId(), _PointLightIntensity);
            Shader.SetGlobalFloatArray(pointLightRadius.GetId(), _PointLightRadius);
            Shader.SetGlobalVectorArray(pointLightColor.GetId(), _PointLightColor);
            Shader.SetGlobalVectorArray(pointLightPosition.GetId(), _PointLightPosition);
            
        }
        if (additionalLights.TryGetValue(typeof(BaconSpotLight), out List<BaconAdditionalLight> spotLights)) {
            int i = 0;
            foreach (BaconSpotLight spot in spotLights) {
                _SpotLightColor[i] = spot.color;
                _SpotLightIntensity[i] = spot.intensity;
                _SpotLightPosition[i] = spot.transform.position;
                _SpotLightRadius[i] = spot.radius;
                _SpotLightAngle[i] = 1f - spot.angle / 180f;
                _SpotLightDir[i] = spot.transform.forward;
                i++;
                if (i >= maxSpotLights) {
                    break;
                }
            }
            i = Mathf.Min(maxSpotLights, i);
            Shader.SetGlobalInt(spotLightNumber.GetId(), i);
            Shader.SetGlobalFloatArray(spotLightIntensity.GetId(), _SpotLightIntensity);
            Shader.SetGlobalFloatArray(spotLightRadius.GetId(), _SpotLightRadius);
            Shader.SetGlobalVectorArray(spotLightColor.GetId(), _SpotLightColor);
            Shader.SetGlobalVectorArray(spotLightPosition.GetId(), _SpotLightPosition);
            Shader.SetGlobalFloatArray(spotLightAngle.GetId(), _SpotLightAngle);
            Shader.SetGlobalVectorArray(spotLightDir.GetId(), _SpotLightDir);
        }

    }

    [Button]
    public void UpdateVariables() {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            RefreshAdditionalLights();
           // SetupProperty();
        }
#endif
       
        SetupProperty();
        Shader.SetGlobalColor(mainColorProperty.GetId(), colorLight);
        Shader.SetGlobalColor(shadowColorProperty.GetId(), colorShadow);
        Shader.SetGlobalVector(mainDirectionProperty.GetId(), transform.forward);
        Shader.SetGlobalFloat(mainIntensityProperty.GetId(), intensity);
        UpdateAdditionalLights();
    }

    protected override void OnInit() {
        base.OnInit();
        pointLightsOnUpdate = false;
        SetupProperty();
    }

    void SetupProperty() {
        shadowColorProperty = new PropertyID(shadowColorName);
        if (!wasInitProperties) {
            mainColorProperty ??= new PropertyID(mainColorName);
            
            mainDirectionProperty ??= new PropertyID(mainDirectionName);
            mainIntensityProperty ??= new PropertyID(mainIntensityName);
            pointLightNumber ??= new PropertyID("_PointLightNumber");
            pointLightIntensity ??= new PropertyID("_PointLightIntensity");
            pointLightRadius ??= new PropertyID("_PointLightRadius");
            pointLightColor ??= new PropertyID("_PointLightColor");
            pointLightPosition ??= new PropertyID("_PointLightPosition");
            spotLightNumber ??= new PropertyID("_SpotLightNumber");
            spotLightIntensity ??= new PropertyID("_SpotLightIntensity");
            spotLightRadius ??= new PropertyID("_SpotLightRadius");
            spotLightColor ??= new PropertyID("_SpotLightColor");
            spotLightPosition ??= new PropertyID("_SpotLightPosition");
            spotLightAngle ??= new PropertyID("_SpotLightAngle");
            spotLightDir ??= new PropertyID("_SpotLightDir");
            wasInitProperties = Application.isPlaying;
        }
    }

    void RefreshAdditionalLights() {
        pointLightsOnUpdate = false;
        additionalLights.Clear();
        Add<BaconPointLight>();
        Add<BaconSpotLight>();

        void Add<T>() where T : BaconAdditionalLight{
            List<BaconAdditionalLight> lights = FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Select(x => x as BaconAdditionalLight).ToList();
            additionalLights.Add(typeof(T), lights);
        }
    }

    [System.Serializable]
    class PropertyID {
        public int id; 
        public string name;

        public PropertyID(string name) {
            this.name = name;
            id = Shader.PropertyToID(name);
        }

        public int GetId() {
            return id;
        }
    }

}
