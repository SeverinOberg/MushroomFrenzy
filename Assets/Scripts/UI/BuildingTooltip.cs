using UnityEngine;
using TMPro;

public class BuildingTooltip : MonoBehaviour 
{

    [SerializeField] private GameObject      tooltip;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI resources;

    public System.Action<string, string, string> OnTooltip;

    private void OnEnable()
    {
        OnTooltip += OnTooltipCallback;
    }

    private void OnDisable()
    {
        OnTooltip -= OnTooltipCallback;
    }

    private void OnTooltipCallback(string title, string description, string resources)
    {
        if (title == "")
        {
            tooltip.SetActive(false);
            return;
        }

        tooltip.SetActive(true);
        this.title.text       = title;
        this.description.text = description;
        this.resources.text   = resources;
    }

}
