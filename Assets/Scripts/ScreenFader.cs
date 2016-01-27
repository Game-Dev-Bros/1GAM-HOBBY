using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    private Image fadeImage;
    public float fadeTime = 1.5f;
    public bool isRunning = false;

    void Awake()
    {
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        fadeImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeImage.enabled = true;
        fadeImage.color = Color.black;
        var isForceSleeping = (PlayerPrefs.GetInt(Constants.Prefs.FORCE_SLEEPING, Constants.Prefs.Defaults.FORCE_SLEEPING) == 1);

        if (!isForceSleeping) //|| PlayerPrefs.GetString(Constants.Prefs.LAST_ACTIVITIES) != Constants.Actions.SLEEP_NIGHT
            StartCoroutine(FadeToColor(Color.clear, fadeTime));
    }

    public IEnumerator FadeToColor(Color endColor, float fadeTime, int steps = 60)
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

        yield return StartCoroutine(FadeToColor(Constants.Colors.FADE, fadeTime));
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SavePlayerData();
        SceneManager.LoadScene(sceneName);
    }
}

