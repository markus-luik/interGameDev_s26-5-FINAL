using UnityEngine;

public class NewGridManager2D : MonoBehaviour
{
    public float cellSize = 1f;

    public Vector2 SnapToGrid(Vector2 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x / cellSize) * cellSize,
            Mathf.Round(pos.y / cellSize) * cellSize
        );
    }

    public Vector2 GetNeighbour(Vector2 pos, int dx, int dy)
    {
        return new Vector2(pos.x + dx * cellSize, pos.y + dy * cellSize);
    }
}