using UnityEngine;
using System.Collections.Generic;

public class Stickable : MonoBehaviour
{


    // public List<Sticky> stuckObj = new List<Sticky>();

    // #region Sticky Object Methods
    // public void AddSticky(Sticky sticky)
    // {
    //     stuckObj.Add(sticky);
    // }
    
    // /// <summary>
    // /// Calls when the attached block moves on the gridf
    // /// </summary>
    // void BlockMoved(Vector2Int _change)
    // {
    //     if (stuckObj.Count > 0) MoveStuckObjs(_change.x, _change.y);
    // }

    // /// <summary>
    // /// Moves an sticky blocks attached to this block
    // /// </summary>
    // private void MoveStuckObjs(int _deltaX, int _deltaY)
    // {
    //     List<int> _removeList = new List<int>();
    //     for (int i = 0; i < stuckObj.Count; i++)
    //     {
    //         stuckObj[i].StickyCheckMove(_deltaX, _deltaY, gameObject);
    //     }
    //     RemoveStuckObjs(_removeList);
    // }
    
    // /// <summary>
    // /// Removes any sticky blocks attached to this block
    // /// </summary>
    // private void RemoveStuckObjs(List<int> _removeList)
    // {
    //     foreach (int obj in _removeList)
    //     {
    //         stuckObj.RemoveAt(obj);
    //     }
    // }

    // #endregion
}
