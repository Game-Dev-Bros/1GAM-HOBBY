using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu, optionsMenu;
    public Button continueButton;
    public Toggle use24HourClock;
    public Slider volume;

    void Awake()
    {
        bool hasSavedGame = (PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME) > 0);
        continueButton.interactable = hasSavedGame;

        ShowMain();
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.SetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME);
        PlayerPrefs.SetFloat(Constants.Prefs.PLAYER_STATUS, Constants.Prefs.Defaults.PLAYER_STATUS);
        PlayerPrefs.SetString(Constants.Prefs.LAST_ACTIVITIES, Constants.Prefs.Defaults.LAST_ACTIVITIES);
        PlayerPrefs.Save();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString(Constants.Prefs.LAST_LEVEL, Constants.Prefs.Defaults.LAST_LEVEL));
    }
    
    public void StartGame()
    {
        ResetPlayerPrefs();
        SceneManager.LoadScene(Constants.Levels.LEVEL_0);
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

        use24HourClock.isOn = PlayerPrefs.GetInt(Constants.Prefs.USE_24_HOUR_CLOCK, Constants.Prefs.Defaults.USE_24_HOUR_CLOCK) == 1;
        volume.value = PlayerPrefs.GetFloat(Constants.Prefs.VOLUME, Constants.Prefs.Defaults.VOLUME);
    }
    
    public void Update24HourClock()
    {
        PlayerPrefs.SetInt(Constants.Prefs.USE_24_HOUR_CLOCK, use24HourClock.isOn ? 1 : 0);
    }

    public void UpdateVolume()
    {
        // TODO: update audio source
        PlayerPrefs.SetFloat(Constants.Prefs.VOLUME, volume.value);
    }
}
