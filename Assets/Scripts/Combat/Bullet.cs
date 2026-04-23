using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private WeaponType weaponType = WeaponType.A;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private bool destroyOnAnyCollision = true;

    private Rigidbody2D rb;
    private Collider2D bulletCollider;
    private GameObject owner;
    private bool hasHit;

    public void SetWeaponType(WeaponType newType)
    {
        weaponType = newType;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        hasHit = false;

        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector2 direction, float speed, GameObject bulletOwner = null, WeaponType type = WeaponType.A)
    {
        owner = bulletOwner;
        weaponType = type;

        Debug.Log("Bullet owner: " + (owner != null ? owner.name : "NULL"));

        if (bulletCollider != null && owner != null)
        {
            Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();
            for (int i = 0; i < ownerColliders.Length; i++)
            {
                Physics2D.IgnoreCollision(bulletCollider, ownerColliders[i], true);
            }
        }

        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector2 dir = direction.normalized;
            transform.right = new Vector3(dir.x, dir.y, 0f);

            if (rb != null)
                rb.linearVelocity = dir * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet hit trigger: " + other.name);
        HandleHit(other.gameObject, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet hit collision: " + collision.gameObject.name);
        HandleHit(collision.gameObject, false);
    }

    private void HandleHit(GameObject otherObj, bool collidedWithTrigger)
    {
        if (hasHit || otherObj == null)
            return;

        if (owner != null)
        {
            if (otherObj == owner)
                return;

            if (otherObj.transform.IsChildOf(owner.transform))
                return;

            if (owner.transform.IsChildOf(otherObj.transform))
                return;
        }

        IHitReceiver receiver = otherObj.GetComponentInParent<IHitReceiver>();
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
        
        if (destroyOnAnyCollision && !collidedWithTrigger)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}