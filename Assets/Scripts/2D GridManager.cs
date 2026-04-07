using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GridManager2D : MonoBehaviour
{
    [Header("Grid parameters")]
    [Tooltip("Size of individual block in the grid.")]
    public float cellSize = 1f;

    [SerializeField]
    [Tooltip("GameObject that instantiates in every cell of the grid. Should have a Cell component on it.")]
    private GameObject emptyCell;

    [SerializeField]
    [Tooltip("GameObject that forms the width and height of your puzzle grid.")]
    private GameObject gridBounds;

    [SerializeField]
    [Tooltip("Radius of the overlap check for finding blocks in a cell.")]
    private float overlapRadius = 0.2f;

    private Vector2Int gridSize;

    [Header("Debug settings")]
    [SerializeField]
    private float debugZ = 0f;

    [SerializeField]
    private bool debugMode = false;

    [SerializeField]
    private GameObject canvas, label;

    public List<List<GameObject>> gridList = new List<List<GameObject>>();

    private void Awake()
    {
        MakeGrid();
    }

    /// <summary>
    /// Makes grid data structure based on level placement in the scene
    /// </summary>
    private void MakeGrid()
    {
        // Clear old grid in case this is called again
        gridList.Clear();

        // Get the grid's width and height from the gridBounds object
        gridSize = new Vector2Int(
            Mathf.RoundToInt(gridBounds.transform.localScale.x / cellSize),
            Mathf.RoundToInt(gridBounds.transform.localScale.y / cellSize)
        );

        // Starting position of the bounds, beginning from top-left
        float startX = gridBounds.transform.position.x - (gridBounds.transform.localScale.x / 2f) + (cellSize / 2f);
        float startY = gridBounds.transform.position.y + (gridBounds.transform.localScale.y / 2f) - (cellSize / 2f);

        // Loop through width
        for (int x = 0; x < gridSize.x; x++)
        {
            gridList.Add(new List<GameObject>());

            // Loop through height
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 checkSpot = new Vector3(
                    startX + (x * cellSize),
                    startY - (y * cellSize),
                    0f
                );

                // If we're in debug mode, add the cell coordinate UI
                if (debugMode)
                {
                    AddDebugUI(checkSpot, x, y);
                }

                // Create a new cell
                GameObject newCell = Instantiate(emptyCell, checkSpot, Quaternion.identity);

                // Set the cell's parent to this GridManager object
                newCell.transform.SetParent(transform);

                // Add the cell to the grid structure
                gridList[x].Add(newCell);

                // Check if there is a 2D object already in this cell
                Collider2D hit = Physics2D.OverlapCircle(checkSpot, overlapRadius);

                // If it hits something tagged as block or player
                if (hit != null && (hit.CompareTag("block") || hit.CompareTag("Player")))
                {
                    Block2D block = hit.GetComponent<Block2D>();
                    if (block != null)
                    {
                        block.SetupNewGridBlock(this, newCell, x, y);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Includes grid coordinate text for each cell in the game view
    /// </summary>
    private void AddDebugUI(Vector3 pos, int gridX, int gridY)
    {
        pos.z = -1f;

        GameObject newLabel = Instantiate(label, pos, Quaternion.identity, transform);
        TMP_Text text = newLabel.GetComponent<TMP_Text>();

        if (text != null)
        {
            text.text = gridX + "," + gridY;
        }
    }
    public void UpdateGrid()
    {
        BroadcastMessage("GridChanged");
    }

    // Optional: lets you see the overlap radius in Scene view
    private void OnDrawGizmosSelected()
    {
        if (!debugMode || gridBounds == null) return;

        Vector2Int previewGridSize = new Vector2Int(
            Mathf.RoundToInt(gridBounds.transform.localScale.x / cellSize),
            Mathf.RoundToInt(gridBounds.transform.localScale.y / cellSize)
        );

        float startX = gridBounds.transform.position.x - (gridBounds.transform.localScale.x / 2f) + (cellSize / 2f);
        float startY = gridBounds.transform.position.y + (gridBounds.transform.localScale.y / 2f) - (cellSize / 2f);

        for (int x = 0; x < previewGridSize.x; x++)
        {
            for (int y = 0; y < previewGridSize.y; y++)
            {
                Vector3 checkSpot = new Vector3(
                    startX + (x * cellSize),
                    startY - (y * cellSize),
                    0f
                );

                Gizmos.DrawWireSphere(checkSpot, overlapRadius);
            }
        }
    }
}