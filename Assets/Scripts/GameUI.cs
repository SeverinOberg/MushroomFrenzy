using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Quit()
    {
        SceneManager.LoadScene(0);
    }

}
