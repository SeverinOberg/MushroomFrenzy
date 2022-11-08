using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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
    [SerializeField] private Image smelterUIProgressBarImage;

    [SerializeField] private GameObject armoryUI;
    [SerializeField] private GameObject armoryUIBowSlot;
    [SerializeField] private GameObject armoryUIMagicStaffSlot;

    [SerializeField] private GameObject merchantUI;

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

        player.ResourceManager.OnSetSpiritEssence += SetSpiritEssenceText;
        player.ResourceManager.OnSetWood          += SetWoodText;
        player.ResourceManager.OnSetStone         += SetStoneText;
        player.ResourceManager.OnSetIronOre       += SetIronOreText;
        player.ResourceManager.OnSetIronBar       += SetIronBarText;
    }

    private void OnDisable()
    {
        transform.DOKill();
        OnLogToScreen -= SetLogToScreen;

        player.ResourceManager.OnSetSpiritEssence -= SetSpiritEssenceText;
        player.ResourceManager.OnSetWood          -= SetWoodText;
        player.ResourceManager.OnSetStone         -= SetStoneText;
        player.ResourceManager.OnSetIronOre       -= SetIronOreText;
        player.ResourceManager.OnSetIronBar       -= SetIronBarText;
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

    public void SetSmelterUIProgressBarFillAmount(float reloadSeconds, float smeltSeconds)
    {
        smelterUIProgressBarImage.DOFillAmount(1, reloadSeconds).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            smelterUIProgressBarImage.DOFillAmount(0, smeltSeconds);
        });
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

    public void SetMerchantUIActive(bool value)
    {
        merchantUI.SetActive(value);
    }

    public void DeactivateMerchantUI()
    {
        merchantUI.SetActive(false);
    }

    // + Armory +
    public void SetArmoryUI(bool value)
    {
        armoryUI.SetActive(value);
    }

    public void EnableArmoryBowSlot()
    {
        armoryUIBowSlot.SetActive(true);

    }

    public void EnableArmoryMagicStaffSlot()
    {
        armoryUIMagicStaffSlot.SetActive(true);
    }
    // - Armory -

    public static void Quit()
    {
        GameManager.Instance.HasLost = false;
        GameManager.Instance.HasWon  = false;

        SceneManager.LoadScene(0);
    }

}