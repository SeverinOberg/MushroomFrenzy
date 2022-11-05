using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/Item", order = 0)]
public class ItemSO : ScriptableObject 
{
    public string            id;
    public GameObject        prefab;
    public Sprite            icon;
    public string            title;
    [TextArea] public string description;
    public ResourceObject    buyPrice;
    public ResourceObject    sellPrice;

}
