using UnityEngine;

public class Base : Building 
{
    public override void Die(float destroyDelaySeconds = 3)
    {
        base.Die(destroyDelaySeconds);
        GameManager.Instance.LoseGame();
    }
}
