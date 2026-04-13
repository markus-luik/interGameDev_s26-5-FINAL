using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Weapon category carried by this projectile (controls enemy reaction branch).
    [Header("Damage")]
    [SerializeField] private WeaponType weaponType = WeaponType.A;

    // Auto-cleanup and generic collision behavior for non-target hits.
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private bool destroyOnAnyCollision = true;

    private Rigidbody2D rb;
    private GameObject owner;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Reset per-shot guard and schedule cleanup.
        hasHit = false;
        if (lifeTime > 0f)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    public void Initialize(Vector2 direction, float speed, GameObject bulletOwner = null, WeaponType type = WeaponType.A)
    {
        // Set runtime context from shooter at spawn time.
        owner = bulletOwner;
        weaponType = type;

        // Orient and launch using rigidbody velocity when available.
        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector2 dir = direction.normalized;
            transform.up = new Vector3(dir.x, dir.y, 0f);
            if (rb != null)
            {
                rb.linearVelocity = dir * speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject, other.isTrigger);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject, false);
    }

    private void HandleHit(GameObject otherObj, bool collidedWithTrigger)
    {
        // Ignore invalid, repeated, or self-owner collisions.
        if (hasHit || otherObj == null)
        {
            return;
        }

        if (owner != null && otherObj == owner)
        {
            return;
        }

        // Deliver a typed hit event to any compatible receiver, then consume bullet.
        IHitReceiver receiver = otherObj.GetComponent<IHitReceiver>();
        if (receiver != null)
        {
            hasHit = true;
            receiver.OnHit(new HitInfo
            {
                weaponType = weaponType,
                source = gameObject
            });
            Destroy(gameObject);
            return;
        }

        // Optionally consume bullet when colliding with non-trigger world geometry.
        if (destroyOnAnyCollision && !collidedWithTrigger)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
