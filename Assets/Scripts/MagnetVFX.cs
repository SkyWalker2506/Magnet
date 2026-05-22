using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] public Transform Pos1;
    [SerializeField] public Transform Pos2;
    [SerializeField] public Transform Pos3;
    [SerializeField] public Transform Pos4;

    [Header("WebGL-compatible Beam")]
    [SerializeField, ColorUsage(true, true)] private Color beamColor = new Color(0.1f, 1.6f, 8f, 1f);
    [SerializeField] private float beamWidth = 0.32f;
    [SerializeField] private int beamSegments = 40;
    [SerializeField] private float intensityBoost = 4f;

    [Header("Electric Jitter")]
    [SerializeField] private float jitterAmplitude = 0.35f;
    [SerializeField] private float jitterFrequency = 6f;
    [SerializeField] private float jitterSpeed = 14f;
    [SerializeField] private float widthPulseAmount = 0.4f;
    [SerializeField] private float widthPulseSpeed = 18f;

    private LineRenderer line;
    private float noiseSeed;
    private MaterialPropertyBlock mpb;
    private static Material sharedBeamMaterial;
    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    public bool IsActive => line != null && line.enabled;

    private void Awake()
    {
        if (vfx != null) vfx.SetActive(false);
        noiseSeed = Random.Range(0f, 1000f);
        EnsureLine();
        SetActive(false);
    }

    public void SetTargets(Transform target1, Transform target2)
    {
        Pos1.parent = target1;
        Pos1.localPosition = Vector3.up;
        Pos4.parent = target2;
        Pos4.localPosition = Vector3.zero;
    }

    public void SetActive(bool isActive)
    {
        if (this == null) return;
        if (line == null) EnsureLine();
        if (line != null) line.enabled = isActive;
    }

    private void LateUpdate()
    {
        if (line == null || !line.enabled) return;
        if (line.positionCount != beamSegments + 1)
            line.positionCount = beamSegments + 1;

        Vector3 p0 = Pos1.position, p1 = Pos2.position, p2 = Pos3.position, p3 = Pos4.position;
        Vector3 chord = p3 - p0;
        Vector3 normal = Vector3.Cross(chord, Vector3.up);
        if (normal.sqrMagnitude < 0.0001f) normal = Vector3.Cross(chord, Vector3.right);
        normal.Normalize();
        Vector3 binormal = Vector3.Cross(chord.normalized, normal);

        float t0 = Time.time * jitterSpeed;
        for (int i = 0; i <= beamSegments; i++)
        {
            float t = (float)i / beamSegments;
            float u = 1f - t;
            Vector3 point = u * u * u * p0
                          + 3f * u * u * t * p1
                          + 3f * u * t * t * p2
                          + t * t * t * p3;

            float taper = Mathf.Sin(t * Mathf.PI);
            float nx = (Mathf.PerlinNoise(t * jitterFrequency + noiseSeed, t0) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(t * jitterFrequency + noiseSeed + 17.3f, t0 + 41.7f) - 0.5f) * 2f;
            point += (normal * nx + binormal * ny) * (jitterAmplitude * taper);

            line.SetPosition(i, point);
        }

        float pulse = 1f + Mathf.Sin(Time.time * widthPulseSpeed + noiseSeed) * widthPulseAmount * 0.5f;
        float w = beamWidth * pulse;
        line.startWidth = w;
        line.endWidth = w;

        float flicker = 0.85f + Mathf.PerlinNoise(noiseSeed, Time.time * 12f) * 0.6f;
        line.startColor = Color.white;
        line.endColor = Color.white;

        if (mpb == null) mpb = new MaterialPropertyBlock();
        line.GetPropertyBlock(mpb);
        mpb.SetColor(ColorID, beamColor);
        mpb.SetFloat(IntensityID, intensityBoost * flicker);
        line.SetPropertyBlock(mpb);
    }

    private void EnsureLine()
    {
        if (line != null) return;
        line = GetComponent<LineRenderer>();
        if (line == null) line = gameObject.AddComponent<LineRenderer>();

        line.sharedMaterial = GetSharedMaterial();
        line.startColor = beamColor;
        line.endColor = beamColor;
        line.startWidth = beamWidth;
        line.endWidth = beamWidth;
        line.useWorldSpace = true;
        line.numCornerVertices = 4;
        line.numCapVertices = 4;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        line.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
    }

    private static Material GetSharedMaterial()
    {
        if (sharedBeamMaterial != null) return sharedBeamMaterial;
        var shader = Resources.Load<Shader>("Shaders/MagnetBeam");
        if (shader == null) shader = Shader.Find("Magnet/Beam");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            Debug.LogWarning("MagnetVFX: custom Magnet/Beam shader not found, falling back to " + (shader != null ? shader.name : "null"));
        }
        sharedBeamMaterial = new Material(shader) { name = "MagnetBeamRuntime" };
        if (sharedBeamMaterial.HasProperty("_Color")) sharedBeamMaterial.SetColor("_Color", new Color(0.1f, 1.6f, 8f, 1f));
        if (sharedBeamMaterial.HasProperty("_Intensity")) sharedBeamMaterial.SetFloat("_Intensity", 1f);
        sharedBeamMaterial.renderQueue = 3000;
        return sharedBeamMaterial;
    }
}
