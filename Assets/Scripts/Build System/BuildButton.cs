using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour 
{

    #region Variables/Properties

    [SerializeField] private UnitSO unitData;
    [SerializeField] private BuildingSO buildingData;
    

    private Button button;
    public static System.Action<GameObject> OnBuildBuilding;
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
        if (ResourceManager.Instance.HasSufficientResources(buildingData.woodCost, buildingData.stoneCost))
        {
            OnBuildBuilding?.Invoke(unitData.prefab);
        }
        else
        {
            OnNotEnoughResources?.Invoke($"Not enough resources to build {unitData.title}");
        }  
    }

    #endregion
}
