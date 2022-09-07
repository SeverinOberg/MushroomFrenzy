using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour 
{

    [SerializeField] private TurretSO turretSO;

    private Button button;
    public static System.Action<GameObject> OnBuildTurret;
    public static System.Action<string> OnNotEnoughResources;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BuildButtonClicked);
        //button.OnPointerEnter()
        //button.OnPointerEnter()
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void BuildButtonClicked()
    {
        if (ResourceManager.Instance.CanBuild(turretSO.woodCost, turretSO.stoneCost))
        {
            OnBuildTurret?.Invoke(turretSO.prefab);
        }
        else
        {
            OnNotEnoughResources?.Invoke($"Not enough resources to build {turretSO.title}");
        }  
    }
}
