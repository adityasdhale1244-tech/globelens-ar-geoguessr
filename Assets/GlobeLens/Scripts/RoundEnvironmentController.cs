using UnityEngine;

/// <summary>Creates a calm, optional 3D destination cue and eases the scene ambience per round.</summary>
public sealed class RoundEnvironmentController : MonoBehaviour
{
    [Header("Scene lighting")]
    [SerializeField] private Light keyLight;
    [SerializeField] private Color defaultAmbient = new Color(0.18f, 0.22f, 0.2f, 1f);
    [SerializeField] private Color defaultKey = new Color(0.82f, 0.9f, 0.84f, 1f);
    [SerializeField, Range(0f, 2f)] private float keyIntensity = 0.7f;
    [SerializeField, Min(0.1f)] private float transitionSeconds = 1.2f;

    [Header("3D cue")]
    [SerializeField] private Transform cueAnchor;
    [SerializeField] private Vector3 cueLocalPosition = new Vector3(-0.75f, 1.45f, 2.4f);
    [SerializeField] private float cueScale = 0.28f;
    [SerializeField] private float floatAmplitude = 0.025f;
    [SerializeField] private float floatFrequency = 0.7f;

    private Color originalAmbient;
    private Color ambientTarget;
    private Color keyTarget;
    private Color keyBaseColor;
    private Transform activeCue;
    private float ambientT;
    private float cueBaseY;
    private bool hasOriginalAmbient;

    private void Awake()
    {
        originalAmbient = RenderSettings.ambientLight;
        hasOriginalAmbient = true;
        ambientTarget = defaultAmbient;
        keyTarget = defaultKey;
        if (keyLight != null) keyBaseColor = keyLight.color;
        if (cueAnchor == null) cueAnchor = transform;
    }

    private void Update()
    {
        float duration = Mathf.Max(0.1f, transitionSeconds);
        ambientT = Mathf.MoveTowards(ambientT, 1f, Time.deltaTime / duration);
        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientTarget, Time.deltaTime / duration);
        if (keyLight != null)
        {
            keyLight.color = Color.Lerp(keyLight.color, keyTarget, Time.deltaTime / duration);
            keyLight.intensity = Mathf.Lerp(keyLight.intensity, keyIntensity, Time.deltaTime / duration);
        }
        if (activeCue != null)
        {
            activeCue.localPosition = new Vector3(activeCue.localPosition.x,
                cueBaseY + Mathf.Sin(Time.time * floatFrequency * Mathf.PI * 2f) * floatAmplitude,
                activeCue.localPosition.z);
            activeCue.Rotate(Vector3.up, 12f * Time.deltaTime, Space.Self);
        }
    }

    public void ApplyRound(string cuePrefabName, string caption, string ambienceHex)
    {
        Color accent = defaultKey;
        if (!string.IsNullOrWhiteSpace(ambienceHex) && ColorUtility.TryParseHtmlString(ambienceHex, out Color parsed))
            accent = parsed;
        ambientTarget = Color.Lerp(defaultAmbient, accent, 0.42f);
        keyTarget = Color.Lerp(defaultKey, accent, 0.28f);
        ambientT = 0f;
        CreateCue(cuePrefabName);
    }

    private void CreateCue(string prefabName)
    {
        if (activeCue != null) Destroy(activeCue.gameObject);
        GameObject prefab = string.IsNullOrWhiteSpace(prefabName) ? null : Resources.Load<GameObject>("Cues/" + prefabName);
        GameObject cue = prefab != null ? Instantiate(prefab, cueAnchor) : CreateFallbackCue(cueAnchor);
        activeCue = cue.transform;
        activeCue.localPosition = cueLocalPosition;
        activeCue.localRotation = Quaternion.identity;
        activeCue.localScale = Vector3.one * cueScale;
        cueBaseY = cueLocalPosition.y;
    }

    private static GameObject CreateFallbackCue(Transform parent)
    {
        GameObject root = new GameObject("GlobeLens 3D clue beacon");
        root.transform.SetParent(parent, false);
        CreatePart("Beacon pedestal", PrimitiveType.Cylinder, root.transform, new Vector3(0f, -0.5f, 0f), new Vector3(0.65f, 0.08f, 0.65f), new Color(0.14f, 0.26f, 0.22f));
        CreatePart("Beacon stem", PrimitiveType.Cylinder, root.transform, new Vector3(0f, -0.15f, 0f), new Vector3(0.12f, 0.38f, 0.12f), new Color(0.42f, 0.72f, 0.52f));
        CreatePart("Beacon core", PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.38f, 0f), new Vector3(0.4f, 0.4f, 0.4f), new Color(0.65f, 0.95f, 0.68f));
        CreatePart("Beacon halo", PrimitiveType.Cylinder, root.transform, new Vector3(0f, 0.38f, 0f), new Vector3(0.72f, 0.035f, 0.72f), new Color(0.3f, 0.78f, 0.62f));
        Light glow = root.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = new Color(0.5f, 0.9f, 0.68f);
        glow.intensity = 0.45f;
        glow.range = 2.2f;
        glow.shadows = LightShadows.None;
        return root;
    }

    private static void CreatePart(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        Renderer renderer = part.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader != null) renderer.material = new Material(shader) { color = color };
        }
    }

    private void OnDisable()
    {
        if (hasOriginalAmbient) RenderSettings.ambientLight = originalAmbient;
        if (keyLight != null) keyLight.color = keyBaseColor;
    }
}
