using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<string> rules = new(); //rules list
    
    private Dictionary<string, (string who, string what)> activeRules = new();
    private Dictionary<string, (string who, string what)> previousActiveRules = new();
    
    EventBroadcaster _eventBroadcaster;
    
    private void Awake()
    {
        //Find EventBroadcaster
        _eventBroadcaster = FindObjectOfType<EventBroadcaster>();
        if  (_eventBroadcaster == null) Debug.Log("No EventBroadcaster found on RuleManager! Add EventBroadcaster.cs as object in scene.");
    }
    
    public int AddToList(string rule, int index)
    {
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

    public (string who, string what) ParseRule(string rule, int index)
    {
        if (string.IsNullOrEmpty(rule)) return (null, null);
        
        string[] ruleSplit = rule.Split(' '); // split rule by space
        
        if (ruleSplit[0] == "IS" || ruleSplit[2] == "IS") //Check rule validty
        {
            RemoveFromList(index);
            return (null, null);
        }else if (ruleSplit[1] != "IS")
        {
            RemoveFromList(index);
            return (null, null);
        }
        
        return (ruleSplit[0], ruleSplit[2]);
    }

    public void Update()
    {
        if (_eventBroadcaster == null)
        {
            Debug.Log("RuleManager can not broadcast event! No EventBroadcaster found.");
            return;
        }

        activeRules.Clear();

        for (int i = 0; i < rules.Count; i++)
        {
            var (whoIs, whatIs) = ParseRule(rules[i], i);

            if (string.IsNullOrEmpty(whoIs) || string.IsNullOrEmpty(whatIs))
            {
                continue;
            }

            string ruleKey = GetRuleKey(whoIs, whatIs);
            activeRules[ruleKey] = (whoIs, whatIs);
        }

        BroadcastRuleChanges();

        previousActiveRules.Clear();

        foreach (var rule in activeRules)
        {
            previousActiveRules[rule.Key] = rule.Value;
        }
    }

    private void BroadcastRuleChanges()
    {
        foreach (var rule in activeRules)
        {
            if (!previousActiveRules.ContainsKey(rule.Key))
            {
                _eventBroadcaster.ParseEvent(rule.Value.who, rule.Value.what, true);
            }
        }

        foreach (var rule in previousActiveRules)
        {
            if (!activeRules.ContainsKey(rule.Key))
            {
                _eventBroadcaster.ParseEvent(rule.Value.who, rule.Value.what, false);
            }
        }
    }

    private string GetRuleKey(string who, string what)
    {
        return $"{who} IS {what}";
    }
}
