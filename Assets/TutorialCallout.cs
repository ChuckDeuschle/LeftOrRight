using UnityEngine;

// Keeps a tutorial callout positioned relative to a live target so the arrows stay
// aligned as the UI scales for different resolutions. Set either uiTarget (a UI
// RectTransform — player health, enemy status, etc.) or worldTarget (a world-space
// Transform — the current Card, for example).
//
// pixelOffset is specified in the design canvas's logical units (1280x720 reference).
// At runtime it is multiplied by the parent Canvas's scaleFactor so the offset tracks
// the element's rendered size — otherwise a 160-pixel gap designed at 720p would look
// cramped at 1080p (where UI elements render 1.5x larger but a raw 160 screen-pixel
// gap stays the same).
public class TutorialCallout : MonoBehaviour
{
    public RectTransform uiTarget;
    public Transform worldTarget;
    public Camera worldCamera;
    public Vector2 pixelOffset;

    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void LateUpdate()
    {
        float scale = parentCanvas != null ? parentCanvas.scaleFactor : 1f;
        Vector2 scaledOffset = pixelOffset * scale;

        if (uiTarget != null)
        {
            // Screen Space Overlay: RectTransform.position is already in screen pixels.
            transform.position = uiTarget.position + (Vector3)scaledOffset;
            return;
        }

        if (worldTarget != null)
        {
            Camera cam = worldCamera != null ? worldCamera : Camera.main;
            if (cam == null) { return; }
            Vector3 screen = cam.WorldToScreenPoint(worldTarget.position);
            if (screen.z < 0f) { return; }
            transform.position = new Vector3(screen.x + scaledOffset.x, screen.y + scaledOffset.y, 0f);
        }
    }
}
