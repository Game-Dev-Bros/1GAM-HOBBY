using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeTime = 1.5f;
    private float fadeStep = 0;
    public bool isRunning = false;

    void Awake()
    {
        fadeImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeImage.enabled = true;
        fadeImage.color = Color.black;
        StartCoroutine(FadeToColor(Color.clear, fadeTime));
    }

    IEnumerator FadeToColor(Color endColor, float fadeTime, int steps = 60)
    {
        isRunning = true;

        Color startColor = fadeImage.color;

        for(float s = 0; s <= steps; s++)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, s / steps);
            yield return new WaitForSeconds(fadeTime / steps);
        }

        isRunning = false;
    }

    public IEnumerator FadeToScene(string sceneName)
    {
        fadeImage.enabled = true;
        fadeImage.color = Color.clear;

        yield return StartCoroutine(FadeToColor(Color.black, fadeTime));
        SceneManager.LoadScene(sceneName);
    }
}

