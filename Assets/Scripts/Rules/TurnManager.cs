// using UnityEngine;
// using System.Linq;
//
// /// <summary>
// /// Reads player input, triggers NewBlock2D movement, re-parses rules, checks win.
// /// All movement and push logic lives in NewBlock2D.
// /// </summary>
// public class TurnManager : MonoBehaviour {
//     public static TurnManager Instance;
//
//     private void Awake() {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }
//
//     private void Update() {
//         Vector2Int dir = Vector2Int.zero;
//
//         if      (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
//         else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;
//         else if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
//         else if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;
//
//         if (dir != Vector2Int.zero)
//             ExecuteTurn(dir);
//     }
//
//     public void ExecuteTurn(Vector2Int dir) {
//         // Move every YOU entity — NewBlock2D handles push chains internally
//         foreach (var entity in BabaGridIndex.Instance.GetAll().Where(e => e.isYou).ToList()) {
//             var block = entity.GetComponent<NewBlock2D>();
//             block?.TryMove(dir.x, dir.y);
//         }
//
//         RuleParser.Instance.ParseAllRules();
//         CheckWin();
//     }
//
//     private void CheckWin() {
//         foreach (var entity in BabaGridIndex.Instance.GetAll()) {
//             if (!entity.isYou) continue;
//
//             if (entity.isWin) {
//                 Debug.Log("YOU WIN — you are win!");
//                 return;
//             }
//
//             foreach (var occupant in BabaGridIndex.Instance.GetAt(entity.gridPos)) {
//                 if (occupant != entity && occupant.isWin) {
//                     Debug.Log("YOU WIN — stepped on win!");
//                     return;
//                 }
//             }
//         }
//     }
// }