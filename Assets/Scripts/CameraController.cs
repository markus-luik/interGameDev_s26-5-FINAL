using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float mouseOffsetMax = 1.25f;
    [SerializeField]
    private float mouseFollowSmooth = 8f;
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
        CameraSnapToPlayer();
        CameraSmoothToMouse();
    }

    //camera snap to player 2d pos
    private void CameraSnapToPlayer(){
        if (player == null) return;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, fixedZ);
    }
    //camera move subtly from direction around mouse
    private void CameraSmoothToMouse(){
        //player to mouse dir normalized
        // map distance to camera target distance
        // slerp
        if (player == null) return;
        if (Camera.main == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.transform.position.z;
        Vector2 fromPlayerToMouse = (Vector2)(mouseWorld - player.transform.position);

        float distance = fromPlayerToMouse.magnitude;
        Vector2 dir = distance > 0.0001f ? fromPlayerToMouse.normalized : Vector2.zero;
        float mappedDist = Mathf.Clamp(distance * 0.2f, 0f, mouseOffsetMax);
        Vector3 targetOffset = new Vector3(dir.x, dir.y, 0f) * mappedDist;

        smoothOffset = Vector3.Lerp(smoothOffset, targetOffset, Time.deltaTime * mouseFollowSmooth);
        transform.position += smoothOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y, fixedZ);
    }
}
