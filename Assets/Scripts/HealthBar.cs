using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour 
{

    [SerializeField] private GameObject uiUnitCanvas;
    [SerializeField] private Image uiHealth;

    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        uiHealth.fillMethod = Image.FillMethod.Horizontal;
    }

    private void OnEnable()
    {
        unit.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        unit.OnHealthChanged -= UpdateHealthBar;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            unit.TakeDamage(1);
        }
    }

    private void UpdateHealthBar()
    {
        // Only render the Unit's Canvas if it's below the max health of a Unit.
        if (!uiUnitCanvas.activeSelf && unit.health < unit.unitData.health)
        {
            uiUnitCanvas.SetActive(true);
        } 
        // Hide Unit's Canvas again if their health returns to their max health.
        else if (unit.health >= unit.unitData.health)
        {
            uiUnitCanvas.SetActive(false);
        }

        uiHealth.fillAmount = CalculateHealthPercentage();

        Debug.Log(CalculateHealthPercentage());
        Debug.Log(unit.unitData.health);
        Debug.Log(unit.health);
    }

    private float CalculateHealthPercentage()
    {
        return (100.0f / unit.unitData.health) * unit.health;
    }



}
