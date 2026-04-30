using UnityEngine;

public class NewBlock2D : MonoBehaviour
{
    [Header("Cell")]
    #region Cell
    [SerializeField] private float cellSize = 1f;
    #endregion
    
    [Header("Movement")]
    #region Movement
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveRepeatDelay = 0.15f; // time between steps
    [SerializeField] private float dashRepeatDelay = 0.03f; //time between steps when dashing
    private float moveTimer = 0f;
    #endregion
    
    [Header("Properites")]
    #region Properites
    public bool isPlayer = false;
    [SerializeField] private bool canBePushed = true;
    [SerializeField] private bool isPassable = false;
    #endregion
    
    protected bool isMoving = false;
    protected Vector2 targetPos;
    protected NewGridManager2D grid;
    private Collider2D col;
    
    private Vector2 currentDirection = Vector2.zero;
    


    // Store last ray info for gizmos
    private Vector2 lastRayOrigin;
    private Vector2 lastRayDirection;

    protected virtual void Awake()
    {
        grid = FindObjectOfType<NewGridManager2D>();
        targetPos = grid.SnapToGrid(transform.position);
        col = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;

            // Re-enable collider when movement finishes
            if (col != null) col.enabled = true;
        }

        if (isPlayer && !isMoving)
        {
            Vector2 direction = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W)) direction = Vector2.up;
            else if (Input.GetKey(KeyCode.S)) direction = Vector2.down;
            else if (Input.GetKey(KeyCode.A)) direction = Vector2.left;
            else if (Input.GetKey(KeyCode.D)) direction = Vector2.right;

            if (direction != Vector2.zero)
            {
                moveTimer -= Time.deltaTime;

                if (moveTimer <= 0f)
                {
                    TryMove((int)direction.x, (int)direction.y);
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        moveTimer = dashRepeatDelay;
                    }
                    else{                    
                        moveTimer = moveRepeatDelay;
                    }

                }
            }
            else
            {
                moveTimer = 0f; // reset when no input
            }
        }
    }

    public bool TryMove(int dx, int dy)
    {
        if (isMoving) return false;

        Vector2 destination = new Vector2(
            targetPos.x + dx * cellSize,
            targetPos.y + dy * cellSize
        );

        Vector2 direction = new Vector2(dx, dy);

        // Save ray info for gizmos
        lastRayOrigin = targetPos;
        lastRayDirection = direction;

        RaycastHit2D[] hits = Physics2D.RaycastAll(targetPos, direction, cellSize);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            NewBlock2D other = hit.collider.GetComponent<NewBlock2D>();

            if (other != null && other.isPassable) continue;

            if (other != null && other.canBePushed && !other.isMoving)
            {
                if (other.TryMove(dx, dy))
                {
                    MoveTo(destination);
                    return true;
                }
            }

            return false;
        }

        MoveTo(destination);
        return true;
    }

    protected void MoveTo(Vector2 destination)
    {
        targetPos = destination;
        isMoving = true;

        if (col != null) col.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.red;

        Vector2 origin = Application.isPlaying ? targetPos : (Vector2)transform.position;
        Vector2 dir = currentDirection;

        if (dir == Vector2.zero) return;

        Gizmos.DrawLine(origin, origin + dir * cellSize);
        Gizmos.DrawSphere(origin + dir * cellSize, 0.1f);
    }
}