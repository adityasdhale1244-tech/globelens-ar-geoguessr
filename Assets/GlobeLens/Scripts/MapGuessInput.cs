using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Maps world-space UI clicks to equirectangular latitude/longitude and presents guess/reveal pins.</summary>
[RequireComponent(typeof(Graphic))]
public sealed class MapGuessInput : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GeoGuessrGameManager gameManager;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RectTransform guessMarker;
    [SerializeField] private RectTransform answerMarker;
    [SerializeField] private RectTransform connector;

    private void Awake()
    {
        if (mapRect == null) mapRect = transform as RectTransform;
        GetComponent<Graphic>().raycastTarget = true;
        Hide(guessMarker);
        Hide(answerMarker);
        Hide(connector);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null || mapRect == null || guessMarker == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapRect, eventData.position, eventData.pressEventCamera, out Vector2 local)) return;

        Rect rect = mapRect.rect;
        if (rect.width <= 0f || rect.height <= 0f) return;
        float x = Mathf.Clamp01((local.x - rect.xMin) / rect.width);
        float y = Mathf.Clamp01((local.y - rect.yMin) / rect.height);
        // Equirectangular projection; top of map is north, bottom is south.
        float longitude = x * 360f - 180f;
        float latitude = y * 180f - 90f;

        if (!gameManager.PlaceGuess(latitude, longitude)) return;
        PlaceMarker(guessMarker, x, y);
        Hide(answerMarker);
        Hide(connector);
    }

    public void RevealAnswer(float latitude, float longitude, bool hasGuess)
    {
        if (mapRect == null || answerMarker == null) return;
        Vector2 answer = LatLonToNormalized(latitude, longitude);
        PlaceMarker(answerMarker, answer.x, answer.y);

        if (hasGuess && guessMarker != null && connector != null)
            SetConnector(guessMarker.anchoredPosition, answerMarker.anchoredPosition);
        else
            Hide(connector);
    }

    public void ClearMarkers()
    {
        Hide(guessMarker);
        Hide(answerMarker);
        Hide(connector);
    }

    private Vector2 LatLonToNormalized(float latitude, float longitude)
    {
        return new Vector2((longitude + 180f) / 360f, (latitude + 90f) / 180f);
    }

    private void PlaceMarker(RectTransform marker, float x, float y)
    {
        marker.anchorMin = new Vector2(x, y);
        marker.anchorMax = new Vector2(x, y);
        marker.anchoredPosition = Vector2.zero;
        marker.gameObject.SetActive(true);
    }

    private void SetConnector(Vector2 from, Vector2 to)
    {
        Vector2 delta = to - from;
        connector.anchorMin = connector.anchorMax = new Vector2(0.5f, 0.5f);
        connector.pivot = new Vector2(0f, 0.5f);
        connector.anchoredPosition = (from + to) * 0.5f;
        connector.sizeDelta = new Vector2(delta.magnitude, Mathf.Max(2f, connector.sizeDelta.y));
        connector.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        connector.gameObject.SetActive(true);
    }

    private static void Hide(RectTransform target)
    {
        if (target != null) target.gameObject.SetActive(false);
    }
}
