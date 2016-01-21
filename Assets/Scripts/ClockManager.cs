using System;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    private float currentGameTime;
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

    void Awake()
    {
        currentGameTime = ((int)DayOfWeek.Friday - totalGameDays + 1) * 24 * 60 * 60;
        maxGameTime = (long)currentGameTime + totalGameDays * 24 * 60 * 60 - 1; // 11:59pm @ Friday
        currentGameTime += 8 * 60 * 60; // 8am @ starting day;
    }

    void Update()
    {
        if (!isFinished)
        {
            currentGameTime += Time.deltaTime * minutesPerSecond * 60;
        }

        if(currentGameTime > maxGameTime && !isFinished)
        {
            GameOver();
        }

        dayText.text = GetDayFromTime((long)currentGameTime);
        hourText.text = GetHourFromTime((long)currentGameTime);
    }

    private string GetDayFromTime(long time)
    {
        int index = Mathf.FloorToInt(time / (24*60*60));
        return Enum.GetName(typeof(DayOfWeek), index);
    }

    private string GetHourFromTime(long time)
    {
        int hours = Mathf.FloorToInt(time / (60 * 60)) % 24;
        int minutes = Mathf.FloorToInt(time / 60) % 60;
        
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

    void GameOver()
    {
        currentGameTime = maxGameTime;
        isFinished = true;
        Debug.Log("Game over!");
    }
}
