using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int coins;

    public event Action<int> OnCoinsChanged;
    private bool isPaused;
    private bool gameEnded;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int GetCoins() => coins;

    public void AddCoin()
    {
        coins++;
        OnCoinsChanged?.Invoke(coins);
    }

    public void ResetCoins()
    {
        coins = 0;
        OnCoinsChanged?.Invoke(coins);
    }

    public void RestartLevel()
    {
        ResetCoins();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        ResetCoins();
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        PauseMenu.Instance.Show();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PauseMenu.Instance.Hide();
    }
    public void TriggerEndGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();

        if (spawner != null)
        {
            spawner.StopSpawning();
        }
        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.EnterEndGameState();
            player.enabled = false;
        }
        StartCoroutine(KillWaiter());

        EndGameUI.Instance.ShowResult(coins);
    }

    IEnumerator KillWaiter()
    {
        yield return new WaitForSeconds(1f);
        SkeletonEnemy[] enemies = FindObjectsByType<SkeletonEnemy>(FindObjectsSortMode.None);

        foreach (SkeletonEnemy enemy in enemies)
        {
            enemy.Die();
        }
    }
}