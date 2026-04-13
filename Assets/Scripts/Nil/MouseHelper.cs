using UnityEngine;

public static class MouseHelper
{
    public static Vector2 GetPlayerToMouseVector2D(Transform playerTransform, Camera targetCamera = null)
    {
        // 1) 防御式判断：没有玩家 Transform 时，无法计算方向。
        if (playerTransform == null)
        {
            return Vector2.zero;
        }

        // 2) 选择用于坐标转换的相机：优先用传入相机，否则退回主相机。
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
        {
            return Vector2.zero;
        }

        // 3) 先拿到鼠标的屏幕坐标（像素坐标）。
        Vector3 mouseScreenPos = Input.mousePosition;
        // 4) 对透视相机，需要指定距离相机多远的深度；2D 正交相机用 0 即可。
        float zDistance = cam.orthographic ? 0f : Mathf.Abs(playerTransform.position.z - cam.transform.position.z);
        mouseScreenPos.z = zDistance;

        // 5) 把鼠标从屏幕坐标转换到世界坐标（保证与玩家坐标在同一空间）。
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        // 6) 方向向量 = 目标点（鼠标）- 起点（玩家）。
        return mouseWorldPos - playerTransform.position;
    }

    public static Vector2 GetDirectionToMouse2D(Transform playerTransform, Camera targetCamera = null)
    {
        Vector2 direction = GetPlayerToMouseVector2D(playerTransform, targetCamera);
        // 归一化后返回纯方向（长度为 1）；避免零向量导致异常。
        return direction.sqrMagnitude > 0f ? direction.normalized : Vector2.zero;
    }

    public static float GetAngleToMouse2D(Transform playerTransform, Camera targetCamera = null)
    {
        // 1) 先复用方向方法，确保角度与方向逻辑一致。
        Vector2 direction = GetDirectionToMouse2D(playerTransform, targetCamera);
        // 2) atan2 得到弧度角，再转为 Unity 常用的角度（度数）。
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
