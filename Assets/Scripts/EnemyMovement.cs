using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : Block2D
{
    [SerializeField] private Vector3 patrolStartPos;
    [SerializeField] private bool useCustomStart = false;

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

    [Header("Patrol")]
    public List<PatrolStep> patrolSteps = new List<PatrolStep>();

    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private Animator myAnim;

    private int currentStepIndex = 0;
    private int movedInCurrentStep = 0;
    void Awake()
    {
        patrolStartPos = transform.position;
    }
    private void Update()
    {
        if (State == MoveStates.idle)
        {
            TakeTurn();
        }
    }
    public void TakeTurn()
    {
        //Debug.Log("Enemy TakeTurn called");
        if (State != MoveStates.idle) return;
        if (patrolSteps == null || patrolSteps.Count == 0) return;

        PatrolStep step = patrolSteps[currentStepIndex];
        Vector2Int dir = GetDirectionVector(step.direction);

        if (enemyVision != null)
            enemyVision.SetFacing(dir);

        if (myAnim != null)
            myAnim.SetBool("isWalking", true);

        // enemyVision.SetFacing(new Vector2(dir.x, dir.y));
        //Debug.Log("Patrol dir = " + dir);
        CheckMove(dir.x, dir.y);
        //Debug.Log("Enemy trying to move: " + dir);
        movedInCurrentStep++;

        if (movedInCurrentStep >= step.amount)
        {
            movedInCurrentStep = 0;
            currentStepIndex++;

            if (currentStepIndex >= patrolSteps.Count)
                currentStepIndex = 0;
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

    void OnDrawGizmos()
    {
        if (patrolSteps == null || patrolSteps.Count == 0)
            return;

        Gizmos.color = Color.cyan;

        Vector3 startPos = Application.isPlaying ? patrolStartPos : transform.position;
        // Vector3 currentPos = startPos;
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

        // draw start point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos, 0.2f);
    }
}