using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "Scriptable Objects/Turret", order = 1)]
public class TurretSO : ScriptableObject
{
    // Get
    public GameObject prefab;
    public string title = "Turret";
    public int woodCost = 5;
    public int stoneCost = 5;

    // Set
    public float Damage = 1;
    public float AttackSpeed = 1;
    public float SlowPercentage = 0;
    public float Range = 15;
}
