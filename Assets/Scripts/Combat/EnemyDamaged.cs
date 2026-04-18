using UnityEngine;

public class EnemyDamaged : MonoBehaviour, IHitReceiver
{
    public enum EnemyState
    {
        Alive,
        KnockedOut,
        Dead
    }

    [Header("State")]
    [SerializeField] private EnemyState currentState = EnemyState.Alive;

    [Header("Weapon B Hits")]
    [SerializeField] private int weaponBHitCount = 0;

    [Header("References")]
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyVision enemyVision;
    [SerializeField] private EnemyShooting enemyShooting;
    [SerializeField] private Animator myAnim;
    [SerializeField] private Collider2D myCollider;

    [Header("Death")]
    [SerializeField] private bool dropWeaponOnDeath = true;
    [SerializeField] private float destroyDelay = 0.5f;
    [SerializeField] private float dropForce = 2f;
    [SerializeField] private float dropSpinMin = -200f;
    [SerializeField] private float dropSpinMax = 200f;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();

        if (enemyVision == null)
            enemyVision = GetComponent<EnemyVision>();

        if (enemyShooting == null)
            enemyShooting = GetComponent<EnemyShooting>();

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (currentState == EnemyState.Dead)
            return;

        switch (hitInfo.weaponType)
        {
            case WeaponType.A:
                EnterDead();
                break;

            case WeaponType.B:
                HandleBatHit();
                break;
        }
    }

    private void HandleBatHit()
    {
        if (currentState == EnemyState.Alive)
        {
            EnterKnockedOut();
        }
        else if (currentState == EnemyState.KnockedOut)
        {
            EnterDead();
        }
    }

    private void EnterKnockedOut()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.KnockedOut;

        if (enemyMovement != null)
            enemyMovement.enabled = false;

        if (enemyVision != null)
            enemyVision.enabled = false;

        if (enemyShooting != null)
            enemyShooting.SetCanShoot(false);

        if (myAnim != null)
            myAnim.SetTrigger("KnockedOut");

        CancelInvoke(nameof(RecoverFromKnockout));
        Invoke(nameof(RecoverFromKnockout), 3f);
    }

    private void RecoverFromKnockout()
    {
        if (currentState != EnemyState.KnockedOut)
            return;

        currentState = EnemyState.Alive;

        if (enemyMovement != null)
            enemyMovement.enabled = true;

        if (enemyVision != null)
            enemyVision.enabled = true;

        if (enemyShooting != null && enemyShooting.CurrentWeapon != null)
            enemyShooting.SetCanShoot(true);

        if (myAnim != null)
            myAnim.SetTrigger("WakeUp");
    }

    private void EnterDead()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Dead;

        CancelInvoke(nameof(RecoverFromKnockout));

        if (enemyMovement != null)
            enemyMovement.enabled = false;

        if (enemyVision != null)
            enemyVision.enabled = false;

        if (enemyShooting != null)
            enemyShooting.SetCanShoot(false);

        if (dropWeaponOnDeath)
            DropWeapon();

        if (myCollider != null)
            myCollider.enabled = false;

        if (myAnim != null)
        {
            myAnim.SetTrigger("Die");
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DropWeapon()
    {
        if (enemyShooting == null)
            return;

        Weapon weapon = enemyShooting.CurrentWeapon;
        if (weapon == null)
            return;

        weapon.transform.SetParent(null);
        weapon.gameObject.SetActive(true);
        weapon.transform.position = transform.position;

        weapon.SetState(Weapon.WeaponState.OnGround);

        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        enemyShooting.ClearWeapon();
    }
}