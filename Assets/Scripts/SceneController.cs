using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    // Llama esto desde otro script, cuando quieras esperar antes de cambiar de escena
    public void SceneChangeWithDelay(string name)
    {
        StartCoroutine(LoadSceneAfterDelay(name, 1f));
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    // Método directo (sin esperar), por compatibilidad
    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}