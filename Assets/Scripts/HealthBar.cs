using Unity.VisualScripting.ReorderableList.Internal;
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
        unit.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        unit.OnHealthChanged -= UpdateHealthBar;
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

        if (unit.isDead)
        {
            uiUnitCanvas.SetActive(false);
            return;
        }

        uiHealth.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalculatePercentageNormalized(unit.unitData.health, unit.health));
 
    }

    private float CalculatePercentageNormalized(float max, float current)
    {
        return (100.0f / max) * current / 100.0f;
    }



}
