using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Map input with a hover preview, committed guess pin, answer pin, and correctly positioned reveal link.</summary>
[RequireComponent(typeof(Graphic))]
public sealed class MapGuessInput : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private GeoGuessrGameManager gameManager;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RectTransform previewMarker;
    [SerializeField] private RectTransform guessMarker;
    [SerializeField] private RectTransform answerMarker;
    [SerializeField] private RectTransform connector;

    private void Awake()
    {
        if (mapRect == null) mapRect = transform as RectTransform;
        GetComponent<Graphic>().raycastTarget = true;
        Hide(previewMarker);
        Hide(guessMarker);
        Hide(answerMarker);
        Hide(connector);
        if (connector != null) connector.SetParent(mapRect, false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (gameManager == null || mapRect == null || previewMarker == null ||
            gameManager.GetSnapshot().phase != GeoGamePhase.Playing) return;
        if (TryGetMapPoint(eventData, out Vector2 normalized)) PlaceMarker(previewMarker, normalized);
    }

    public void OnPointerExit(PointerEventData eventData) => Hide(previewMarker);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null || mapRect == null || guessMarker == null) return;
        if (!TryGetMapPoint(eventData, out Vector2 normalized)) return;
        float longitude = normalized.x * 360f - 180f;
        float latitude = normalized.y * 180f - 90f;
        if (!gameManager.PlaceGuess(latitude, longitude)) return;
        PlaceMarker(guessMarker, normalized);
        Hide(previewMarker);
        Hide(answerMarker);
        Hide(connector);
    }

    public void RevealAnswer(float latitude, float longitude, bool hasGuess)
    {
        if (mapRect == null || answerMarker == null) return;
        PlaceMarker(answerMarker, new Vector2((longitude + 180f) / 360f, (latitude + 90f) / 180f));
        if (hasGuess && guessMarker != null && connector != null)
            SetConnector(MarkerLocalPosition(guessMarker), MarkerLocalPosition(answerMarker));
        else Hide(connector);
    }

    public void ClearMarkers()
    {
        Hide(previewMarker);
        Hide(guessMarker);
        Hide(answerMarker);
        Hide(connector);
    }

    private bool TryGetMapPoint(PointerEventData eventData, out Vector2 normalized)
    {
        normalized = Vector2.zero;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapRect, eventData.position, eventData.pressEventCamera, out Vector2 local)) return false;
        Rect rect = mapRect.rect;
        if (rect.width <= 0f || rect.height <= 0f) return false;
        normalized = new Vector2(Mathf.Clamp01((local.x - rect.xMin) / rect.width),
                                 Mathf.Clamp01((local.y - rect.yMin) / rect.height));
        return true;
    }

    private void PlaceMarker(RectTransform marker, Vector2 normalized)
    {
        marker.anchorMin = marker.anchorMax = normalized;
        marker.anchoredPosition = Vector2.zero;
        marker.gameObject.SetActive(true);
    }

    private Vector2 MarkerLocalPosition(RectTransform marker)
    {
        Vector2 normalized = marker.anchorMin;
        Rect rect = mapRect.rect;
        return new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, normalized.x),
                            Mathf.Lerp(rect.yMin, rect.yMax, normalized.y));
    }

    private void SetConnector(Vector2 from, Vector2 to)
    {
        Vector2 delta = to - from;
        connector.anchorMin = connector.anchorMax = new Vector2(0.5f, 0.5f);
        connector.pivot = new Vector2(0f, 0.5f);
        connector.anchoredPosition = (from + to) * 0.5f;
        connector.sizeDelta = new Vector2(delta.magnitude, Mathf.Max(3f, connector.sizeDelta.y));
        connector.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        connector.gameObject.SetActive(true);
        connector.SetAsFirstSibling();
    }

    private static void Hide(RectTransform target)
    {
        if (target != null) target.gameObject.SetActive(false);
    }
}
