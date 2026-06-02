using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] private TMP_Text coinText;

    [Header("Health")]
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private void Start()
    {
        GameManager.Instance.OnCoinsChanged += UpdateCoins;

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        player.OnHealthChanged += UpdateHealth;

        // init
        UpdateCoins(GameManager.Instance.GetCoins());
        UpdateHealth(player.health);
    }

    private void UpdateCoins(int value)
    {
        coinText.text = value.ToString();
    }

    private void UpdateHealth(int hp)
    {
        // máximo 2

        heart1.sprite = (hp >= 1) ? fullHeart : emptyHeart;
        heart2.sprite = (hp >= 2) ? fullHeart : emptyHeart;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCoinsChanged -= UpdateCoins;

        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
            player.OnHealthChanged -= UpdateHealth;
    }
}