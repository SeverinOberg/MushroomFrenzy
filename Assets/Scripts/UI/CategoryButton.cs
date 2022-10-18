using UnityEngine;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private GameObject[] buildButtons;
    private bool isCategoryButtonActive;

    private void Update()
    {
        if (isCategoryButtonActive && Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0; i < buildButtons.Length; i++)
            {
                buildButtons[i].SetActive(false);
            }
        }
    }

    public void OnMouseDown()
    {
        for (int i = 0; i < buildButtons.Length; i++)
        {
            buildButtons[i].SetActive(!buildButtons[i].activeSelf);
        }
        isCategoryButtonActive = buildButtons[0].activeSelf;
    }

}
