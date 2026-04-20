using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // spawn point, bullet prefab, and camera used for mouse conversion
    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera targetCamera;

    // Tuning values for projectile movement.
    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 16f;
    [SerializeField] private float fireInterval = 0.15f;
    [SerializeField] private bool rotatePlayerToMouse = true;
    private float nextFireTime;

    private void Awake()
    {
        // Fallback setup so the script still works when references are not manually assigned.
        if (shootPoint == null)
        {
            shootPoint = transform;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Keep camera reference valid at runtime (useful when main camera spawns later).
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        // Optional aiming behavior: player continuously faces current mouse direction.
        if (rotatePlayerToMouse)
        {
            FaceToMouseDirection();
        }

        // Hold-to-fire input with a fixed interval.
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void FaceToMouseDirection()
    {
        Vector2 dir = MouseHelper.GetDirectionToMouse2D(transform, targetCamera);
        if (dir.sqrMagnitude <= 0.0001f) return;

        transform.right = new Vector3(dir.x, dir.y, 0f);
    }

    private void Shoot()
    {
        // Shoot direction comes from player-to-mouse world-space direction.
        Vector2 dir = MouseHelper.GetDirectionToMouse2D(transform, targetCamera);
        if (dir.sqrMagnitude <= 0.0001f) return;

        FireBullet(dir);
    }

    private void FireBullet(Vector2 dir)
    {
        // Spawn and orient bullet; if a Rigidbody2D exists, push it using velocity.
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.transform.right = new Vector3(dir.x, dir.y, 0f);

        Bullet bulletComp = bullet.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            bulletComp.Initialize(dir, bulletSpeed, gameObject);
            return;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * bulletSpeed;
        }
    }

}