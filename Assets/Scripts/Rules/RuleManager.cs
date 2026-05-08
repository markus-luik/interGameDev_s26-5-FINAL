using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<string> rules = new(); //rules list

    public int AddToList(string rule, int index)
    {
        //BUG ADDING IS CURRENTLY INDEX BASED BUT REMOVING CHANGES THE INDEXES. SOLUTIONS: CLEARING RULE LIST EACH PARSE or 
        // CHANGING REMOVING TO BE REPLACING WITH ""  
        if (index < 0){ //if Operator has never added a rule to list, create new index
            rules.Add(rule); //adds new rule to list
            if (rules.Count > 0){ //Checks if list has stuff in it!
                int indexOfRule = rules.Count - 1; //gets index of rule
                return indexOfRule; //return index of rule
            }
            else //Just in case
            {
                return -1;
            }
        }
        else
        {
            rules[index] =  rule; //if Operator already has added something to the list, add rule at same index as given
            
            return index; //return same index
        }
    }

    public void RemoveFromList(int index)
    {
        if (index >= 0){ //If operator not -1
            rules[index] = "";
        }
    }

    public void ResetList()
    {
        rules.Clear();
    }
}
