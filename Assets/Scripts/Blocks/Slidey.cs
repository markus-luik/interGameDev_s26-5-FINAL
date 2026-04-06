using UnityEngine;

public class Slidey : Block
{
    protected override void FinishMove()
    {
        base.FinishMove();
        Slide(moveChange.x, moveChange.y);
    }

    private void Slide(int _deltaX, int _deltaY)
    {
        CheckMove(_deltaX, _deltaY);
    }

}
