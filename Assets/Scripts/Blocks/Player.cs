using Unity.Mathematics;
using UnityEngine;

public class Player : Block
{

    [SerializeField]
    private Animator myAnim;

    private void Update()
    {
        if (State == MoveStates.idle) MoveInput();
    }

    private void MoveInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (CheckMove(-1, 0)) transform.rotation = Quaternion.LookRotation(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (CheckMove(1, 0)) transform.rotation = Quaternion.LookRotation(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (CheckMove(0, -1)) transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (CheckMove(0, 1)) transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }
    }

    protected override void StartMove(Cell newParent, int _deltaX, int _deltaY)
    {
        myAnim.SetBool("isMoving", true);
        base.StartMove(newParent, _deltaX, _deltaY);
    }

    protected override void FinishMove()
    {
        myAnim.SetBool("isMoving", false);
        base.FinishMove();
    }

}
