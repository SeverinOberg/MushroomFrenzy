using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building", order = 2)]
public class BuildingSO : ScriptableObject
{
    // Build
    [Header("Build Data")]
    public int woodCost;
    public int stoneCost;
    public int metalCost;

    // Upgrade
    [Header("Upgrade Data")]

    [Tooltip("The maximum level a building can be upgraded to")]
    public int maxLevel;

    [Header("Level 2")]
    public int level2UpgradeWoodCost;
    public int level2UpgradeStoneCost;
    public int level2UpgradeMetalCost;

    [Header("Level 3")]
    public int level3UpgradeWoodCost;
    public int level3UpgradeStoneCost;
    public int level3UpgradeMetalCost;
}
