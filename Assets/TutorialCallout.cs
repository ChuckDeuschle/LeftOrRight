using UnityEngine;

// Keeps a tutorial callout positioned relative to a live target so the arrows stay
// aligned as the UI scales for different resolutions. Set either uiTarget (a UI
// RectTransform — player health, enemy status, etc.) or worldTarget (a world-space
// Transform — the current Card, for example). pixelOffset is applied in screen pixels.
public class TutorialCallout : MonoBehaviour
{
    public RectTransform uiTarget;
    public Transform worldTarget;
    public Camera worldCamera;
    public Vector2 pixelOffset;

    private void LateUpdate()
    {
        if (uiTarget != null)
        {
            // Screen Space Overlay: RectTransform.position is already in screen pixels,
            // so a pixel offset applied directly produces the expected screen shift.
            transform.position = uiTarget.position + (Vector3)pixelOffset;
            return;
        }

        if (worldTarget != null)
        {
            Camera cam = worldCamera != null ? worldCamera : Camera.main;
            if (cam == null) { return; }
            Vector3 screen = cam.WorldToScreenPoint(worldTarget.position);
            if (screen.z < 0f) { return; }
            transform.position = new Vector3(screen.x + pixelOffset.x, screen.y + pixelOffset.y, 0f);
        }
    }
}
