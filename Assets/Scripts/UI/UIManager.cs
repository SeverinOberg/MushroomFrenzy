using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField] private PlayerController player;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject lostScreen;

    [SerializeField] private GameObject escapeMenu;
    [SerializeField] private GameObject keybindingsMenu;

    [SerializeField] private TextMeshProUGUI spiritEssenceResourceText;
    [SerializeField] private TextMeshProUGUI woodResourceText;
    [SerializeField] private TextMeshProUGUI stoneResourceText;
    [SerializeField] private TextMeshProUGUI ironOreResourceText;
    [SerializeField] private TextMeshProUGUI ironBarResourceText;

    [SerializeField] private TextMeshProUGUI logToScreenText;

    [SerializeField] private GameObject selectedTargetUI;
    [SerializeField] private GameObject smelterUI;
    [SerializeField] private TextMeshProUGUI smelterUIResourceAmountText;

    public System.Action OnSmelterUILoadBtnClick;

    public System.Action OnInfiniteModeBtnClick;

    private static System.Action<string, float> OnLogToScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        OnLogToScreen += SetLogToScreen;

        player.resourceManager.OnSetSpiritEssence += SetSpiritEssenceText;
        player.resourceManager.OnSetWood          += SetWoodText;
        player.resourceManager.OnSetStone         += SetStoneText;
        player.resourceManager.OnSetIronOre       += SetIronOreText;
        player.resourceManager.OnSetIronBar       += SetIronBarText;
    }

    private void OnDisable()
    {
        OnLogToScreen -= SetLogToScreen;

        player.resourceManager.OnSetSpiritEssence -= SetSpiritEssenceText;
        player.resourceManager.OnSetWood          -= SetWoodText;
        player.resourceManager.OnSetStone         -= SetStoneText;
        player.resourceManager.OnSetIronOre       -= SetIronOreText;
        player.resourceManager.OnSetIronBar       -= SetIronBarText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerEscapeMenu();
        }
    }

    public static void LogToScreen(string text, float seconds = 1)
    {
        OnLogToScreen?.Invoke(text, seconds);
    }

    private void SetLogToScreen(string text, float seconds)
    {
        logToScreenText.text = text;
        StartCoroutine(RemoveLogScreenTextRoutine(seconds));
    }

    private IEnumerator RemoveLogScreenTextRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        logToScreenText.text = "";
    }

    public void SetSpiritEssenceText(int amount)
    {
        spiritEssenceResourceText.text = $"{amount}";
    }

    public void SetWoodText(int amount)
    {
        woodResourceText.text = $"{amount}";
    }

    public void SetStoneText(int amount)
    {
        stoneResourceText.text = $"{amount}";
    }

    public void SetIronOreText(int amount)
    {
        ironOreResourceText.text = $"{amount}";
    }

    public void SetIronBarText(int amount)
    {
        ironBarResourceText.text = $"{amount}";
    }

    public void TriggerSelectedTargetInterface()
    {
        if (!selectedTargetUI.activeSelf)
        {
            selectedTargetUI.SetActive(true);
        }
        else
        {
            selectedTargetUI.SetActive(false);
        }
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

    public void ToggleWinScreen()
    {
        if (winScreen.activeSelf)
            winScreen.SetActive(false);
        else
            winScreen.SetActive(true);
    }

    public void ToggleLoseScreen()
    {
        if (lostScreen.activeSelf)
            lostScreen.SetActive(false);
        else
            lostScreen.SetActive(true);
    }

    public void ToggleSmelterUI()
    {
        if (smelterUI.activeSelf)
            smelterUI.SetActive(false);
        else
            smelterUI.SetActive(true);
    }

    public void SetSmelterUIActive(bool value)
    {
        smelterUI.SetActive(value);
    }

    public void SetSmelterUIResourceAmount(int value)
    {
        smelterUIResourceAmountText.text = $"{value}";
    }

    public void OnSmelterUILoadBtnClickEvent()
    {
        OnSmelterUILoadBtnClick?.Invoke();
    }

    public void OnInfiniteModeBtnClickCallback()
    {
        ToggleWinScreen();
        GameManager.Instance.HasWon = false;
        OnInfiniteModeBtnClick?.Invoke();
    }

    public static void Restart()
    {
        GameManager.Instance.HasLost = false;
        GameManager.Instance.HasWon  = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
        GameManager.Instance.HasLost = false;
        GameManager.Instance.HasWon  = false;

    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #else
        Application.Quit();
    #endif
    }

}
