using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public sealed class MapGuessInput : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GeoGuessrGameManager gameManager;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RectTransform guessMarker;

    private void Awake()
    {
        if (mapRect == null) mapRect = transform as RectTransform;
        GetComponent<Graphic>().raycastTarget = true;
        if (guessMarker != null) guessMarker.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager == null || mapRect == null || guessMarker == null) return;
        Camera eventCamera = eventData.pressEventCamera;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, eventData.position, eventCamera, out Vector2 local))
            return;

        Rect rect = mapRect.rect;
        float x = Mathf.Clamp01((local.x - rect.xMin) / rect.width);
        float y = Mathf.Clamp01((local.y - rect.yMin) / rect.height);
        float longitude = x * 360f - 180f;
        float latitude = y * 180f - 90f;

        if (!gameManager.PlaceGuess(latitude, longitude)) return;

        guessMarker.anchorMin = new Vector2(x, y);
        guessMarker.anchorMax = new Vector2(x, y);
        guessMarker.anchoredPosition = Vector2.zero;
        guessMarker.gameObject.SetActive(true);
    }

    public void ClearMarker()
    {
        if (guessMarker != null) guessMarker.gameObject.SetActive(false);
    }
}
