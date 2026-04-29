using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform player;

    [Header("Vision")]
    public float viewDistance = 5f;
    [Range(0f, 360f)]
    public float viewAngle = 90f;

    [Header("Layers")]
    public LayerMask obstacleLayer;

    [Header("Facing")]
    public Vector2 facingDirection = Vector2.down;

    public bool canSeePlayer;

    void Update()
    {
        CheckVision();
    }

    void CheckVision()
    {
        canSeePlayer = false;

        if (player == null) return;

        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.position;
        Vector2 toPlayer = playerPos - enemyPos;

        if (toPlayer.magnitude > viewDistance)
            return;

        float angleToPlayer = Vector2.Angle(facingDirection.normalized, toPlayer.normalized);
        if (angleToPlayer > viewAngle * 0.5f)
            return;

        RaycastHit2D hit = Physics2D.Raycast(enemyPos, toPlayer.normalized, toPlayer.magnitude, obstacleLayer);

        if (hit.collider != null)
            return;

        canSeePlayer = true;
        //Debug.Log("Enemy sees player!");
    }

    public void SetFacing(Vector2 newFacing)
    {
        if (newFacing != Vector2.zero)
            facingDirection = new Vector2(newFacing.x, -newFacing.y).normalized;
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;

    Vector3 pos = transform.position;
    Vector2 forward = facingDirection.normalized;

    int rayCount = 30; //more = smoother
    float halfAngle = viewAngle * 0.5f;

    float angleStep = viewAngle / rayCount;

    for (int i = 0; i <= rayCount; i++)
    {
        float angle = -halfAngle + angleStep * i;

        Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;

        RaycastHit2D hit = Physics2D.Raycast(pos, dir, viewDistance, obstacleLayer);

        Vector3 endPoint;

        if (hit.collider != null)
        {
            //stop at obstacle
            endPoint = hit.point;
        }
        else
        {
            //otherwise full length
            endPoint = pos + (Vector3)(dir * viewDistance);
        }

        Gizmos.DrawLine(pos, endPoint);
    }
}
}