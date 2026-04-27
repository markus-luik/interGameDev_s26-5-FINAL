using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    private bool playerInRange = false;
    private PlayerShooting playerShootingInRange;

    private void Awake()
    {
        if (weapon == null)
            weapon = GetComponent<Weapon>();
    }

    private void Update()
    {
        if (!playerInRange || playerShootingInRange == null || weapon == null)
            return;

        if (weapon.CurrentState != Weapon.WeaponState.OnGround)
            return;

        // If player already has a weapon, require Space to pick up
        if (playerShootingInRange.HasWeapon && Input.GetKeyDown(KeyCode.Space))
        {
            playerShootingInRange.EquipWeapon(weapon);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();
        if (playerShooting == null)
            return;

        if (weapon == null)
            return;

        if (weapon.CurrentState != Weapon.WeaponState.OnGround)
            return;

        //No weapon: auto pickup
        if (!playerShooting.HasWeapon)
        {
            playerShooting.EquipWeapon(weapon);
            return;
        }

        //Already has weapon: stay in range and wait for Space
        playerInRange = true;
        playerShootingInRange = playerShooting;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();
        if (playerShooting == null)
            return;

        if (playerShooting == playerShootingInRange)
        {
            playerInRange = false;
            playerShootingInRange = null;
        }
    }
}