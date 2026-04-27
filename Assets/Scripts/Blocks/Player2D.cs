using UnityEngine;

public class Player2D : Block2D
{
    [SerializeField]
    private Animator myAnim;

    [SerializeField]
    private Animator legsAnim;

    [SerializeField]
    private SpriteRenderer mySprite;

    [SerializeField]
    private SpriteRenderer legsSprite;
    
    // Flag that allows for bypassing player input (so You can move as other entities)
    [SerializeField]
    private bool bypassPlayerInput = false;

    private void Update()
    {
        if (bypassPlayerInput) return; //Exits update loop if bypass flag is true (so You can move as other entities)
        if (State == MoveStates.idle)
        {
            MoveInput();
        }
    }

    private void MoveInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (CheckMove(-1, 0))
            {
                SetFacingFlip(true);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (CheckMove(1, 0))
            {
                SetFacingFlip(false);
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (CheckMove(0, -1))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (CheckMove(0, 1))
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
        }
    }

    protected override void StartMove(Cell newParent, int _deltaX, int _deltaY)
    {
        SetMovingAnimation(true);

        base.StartMove(newParent, _deltaX, _deltaY);
    }

    protected override void FinishMove()
    {
        bool keepMoving =
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S);

        SetMovingAnimation(keepMoving);

        base.FinishMove();
    }

    private void SetFacingFlip(bool flipX)
    {
        if (mySprite != null)
        {
            mySprite.flipX = flipX;
        }

        if (legsSprite != null)
        {
            legsSprite.flipX = flipX;
        }
    }

    private void SetMovingAnimation(bool isMoving)
    {
        if (myAnim != null)
        {
            myAnim.SetBool("isMoving", isMoving);
        }

        if (legsAnim != null)
        {
            legsAnim.SetBool("isMoving", isMoving);
        }
    }
}