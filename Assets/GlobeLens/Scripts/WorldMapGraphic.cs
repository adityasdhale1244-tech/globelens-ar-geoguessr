using UnityEngine;
using UnityEngine.UI;

/// <summary>Draws a lightweight equirectangular world-map silhouette and graticule as UI geometry.</summary>
public sealed class WorldMapGraphic : MaskableGraphic
{
    [SerializeField] private Color landColor = new Color(0.35f, 0.62f, 0.43f, 1f);
    [SerializeField] private Color gridColor = new Color(0.55f, 0.8f, 0.66f, 0.25f);
    [SerializeField, Range(0.5f, 4f)] private float gridThickness = 1f;

    // Approximate, deliberately stylized continent silhouettes (normalized x/y; y=0 is bottom).
    private static readonly Vector2[][] Continents =
    {
        new[] { P(.07f,.80f), P(.16f,.86f), P(.22f,.79f), P(.20f,.68f), P(.25f,.59f), P(.22f,.48f), P(.18f,.39f), P(.16f,.24f), P(.11f,.18f), P(.09f,.35f), P(.06f,.48f), P(.07f,.64f) },
        new[] { P(.21f,.42f), P(.28f,.42f), P(.31f,.34f), P(.29f,.22f), P(.25f,.08f), P(.22f,.17f), P(.20f,.29f) },
        new[] { P(.39f,.78f), P(.47f,.84f), P(.53f,.78f), P(.58f,.72f), P(.63f,.73f), P(.68f,.65f), P(.65f,.56f), P(.61f,.52f), P(.57f,.59f), P(.52f,.58f), P(.49f,.50f), P(.43f,.53f), P(.39f,.62f) },
        new[] { P(.48f,.49f), P(.55f,.47f), P(.58f,.38f), P(.56f,.29f), P(.53f,.17f), P(.49f,.12f), P(.46f,.23f), P(.45f,.34f) },
        new[] { P(.63f,.68f), P(.69f,.72f), P(.74f,.66f), P(.78f,.60f), P(.76f,.51f), P(.71f,.49f), P(.67f,.55f), P(.64f,.60f) },
        new[] { P(.72f,.48f), P(.78f,.47f), P(.81f,.40f), P(.79f,.31f), P(.76f,.25f), P(.73f,.32f), P(.70f,.40f) },
        new[] { P(.81f,.30f), P(.88f,.34f), P(.93f,.29f), P(.94f,.20f), P(.89f,.15f), P(.84f,.18f), P(.81f,.24f) }
    };

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = rectTransform.rect;
        AddGrid(vh, r);
        foreach (Vector2[] polygon in Continents) AddPolygon(vh, r, polygon);
    }

    private void AddGrid(VertexHelper vh, Rect r)
    {
        for (int i = 1; i < 6; i++)
        {
            float x = Mathf.Lerp(r.xMin, r.xMax, i / 6f);
            AddQuad(vh, new Vector2(x - gridThickness * .5f, r.yMin), new Vector2(x + gridThickness * .5f, r.yMax), gridColor);
        }
        for (int i = 1; i < 4; i++)
        {
            float y = Mathf.Lerp(r.yMin, r.yMax, i / 4f);
            AddQuad(vh, new Vector2(r.xMin, y - gridThickness * .5f), new Vector2(r.xMax, y + gridThickness * .5f), gridColor);
        }
    }

    private void AddPolygon(VertexHelper vh, Rect r, Vector2[] normalized)
    {
        Vector2 center = Vector2.zero;
        foreach (Vector2 point in normalized) center += ToLocal(r, point);
        center /= normalized.Length;
        int centerIndex = vh.currentVertCount;
        AddVert(vh, center, landColor);
        int first = vh.currentVertCount;
        foreach (Vector2 point in normalized) AddVert(vh, ToLocal(r, point), landColor);
        for (int i = 0; i < normalized.Length; i++)
        {
            int next = (i + 1) % normalized.Length;
            vh.AddTriangle(centerIndex, first + i, first + next);
        }
    }

    private void AddQuad(VertexHelper vh, Vector2 min, Vector2 max, Color tint)
    {
        int start = vh.currentVertCount;
        AddVert(vh, new Vector2(min.x, min.y), tint);
        AddVert(vh, new Vector2(min.x, max.y), tint);
        AddVert(vh, new Vector2(max.x, max.y), tint);
        AddVert(vh, new Vector2(max.x, min.y), tint);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private void AddVert(VertexHelper vh, Vector2 position, Color tint)
    {
        vh.AddVert(position, tint, Vector2.zero);
    }

    private static Vector2 ToLocal(Rect rect, Vector2 normalized)
    {
        return new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, normalized.x), Mathf.Lerp(rect.yMin, rect.yMax, normalized.y));
    }

    private static Vector2 P(float x, float y) => new Vector2(x, y);
}
