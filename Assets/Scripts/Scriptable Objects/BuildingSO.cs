using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building", order = 2)]
public class BuildingSO : ScriptableObject
{
    public int      woodCost;
    public int      stoneCost;
}
