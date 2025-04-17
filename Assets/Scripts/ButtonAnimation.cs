using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private const float scaleDownMultiplier = 0.7f;
    private const float scaleUpMultiplier = 1.2f;

    private Vector2 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = defaultScale * scaleUpMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = defaultScale * scaleDownMultiplier;
    }
}
