using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private int _current;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _current = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            Reload();
        if (Input.GetKeyDown(KeyCode.Escape))
            LoadMainScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(++_current, LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ApplicationExit()
    {
        Application.Quit();
    }

    public void Reload()
    {
        var scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        Time.timeScale = 1;
    }
}
