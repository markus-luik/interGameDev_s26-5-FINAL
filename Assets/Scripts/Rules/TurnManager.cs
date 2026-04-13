using UnityEngine;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance;

    [SerializeField]
    private bool bypassPlayerInput = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update() {
        if (!bypassPlayerInput) return;

        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.W)) dir = new Vector2Int(0, -1);
        else if (Input.GetKeyDown(KeyCode.S)) dir = new Vector2Int(0, 1);

        if (dir == Vector2Int.zero) return;

        ExecuteTurn(dir);
    }

    public void ExecuteTurn(Vector2Int dir) {
        EntityMover.Instance.MoveAllYou(dir);
        RuleParser.Instance.ParseAllRules();
        CheckWin();
    }

    private void CheckWin() {
        foreach (var entity in BabaGridIndex.Instance.GetAll()) {
            if (!entity.isYou) continue;

            // Check if this YOU entity is itself WIN
            if (entity.isWin) {
                Debug.Log("YOU WIN — you are win!");
                return;
            }

            // Check if a WIN entity shares this cell
            foreach (var occupant in BabaGridIndex.Instance.GetAt(entity.gridPos)) {
                if (occupant != entity && occupant.isWin) {
                    Debug.Log("YOU WIN — stepped on win!");
                    return;
                }
            }
        }
    }
}