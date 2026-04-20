using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Collider2D ownerCollider;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 16f;
    [SerializeField] private float fireInterval = 0.15f;
    [SerializeField] private bool rotatePlayerToMouse = true;
    [SerializeField] private bool canShoot = false;

    [Header("Throwing")]
    [SerializeField] private float throwForce = 8f;
    [SerializeField] private float throwSpinSpeed = 720f;
    [SerializeField] private bool canThrow = true;
    [SerializeField] private BatMelee batMelee;
    [SerializeField] private Transform weaponHoldPoint;

    public bool HasWeapon => currentWeapon != null;
    public Weapon CurrentWeapon => currentWeapon;

    private float nextFireTime;

    private void Awake()
    {
        if (shootPoint == null)
            shootPoint = transform;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (ownerCollider == null)
            ownerCollider = GetComponent<Collider2D>();
    }
    private bool CanFireCurrentWeapon()
    {
        return currentWeapon != null && currentWeapon.WeaponType == WeaponType.A;
    }
    private void Update()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        if (rotatePlayerToMouse)
        {
            Vector2 dir = MouseHelper.GetDirectionToMouse2D(transform, targetCamera);
            if (dir.sqrMagnitude > 0.0001f)
                transform.right = new Vector3(dir.x, dir.y, 0f);
        }

        if (canShoot && CanFireCurrentWeapon() && Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Vector2 dir = MouseHelper.GetDirectionToMouse2D(transform, targetCamera);
            if (dir.sqrMagnitude > 0.0001f)
            {
                FireBullet(dir.normalized);
                nextFireTime = Time.time + fireInterval;
            }
        }

        if (canThrow && canShoot && currentWeapon != null && Input.GetKeyDown(KeyCode.Space))
        {
            ThrowWeapon();
        }
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        canShoot = (newWeapon != null);

        if (newWeapon != null)
        {
            newWeapon.SetState(Weapon.WeaponState.HeldByPlayer);
            newWeapon.transform.SetParent(weaponHoldPoint != null ? weaponHoldPoint : transform);
            newWeapon.transform.localPosition = new Vector3(0.6f, 0f, 0f);
            newWeapon.transform.localRotation = Quaternion.identity;
        }

        if (batMelee != null)
        {
            batMelee.SetWeapon(currentWeapon);
        }
    }

    private void ThrowWeapon()
    {
        if (currentWeapon == null) return;

        Vector2 dir = MouseHelper.GetDirectionToMouse2D(transform, targetCamera);
        if (dir.sqrMagnitude <= 0.0001f)
            dir = transform.right;

        currentWeapon.transform.position = shootPoint.position + (Vector3)(dir.normalized * 0.4f);
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.Throw(dir.normalized, throwForce, throwSpinSpeed, ownerCollider);

        currentWeapon = null;
        canShoot = false;
    }

    private void FireBullet(Vector2 dir)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.transform.right = new Vector3(dir.x, dir.y, 0f);

        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            if (currentWeapon != null)
                bulletComp.SetWeaponType(currentWeapon.WeaponType);

            bulletComp.Initialize(dir, bulletSpeed, gameObject);
            return;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;
    }
    
}