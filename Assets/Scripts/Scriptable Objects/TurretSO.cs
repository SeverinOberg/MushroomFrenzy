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
    public float health = 10;
    public float damage = 1;
    public float range = 15;
    public float slowPercentage = 0;
    public float slowDuration = 0;
    public float shootCooldown = 2;
}
