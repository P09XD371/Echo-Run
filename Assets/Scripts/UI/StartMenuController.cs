using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public SceneFader fader;

    public void OnStartClick()
    {
        fader.FadeToScene("Level1");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    void Awake()
    {
        if (fader == null)
            fader = FindObjectOfType<SceneFader>();
    }
}
