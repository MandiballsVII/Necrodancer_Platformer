using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField] private GameObject panel;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void OnResumeButton()
    {
        GameManager.Instance.ResumeGame();
    }

    public void OnRestartButton()
    {
        GameManager.Instance.RestartLevel();
    }

    public void OnExitButton()
    {
        GameManager.Instance.LoadLevel(0);
    }
}
