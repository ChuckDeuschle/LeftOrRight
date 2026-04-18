using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        if (image != null)
        {
            image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 7.7f;
        transform.position = cam.ScreenToWorldPoint(mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (image != null)
        {
            image.raycastTarget = true;
        }
        // For now we'll snap the card back to its original position when the drag ends
        transform.position = originalPosition;
    }
}
