using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float mouseOffsetMax = 1.25f;
    [SerializeField]
    private float mouseFollowSmooth = 8f;
    [SerializeField]
    private float shiftMoveDistance = 10f;
    [SerializeField]
    private float shiftMoveSmooth = 12f;
    private Vector3 smoothOffset = Vector3.zero;
    private float fixedZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fixedZ = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isShiftPressed)
        {
            CameraMoveToMouseDirection();
            return;
        }

        CameraSmoothToMouse();
    }

    //camera move subtly from direction around mouse
    private void CameraSmoothToMouse(){
        //player to mouse dir normalized
        // map distance to camera target distance
        // slerp
        if (player == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 fromPlayerToMouse = MouseHelper.GetPlayerToMouseVector2D(player.transform, cam);

        float distance = fromPlayerToMouse.magnitude;
        Vector2 dir = MouseHelper.GetDirectionToMouse2D(player.transform, cam);
        float mappedDist = Mathf.Clamp(distance * 0.2f, 0f, mouseOffsetMax);
        Vector3 targetOffset = new Vector3(dir.x, dir.y, 0f) * mappedDist;

        smoothOffset = Vector3.Lerp(smoothOffset, targetOffset, Time.deltaTime * mouseFollowSmooth);
        Vector3 targetPosition = player.transform.position + smoothOffset;
        targetPosition.z = fixedZ;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * mouseFollowSmooth);
    }

    private void CameraMoveToMouseDirection(){
        if (player == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        // Move camera toward mouse direction while Shift is held.
        Vector2 dir = MouseHelper.GetDirectionToMouse2D(player.transform, cam);
        Vector3 targetPosition = player.transform.position + (Vector3)(dir * shiftMoveDistance);
        targetPosition.z = fixedZ;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * shiftMoveSmooth);
    }
}
