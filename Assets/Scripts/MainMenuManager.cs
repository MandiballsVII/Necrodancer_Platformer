using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        GameManager.Instance.ResetCoins();
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelIndex);
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlayButtonClick();
    }

    public void OnButtonHover()
    {
        AudioManager.Instance.PlayButtonHover();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }
}
