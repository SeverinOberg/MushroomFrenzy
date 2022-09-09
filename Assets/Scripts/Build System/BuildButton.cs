using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour 
{

#region Variables/Properties
    [SerializeField] private UnitSO unitData;
    [SerializeField] private TurretSO turretData;
    

    private Button button;
    public static System.Action<GameObject> OnBuildTurret;
    public static System.Action<string>     OnNotEnoughResources;
#endregion

#region Unity
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(BuildButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
#endregion

#region Methods
    public void BuildButtonClicked()
    {
        if (ResourceManager.Instance.CanBuild(turretData.woodCost, turretData.stoneCost))
        {
            OnBuildTurret?.Invoke(unitData.prefab);
        }
        else
        {
            OnNotEnoughResources?.Invoke($"Not enough resources to build {unitData.title}");
        }  
    }
#endregion
}
