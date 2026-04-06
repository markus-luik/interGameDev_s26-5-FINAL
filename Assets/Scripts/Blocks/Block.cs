using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Block : MonoBehaviour
{
    [Header("Movement Control")]
    #region Movement
    
    [SerializeField]
    protected AnimationCurve moveCurve;
    public float speed;
    [SerializeField]
    private float lerpSnapDist;
    private float lerpTime = 0;
    private Vector3 startPos = Vector3.zero;
    public Vector3 targetPos = Vector3.zero;
    // reference to the currently running move coroutine so it can be stopped safely
    private Coroutine moveCoroutine = null;
    #endregion

    [Header("Block Attributes")]
    #region Block Attributes
    public bool canMove;
    protected GridManager gridManager;
    #endregion

    [HideInInspector]
    public Vector2Int gridPos, moveChange;

    public enum MoveStates
    {
        idle,
        attemptingMove,
        moving
    }

    public MoveStates state;
    public MoveStates State { get => state; set => state = value; }

    

    protected virtual void Start()
    {
        startPos = transform.position;
        targetPos = transform.position;
    }

    #region Movement Methods
    /// <summary>
    /// Checks if it is possible to move in the grid
    /// </summary>
    public virtual bool CheckMove(int _deltaX, int _deltaY)
    {
        if (State == MoveStates.idle)
        {
            State = MoveStates.attemptingMove;
            if (canMove && InGrid(gridPos.x + _deltaX, gridPos.y + _deltaY))
            {
                Cell checkCell = gridManager.gridList[gridPos.x + _deltaX][gridPos.y + _deltaY].GetComponent<Cell>();
                if (!checkCell.CheckContainObj() ||
                CheckHit(checkCell.ContainObj.GetComponent<Block>(), _deltaX, _deltaY))
                {
                    StartMove(checkCell, _deltaX, _deltaY);
                    BroadcastMessage("BlockMoved", new Vector2Int(_deltaX, _deltaY), SendMessageOptions.DontRequireReceiver);
                    return true;
                }
            }
            State = MoveStates.idle;
        }
        return false;
    }

    /// <summary>
    /// Checks how close the target position is to a compared position
    /// </summary>
    public bool CheckLerpDist(Vector3 _comparePos, float _maxVal)
    {
        if (Vector3.Distance(_comparePos, targetPos) < _maxVal) return true;
        return false;
    }

    /// <summary>
    /// Lerps block from one position to another based on animation curve set in the inspector
    /// </summary>
    protected Vector3 Move()
    {
        lerpTime += Time.deltaTime * speed;
        float percent = Mathf.Clamp01(moveCurve.Evaluate(lerpTime));
        Vector3 newPos = Vector3.Lerp(startPos, targetPos, percent);
        return newPos;
    }

    /// <summary>
    /// Sets initial values needed for moving on the grid and changes to the moving state
    /// </summary>
    protected virtual void StartMove(Cell _newParent, int _deltaX, int _deltaY)
    {
        State = MoveStates.moving;
        moveChange.Set(_deltaX, _deltaY);
        RefreshGridData(_newParent.gameObject, gridPos.x + _deltaX, gridPos.y + _deltaY);
        StartLerp(transform.position, transform.parent.transform.position);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(MoveLoop());
    }

    /// <summary>
    /// Sets the block's position to the target position and change to the idle state
    /// </summary>
    protected virtual void FinishMove()
    {
        transform.position = targetPos;
        State = MoveStates.idle;
    }

    /// <summary>
    /// Sets initial values needed for a lerp
    /// </summary>
    private void StartLerp(Vector3 _start, Vector3 _target)
    {
        lerpTime = 0;
        startPos = _start;
        targetPos = _target;
    }

    /// <summary>
    /// While the block has not reached the target position, move the block
    /// </summary>
    private IEnumerator MoveLoop()
    {
        const float epsilon = 0.0001f;
        while (Vector3.Distance(transform.position, targetPos) > epsilon)
        {
            transform.position = Move();
            if (Vector3.Distance(transform.position, targetPos) < lerpSnapDist)
            {
                FinishMove();
                break;
            }
            else
            {
                yield return null;
            }
        }
        if (Vector3.Distance(transform.position, targetPos) <= lerpSnapDist)
        {
            FinishMove();
        }
        moveCoroutine = null;
    }

    /// <summary>
    /// Determines if a block that we run into can move
    /// </summary>
    private bool CheckHit(Block _hitObj, int _deltaX, int _deltaY)
    {
        if (GetComponent<Slidey>() == null && _hitObj.CheckMove(_deltaX, _deltaY)) return true;
        return false;
    }

    #endregion

    #region Grid Information Methods

    /// <summary>
    /// Sets parent grid cell and initial grid position
    /// </summary>
    public void SetNewGridPos(GameObject _parent, int _gridX, int _gridY)
    {
        _parent.GetComponent<Cell>().ContainObj = gameObject;
        transform.SetParent(_parent.transform);
        gridPos.Set(_gridX, _gridY);
    }

    /// <summary>
    /// Checks if the provided coordinates are within the grid
    /// </summary>
    private bool InGrid(int _newX, int _newY)
    {
        if (_newX >= 0 && _newX < gridManager.gridList.Count &&
            _newY >= 0 && _newY < gridManager.gridList[0].Count) return true;
        return false;
    }

    /// <summary>
    /// Removes previous parent grid cell and sets our new grid data
    /// </summary>
    private void RefreshGridData(GameObject _newParent, int _newX, int _newY)
    {
        if (transform.parent.TryGetComponent<Cell>(out Cell _pCell)) _pCell.RemoveContainObj();
        SetNewGridPos(_newParent.gameObject, _newX, _newY);
        gridManager.UpdateGrid();
    }

    /// <summary>
    /// Removes previous parent grid cell and sets our new grid data
    /// </summary>
    public void SetupNewGridBlock(GridManager _gM, GameObject _parent, int _gridX, int _gridY)
    {
        gridManager = _gM;
        SetNewGridPos(_parent, _gridX, _gridY);
    }

    protected virtual void GridChanged(){}

    #endregion 
}
