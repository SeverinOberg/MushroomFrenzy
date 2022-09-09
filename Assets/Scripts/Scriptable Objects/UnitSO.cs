using UnityEngine;

[CreateAssetMenu(fileName = "UnitSO", menuName = "Scriptable Objects/Unit", order = 0)]
public class UnitSO : ScriptableObject 
{
    public GameObject   prefab;
    public string       title;
    public Factions     faction;
    public float        health;
    public float        movementSpeed;
}
