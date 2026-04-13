using UnityEngine;

[CreateAssetMenu(menuName = "Baba/Entity Type", fileName = "New EntityType")]
public class EntityType : ScriptableObject {
    public string entityName;
    public Sprite sprite;
    public bool isTextBlock;
}