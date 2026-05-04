using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponState
    {
        OnGround,
        HeldByPlayer,
        HeldByEnemy,
        Projectile
    }

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private WeaponState currentState = WeaponState.OnGround;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D pickupTrigger;
    [SerializeField] private Collider2D physicsCollider;
    [SerializeField] private float stopSpeedThreshold = 0.15f;
    [SerializeField] private float stopSpinThreshold = 15f;

    [SerializeField] private SpriteRenderer mySprite;
    [SerializeField] private WeaponType thrownHitType = WeaponType.B;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 15;
    [SerializeField] private int currentAmmo = 15;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public bool UseAmmo()
    {
        if (weaponType != WeaponType.A)
            return true; //non-guns don't use ammo

        if (currentAmmo <= 0)
            return false;

        currentAmmo--;
        return true;
    }
    private void Update()
    {
        if (currentState != WeaponState.Projectile || rb == null)
            return;

        if (rb.linearVelocity.magnitude <= stopSpeedThreshold &&
            Mathf.Abs(rb.angularVelocity) <= stopSpinThreshold)
        {
            SetState(WeaponState.OnGround);
        }
    }
    private Collider2D ownerCollider;

    public WeaponType WeaponType => weaponType;
    public WeaponState CurrentState => currentState;

    private void Awake()
    {
        if (mySprite == null)
            mySprite = GetComponent<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        ApplyStateSettings();
        
    }

    public void SetState(WeaponState newState)
    {
        currentState = newState;
        ApplyStateSettings();
    }

    private void ApplyStateSettings()
    {
        if (rb == null) return;

        switch (currentState)
        {
            case WeaponState.OnGround:

                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = true;
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                if (pickupTrigger != null) pickupTrigger.enabled = true;
                if (physicsCollider != null) physicsCollider.enabled = false;
                if (mySprite != null) mySprite.enabled = true;
                break;

            case WeaponState.HeldByPlayer:
                rb.simulated = false;

                if (pickupTrigger != null) pickupTrigger.enabled = false;
                if (physicsCollider != null) physicsCollider.enabled = false;

                if (mySprite != null)
                    mySprite.enabled = (weaponType == WeaponType.B);

                break;

            case WeaponState.HeldByEnemy:
            rb.simulated = false;

            if (pickupTrigger != null) pickupTrigger.enabled = false;
            if (physicsCollider != null) physicsCollider.enabled = false;

            if (mySprite != null)
                mySprite.enabled = (weaponType == WeaponType.B); //show bat, hide gun

            break;

            case WeaponState.Projectile:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.simulated = true;
                rb.gravityScale = 0f;

                if (pickupTrigger != null) pickupTrigger.enabled = false;
                if (physicsCollider != null) physicsCollider.enabled = true;
                if (mySprite != null) mySprite.enabled = true;
                break;
        }
    }

    public void Throw(Vector2 direction, float throwForce, float spinSpeed, Collider2D throwerCollider)
    {
        ownerCollider = throwerCollider;
        
        SetState(WeaponState.Projectile);

        transform.parent = null;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (physicsCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(physicsCollider, ownerCollider, true);
        }

        rb.AddForce(direction.normalized * throwForce, ForceMode2D.Impulse);

        float spinDir = Random.value < 0.5f ? -1f : 1f;
        rb.angularVelocity = spinSpeed * spinDir;

        Invoke(nameof(ReenableOwnerCollision), 0.5f);
    }

    private void ReenableOwnerCollision()
    {
        if (physicsCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(physicsCollider, ownerCollider, false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHitReceiver(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitReceiver(other);
    }
    private void TryHitReceiver(Collider2D other)
    {
        if (currentState != WeaponState.Projectile)
            return;

        if (other == null)
            return;

        if (ownerCollider != null)
        {
            if (other == ownerCollider)
                return;

            if (other.transform.IsChildOf(ownerCollider.transform))
                return;

            if (ownerCollider.transform.IsChildOf(other.transform))
                return;
        }

        EnemyDamaged enemy = other.GetComponentInParent<EnemyDamaged>();
        if (enemy == null)
            return;

        enemy.OnHit(new HitInfo
        {
            weaponType = thrownHitType,
            source = gameObject
        });

        SetState(WeaponState.OnGround);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}