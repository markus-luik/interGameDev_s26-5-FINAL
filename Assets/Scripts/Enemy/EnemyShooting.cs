using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private Transform weaponHoldPoint;

    [Header("Weapon")]
    [SerializeField] private Weapon startingWeapon;
    [SerializeField] private bool canShoot = false;
    [SerializeField] private BatMelee batMelee;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 16f;
    [SerializeField] private float fireInterval = 0.5f;

    private Weapon currentWeapon;
    private float nextFireTime;

    public Weapon CurrentWeapon => currentWeapon;
    private bool CanFireCurrentWeapon()
    {
        return currentWeapon != null &&
            currentWeapon.WeaponType == WeaponType.A &&
            currentWeapon.HasAmmo();
    }
    private void Awake()
    {
        if (shootPoint == null)
            shootPoint = transform;

        if (enemyVision == null)
            enemyVision = GetComponent<EnemyVision>();

        if (batMelee == null)
        batMelee = GetComponent<BatMelee>();
    }

    private void Start()
    {
        if (startingWeapon != null)
        {
            Weapon weaponInstance = startingWeapon;

            if (startingWeapon.gameObject.scene.rootCount == 0)
            {
                weaponInstance = Instantiate(startingWeapon, transform.position, Quaternion.identity);
            }

            EquipWeapon(weaponInstance);
        }
    }

    private void Update()
    {
        if (!canShoot) return;
        if (currentWeapon == null) return;
        if (targetPlayer == null) return;

        PlayerDamaged playerDamaged = targetPlayer.GetComponent<PlayerDamaged>();
        if (playerDamaged != null && playerDamaged.IsDead)
            return;

        if (enemyVision != null && !enemyVision.canSeePlayer)
            return;

        Vector2 dir = ((Vector2)targetPlayer.position - (Vector2)transform.position).normalized;
        if (dir.sqrMagnitude <= 0.0001f) return;

        transform.right = new Vector3(dir.x, dir.y, 0f);

        if (currentWeapon.WeaponType == WeaponType.A)
        {
            if (!CanFireCurrentWeapon()) return;
            if (Time.time < nextFireTime) return;

            FireBullet(dir);
            nextFireTime = Time.time + fireInterval;
        }
        else if (currentWeapon.WeaponType == WeaponType.B)
        {
            if (batMelee != null)
                batMelee.TrySwing();
        }
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;

        if (currentWeapon == null)
        {
            canShoot = false;
            if (batMelee != null)
            batMelee.SetWeapon(currentWeapon);
            return;
        }

        currentWeapon.transform.SetParent(weaponHoldPoint != null ? weaponHoldPoint : transform);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.SetState(Weapon.WeaponState.HeldByEnemy);
        if (batMelee != null)
            batMelee.SetWeapon(currentWeapon);

        canShoot = true;
    }

    public void ClearWeapon()
    {
        currentWeapon = null;
        canShoot = false;

        if (batMelee != null)
            batMelee.SetWeapon(null);
    }

    private void FireBullet(Vector2 dir)
    {
        
        if (bulletPrefab == null) return;

        Vector3 spawnPos = shootPoint.position + (Vector3)(dir.normalized * 0.35f);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        bullet.transform.right = new Vector3(dir.x, dir.y, 0f);

        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            if (currentWeapon != null)
                bulletComp.SetWeaponType(currentWeapon.WeaponType);

            bulletComp.Initialize(dir, bulletSpeed, gameObject);
            currentWeapon.UseAmmo();
            return;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;
        // Debug.Log("Enemy spawned bullet prefab: " + bullet.name);

        // Bullet bulletComp = bullet.GetComponent<Bullet>();
        // Debug.Log("Bullet component found? " + (bulletComp != null));
    }
}