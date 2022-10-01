using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour 
{

    public static GameManager Instance;
    public bool hasWon { get; private set; }
    public bool hasLost { get; private set; }

    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LostScreen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(AstarPathScan(3));
    }

    private IEnumerator AstarPathScan(float everySeconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(everySeconds);
            AstarPath.active.Scan();
        }
    }

    public void WinGame()
    {
        hasWon = true;
    }

    public void LoseGame()
    {
        hasLost = true;
        LostScreen.SetActive(true);
    }
}
