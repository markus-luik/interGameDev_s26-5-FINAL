using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EntityMover : MonoBehaviour {
    public static EntityMover Instance;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Main entry point — called by TurnManager with the player's input direction
    public void MoveAllYou(Vector2Int dir) {
        var youEntities = BabaGridIndex.Instance.GetAll()
            .Where(e => e.isYou)
            .ToList();

        foreach (var you in youEntities)
            TryMove(you, dir);
    }

    private bool TryMove(GridEntity mover, Vector2Int dir) {
        // Collect the push chain starting from mover
        var chain = new List<GridEntity>();

        if (!CollectChain(mover, dir, chain))
            return false;

        // Execute the move — furthest entity first
        chain.Reverse();
        foreach (var entity in chain)
            ExecuteMove(entity, dir);

        return true;
    }

    // Recursively collects all entities that need to move
    // Returns false if the move is blocked
    private bool CollectChain(GridEntity mover, Vector2Int dir, List<GridEntity> chain) {
        var targetPos = mover.gridPos + dir;
        var occupants = BabaGridIndex.Instance.GetAt(targetPos);

        foreach (var occupant in occupants) {
            // STOP blocks the chain unless it is also PUSH
            if (occupant.isStop && !occupant.isPush)
                return false;

            // If pushable, add to chain and continue checking ahead
            if (occupant.isPush) {
                if (!CollectChain(occupant, dir, chain))
                    return false;
            }
        }

        chain.Add(mover);
        return true;
    }

    private void ExecuteMove(GridEntity entity, Vector2Int dir) {
        var block = entity.GetComponent<Block2D>();
        if (block != null)
            block.ForceMove(dir.x, dir.y);
    }
}