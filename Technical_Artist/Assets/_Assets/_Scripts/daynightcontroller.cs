using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

[ExecuteAlways]
public class daynightcontroller : MonoBehaviour
{
    [Header("Cycle")]
    [SerializeField] private float cycleDuration = 45f;
    [SerializeField] private bool autoCycle = true;
    [SerializeField] private bool useManualBlend = false;
    [SerializeField][Range(0f, 1f)] private float manualBlend = 0f;

    [Header("References")]
    [SerializeField] private Light sunLight;
    [SerializeField] private MeshRenderer domeRenderer;
    [SerializeField] private Material runtimeSkyboxMaterial;

    [Header("Sky Textures")]
    [SerializeField] private Texture2D dayTexture;
    [SerializeField] private Texture2D nightTexture;

    [Header("Sky Gradient Colors")]
    [SerializeField] private Color dayTopColor = new Color(0.30f, 0.60f, 1.00f);
    [SerializeField] private Color dayBottomColor = new Color(0.85f, 0.92f, 1.00f);

    [SerializeField] private Color sunsetTopColor = new Color(0.40f, 0.30f, 0.75f);
    [SerializeField] private Color sunsetBottomColor = new Color(1.00f, 0.62f, 0.32f);

    [SerializeField] private Color nightTopColor = new Color(0.05f, 0.09f, 0.22f);
    [SerializeField] private Color nightBottomColor = new Color(0.16f, 0.25f, 0.46f);

    [Header("Sky Texture Controls")]
    [SerializeField] private float cloudStrength = 1.0f;
    [SerializeField] private float textureBrightness = 1.0f;

    [Header("Sun Rotation")]
    [SerializeField] private Vector3 dayRotation = new Vector3(50f, -30f, 0f);
    [SerializeField] private Vector3 sunsetRotation = new Vector3(18f, -30f, 0f);
    [SerializeField] private Vector3 nightRotation = new Vector3(-18f, -30f, 0f);

    [Header("Ambient Colors")]
    [SerializeField] private Color dayAmbientColor = new Color(0.85f, 0.90f, 1f);
    [SerializeField] private Color sunsetAmbientColor = new Color(0.82f, 0.62f, 0.48f);
    [SerializeField] private Color nightAmbientColor = new Color(0.36f, 0.42f, 0.62f);

