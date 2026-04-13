using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Block2D
{
    [System.Serializable]
    public class PatrolStep
    {
        public Direction direction;
        public int amount = 1;
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum EnemyMode
    {
        Patrol,
        Chase,
        ReturnToPatrol
    }

    [Header("Patrol")]
    public List<PatrolStep> patrolSteps = new List<PatrolStep>();

    [Header("Chase")]
    [SerializeField] private Transform player;
    [SerializeField] private float lostSightTime = 3f;

    [Header("References")]
    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private Animator myAnim;

    [Header("Debug")]
    [SerializeField] private Vector3 patrolStartPos;
    [SerializeField] private bool useCustomStart = false;

    private EnemyMode currentMode = EnemyMode.Patrol;

    private int currentStepIndex = 0;
    private int movedInCurrentStep = 0;

    private float loseSightTimer = 0f;

    private Vector2Int patrolStartGridPos;
    private Vector2Int lastKnownPlayerGridPos;

    protected override void Start()
    {
        base.Start();

        if (!useCustomStart)
            patrolStartPos = transform.position;

        patrolStartGridPos = gridPos;
    }

    private void Update()
    {
        UpdateMode();

        if (State != MoveStates.idle)
            return;

        switch (currentMode)
        {
            case EnemyMode.Patrol:
                PatrolMove();
                break;

            case EnemyMode.Chase:
                ChaseMove();
                break;

            case EnemyMode.ReturnToPatrol:
                ReturnToPatrolMove();
                break;
        }
    }

    private void UpdateMode()
    {
        if (enemyVision != null && enemyVision.canSeePlayer && player != null)
        {
            currentMode = EnemyMode.Chase;
            loseSightTimer = 0f;

            Block2D playerBlock = player.GetComponent<Block2D>();
            if (playerBlock != null)
                lastKnownPlayerGridPos = playerBlock.gridPos;

            return;
        }

        if (currentMode == EnemyMode.Chase)
        {
            loseSightTimer += Time.deltaTime;

            if (loseSightTimer >= lostSightTime)
            {
                currentMode = EnemyMode.ReturnToPatrol;
            }
        }
    }

    private void PatrolMove()
    {
        if (patrolSteps == null || patrolSteps.Count == 0)
            return;

        PatrolStep step = patrolSteps[currentStepIndex];
        Vector2Int dir = GetDirectionVector(step.direction);

        if (enemyVision != null)
            enemyVision.SetFacing(dir);

        if (myAnim != null)
            myAnim.SetBool("isWalking", true);

        CheckMove(dir.x, dir.y);

        movedInCurrentStep++;

        if (movedInCurrentStep >= step.amount)
        {
            movedInCurrentStep = 0;
            currentStepIndex++;

            if (currentStepIndex >= patrolSteps.Count)
                currentStepIndex = 0;
        }
    }

    private void ChaseMove()
    {
        Block2D playerBlock = player != null ? player.GetComponent<Block2D>() : null;
        if (playerBlock == null)
            return;

        Vector2Int targetGrid = playerBlock.gridPos;
        lastKnownPlayerGridPos = targetGrid;

        Vector2Int? nextStep = GetNextStepTowards(gridPos, targetGrid);

        if (nextStep.HasValue)
        {
            Vector2Int dir = nextStep.Value - gridPos;

            if (enemyVision != null)
                enemyVision.SetFacing(dir);

            if (myAnim != null)
                myAnim.SetBool("isWalking", true);

            CheckMove(dir.x, dir.y);
        }
    }

    private void ReturnToPatrolMove()
    {
        if (gridPos == patrolStartGridPos)
        {
            currentMode = EnemyMode.Patrol;
            currentStepIndex = 0;
            movedInCurrentStep = 0;
            return;
        }

        Vector2Int? nextStep = GetNextStepTowards(gridPos, patrolStartGridPos);

        if (nextStep.HasValue)
        {
            Vector2Int dir = nextStep.Value - gridPos;

            if (enemyVision != null)
                enemyVision.SetFacing(dir);

            if (myAnim != null)
                myAnim.SetBool("isWalking", true);

            CheckMove(dir.x, dir.y);
        }
    }

    protected override void FinishMove()
    {
        base.FinishMove();

        if (myAnim != null)
            myAnim.SetBool("isWalking", false);
    }

    private Vector2Int GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return new Vector2Int(0, -1);
            case Direction.Down: return new Vector2Int(0, 1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Right: return new Vector2Int(1, 0);
        }

        return Vector2Int.zero;
    }

    private Vector2Int[] GetFourDirections()
    {
        return new Vector2Int[]
        {
            new Vector2Int(0, -1), // Up in grid
            new Vector2Int(0, 1),  // Down in grid
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };
    }

    private Vector2Int? GetNextStepTowards(Vector2Int start, Vector2Int goal)
    {
        if (start == goal)
            return null;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (Vector2Int dir in GetFourDirections())
            {
                Vector2Int next = current + dir;

                if (cameFrom.ContainsKey(next))
                    continue;

                // allow goal even if occupied by player
                if (next != goal && !CanPathThrough(next))
                    continue;

                cameFrom[next] = current;
                frontier.Enqueue(next);
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = goal;

        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Reverse();

        if (path.Count == 0)
            return null;

        return path[0];
    }

    private bool CanPathThrough(Vector2Int pos)
    {

        if (gridManager == null) return false;

        if (pos.x < 0 || pos.y < 0 || pos.x >= gridManager.gridList.Count || pos.y >= gridManager.gridList[0].Count)
            return false;
        // get world position of this grid cell
        GameObject cell = gridManager.gridList[pos.x][pos.y];
        Vector3 worldPos = cell.transform.position;

        Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.2f);

        // empty cell
        if (hit == null) return true;

        //player as target
        if (hit.CompareTag("Player")) return true;

        // anything else is obstacle
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Die();

            // optional: destroy bullet too
            Destroy(other.gameObject);
        }
    }
    void Die()
    {
         Debug.Log("Enemy died");

        // stop everything
        enabled = false;

        //stop movement animation
        if (myAnim != null)
            myAnim.SetBool("isWalking", false);

        // play death animation
        // myAnim.SetTrigger("Die");

        // destroy enemy
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        if (patrolSteps == null || patrolSteps.Count == 0)
            return;

        Gizmos.color = Color.cyan;

        Vector3 startPos = Application.isPlaying ? patrolStartPos : transform.position;
        Vector3 currentPos = startPos;

        for (int i = 0; i < patrolSteps.Count; i++)
        {
            PatrolStep step = patrolSteps[i];
            Vector2Int dir = GetDirectionVector(step.direction);

            Vector3 drawDir = new Vector3(dir.x, -dir.y, 0);

            for (int j = 0; j < step.amount; j++)
            {
                Vector3 nextPos = currentPos + drawDir;
                Gizmos.DrawLine(currentPos, nextPos);
                Gizmos.DrawSphere(nextPos, 0.1f);
                currentPos = nextPos;
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos, 0.2f);
    }

}