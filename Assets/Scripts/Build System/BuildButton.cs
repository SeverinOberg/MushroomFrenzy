using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour 
{

    [SerializeField] private TurretSO turretSO;

    private Button button;
    public static System.Action<GameObject> OnBuildTurret;
    public static System.Action<string> OnNotEnoughResources;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BuildButtonClicked);
    }

    private void BuildButtonClicked()
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
