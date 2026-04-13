using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RuleParser : MonoBehaviour {
    public static RuleParser Instance;

    // Stores active noun -> property rules after each parse
    // e.g. "BABA" -> [YOU, WIN]
    private Dictionary<string, List<string>> _activeRules = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    //Runs one frame after all the other starts (after BabaGridIndex is full) before checking for rules
    //BUT this is a bit risky.
    private IEnumerator Start() {
        yield return null; // wait one frame for all GridEntity.Start() calls to finish
        ParseAllRules();
    }

    public void ParseAllRules() {
        // Step 1 — clear all runtime properties on every entity
        foreach (var entity in BabaGridIndex.Instance.GetAll())
            entity.ClearProperties();

        // Step 2 — clear last turn's rule dictionary
        _activeRules.Clear();

        // Step 3 — scan every entity for noun text blocks
        foreach (var entity in BabaGridIndex.Instance.GetAll()) {
            if (!entity.type.isTextBlock) continue;
            if (entity.type.entityName == "IS") continue;
            if (entity.type.entityName == "YOU" ||
                entity.type.entityName == "WIN" ||
                entity.type.entityName == "PUSH" ||
                entity.type.entityName == "STOP" ||
                entity.type.entityName == "SINK" ||
                entity.type.entityName == "DEFEAT") continue;

            // This entity is a noun — check right and down for IS + PROPERTY
            TryMatchRule(entity, Vector2Int.right);
            TryMatchRule(entity, Vector2Int.up);
        }

        // Step 4 — apply collected rules to all entities
        ApplyRules();
    }

    private void TryMatchRule(GridEntity noun, Vector2Int dir) {
        var isPos = noun.gridPos + dir;
        var propPos = noun.gridPos + dir * 2;

        // Find IS at isPos
        var isBlock = BabaGridIndex.Instance.GetAt(isPos)
            .Find(e => e.type.isTextBlock && e.type.entityName == "IS");
        if (isBlock == null) return;

        // Find a property or noun at propPos
        var propBlock = BabaGridIndex.Instance.GetAt(propPos)
            .Find(e => e.type.isTextBlock && e.type.entityName != "IS");
        if (propBlock == null) return;

        // Valid rule found — store it
        if (!_activeRules.ContainsKey(noun.type.entityName))
            _activeRules[noun.type.entityName] = new List<string>();

        _activeRules[noun.type.entityName].Add(propBlock.type.entityName);

        Debug.Log($"Rule found: {noun.type.entityName} IS {propBlock.type.entityName}");
    }

    private void ApplyRules() {
        foreach (var entity in BabaGridIndex.Instance.GetAll()) {
            if (entity.type.isTextBlock) continue;

            if (!_activeRules.TryGetValue(entity.type.entityName, out var props))
                continue;

            foreach (var prop in props) {
                switch (prop) {
                    case "YOU":    entity.isYou    = true; break;
                    case "WIN":    entity.isWin    = true; break;
                    case "PUSH":   entity.isPush   = true; break;
                    case "STOP":   entity.isStop   = true; break;
                    case "SINK":   entity.isSink   = true; break;
                    case "DEFEAT": entity.isDefeat = true; break;
                }
            }
        }
    }

    // Called via BroadcastMessage from GridManager.UpdateGrid()
    public void GridChanged() {
        ParseAllRules();
    }
}