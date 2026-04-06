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
    private LayerMask mousePlaneMask = ~0;
    private Vector3 smoothOffset = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraSnapToPlayer();
        CameraSmoothToMouse();
    }

    //camera snap to player 2d pos
    private void CameraSnapToPlayer(){
        transform.position = new Vector3(player.transform.position.x,10f,player.transform.position.z);
    }
    //camera move subtly from direction around mouse
    private void CameraSmoothToMouse(){
        //player to mouse dir normalized
        // map distance to camera target distance
        // slerp
        if (player == null) return;
        if (Camera.main == null) return;

        Plane movePlane = new Plane(Vector3.up, new Vector3(0f, player.transform.position.y, 0f));
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!movePlane.Raycast(mouseRay, out float enter)) return;

        Vector3 mouseWorld = mouseRay.GetPoint(enter);
        Vector3 fromPlayerToMouse = mouseWorld - player.transform.position;
        fromPlayerToMouse.y = 0f;

        float distance = fromPlayerToMouse.magnitude;
        Vector3 dir = distance > 0.0001f ? fromPlayerToMouse.normalized : Vector3.zero;
        float mappedDist = Mathf.Clamp(distance * 0.2f, 0f, mouseOffsetMax);
        Vector3 targetOffset = dir * mappedDist;

        smoothOffset = Vector3.Lerp(smoothOffset, targetOffset, Time.deltaTime * mouseFollowSmooth);
        transform.position += smoothOffset;
    }
}
