using UnityEngine;

public class Player2D : Block2D
{
    [SerializeField]
    private Animator myAnim;

    [SerializeField]
    private SpriteRenderer mySprite;

    private void Update()
    {
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
                if (mySprite != null) mySprite.flipX = true;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (CheckMove(1, 0))
            {
                if (mySprite != null) mySprite.flipX = false;
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
        if (myAnim != null)
        {
            myAnim.SetBool("isMoving", true);
        }

        base.StartMove(newParent, _deltaX, _deltaY);
    }

    protected override void FinishMove()
    {
        if (myAnim != null)
        {
            myAnim.SetBool("isMoving", false);
        }

        base.FinishMove();
    }
}