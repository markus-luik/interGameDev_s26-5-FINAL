using UnityEngine;

/// <summary>
/// Grid-aligned block. Handles all movement and push chain resolution.
/// 
/// Push/stop decisions respect Baba's rule system: if this GameObject has a
/// GridEntity component, its runtime isPush/isStop flags take precedence over
/// the serialized canBePushed field.
/// 
/// IMPORTANT: Set isPlayer = false on player objects when using TurnManager —
/// TurnManager drives all WASD input; the isPlayer path is for standalone use
/// without the Baba rule system.
/// </summary>
public class NewBlock2D : MonoBehaviour
{
    [Header("Cell")]
    [SerializeField] private float cellSize = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveRepeatDelay = 0.15f;
    [SerializeField] private float dashRepeatDelay = 0.03f;
    private float moveTimer = 0f;

    [Header("Properties")]
    public bool isPlayer = false;
    [SerializeField] private bool canBePushed = true;
    [SerializeField] private bool isPassable = false;

    protected bool isMoving = false;

    // Exposed so BabaGridIndex / GridEntity can read the snapped world position
    public Vector2 TargetPos => targetPos;
    protected Vector2 targetPos;

    protected NewGridManager2D grid;
    private Collider2D col;
    private Vector2 currentDirection = Vector2.zero;

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
            if (col != null) col.enabled = true;
        }

        // Only poll input here when NOT managed by TurnManager
        if (isPlayer && !isMoving)
        {
            Vector2 direction = Vector2.zero;

            if      (Input.GetKey(KeyCode.W)) direction = Vector2.up;
            else if (Input.GetKey(KeyCode.S)) direction = Vector2.down;
            else if (Input.GetKey(KeyCode.A)) direction = Vector2.left;
            else if (Input.GetKey(KeyCode.D)) direction = Vector2.right;

            if (direction != Vector2.zero)
            {
                moveTimer -= Time.deltaTime;
                if (moveTimer <= 0f)
                {
                    TryMove((int)direction.x, (int)direction.y);
                    moveTimer = Input.GetKey(KeyCode.LeftShift) ? dashRepeatDelay : moveRepeatDelay;
                }
            }
            else
            {
                moveTimer = 0f;
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

        RaycastHit2D[] hits = Physics2D.RaycastAll(targetPos, new Vector2(dx, dy), cellSize);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            NewBlock2D other = hit.collider.GetComponent<NewBlock2D>();
            if (other == null) return false; // solid non-block wall

            if (other.isPassable) continue;

            // Check rule-system flags first; fall back to serialized canBePushed
            var otherEntity = other.GetComponent<GridEntity>();
            bool ruleStop = otherEntity != null && otherEntity.isStop && !otherEntity.isPush;
            bool rulePush = otherEntity != null ? otherEntity.isPush : other.canBePushed;

            if (ruleStop) return false;

            if (rulePush)
            {
                if (other.TryMove(dx, dy))
                {
                    MoveTo(destination);
                    return true;
                }
                return false;
            }

            return false; // not passable, not pushable → blocked
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
        if (currentDirection == Vector2.zero) return;
        Gizmos.DrawLine(origin, origin + currentDirection * cellSize);
        Gizmos.DrawSphere(origin + currentDirection * cellSize, 0.1f);
    }
}