using UnityEngine;

//This is all by Claude
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
        // Text blocks are always pushable — hardcoded, not rule-derived
        if (type != null && type.isTextBlock)
            isPush = true;
    }

    private void Start() {
        Block block = GetComponent<Block>();
        if (block != null) {
            gridPos = block.gridPos;
        } else {
            Block2D block2D = GetComponent<Block2D>();
            if (block2D != null)
                gridPos = block2D.gridPos;
        }
        BabaGridIndex.Instance.Register(this);
    }

    public void ClearProperties() {
        isYou = isWin = isStop = isSink = isDefeat = false;
        // Text blocks keep isPush — reset it only for non-text entities
        if (type != null && !type.isTextBlock)
            isPush = false;
    }
}