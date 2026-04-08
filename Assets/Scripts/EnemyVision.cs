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
        Debug.Log("Enemy sees player!");
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
        Vector3 forward = new Vector3(facingDirection.x, facingDirection.y, 0f);

        Gizmos.DrawLine(pos, pos + forward * viewDistance);

        Vector3 leftBound = Quaternion.Euler(0, 0, viewAngle * 0.5f) * forward;
        Vector3 rightBound = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * forward;

        Gizmos.DrawLine(pos, pos + leftBound.normalized * viewDistance);
        Gizmos.DrawLine(pos, pos + rightBound.normalized * viewDistance);
    }
}