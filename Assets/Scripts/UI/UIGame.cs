using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIGame : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI woodResourceText;
    [SerializeField] private TextMeshProUGUI stoneResourceText;
    [SerializeField] private TextMeshProUGUI infoText;

    #region Subscriptions

    private void OnEnable()
    {
        ResourceManager.OnWoodChanged += SetWoodText;
        ResourceManager.OnStoneChanged += SetStoneText;
        BuildButton.OnNotEnoughResources += SetInfoText;
    }

    private void OnDisable()
    {
        ResourceManager.OnWoodChanged -= SetWoodText;
        ResourceManager.OnStoneChanged -= SetStoneText;
        BuildButton.OnNotEnoughResources -= SetInfoText;
    }

    #endregion

    private void SetInfoText(string text)
    {
        infoText.text = text;
        StartCoroutine(RemoveInfoText());
    }

    private IEnumerator RemoveInfoText()
    {
        yield return new WaitForSeconds(1);
        infoText.text = "";
    }

    public void SetWoodText(int amount)
    {
        woodResourceText.text = $"Wood: {amount}";
    }

    public void SetStoneText(int amount)
    {
        stoneResourceText.text = $"Stone: {amount}";
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
        SceneManager.LoadScene(0);
    }

}
