using UnityEngine;
using System.Collections;

public class BatMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponHoldPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Swing")]
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float swingDuration = 0.12f;
    [SerializeField] private float leftAngle = 60f;
    [SerializeField] private float rightAngle = -70f;
    [SerializeField] private float hitRadius = 0.8f;

    [Header("HitRange")]
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackAngle = 100f;
    [SerializeField] private Transform attackOrigin;

    private Weapon currentWeapon;
    private bool isSwinging = false;
    private float nextAttackTime = 0f;
    private bool isOnLeftSide = true;

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void Start()
    {
        if (weaponHoldPoint != null)
        {
            weaponHoldPoint.localRotation = Quaternion.Euler(0f, 0f, leftAngle);
            isOnLeftSide = true;
        }
    }

    private void Update()
    {
        if (currentWeapon == null) return;
        if (currentWeapon.WeaponType != WeaponType.B) return;
        if (weaponHoldPoint == null) return;
        if (isSwinging) return;
        if (Time.time < nextAttackTime) return;

        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime && !isSwinging)
        {
            StartCoroutine(SwingBat());
        }
    }

    private IEnumerator SwingBat()
    {
        isSwinging = true;
        nextAttackTime = Time.time + attackCooldown;

        float startAngle = isOnLeftSide ? leftAngle : rightAngle;
        float endAngle = isOnLeftSide ? rightAngle : leftAngle;

        float timer = 0f;
        bool didHit = false;

        while (timer < swingDuration)
        {
            timer += Time.deltaTime;
            float t = timer / swingDuration;

            float z = Mathf.Lerp(startAngle, endAngle, t);
            weaponHoldPoint.localRotation = Quaternion.Euler(0f, 0f, z);

            if (!didHit && t >= 0.5f)
            {
                DoHit();
                didHit = true;
            }

            yield return null;
        }

        weaponHoldPoint.localRotation = Quaternion.Euler(0f, 0f, endAngle);
        isOnLeftSide = !isOnLeftSide;
        isSwinging = false;
    }

    private void DoHit()
    {
        Vector2 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector2 forward = weaponHoldPoint != null ? weaponHoldPoint.right : transform.right;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRadius, enemyLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 toTarget = (Vector2)hits[i].transform.position - origin;

            if (toTarget.magnitude > attackRadius)
                continue;

            float angle = Vector2.Angle(forward, toTarget.normalized);
            if (angle > attackAngle * 0.5f)
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

    private void OnDrawGizmosSelected()
    {
        Transform originTransform = attackOrigin != null ? attackOrigin : transform;
        Vector3 origin = originTransform.position;

        Vector3 forward = weaponHoldPoint != null ? weaponHoldPoint.right : transform.right;

        float halfAngle = attackAngle * 0.5f;

        Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * forward;
        Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * forward;

        Gizmos.color = Color.red;

        //two sides
        Gizmos.DrawLine(origin, origin + leftDir.normalized * attackRadius);
        Gizmos.DrawLine(origin, origin + rightDir.normalized * attackRadius);

        //arc
        int segments = 20;
        Vector3 prevPoint = origin + rightDir.normalized * attackRadius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * forward;
            Vector3 nextPoint = origin + dir.normalized * attackRadius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}