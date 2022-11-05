using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MenuUIManager : MonoBehaviour 
{

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenLoadUI()
    {
        // Hide Home UI body
        // Show Load UI
    }

    public void OpenSettingsUI()
    {
        // Hide Home UI body
        // Show Settings UI
    }

    public void Quit()
    {
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #else
        Application.Quit();
    #endif
    }

}
