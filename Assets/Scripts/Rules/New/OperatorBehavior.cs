using System;
using System.Collections.Generic;
using UnityEngine;

public class OperatorBehavior : MonoBehaviour
{
    [SerializeField, ReadOnly] private string operatorName = "IS";
    [Header("Raycast variables")]
    [SerializeField] private int rayCount = 4;
    [SerializeField] private float rayCheckDist = 0.25f;
    [SerializeField] private float rayCheckGap = 0.01f;
    private float myColliderRadius;
    [SerializeField, ReadOnly] private List<string> Neighbours = new(); //Neighbour list
    [SerializeField, ReadOnly] private List<string> RuleHor = new(); //Horizontal Rule list
    [SerializeField, ReadOnly] private List<string> RuleVer = new(); //Vertical Rule list
    

    private void Awake()
    {
        RuleHor = new List<string>(new string[3]); //Fills list with empty strings
        RuleHor[1] = operatorName; //Adds operator name to second position in list (so "__ is __")
        RuleVer = new List<string>(new string[3]);
        RuleVer[1] = operatorName;
    }

    private void Update()
    {  
        RayCheckMyNeighbour();
    }

    void RayCheckMyNeighbour()
    {
        myColliderRadius = GetComponent<BoxCollider2D>().bounds.extents.x + rayCheckGap;
        float rayGap = 360f / rayCount; //divides rays by circle so there can be more than 4 rays! On a square, NOT advisable
        
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 0 + i * rayGap; //Calculates angle
            Vector2 direction = Quaternion.Euler(0,0,-angle)  * Vector3.up; //Note: Unity uses left-handed coordinates so angle is negative //calculates direction by adding angle to up direction (like clock ticking right)
            Vector2 rayOrigin = (Vector2)transform.position + direction * myColliderRadius; //starts ray at 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayCheckDist); //Shoots ray and returns what is hit

            if (hit.collider != null) //Checks if actually hit something with a collider
            {
                GridEntity entityHit = hit.collider.GetComponent<GridEntity>(); //Tries to find the object's entity info
                if (entityHit != null && entityHit.type.isTextBlock) //Proceeds if info exists and obj is text
                {
                    //Debug.Log("Hello neighbour " + entityHit.type.entityName + " at angle " +  angle); //DEBUG
                    Neighbours.Add(entityHit.type.entityName); //Records entity's name in neighbour list
                }
                else
                {
                    Neighbours.Add(""); //Adds nothing to the list if no hit at given angle
                }
            }
            else
            {
                Neighbours.Add(""); //Adds nothing to the list if no hit at given angle
            }

        }
        
        //Updating rules with hits
        RuleVer[0] = Neighbours[0]; //Vertical
        RuleVer[2] = Neighbours[2];
        RuleHor[0] = Neighbours[3]; //Vertical
        RuleHor[2] = Neighbours[1];
        
        //Debug.Log("Hits " + string.Join(", ", Neighbours)); //DEBUG
        Debug.Log("Horizontal Rule " + string.Join(" ", RuleHor));
        Debug.Log("Vertical Rule " + string.Join(" ", RuleVer));
        Neighbours.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        float rayGap = 360f / rayCount;
        for (int i = 0; i < rayCount; i++)
        {
            float angle = 0 + i * rayGap; //Calculates angle
            Vector2 direction = Quaternion.Euler(0,0,-angle)  * Vector3.up; //calculates direction by adding angle to up direction (like clock ticking right)
            Vector2 rayOrigin = (Vector2)transform.position + direction * myColliderRadius;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayCheckDist); //Shoots ray and returns what is hit
            
            Gizmos.color = hit.collider != null ? Color.red : Color.green;
            Gizmos.DrawRay(rayOrigin, direction * rayCheckDist);
            

        }
    }
}
