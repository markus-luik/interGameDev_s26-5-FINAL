using System;
using UnityEngine;

public class OperatorBehavior : MonoBehaviour
{
    [Header("Raycast variables")]
    [SerializeField] private int rayCount = 4;
    [SerializeField] private float rayCheckDist = 0.25f;
    [SerializeField] private float rayCheckGap = 0.01f;
    private float myColliderRadius;

    private void Awake()
    {
        
    }

    private void Update()
    {  
        RayCheckMyNeighbour();
    }

    void RayCheckMyNeighbour()
    {
        myColliderRadius = GetComponent<BoxCollider2D>().bounds.extents.x + rayCheckGap;
        float rayGap = 360f / rayCount;
        
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 0 + i * rayGap; //Calculates angle
            Vector2 direction = Quaternion.Euler(0,0,angle)  * Vector3.up; //calculates direction by adding angle to up direction (like clock ticking right)
            Vector2 rayOrigin = (Vector2)transform.position + direction * myColliderRadius; //starts ray at 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayCheckDist); //Shoots ray and returns what is hit

            if (hit.collider != null)
            {
                Debug.Log("Hello neighbour!" + hit.collider.name);
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        float rayGap = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 0 + i * rayGap; //Calculates angle
            Vector2 direction = Quaternion.Euler(0,0,angle)  * Vector3.up; //calculates direction by adding angle to up direction (like clock ticking right)
            Vector2 rayOrigin = (Vector2)transform.position + direction * myColliderRadius;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayCheckDist); //Shoots ray and returns what is hit
            
            Gizmos.color = hit.collider != null ? Color.red : Color.green;
            Gizmos.DrawRay(rayOrigin, direction * rayCheckDist);
            

        }
    }
}
