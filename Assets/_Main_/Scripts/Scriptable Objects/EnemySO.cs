using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/Enemy", order = 0)]
public class EnemySO : ScriptableObject 
{
    [Header("General")]
    public Enemy.EnemyTypes type;
    public float scanRange;

    [Header("Melee")]
    public bool  isMelee;
    public float meleeAttackRange;
    public float meleeAttackSpeed;
    public float minMeleeDamage;
    public float maxMeleeDamage;
    public float meleeKnockbackForce;

    [Header("Ranged")]
    public bool  isRanged;
    public float rangedAttackRange;
    public float rangedAttackSpeed;
    public float minRangedDamage;
    public float maxRangedDamage;
    public float rangedKnockbackForce;
    public float minRangedAccuracyOffset;
    public float maxRangedAccuracyOffset;
}
