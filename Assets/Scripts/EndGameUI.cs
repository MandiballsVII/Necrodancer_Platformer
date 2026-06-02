using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float holdTime = 5f;

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0f;
        //canvasGroup.gameObject.SetActive(false);
    }

    public void ShowResult(int coins)
    {
        //canvasGroup.gameObject.SetActive(true);

        if (coins < 10)
        {
            messageText.text =
                "Has llegado al final pero no has recogido suficientes cráneos para realizar el hechizo que salvará a la humanidad, una pena.";
        }
        else
        {
            messageText.text =
                "¡Buen trabajo! Con 10 cráneos podrás realizar el hechizo que salve a la humanidad.";
        }

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(holdTime);

        GameManager.Instance.LoadLevel(0);
    }
}