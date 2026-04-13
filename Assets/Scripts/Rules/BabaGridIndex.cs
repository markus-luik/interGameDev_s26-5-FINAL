using UnityEngine;
using System.Collections.Generic;

public class BabaGridIndex : MonoBehaviour {
    public static BabaGridIndex Instance;

    // Our parallel grid — multiple entities can share a cell
    private Dictionary<Vector2Int, List<GridEntity>> _cells = new();
    
    private List<GridEntity> _allEntities = new();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Called by GridEntity.Start() for every entity in the scene
    public void Register(GridEntity entity) {
        if (!_allEntities.Contains(entity))
            _allEntities.Add(entity);
        if (!_cells.ContainsKey(entity.gridPos))
            _cells[entity.gridPos] = new List<GridEntity>();
        _cells[entity.gridPos].Add(entity);
    }

    public void Unregister(GridEntity entity) {
        _allEntities.Remove(entity);
        if (_cells.ContainsKey(entity.gridPos))
            _cells[entity.gridPos].Remove(entity);
    }

    // // Move an entity from one cell to another in our index
    // public void UpdatePosition(GridEntity entity, Vector2Int oldPos, Vector2Int newPos) {
    //     Unregister(entity);
    //     entity.gridPos = newPos;
    //     Register(entity);
    // }

    // Get all entities at a position
    public List<GridEntity> GetAt(Vector2Int pos) {
        if (_cells.TryGetValue(pos, out var list))
            return list;
        return new List<GridEntity>();
    }

    // Get every registered entity — used by RuleParser
    public IEnumerable<GridEntity> GetAll() {
        return _allEntities;
    }

    // Called via BroadcastMessage from GridManager.UpdateGrid()
    public void GridChanged() {
        RebuildIndex();
    }

    // Rebuild from scratch by asking every GridEntity for its current position
    // Replace RebuildIndex with this
    private void RebuildIndex() {
        _cells.Clear();
        foreach (var entity in _allEntities) {
            var block = entity.GetComponent<Block2D>();
            if (block != null)
                entity.gridPos = block.gridPos;
            Register(entity);
        }
    }
    
    
    [ContextMenu("(Debug) Print all Entity locations")]
    public void DebugPrintAll() {
        foreach (var entity in GetAll()) {
            Debug.Log($"{entity.type.entityName} at {entity.gridPos}");
        }
    }
}