using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour 
{

    public static GameManager Instance;

    private bool hasWon;
    private bool hasLost;

    public bool HasWon  { get { return hasWon;  } set { hasWon  = value; } }
    public bool HasLost { get { return hasLost; } set { hasLost = value; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(AstarPathScan(3));
        }

        UIManager.LogToScreen("The monsters are hungry for mushrooms and are drawing near. Protect yourself!", 5);
    }

    private IEnumerator AstarPathScan(float everySeconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(everySeconds);
            if (!AstarPath.active)
            {
                continue;
            }
            AstarPath.active.Scan();
        }
    }

    public void WinGame()
    {
        UIManager.Instance.ToggleWinScreen();
        HasWon = true;
    }

    public void LoseGame()
    {
        HasLost = true;
        UIManager.Instance.ToggleLoseScreen();
    }
}
