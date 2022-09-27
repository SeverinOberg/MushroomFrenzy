using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour 
{

    [SerializeField] private GameObject      tooltip;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI resources;


    public static System.Action<string, string, string> OnTooltip;

    private void OnEnable()
    {
        OnTooltip += SetTooltip;
    }

    private void OnDisable()
    {
        OnTooltip -= SetTooltip;
    }

    private void SetTooltip(string title, string description, string resources)
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