    [Header("Fog")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private Color dayFogColor = new Color(0.74f, 0.88f, 1f);
    [SerializeField] private Color sunsetFogColor = new Color(1.00f, 0.64f, 0.42f);
    [SerializeField] private Color nightFogColor = new Color(0.12f, 0.18f, 0.32f);

    [Header("Sun Light")]
    [SerializeField] private Color dayLightColor = new Color(1f, 0.96f, 0.82f);
    [SerializeField] private Color sunsetLightColor = new Color(1f, 0.60f, 0.38f);
    [SerializeField] private Color nightLightColor = new Color(0.45f, 0.52f, 0.78f);

    [SerializeField] private float dayLightIntensity = 1.10f;
    [SerializeField] private float sunsetLightIntensity = 0.80f;
    [SerializeField] private float nightLightIntensity = 0.35f;

    private float timer;

    private static readonly int DayTexID = Shader.PropertyToID("_DayTex");
    private static readonly int NightTexID = Shader.PropertyToID("_NightTex");
    private static readonly int BlendID = Shader.PropertyToID("_Blend");
    private static readonly int TopColorID = Shader.PropertyToID("_TopColor");
    private static readonly int BottomColorID = Shader.PropertyToID("_BottomColor");
    private static readonly int CloudStrengthID = Shader.PropertyToID("_CloudStrength");
    private static readonly int TextureBrightnessID = Shader.PropertyToID("_TextureBrightness");

    private void OnEnable()
    {
        ApplyImmediate();
    }

    private void Start()
    {
        ApplyImmediate();
    }

    private void Update()
    {
        ApplyImmediate();
    }

    private void OnValidate()
    {
        ApplyImmediate();
    }

    private void ApplyImmediate()
    {
        Material targetMaterial = GetTargetMaterial();
        if (targetMaterial == null)
        {
            return;
        }

        ApplyStaticMaterialValues(targetMaterial);

        float blend;

        if (useManualBlend)
        {
            blend = Mathf.Clamp01(manualBlend);
        }
        else
        {
            if (!UnityEngine.Application.isPlaying)
            {
                blend = Mathf.Clamp01(manualBlend);
            }
            else
            {
                if (!autoCycle)
                {
                    blend = Mathf.Clamp01(manualBlend);
                }
                else
                {
                    if (cycleDuration <= 0.01f)
                    {
                        cycleDuration = 0.01f;
                    }

                    timer += Time.deltaTime;
                    float normalizedTime = (timer % cycleDuration) / cycleDuration;
                    blend = GetBlendFromCycle(normalizedTime);
                }
            }
        }

        ApplyCycle(targetMaterial, blend);
    }

    private Material GetTargetMaterial()
    {
        if (domeRenderer != null)
        {
            if (UnityEngine.Application.isPlaying)
            {
                if (domeRenderer.material != null)
                {
                    return domeRenderer.material;
                }
            }
            else
            {
                if (domeRenderer.sharedMaterial != null)
                {
                    return domeRenderer.sharedMaterial;
                }
            }
        }

        return runtimeSkyboxMaterial;
    }

    private void ApplyStaticMaterialValues(Material targetMaterial)
    {
        if (dayTexture != null)
        {
            targetMaterial.SetTexture(DayTexID, dayTexture);
        }

        if (nightTexture != null)
        {
            targetMaterial.SetTexture(NightTexID, nightTexture);
        }

        targetMaterial.SetFloat(CloudStrengthID, cloudStrength);
        targetMaterial.SetFloat(TextureBrightnessID, textureBrightness);
    }

    private float GetBlendFromCycle(float t)
    {
        if (t < 0.4f)
        {
            return Mathf.Lerp(0f, 0.35f, t / 0.4f);
        }

        if (t < 0.7f)
        {
            return Mathf.Lerp(0.35f, 1f, (t - 0.4f) / 0.3f);
        }

        return Mathf.Lerp(1f, 0f, (t - 0.7f) / 0.3f);
    }

    private void ApplyCycle(Material targetMaterial, float blend)
    {
        blend = Mathf.Clamp01(blend);

        Color topColor;
        Color bottomColor;
        Color ambientColor;
        Color fogColor;
        Color lightColor;
        float lightIntensity;
        Vector3 lightRotation;

        if (blend < 0.35f)
        {
            float t = Mathf.InverseLerp(0f, 0.35f, blend);

            topColor = Color.Lerp(dayTopColor, sunsetTopColor, t);
            bottomColor = Color.Lerp(dayBottomColor, sunsetBottomColor, t);

            ambientColor = Color.Lerp(dayAmbientColor, sunsetAmbientColor, t);
            fogColor = Color.Lerp(dayFogColor, sunsetFogColor, t);
            lightColor = Color.Lerp(dayLightColor, sunsetLightColor, t);
            lightIntensity = Mathf.Lerp(dayLightIntensity, sunsetLightIntensity, t);
            lightRotation = Vector3.Lerp(dayRotation, sunsetRotation, t);
        }
        else
        {
            float t = Mathf.InverseLerp(0.35f, 1f, blend);

            topColor = Color.Lerp(sunsetTopColor, nightTopColor, t);
            bottomColor = Color.Lerp(sunsetBottomColor, nightBottomColor, t);

            ambientColor = Color.Lerp(sunsetAmbientColor, nightAmbientColor, t);
            fogColor = Color.Lerp(sunsetFogColor, nightFogColor, t);
            lightColor = Color.Lerp(sunsetLightColor, nightLightColor, t);
            lightIntensity = Mathf.Lerp(sunsetLightIntensity, nightLightIntensity, t);
            lightRotation = Vector3.Lerp(sunsetRotation, nightRotation, t);
        }

        targetMaterial.SetFloat(BlendID, blend);
        targetMaterial.SetColor(TopColorID, topColor);
        targetMaterial.SetColor(BottomColorID, bottomColor);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;

        if (useFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
        }
        else
        {
            RenderSettings.fog = false;
        }

        if (sunLight != null)
        {
            sunLight.color = lightColor;
            sunLight.intensity = lightIntensity;
            sunLight.transform.rotation = Quaternion.Euler(lightRotation);
        }
    }
}