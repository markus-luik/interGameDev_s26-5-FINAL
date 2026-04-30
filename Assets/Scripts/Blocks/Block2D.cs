using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block2D : MonoBehaviour
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

    private Coroutine moveCoroutine = null;

    #endregion

    [Header("Block Attributes")]
    #region Block Attributes

    public bool canMove;

    protected GridManager2D gridManager; // ✅ changed to 2D
    
    public bool isPassable = false; //Overrides the block pushing

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

    public virtual bool CheckMove(int _deltaX, int _deltaY)
    {
        if (State == MoveStates.idle)
        {
            State = MoveStates.attemptingMove;

            if (canMove && InGrid(gridPos.x + _deltaX, gridPos.y + _deltaY))
            {
                Cell checkCell = gridManager.gridList[gridPos.x + _deltaX][gridPos.y + _deltaY].GetComponent<Cell>();

                Block2D occupant = checkCell.ContainObj?.GetComponent<Block2D>();
                if (!checkCell.CheckContainObj() || (occupant != null && occupant.isPassable) || CheckHit(occupant, _deltaX, _deltaY))
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
    /// Moves the block to a target cell without any collision or canMove checks.
    /// Used by the Baba rule system which handles its own movement validation.
    /// </summary>
    public void ForceMove(int _deltaX, int _deltaY) {
        if (State != MoveStates.idle) return;

        int newX = gridPos.x + _deltaX;
        int newY = gridPos.y + _deltaY;

        if (newX < 0 || newX >= gridManager.gridList.Count) return;
        if (newY < 0 || newY >= gridManager.gridList[0].Count) return;

        Cell targetCell = gridManager.gridList[newX][newY].GetComponent<Cell>();
        if (targetCell == null) return;

        StartMove(targetCell, _deltaX, _deltaY);
        BroadcastMessage("BlockMoved", new Vector2Int(_deltaX, _deltaY), SendMessageOptions.DontRequireReceiver);
    }

    public bool CheckLerpDist(Vector3 _comparePos, float _maxVal)
    {
        return Vector3.Distance(_comparePos, targetPos) < _maxVal;
    }

    protected Vector3 Move()
    {
        lerpTime += Time.deltaTime * speed;
        float percent = Mathf.Clamp01(moveCurve.Evaluate(lerpTime));
        return Vector3.Lerp(startPos, targetPos, percent);
    }

    protected virtual void StartMove(Cell _newParent, int _deltaX, int _deltaY)
    {
        State = MoveStates.moving;

        moveChange.Set(_deltaX, _deltaY);

        RefreshGridData(_newParent.gameObject, gridPos.x + _deltaX, gridPos.y + _deltaY);

        StartLerp(transform.position, _newParent.transform.position);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveLoop());
    }

    protected virtual void FinishMove()
    {
        transform.position = targetPos;
        State = MoveStates.idle;
    }

    private void StartLerp(Vector3 _start, Vector3 _target)
    {
        lerpTime = 0;
        startPos = _start;
        targetPos = _target;
    }

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

            yield return null;
        }

        FinishMove();
        moveCoroutine = null;
    }

    private bool CheckHit(Block2D _hitObj, int _deltaX, int _deltaY)
    {
        if (_hitObj == null) return false;

        // if not slide block, try to push
        if (GetComponent<Slidey>() == null && _hitObj.CheckMove(_deltaX, _deltaY))
            return true;

        return false;
    }

    #endregion

    #region Grid Information Methods

    public void SetNewGridPos(GameObject _parent, int _gridX, int _gridY)
    {
        _parent.GetComponent<Cell>().ContainObj = gameObject;
        transform.SetParent(_parent.transform);
        gridPos.Set(_gridX, _gridY);
    }

    private bool InGrid(int _newX, int _newY)
    {
        return _newX >= 0 && _newX < gridManager.gridList.Count &&
               _newY >= 0 && _newY < gridManager.gridList[0].Count;
    }

    private void RefreshGridData(GameObject _newParent, int _newX, int _newY)
    {
        if (transform.parent.TryGetComponent<Cell>(out Cell oldCell))
        {
            oldCell.RemoveContainObj();
        }

        SetNewGridPos(_newParent, _newX, _newY);
        gridManager.UpdateGrid();
    }

    public void SetupNewGridBlock(GridManager2D _gM, GameObject _parent, int _gridX, int _gridY)
    {
        Debug.Log($"{gameObject.name} assigned to grid at {_gridX},{_gridY}");
        gridManager = _gM;
        SetNewGridPos(_parent, _gridX, _gridY);
    }

    protected virtual void GridChanged() { }

    #endregion
}