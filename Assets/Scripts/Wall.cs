using UnityEngine;

public class Wall : Building 
{

    protected override void Awake()
    {
        base.Awake();
        type = UnitTypes.Obstacle;
    }

}
