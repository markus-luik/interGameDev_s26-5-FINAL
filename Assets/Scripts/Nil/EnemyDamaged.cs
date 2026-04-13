using UnityEngine;

public class EnemyDamaged : MonoBehaviour, IHitReceiver
{
    // Enemy reaction state driven by weapon types.
    public enum EnemyState
    {
        Alive,
        KnockedOut,
        Dead
    }

    [SerializeField] private EnemyState currentState = EnemyState.Alive;

    public void OnHit(HitInfo hitInfo)
    {
        // Dead targets ignore any further hit events.
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        // Route hit behavior by weapon category instead of health subtraction.
        switch (hitInfo.weaponType)
        {
            case WeaponType.A:
                EnterKnockedOut();
                break;
            case WeaponType.B:
                EnterDead();
                break;
        }
    }

    private void EnterKnockedOut()
    {
        // Prevent state rollback from Dead to KnockedOut.
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        currentState = EnemyState.KnockedOut;
        // Leave this block open for custom stun/downed flow.
        // Implement KnockedOut behavior here (animation, AI disable, timer, etc.).
    }

    private void EnterDead()
    {
        
        currentState = EnemyState.Dead;
        //  play death animation/effects first, then destroy.
        Destroy(gameObject);
    }
}
