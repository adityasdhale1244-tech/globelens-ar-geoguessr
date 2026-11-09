using UnityEngine;

/// <summary>Displays locally bundled 2:1 equirectangular panoramas on an inward-facing sphere.</summary>
public sealed class PanoramaDisplay : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string resourcesFolder = "Panoramas";
    [SerializeField] private Texture fallbackTexture;
    [SerializeField, Min(20f)] private float domeDiameter = 80f;
    [SerializeField] private Camera followCamera;

    private MaterialPropertyBlock properties;

    private void Awake()
    {
        if (followCamera == null) followCamera = Camera.main;
        properties = new MaterialPropertyBlock();
        if (targetRenderer == null) CreateDome();
    }

    private void LateUpdate()
    {
        if (targetRenderer != null && followCamera != null)
            targetRenderer.transform.position = followCamera.transform.position;
    }

    public void Show(string textureName)
    {
        if (targetRenderer == null) return;
        Texture texture = string.IsNullOrWhiteSpace(textureName)
            ? null
            : Resources.Load<Texture>($"{resourcesFolder}/{textureName}");
        if (texture == null) texture = fallbackTexture;
        targetRenderer.GetPropertyBlock(properties);
        properties.SetTexture("_BaseMap", texture);
        properties.SetTexture("_MainTex", texture);
        targetRenderer.SetPropertyBlock(properties);
    }

    private void CreateDome()
    {
        GameObject dome = new GameObject("Generated Panorama Dome");
        dome.transform.SetParent(transform, false);
        dome.transform.localScale = Vector3.one * domeDiameter;

        MeshFilter filter = dome.AddComponent<MeshFilter>();
        MeshRenderer renderer = dome.AddComponent<MeshRenderer>();
        filter.sharedMesh = CreateInwardSphere(48, 32);
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Texture");
        if (shader == null)
        {
            Debug.LogError("No unlit shader found. Assign a Renderer with an unlit material to PanoramaDisplay.", this);
            Destroy(dome);
            return;
        }

        Material material = new Material(shader);
        material.name = "GlobeLens Panorama Unlit";
        renderer.sharedMaterial = material;
        targetRenderer = renderer;
    }

    private static Mesh CreateInwardSphere(int longitudeSegments, int latitudeSegments)
    {
        var vertices = new Vector3[(longitudeSegments + 1) * (latitudeSegments + 1)];
        var normals = new Vector3[vertices.Length];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[longitudeSegments * latitudeSegments * 6];
        int v = 0;

        for (int y = 0; y <= latitudeSegments; y++)
        {
            float lat = Mathf.PI * y / latitudeSegments;
            for (int x = 0; x <= longitudeSegments; x++)
            {
                float lon = 2f * Mathf.PI * x / longitudeSegments;
                Vector3 outward = new Vector3(Mathf.Sin(lat) * Mathf.Cos(lon), Mathf.Cos(lat), Mathf.Sin(lat) * Mathf.Sin(lon));
                vertices[v] = outward;
                normals[v] = -outward;
                uv[v] = new Vector2((float)x / longitudeSegments, 1f - (float)y / latitudeSegments);
                v++;
            }
        }

        int t = 0;
        for (int y = 0; y < latitudeSegments; y++)
        {
            for (int x = 0; x < longitudeSegments; x++)
            {
                int a = y * (longitudeSegments + 1) + x;
                int b = a + longitudeSegments + 1;
                triangles[t++] = a; triangles[t++] = b; triangles[t++] = a + 1;
                triangles[t++] = a + 1; triangles[t++] = b; triangles[t++] = b + 1;
            }
        }

        var mesh = new Mesh { name = "GlobeLens Inward Panorama Sphere" };
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        return mesh;
    }
}
