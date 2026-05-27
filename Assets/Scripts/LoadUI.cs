using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadUI : MonoBehaviour
{
    private const int PlayerUIScene = 1;
    void Start()
    {
        SceneManager.LoadScene(PlayerUIScene, LoadSceneMode.Additive);
    }
}
