using UnityEngine;

public class OperatorBehavior : MonoBehaviour
{
    [Header("Raycast variables")]
    [SerializeField] private int rayCount = 4;
    private float rayCheckDist = 0.25f;

    void RayCheckMyNeighbour()
    {
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 0 + i * 90f;
            
            Vector2 direction = Quaternion.Euler(0,0,angle)  * Vector3.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayCheckDist);

            if (hit.collider != null)
            {
                Debug.Log("Hello world!");
            }

        }
    }
}
