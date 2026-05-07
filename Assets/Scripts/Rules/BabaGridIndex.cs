// using UnityEngine;
// using System.Collections.Generic;
//
// public class BabaGridIndex : MonoBehaviour {
//     public static BabaGridIndex Instance;
//
//     private Dictionary<Vector2Int, List<GridEntity>> _cells = new();
//     private List<GridEntity> _allEntities = new();
//
//     private NewGridManager2D _grid;
//
//     private void Awake() {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//         _grid = FindObjectOfType<NewGridManager2D>();
//     }
//
//     public void Register(GridEntity entity) {
//         if (!_allEntities.Contains(entity))
//             _allEntities.Add(entity);
//         if (!_cells.ContainsKey(entity.gridPos))
//             _cells[entity.gridPos] = new List<GridEntity>();
//         _cells[entity.gridPos].Add(entity);
//     }
//
//     public void Unregister(GridEntity entity) {
//         _allEntities.Remove(entity);
//         if (_cells.ContainsKey(entity.gridPos))
//             _cells[entity.gridPos].Remove(entity);
//     }
//
//     public List<GridEntity> GetAt(Vector2Int pos) {
//         if (_cells.TryGetValue(pos, out var list))
//             return list;
//         return new List<GridEntity>();
//     }
//
//     public IEnumerable<GridEntity> GetAll() {
//         return _allEntities;
//     }
//
//     public void GridChanged() {
//         RebuildIndex();
//     }
//
//     private void RebuildIndex() {
//         _cells.Clear();
//         foreach (var entity in _allEntities) {
//             // Sync gridPos from NewBlock2D's world position via the grid snapper
//             var block = entity.GetComponent<NewBlock2D>();
//             if (block != null && _grid != null) {
//                 Vector2 snapped = _grid.SnapToGrid(block.TargetPos);
//                 entity.gridPos = new Vector2Int(
//                     Mathf.RoundToInt(snapped.x),
//                     Mathf.RoundToInt(snapped.y)
//                 );
//             }
//             Register(entity);
//         }
//     }
//
//     [ContextMenu("(Debug) Print all Entity locations")]
//     public void DebugPrintAll() {
//         foreach (var entity in GetAll())
//             Debug.Log($"{entity.type.entityName} at {entity.gridPos}");
//     }
// }