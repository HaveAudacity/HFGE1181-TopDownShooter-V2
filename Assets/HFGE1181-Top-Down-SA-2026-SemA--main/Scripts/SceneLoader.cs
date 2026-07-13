using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit Game");
#endif

        Application.Quit();
    }
}