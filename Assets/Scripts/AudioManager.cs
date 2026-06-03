using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Player Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip endSound;

    [Header("Enemy Clips")]
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip enemyRiseFallSound;

    [Header("UI Clips")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHover;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayJump()
    {
        sfxSource.PlayOneShot(jumpSound);
    }

    public void PlayAttack()
    {
        sfxSource.PlayOneShot(attackSound);
    }

    public void PlayCoin()
    {
        sfxSource.PlayOneShot(coinSound);
    }

    public void PlayHit()
    {
        sfxSource.PlayOneShot(hitSound);
    }
    public void PlayHeal()
    {
        sfxSource.PlayOneShot(healSound);
    }
    public void PlayEnemyHit()
    {
        sfxSource.PlayOneShot(enemyHitSound);
    }
    
    public void PlayEnemyRiseFall()
    {
        sfxSource.PlayOneShot(enemyRiseFallSound);
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayButtonHover()
    {
        sfxSource.PlayOneShot(buttonHover);
    }

    public void PlayPlayerDeath()
    {
        sfxSource.PlayOneShot(deathSound);
    }
    public void PlayEndGame()
    {
        sfxSource.PlayOneShot(endSound);
    }

    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic)
            return;

        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayLevelMusic()
    {
        if (musicSource.clip == levelMusic)
            return;

        musicSource.clip = levelMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}