using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_camera == null) return;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 7.7f;
        transform.position = _camera.ScreenToWorldPoint(mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // For now we'll snap the card back to its original position when the drag ends
        transform.position = originalPosition;
    }
}
