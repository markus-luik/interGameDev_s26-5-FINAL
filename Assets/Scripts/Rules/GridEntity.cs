using UnityEngine;

public class GridEntity : MonoBehaviour {
    [Header("Entity definition")]
    public EntityType type;

    [Header("Grid position")]
    public Vector2Int gridPos;

    // Runtime properties — reset and repopulated every turn by RuleParser
    [HideInInspector] public bool isYou;
    [HideInInspector] public bool isWin;
    [HideInInspector] public bool isPush;
    [HideInInspector] public bool isStop;
    [HideInInspector] public bool isSink;
    [HideInInspector] public bool isDefeat;

    private void Awake() {
        if (type != null && type.isTextBlock)
            isPush = true;
    }

    private void Start() {
        // Derive gridPos from NewBlock2D's snapped world position
        var block = GetComponent<NewBlock2D>();
        if (block != null) {
            var grid = FindObjectOfType<NewGridManager2D>();
            if (grid != null) {
                Vector2 snapped = grid.SnapToGrid(block.TargetPos);
                gridPos = new Vector2Int(Mathf.RoundToInt(snapped.x), Mathf.RoundToInt(snapped.y));
            }
        }
        BabaGridIndex.Instance.Register(this);
    }

    public void ClearProperties() {
        isYou = isWin = isStop = isSink = isDefeat = false;
        if (type != null && !type.isTextBlock)
            isPush = false;
    }
}