using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIGame : MonoBehaviour
{

    #region Variables & Properties

    [SerializeField] private GameObject      escapeMenu;
    [SerializeField] private GameObject      keybindingsMenu;

    [SerializeField] private TextMeshProUGUI woodResourceText;
    [SerializeField] private TextMeshProUGUI stoneResourceText;
    [SerializeField] private TextMeshProUGUI metalResourceText;

    [SerializeField] private TextMeshProUGUI logToScreenText;

    private static System.Action<string> OnLogToScreen;

    #endregion

    #region Unity

    #region Subscriptions

    private void OnEnable()
    {
        OnLogToScreen += SetLogToScreen;

        ResourceManager.OnWoodChanged  += SetWoodText;
        ResourceManager.OnStoneChanged += SetStoneText;
        ResourceManager.OnMetalChanged += SetMetalText;
    }

    private void OnDisable()
    {
        OnLogToScreen -= SetLogToScreen;

        ResourceManager.OnWoodChanged  -= SetWoodText;
        ResourceManager.OnStoneChanged -= SetStoneText;
        ResourceManager.OnMetalChanged -= SetMetalText;
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerEscapeMenu();
        }
    }

    #endregion

    #region Methods

    public static void LogToScreen(string text)
    {
        OnLogToScreen?.Invoke(text);
    }

    private void SetLogToScreen(string text)
    {
        logToScreenText.text = text;
        StartCoroutine(RemoveLogScreenTextRoutine());
    }

    private IEnumerator RemoveLogScreenTextRoutine()
    {
        yield return new WaitForSeconds(1);
        logToScreenText.text = "";
    }

    public void SetWoodText(int amount)
    {
        woodResourceText.text = $"Wood: {amount}";
    }

    public void SetStoneText(int amount)
    {
        stoneResourceText.text = $"Stone: {amount}";
    }

    public void SetMetalText(int amount)
    {
        metalResourceText.text = $"Metal: {amount}";
    }

    public void TriggerEscapeMenu()
    {
        if (!escapeMenu.activeSelf)
        {
            escapeMenu.SetActive(true);
        }
        else
        {
            escapeMenu.SetActive(false);
        }
    }

    public void TriggerKeyBindingsMenu()
    {
        if (!keybindingsMenu.activeSelf)
        {
            keybindingsMenu.SetActive(true);
        }
        else
        {
            keybindingsMenu.SetActive(false);
        }
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #else
        Application.Quit();
    #endif
    }

    #endregion

}
