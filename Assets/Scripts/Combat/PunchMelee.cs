using UnityEngine;
using System.Collections;
    #if UNITY_EDITOR
using UnityEditor;
#endif
public class PunchMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private Transform punchOrigin;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Punch")]
    [SerializeField] private float punchRadius = 0.6f;
    [SerializeField] private float punchAngle = 80f;
    [SerializeField] private float punchCooldown = 0.3f;
    [SerializeField] private float punchDuration = 0.08f;

    private bool isPunching = false;
    private float nextPunchTime = 0f;

    private void Awake()
    {
        if (playerShooting == null)
            playerShooting = GetComponent<PlayerShooting>();
    }

    private void Update()
    {
        if (playerShooting == null) return;

        // Only punch when player has NO weapon
        if (playerShooting.HasWeapon) return;

        if (isPunching) return;
        if (Time.time < nextPunchTime) return;

        if (Input.GetMouseButton(0))
        {
            StartCoroutine(Punch());
        }
    }

    private IEnumerator Punch()
    {
        isPunching = true;
        nextPunchTime = Time.time + punchCooldown;

        yield return new WaitForSeconds(punchDuration * 0.5f);

        DoPunchHit();

        yield return new WaitForSeconds(punchDuration * 0.5f);

        isPunching = false;
    }

    private void DoPunchHit()
    {
        Vector2 origin = punchOrigin != null ? punchOrigin.position : transform.position;
        Vector2 forward = transform.right;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, punchRadius, enemyLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 toTarget = (Vector2)hits[i].transform.position - origin;

            if (toTarget.magnitude > punchRadius)
                continue;

            float angle = Vector2.Angle(forward, toTarget.normalized);
            if (angle > punchAngle * 0.5f)
                continue;

            IHitReceiver receiver = hits[i].GetComponentInParent<IHitReceiver>();
            if (receiver != null)
            {
                receiver.OnHit(new HitInfo
                {
                    weaponType = WeaponType.B,
                    source = gameObject
                });
            }
        }
    }

   

    private void OnDrawGizmos()
    {
        Transform originTransform = punchOrigin != null ? punchOrigin : transform;
        Vector3 origin = originTransform.position;
        Vector3 forward = transform.right;

        float halfAngle = punchAngle * 0.5f;

        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * forward;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * forward;

        //outline
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + leftDir.normalized * punchRadius);
        Gizmos.DrawLine(origin, origin + rightDir.normalized * punchRadius);

        int segments = 16;
        Vector3 prevPoint = origin + rightDir.normalized * punchRadius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * forward;
            Vector3 nextPoint = origin + dir.normalized * punchRadius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        //filled when punching
    #if UNITY_EDITOR
        if (isPunching)
        {
            Handles.color = new Color(1f, 0.8f, 0f, 0.25f); // transparent fill
            Handles.DrawSolidArc(origin, Vector3.forward, rightDir, punchAngle, punchRadius);
        }
    #endif
    }
}