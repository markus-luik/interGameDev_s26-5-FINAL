using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Sticky : Block
{
    private Vector2Int[] touchDirs = {
        new Vector2Int(1, 0), 
        new Vector2Int(-1, 0), 
        new Vector2Int(0, 1), 
        new Vector2Int(0, -1)
    };

    public Block leadBlock;

    public Block LeadBlock
    {
        get => leadBlock;
        set
        {
            leadBlock = value;
        }
    }

    private int leadDirIndex = -1;
    bool leadMoved = false;

    protected override void Start()
    {
        base.Start();
        CheckAdjacent();
    }

    public override bool CheckMove(int _deltaX, int _deltaY)
    {
        if(state == MoveStates.idle)
        {
            if (LeadBlock != null && !leadMoved)
            {
                if (!LeadBlock.CheckMove(_deltaX, _deltaY))
                {
                    leadDirIndex = -1;
                    LeadBlock = null;
                }
            }
            
        }
        return base.CheckMove(_deltaX, _deltaY);
    }

    protected override void GridChanged()
    {
        CheckAdjacent();
        if (LeadBlock != null)
        {
            if (LeadBlock.gridPos - gridPos != touchDirs[leadDirIndex])
            {
                leadMoved = true;
                if(!CheckMove(LeadBlock.moveChange.x, LeadBlock.moveChange.y)){
                    leadDirIndex = -1;
                    LeadBlock = null;
                }
                leadMoved = false;
            }
        } 
    }

    /// <summary>
    /// Checks all orthogonal cells to see if there is a block this block can stick to
    /// </summary>
    private void CheckAdjacent()
    {
        if (LeadBlock == null)
        {
            for (int i = 0; i < touchDirs.Length; i++)
            {
                Cell checkCell = gridManager.gridList[gridPos.x + touchDirs[i].x][gridPos.y + touchDirs[i].y].GetComponent<Cell>();
                if (checkCell.ContainObj != null && checkCell.ContainObj.GetComponent<Block>().canMove)
                {
                    leadDirIndex = i;
                    LeadBlock = checkCell.ContainObj.GetComponent<Block>();
                    break;
                }
            }
        }
        
    }

}
