using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu, optionsMenu;
    public Button continueButton;
    public Toggle use24HourClock;
    public Slider volume;
    private Image title;
    public float minScale = 0.95f;
    public float maxScale = 1.05f;
    public float scaleDuration = 4f;

    void Awake()
    {
        bool hasSavedGame = (PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME) > 0);
        continueButton.interactable = hasSavedGame;

        title = GameObject.Find("Title").GetComponent<Image>();
        use24HourClock.isOn = PlayerPrefs.GetInt(Constants.Prefs.USE_24_HOUR_CLOCK, Constants.Prefs.Defaults.USE_24_HOUR_CLOCK) == 1;
        volume.value = PlayerPrefs.GetFloat(Constants.Prefs.VOLUME, Constants.Prefs.Defaults.VOLUME);
        AudioListener.volume = volume.value;
        
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        title.transform.localScale = Vector3.one * minScale;
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        yield return StartCoroutine(FadeInTitle(1));
        StartCoroutine(ScaleTitleUp(scaleDuration));
        ShowMain();
    }

    IEnumerator ScaleTitleDown(float duration)
    {
        title.enabled = true;
        float time = 0;
        title.transform.localScale = Vector3.one * maxScale;
        while (time < duration)
        {
            time += Time.deltaTime;
            title.transform.localScale = Vector3.one * Mathf.Lerp(maxScale, minScale, time / duration);
            yield return new WaitForEndOfFrame();
        }
        title.transform.localScale = Vector3.one * minScale;
        StartCoroutine(ScaleTitleUp(scaleDuration));
    }

    IEnumerator ScaleTitleUp(float duration)
    {
        title.enabled = true;
        float time = 0;
        title.transform.localScale = Vector3.one * minScale;
        while (time < duration)
        {
            time += Time.deltaTime;
            title.transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, time / duration);
            yield return new WaitForEndOfFrame();
        }
        title.transform.localScale = Vector3.one * maxScale;
        StartCoroutine(ScaleTitleDown(scaleDuration));
    }

    IEnumerator FadeInTitle(float seconds, int steps = 60)
    {
        var c = title.color;
        var step = 1f / steps;
        var newA = 0f;
        for (int i = 0; i < steps; i++)
        {
            newA += step;
            title.color = new Color(c.r, c.g, c.b, newA);
            yield return new WaitForSeconds(seconds / steps);
        }
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME);
        PlayerPrefs.SetFloat(Constants.Prefs.PLAYER_STATUS, Constants.Prefs.Defaults.PLAYER_STATUS);
        PlayerPrefs.SetString(Constants.Prefs.LAST_ACTIVITIES, Constants.Prefs.Defaults.LAST_ACTIVITIES);
        PlayerPrefs.SetInt(Constants.Prefs.FORCE_SLEEPING, Constants.Prefs.Defaults.FORCE_SLEEPING);
        PlayerPrefs.SetInt(Constants.Prefs.CHANGING_FLOOR, Constants.Prefs.Defaults.CHANGING_FLOOR);
        PlayerPrefs.SetFloat(Constants.Prefs.VOLUME, volume.value);
        PlayerPrefs.SetInt(Constants.Prefs.USE_24_HOUR_CLOCK, use24HourClock.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString(Constants.Prefs.LAST_LEVEL, Constants.Prefs.Defaults.LAST_LEVEL));
    }
    
    public void StartGame()
    {
        ResetPlayerPrefs();
        SceneManager.LoadScene(Constants.Levels.LEVEL_1);
    }

    public void ShowMain()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        
        PlayerPrefs.Save();
    }

    public void ShowOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    public void Update24HourClock()
    {
        PlayerPrefs.SetInt(Constants.Prefs.USE_24_HOUR_CLOCK, use24HourClock.isOn ? 1 : 0);
    }

    public void UpdateVolume()
    {
        AudioListener.volume = volume.value;
        PlayerPrefs.SetFloat(Constants.Prefs.VOLUME, volume.value);
    }
}
