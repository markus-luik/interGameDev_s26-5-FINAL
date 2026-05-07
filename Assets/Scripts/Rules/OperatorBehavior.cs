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
    [SerializeField, ReadOnly] private List<string> neighbours = new(); //Neighbour list
    [SerializeField, ReadOnly] private List<string> ruleHor = new(); //Horizontal Rule list
    private int ruleHorIndex = -1;
    [SerializeField, ReadOnly] private List<string> ruleVer = new(); //Vertical Rule list
    private int ruleVerIndex = -1;
    
    RuleManager _ruleManager;
    

    private void Awake()
    {
        //Setting up rules lists
        ruleHor = new List<string>(new string[3]); //Fills list with empty strings
        ruleHor[1] = operatorName; //Adds operator name to second position in list (so "__ is __")
        ruleVer = new List<string>(new string[3]);
        ruleVer[1] = operatorName;
        
        //Find RuleManager
        _ruleManager = FindObjectOfType<RuleManager>();
        if  (_ruleManager == null) Debug.Log("No RuleManager found on Operator! Add RuleManager.cs as object in scene.");
    }

    private void Update()
    {   
        if(Input.GetKeyDown(KeyCode.N)){
            RayCheckMyNeighbour();
        }
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
                    neighbours.Add(entityHit.type.entityName); //Records entity's name in neighbour list
                }
                else
                {
                    neighbours.Add(""); //Adds nothing to the list if no hit at given angle
                }
            }
            else
            {
                neighbours.Add(""); //Adds nothing to the list if no hit at given angle
            }

        }
        
        //Updating rules with hits
        ruleVer[0] = neighbours[0]; //Vertical
        ruleVer[2] = neighbours[2];
        ruleHor[0] = neighbours[3]; //Vertical
        ruleHor[2] = neighbours[1];
        
        //DEBUG
        //Debug.Log("Hits " + string.Join(", ", neighbours));
        //Debug.Log("Horizontal Rule " + string.Join(" ", ruleHor));
        //Debug.Log("Vertical Rule " + string.Join(" ", ruleVer));
            
        //Updates rules in RuleManager
        if (_ruleManager != null){
            //ADD complete rules to list, REMOVE incomplete rules
            if (!ruleVer.Contains(null) && !ruleVer.Contains("")) //VERTICAL 
            {
                string verticalRule = string.Join(" ", ruleVer);
                //Debug.Log(verticalRule); //DEBUG
                ruleVerIndex = _ruleManager.AddToList(verticalRule, ruleVerIndex); //add
            }
            else
            {
                _ruleManager.RemoveFromList(ruleVerIndex); //remove but retain index
            }
            
            if (!ruleHor.Contains(null) && !ruleHor.Contains("")) //HORIZONTAL
            {
                string horizontalRule = string.Join(" ", ruleHor);
                //Debug.Log(horizontalRule); //DEBUG
                ruleHorIndex = _ruleManager.AddToList(horizontalRule, ruleHorIndex); //add
            }
            else
            {
                _ruleManager.RemoveFromList(ruleHorIndex); //remove but retain index
            }
        }
        
        neighbours.Clear();
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
