using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainMenuUI : MonoBehaviour
{
    private const int tutorialLevelScene = 2;
    private const int PlayerUIScene = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        SceneManager.LoadScene(tutorialLevelScene);
        SceneManager.LoadScene(PlayerUIScene, LoadSceneMode.Additive);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
