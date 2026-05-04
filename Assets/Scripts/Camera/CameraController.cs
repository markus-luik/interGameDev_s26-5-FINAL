using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float mouseOffsetMax = 1.25f;
    [SerializeField]
    private float mouseFollowSmooth = 30f;
    [SerializeField]
    private float maxFollowOffset = 0.12f;
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
        if (player == null) return;
        smoothOffset = Vector3.zero;
        Vector3 targetPosition = player.transform.position;
        targetPosition.z = fixedZ;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * mouseFollowSmooth);
        Vector3 toTarget = targetPosition - smoothedPosition;
        if (toTarget.sqrMagnitude > maxFollowOffset * maxFollowOffset)
        {
            smoothedPosition = targetPosition - toTarget.normalized * maxFollowOffset;
        }

        transform.position = smoothedPosition;
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
