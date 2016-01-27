using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    public float currentGameTime;
    private long maxGameTime;
    public int minutesPerSecond;
    [Range(1, 7)]
    public int totalGameDays;
    public bool use24hour;
    public enum DayOfWeek
    {
        Saturday,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    public Text dayText;
    public Text hourText;
    private bool isFinished;
    private bool paused;

    void Awake()
    {
        paused = false;
        if (PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME) == 0)
        {
            currentGameTime = ((int)DayOfWeek.Friday - totalGameDays + 1) * 24 * 60 * 60;
            currentGameTime += 8 * 60 * 60; // 8am @ starting day;
        }
        else {
            currentGameTime = PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME);
        }

        use24hour = (PlayerPrefs.GetInt(Constants.Prefs.USE_24_HOUR_CLOCK, Constants.Prefs.Defaults.USE_24_HOUR_CLOCK)  == 1);

        maxGameTime = (long)(DayOfWeek.Friday + 1) * 24 * 60 * 60 - 1; // 11:59pm @ Friday
    }

    void Update()
    {
        if (!paused)
        {
            currentGameTime += Time.deltaTime * minutesPerSecond * 60;
            currentGameTime = Mathf.Min(currentGameTime, maxGameTime);

            if (currentGameTime >= maxGameTime && !isFinished)
            {
                GameOver();
            }

            dayText.text = GetDayFromTimeStringified((long)currentGameTime);
            hourText.text = GetHourFromTimeStringified((long)currentGameTime);
        }
    }

    private string GetDayFromTimeStringified(long time)
    {
        int index = GetDays(time);
        return Enum.GetName(typeof(DayOfWeek), index);
    }

    private string GetHourFromTimeStringified(long time)
    {
        int hours = GetHours(time);
        int minutes = GetMinutes(time);
        
        bool am = false;
        if(!use24hour)
        {
            am = (hours < 12);
            hours = hours % 12;
            if(hours == 0)
            {
                hours = 12;
            }
        }

        string timeString = hours.ToString("D2") + ":" + minutes.ToString("D2");

        if(!use24hour)
        {
            timeString += " " + (am ? "am" : "pm");
        }

        return timeString;
    }
    
    public int GetHours(long time)
    {
        return Mathf.FloorToInt(time / (60 * 60)) % 24;
    }
    
    public int GetMinutes(long time)
    {
        return Mathf.FloorToInt(time / 60) % 60;
    }

    public int GetDays(long time)
    {
        return Mathf.FloorToInt(time / (24*60*60));
    }

    public void AddMinutes(int minutes)
    {
        currentGameTime += minutes * 60;
    }

    public void Pause()
    {
        paused = true;
    }

    public void Resume()
    {
        paused = false;
    }

    public int GetRemainingDays()
    {
        return totalGameDays - GetDays((long)currentGameTime);
    }

    public void GameOver()
    {
        isFinished = true;
        StartCoroutine(GameObject.FindObjectOfType<GameManager>().EndGame(false));
    }

    public int GetTotalMinutes(long time)
    {
        return (int) (time / 60);
    }

}
