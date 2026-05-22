using UnityEngine;

public class MagnetVFX : MonoBehaviour
{
    [SerializeField] private GameObject vfx;
    [SerializeField] public Transform Pos1;
    [SerializeField] public Transform Pos2;
    [SerializeField] public Transform Pos3;
    [SerializeField] public Transform Pos4;

    [Header("WebGL-compatible Beam")]
    [SerializeField, ColorUsage(true, true)] private Color beamColor = new Color(2.5f, 5f, 8f, 1f);
    [SerializeField] private float beamWidth = 0.35f;
    [SerializeField] private int beamSegments = 28;

    private LineRenderer line;
    private static Material sharedBeamMaterial;

    public bool IsActive => line != null && line.enabled;

    private void Awake()
    {
        if (vfx != null) vfx.SetActive(false);
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
        EnsureLine();
        line.enabled = isActive;
    }

    private void LateUpdate()
    {
        if (line == null || !line.enabled) return;
        if (line.positionCount != beamSegments + 1)
            line.positionCount = beamSegments + 1;

        Vector3 p0 = Pos1.position, p1 = Pos2.position, p2 = Pos3.position, p3 = Pos4.position;
        for (int i = 0; i <= beamSegments; i++)
        {
            float t = (float)i / beamSegments;
            float u = 1f - t;
            Vector3 point = u * u * u * p0
                          + 3f * u * u * t * p1
                          + 3f * u * t * t * p2
                          + t * t * t * p3;
            line.SetPosition(i, point);
        }
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
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        sharedBeamMaterial = new Material(shader) { name = "MagnetBeamRuntime" };
        if (sharedBeamMaterial.HasProperty("_Surface")) sharedBeamMaterial.SetFloat("_Surface", 1f);
        if (sharedBeamMaterial.HasProperty("_Blend")) sharedBeamMaterial.SetFloat("_Blend", 1f);
        if (sharedBeamMaterial.HasProperty("_SrcBlend")) sharedBeamMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        if (sharedBeamMaterial.HasProperty("_DstBlend")) sharedBeamMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        if (sharedBeamMaterial.HasProperty("_ZWrite")) sharedBeamMaterial.SetFloat("_ZWrite", 0f);
        sharedBeamMaterial.renderQueue = 3000;
        return sharedBeamMaterial;
    }
}
