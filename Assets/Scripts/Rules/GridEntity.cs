using UnityEngine;

public class GridEntity : MonoBehaviour {
    [Header("Entity definition")]
    public EntityType type;

    // Runtime properties — reset and repopulated every turn by RuleParser
    public bool isYou;
    public bool isWin;
    public bool isPush;
    public bool isStop;
    public bool isSink;
    public bool isDefeat;

    private void Awake() {
        if (type != null && type.isTextBlock)
            isPush = true;
    }


    public void ClearProperties() {
        isYou = isWin = isStop = isSink = isDefeat = false;
        if (type != null && !type.isTextBlock)
            isPush = false;
    }
}