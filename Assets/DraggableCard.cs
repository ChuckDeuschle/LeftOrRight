using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Image image;
    private Collider meshCollider;

    private void Awake()
    {
        image = GetComponent<Image>();
        meshCollider = GetComponent<Collider>();
    }

    // OnEndDrag doesn't fire when the card is SetActive(false) by DropZone mid-drag,
    // so the collider would stay disabled across a deck cycle. Re-enable on every
    // activation (DrawCard re-activates the card) to guarantee it's pickable.
    private void OnEnable()
    {
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        if (image != null)
        {
            image.raycastTarget = false;
        }
        // Disable the card's own collider during drag so the PhysicsRaycaster
        // can see through it to the drop zones behind. Re-enabled in OnEndDrag.
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
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
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }
        transform.position = originalPosition;
    }
}
