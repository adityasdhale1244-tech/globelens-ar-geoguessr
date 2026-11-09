using UnityEngine;

public sealed class PanoramaDisplay : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string resourcesFolder = "Panoramas";
    [SerializeField] private Texture fallbackTexture;

    public void Show(string textureName)
    {
        if (targetRenderer == null) return;
        Texture texture = string.IsNullOrWhiteSpace(textureName)
            ? null
            : Resources.Load<Texture>($"{resourcesFolder}/{textureName}");
        targetRenderer.material.mainTexture = texture != null ? texture : fallbackTexture;
    }
}
