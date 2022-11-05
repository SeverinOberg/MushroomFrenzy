using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "Scriptable Objects/Turret", order = 1)]
public class TurretSO : ScriptableObject
{
    public float    minDamage;
    public float    maxDamage;
    public float    attackSpeed;
    public float    attackRange;
    public float    scanCooldown;
    public float    scanRange;
    public float    slowPercentage;
    public float    slowDuration;
}
