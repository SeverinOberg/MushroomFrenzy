using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

[TaskDescription("Scans a circle around this object to find a and attach a target to our Enemy target variable AND AIPath/AIDestinationSetter")]
public class Scan : Conditional
{
    [SerializeField] private SharedEnemy self;

    private LayerMask scanLayerMask;
    private float initialMeleeAttackRange;

    public override void OnAwake()
    {
        initialMeleeAttackRange = self.Value.meleeAttackRange;
        scanLayerMask = LayerMask.GetMask("Turret", "Player", "Obstacle");
    }

    public override TaskStatus OnUpdate()
    {
        if (self.Value.target && !self.Value.IsPathPossible(transform.position, self.Value.target.transform.position))
        {
            self.Value.ClearTarget();

            if (ScanForTargets(out List<Unit> targets))
            {
                if (SortTargetsOfType(Unit.UnitTypes.Obstacle, targets, out List<Unit> targetsFound))
                {
                    SortClosestTarget(targetsFound, out Unit target);
                    self.Value.SetTarget(target);
                    self.Value.meleeAttackRange = (target.GetComponent<Collider2D>().bounds.size.x * 0.5f) + initialMeleeAttackRange;
                    return TaskStatus.Success;
                }
            }
        }

        if (!self.Value.target)
        {
            if (ScanForTargets(out List<Unit> targets))
            {
                Unit target;
                List<Unit> targetsFound;

                if (SortTargetsOfType(Unit.UnitTypes.Turret, targets, out targetsFound))
                {
                    SortClosestTarget(targetsFound, out target);
                }
                else if (SortTargetsForPlayer(Unit.UnitTypes.Player, targets, out target))
                { }
                else
                {
                    // If no priority target found, pick whatever is first in our scan
                    target = targets[0];
                }

                if (!self.Value.IsPathPossible(transform.position, target.transform.position))
                {
                    if (SortTargetsOfType(Unit.UnitTypes.Obstacle, targets, out targetsFound))
                    {
                        SortClosestTarget(targetsFound, out target);
                    }
                }

                self.Value.SetTarget(target);
                self.Value.meleeAttackRange = (target.GetComponent<Collider2D>().bounds.size.x * 0.5f) + initialMeleeAttackRange;
                return TaskStatus.Success;
            }
        }
        else
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

    private bool ScanForTargets(out List<Unit> result)
    {
        result = new List<Unit>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, self.Value.scanDiameter, scanLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy") || !colliders[i].TryGetComponent(out Unit unit) || unit.IsDead)
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
        float closestTargetsDistance = Utilities.GetDistanceBetween(transform.position, targets[0].transform.position);
        for (int i = 1; i < targets.Count; i++)
        {
            float distanceFromSortTarget = Utilities.GetDistanceBetween(transform.position, targets[i].transform.position);
            if (distanceFromSortTarget < closestTargetsDistance)
            {
                closestTarget = targets[i];
                closestTargetsDistance = Utilities.GetDistanceBetween(transform.position, targets[i].transform.position);
            }
        }

        target = closestTarget;
        return true;
    }

}
