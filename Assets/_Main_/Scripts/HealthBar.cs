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
    }

    private void OnEnable()
    {
        unit.OnSetHealth    += UpdateHealthBar;
        unit.OnSetMaxHealth += UpdateHealthBar;
        unit.OnSetIsDead    += UpdateHealthBar;
    }

    private void OnDisable()
    {
        unit.OnSetHealth    -= UpdateHealthBar;
        unit.OnSetMaxHealth -= UpdateHealthBar;
        unit.OnSetIsDead    -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        // Only render the Unit's Canvas if it's below the max health of a Unit.
        if (!uiUnitCanvas.activeSelf && unit.Health < unit.MaxHealth)
        {
            uiUnitCanvas.SetActive(true);
        } 
        // Hide Unit's Canvas again if their health returns to their max health.
        else if (unit.Health >= unit.MaxHealth)
        {
            uiUnitCanvas.SetActive(false);
            return;
        }

        uiHealth.transform.localScale = new Vector2
        (
            Utilities.CalculatePercentageNormalized(unit.Health, unit.MaxHealth),
            uiHealth.transform.localScale.y
        );
    }
}
