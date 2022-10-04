using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckFOVScan : Node
{
    private readonly EnemyBT self;

    public CheckFOVScan(EnemyBT self)
    {
        this.self = self;
    }

    public override NodeState Evalute()
    {
        if (self.target && !self.IsPathPossible(self.transform.position, self.target.transform.position))
        {
            self.ClearTarget();
        }

        if (!self.target)
        {
            if (ScanForTargets(out List<Unit> targets))
            {
                Unit target;
                List<Unit> targetsFound;

                if (SortTargetsOfType(Unit.UnitTypes.Turret, targets, out targetsFound))
                {
                    if (SortClosestTarget(targetsFound, out target))
                    {}
                }
                else if (SortTargetsForPlayer(Unit.UnitTypes.Player, targets, out target))
                {}
                else
                {
                    // If no priority target found, pick whatever was found first for now
                    target = targets[0];
                }

                if (!self.IsPathPossible(self.transform.position, target.transform.position))
                {
                    if (SortTargetsOfType(Unit.UnitTypes.Obstacle, targets, out targetsFound))
                    {
                        if (SortClosestTarget(targetsFound, out target))
                        {}
                    }
                }

                SetTarget(target);
                return state = NodeState.SUCCESS;
            }
        }
        else
        {
            return state = NodeState.SUCCESS;
        }

        return state = NodeState.FAILURE;
    }

    private bool ScanForTargets(out List<Unit> result)
    {
        result = new List<Unit>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.transform.position, self.scanDiameter);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy") || !colliders[i].TryGetComponent(out Unit unit) || unit.isDead)
            {
                continue;
            }

             result.Add(unit);
        }

        if (result.Count > 0)
        {
            return true;
        }

        return false;
    }

    private bool SortTargetsForPlayer(Unit.UnitTypes type, List<Unit> targets, out Unit result)
    {
        result = null;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].type == type)
            {
                result = targets[i];
                return true;
            }
        }

        return false;
    }

    private bool SortTargetsOfType(Unit.UnitTypes type, List<Unit> targets, out List<Unit> result)
    {
        result = new List<Unit>();
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].type == type)
            {
                result.Add(targets[i]);
            }
        }

        if (result.Count > 0)
        {
            return true;
        }

        return false;  
    }

    private bool SortClosestTarget(List<Unit> targets, out Unit target)
    {
        target = null;

        if (targets.Count < 1)
        {
            return false;
        }

        Unit closestTarget = targets[0];
        float closestTargetsDistance = Utilities.GetDistanceBetween(self.transform.position, targets[0].transform.position);
        for (int i = 1; i < targets.Count; i++)
        {
            float distanceFromSortTarget = Utilities.GetDistanceBetween(self.transform.position, targets[i].transform.position);
            if (distanceFromSortTarget < closestTargetsDistance)
            {
                closestTarget = targets[i];
                closestTargetsDistance = Utilities.GetDistanceBetween(self.transform.position, targets[0].transform.position);
            }
        }

        target = closestTarget;
        return true;
    }

    private void SetTarget(Unit target)
    {
        self.target = target;
        self.aiDestinationSetter.target = target.transform;
    }
}
