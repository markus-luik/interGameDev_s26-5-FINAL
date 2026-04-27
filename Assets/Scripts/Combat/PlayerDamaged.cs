using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerDamaged : MonoBehaviour, IHitReceiver
{
    public enum PlayerState
    {
        Alive,
        Dead
    }

    [Header("State")]
    [SerializeField] private PlayerState currentState = PlayerState.Alive;

    [Header("References")]
    [SerializeField] private Player2D playerMovement;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private PunchMelee punchMelee;
    [SerializeField] private BatMelee batMelee;
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private Animator myAnim;

    [Header("Restart")]
    [SerializeField] private bool pressRToRestart = true;
    [SerializeField] private GameObject restartText;

    public PlayerState CurrentState => currentState;
    public bool IsDead => currentState == PlayerState.Dead;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<Player2D>();

        if (playerShooting == null)
            playerShooting = GetComponent<PlayerShooting>();

        if (punchMelee == null)
            punchMelee = GetComponent<PunchMelee>();

        if (batMelee == null)
            batMelee = GetComponent<BatMelee>();

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (pressRToRestart && currentState == PlayerState.Dead && Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (currentState == PlayerState.Dead)
            return;

        //Both bullet and baseball bat kill the player
        switch (hitInfo.weaponType)
        {
            case WeaponType.A: //gun/bullet
            case WeaponType.B: //baseball bat
                EnterDead();
                break;
        }
    }

    private void EnterDead()
    {
        if (currentState == PlayerState.Dead)
            return;

        currentState = PlayerState.Dead;

        if (restartText != null)
            restartText.SetActive(true);
            
        if (playerShooting != null)
            playerShooting.DropCurrentWeapon(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerShooting != null)
        {
            playerShooting.SetCanShoot(false);
            playerShooting.enabled = false;
        }

        if (punchMelee != null)
            punchMelee.enabled = false;

        if (batMelee != null)
            batMelee.enabled = false;

        if (myCollider != null)
            myCollider.enabled = false;

        if (myAnim != null)
            myAnim.SetTrigger("Die");

        //Debug.Log("Player died. Press R to restart.");
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}