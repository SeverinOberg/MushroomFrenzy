using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour 
{

    // Do not edit Scriptable Objects, only for reading!
    [SerializeField] private UnitSO unitData;
    [SerializeField] private BuildingSO buildingData;
    //--

    private Button button;

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
        if (ResourceManager.Instance.HasSufficientResourcesToBuild(buildingData))
        {
            BuildingSystem.Instance.InitializeWithObject(unitData.prefab);
        }
    }

    public void OnMouseEnter()
    {
        Tooltip.OnTooltip?.Invoke(unitData.title, unitData.description, $"Wood: {buildingData.woodCost} Stone: {buildingData.stoneCost} Metal: {buildingData.metalCost}");
    }

    public void OnMouseExit()
    {
        Tooltip.OnTooltip?.Invoke("", "", "");
    }

    #endregion
}
