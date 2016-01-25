using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsScript : MonoBehaviour {

    private ClockManager clock;
    private Slider volSlider;
    private Toggle hourToggle;
    private MusicPlayer mplayer;

	// Use this for initialization
	void Awake () {
        clock = GameObject.Find("Clock").GetComponent<ClockManager>();
        hourToggle = GameObject.Find("Toggle").GetComponent<Toggle>();
        volSlider = GameObject.Find("Slider").GetComponent<Slider>();
        if(volSlider != null)
            volSlider.onValueChanged.AddListener(ChangeVolume);
        mplayer = GameObject.Find("PersistentDataObject").GetComponent<MusicPlayer>();
        LoadPrefs();
	}

    private void LoadPrefs()
    {
        clock.use24hour = PlayerPrefs.GetInt(Constants.Prefs.USE_24_HOUR_CLOCK, 1) == 1 ? true : false;
        hourToggle.isOn = clock.use24hour;
        volSlider.value = PlayerPrefs.GetFloat(Constants.Prefs.VOLUME, 1);
    }

    public void ToggleHourMode(bool value)
    {
        if (clock != null)
        {
            clock.use24hour = value;
            PlayerPrefs.SetInt(Constants.Prefs.USE_24_HOUR_CLOCK, value == true ? 1 : 0 );
        }
    }

    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(Constants.Prefs.VOLUME, value);
    }

    public void ReturnToMainMenu()
    {
        mplayer.StopMusic();
        Time.timeScale = 1;
        Destroy(GameObject.Find("PersistentDataObject"));
        SceneManager.LoadScene(Constants.Levels.START_MENU);
    }
}
