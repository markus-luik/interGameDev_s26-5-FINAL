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

    [Header("Patrol")]
    public List<PatrolStep> patrolSteps = new List<PatrolStep>();

    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private Animator myAnim;

    private int currentStepIndex = 0;
    private int movedInCurrentStep = 0;
    
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
        Debug.Log("Patrol dir = " + dir);
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

        Vector3 currentPos = transform.position;

        for (int i = 0; i < patrolSteps.Count; i++)
        {
            PatrolStep step = patrolSteps[i];
            Vector2Int dir = GetDirectionVector(step.direction);

            for (int j = 0; j < step.amount; j++)
            {
                Vector3 nextPos = currentPos + new Vector3(dir.x, dir.y, 0);


                Gizmos.DrawLine(currentPos, nextPos);

                Gizmos.DrawSphere(nextPos, 0.1f);

                currentPos = nextPos;
            }
        }
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(currentPos, transform.position);
    }
}