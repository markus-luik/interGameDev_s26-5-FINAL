using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;

public class GridManager : MonoBehaviour
{

    [Header("Grid parameters")]
    [Tooltip("Size of individual block in the grid.")]
    public float cellSize;
    [SerializeField]
    [Tooltip("Gameobject that instantiates in every cell of the grid. Should have a Cell component on it.")]
    private GameObject emptyCell;
    [SerializeField]
    [Tooltip("Gameobject that forms the width and height of your puzzle grid.")]
    private GameObject gridBounds;
    [SerializeField]
    [Tooltip("How thick our sphere cast should be.")]
    private float raycastRadius;
    [SerializeField]
    [Tooltip("How far our sphere cast should check.")]
    private float raycastDistance;
    [SerializeField]
    [Tooltip("How far above the floor we should check for blocks.")]
    private float raycastCheckY;

    private Vector2 gridSize;

    [Header("Debug settings")]
    [SerializeField]
    private float debugY;
    [SerializeField]
    private bool debugMode;
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
        //get the grid's width and height from the gridBounds object
        gridSize = new Vector2((int)gridBounds.transform.localScale.x / cellSize, (int)gridBounds.transform.localScale.z);
        //vector that'll iterate through all possible grid cells
        Vector3 checkSpot = Vector3.zero;
        //starting position of the bounds, beginning from the top left
        int startX = (int)(gridBounds.transform.position.x - gridBounds.transform.localScale.x / 2);
        int startY = (int)(gridBounds.transform.position.z + gridBounds.transform.localScale.z / 2);

        //loop through the width of the grid
        for (int x = 0; x < gridSize.x; x++)
        {
            //add a list to this index of the list
            gridList.Add(new List<GameObject>());
            //loop through the height of the grid
            for (int y = 0; y < gridSize.y; y++)
            {
                //set check spot to where we want to place a new cell
                checkSpot.Set(startX + x, transform.position.y, startY - y);
                //if we're in debug mode, add the cell coordinate UI
                if (debugMode) AddDebugUI(checkSpot, x, y);
                //create a new cell
                GameObject newCell = Instantiate(emptyCell, checkSpot, Quaternion.identity);
                //set the cell's parent to the object this component is attached to
                newCell.transform.SetParent(transform);
                //add the cell to the grid structure
                gridList[x].Add(newCell);
                //set check spot's y position to where we want to start sphere casting
                checkSpot.y = raycastCheckY;
                //create a new raycast
                RaycastHit hit;
                //if this sphere cast hits something
                if (Physics.SphereCast(checkSpot, raycastRadius, transform.up, out hit, raycastDistance))
                {
                    //and if that something is either a block or the player
                    if (hit.collider.CompareTag("block") || hit.collider.CompareTag("Player"))
                    {
                        //set up that block to be a part of the grid
                        hit.collider.gameObject.GetComponent<Block>().SetupNewGridBlock(this, newCell, x, y);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Includes grid coordinate text for each cell in the game view
    /// </summary>
    private void AddDebugUI(Vector3 _pos, int _gridX, int _gridY)
    {
        _pos.y = debugY;
        GameObject newLabel = Instantiate(label, _pos, Quaternion.identity, canvas.transform);
        newLabel.GetComponent<TMP_Text>().text = _gridX + "," + _gridY;
    }
    
    public void UpdateGrid()
    {
        BroadcastMessage("GridChanged");
    }
}