using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void LoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeOutAndLoad(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = 1f - (timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = timer / fadeDuration;
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        SceneManager.LoadScene(sceneName);
    }
}