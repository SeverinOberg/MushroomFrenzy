using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private GameObject[] buildButtons;
    [SerializeField] private Image categoryImageBackground;
    [SerializeField] private Sprite backgroundSpriteHighlight;
    private Sprite initialBackgroundSprite;
    
    
    private bool isCategoryButtonActive;

    private void Awake()
    {
        initialBackgroundSprite = categoryImageBackground.sprite;
    }

    private void Update()
    {
        if (isCategoryButtonActive && Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0; i < buildButtons.Length; i++)
            {
                buildButtons[i].SetActive(false);
            }
            isCategoryButtonActive = false;
            HandleSelectedHighlight();
        }
    }

    public void OnMouseDown()
    {
        for (int i = 0; i < buildButtons.Length; i++)
        {
            buildButtons[i].SetActive(!buildButtons[i].activeSelf);
        }

        isCategoryButtonActive = buildButtons[0].activeSelf;
        HandleSelectedHighlight();
    }

    private void HandleSelectedHighlight()
    {
        categoryImageBackground.sprite = isCategoryButtonActive ? backgroundSpriteHighlight : initialBackgroundSprite;
    }

}
