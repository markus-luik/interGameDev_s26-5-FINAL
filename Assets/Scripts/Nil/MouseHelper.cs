using UnityEngine;

public static class MouseHelper
{
    public static Vector2 GetPlayerToMouseVector2D(Transform playerTransform, Camera targetCamera = null)
    {
        // Validate required references.
        if (playerTransform == null)
        {
            return Vector2.zero;
        }

        // Use provided camera, fallback to main camera.
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return Vector2.zero;
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        // Set depth before converting screen coordinates to world space.
        float zDistance = cam.orthographic ? 0f : Mathf.Abs(playerTransform.position.z - cam.transform.position.z);
        mouseScreenPos.z = zDistance;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        // Return vector from player to mouse in world space.
        return mouseWorldPos - playerTransform.position;
    }

    public static Vector2 GetDirectionToMouse2D(Transform playerTransform, Camera targetCamera = null)
    {
        Vector2 direction = GetPlayerToMouseVector2D(playerTransform, targetCamera);
        // Normalize safely.
        return direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
    }

    public static float GetAngleToMouse2D(Transform playerTransform, Camera targetCamera = null)
    {
        Vector2 direction = GetDirectionToMouse2D(playerTransform, targetCamera);
        // Convert direction to angle in degrees.
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
