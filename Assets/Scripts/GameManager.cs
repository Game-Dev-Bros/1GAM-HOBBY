using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<Action> actions;

    private ClockManager clock;
    private PlayerController player;
    private SliderController status;

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();

        player = GameObject.FindObjectOfType<PlayerController>();
        status = GameObject.FindObjectOfType<SliderController>();
        clock = GameObject.FindObjectOfType<ClockManager>();

        SetupActions();
        
        if(actions.Count > 0)
        {
            StartCoroutine(UpdateInteractions());
        }
    }
    
    private void SetupActions()
    {
        actions = new List<Action>();

	    InteractableController[] interactables = GameObject.FindObjectsOfType<InteractableController>();
        foreach(InteractableController interactable in interactables)
        {
            foreach(Action action in interactable.actions)
            {
                actions.Add(action);
            }
        }
    }

    public Action GetActionWithTag(string tag)
    {
        foreach(Action action in actions)
        {
            if(action.tag == tag)
            {
                return action;
            }
        }

        return null;
    }

    private Action[] GetActionsWithTag(string tag)
    {
        List<Action> tempActions = new List<Action>();

        foreach(Action action in actions)
        {
            if(action.tag == tag)
            {
                tempActions.Add(action);
            }
        }

        return tempActions.ToArray();
    }

    IEnumerator UpdateInteractions()
    {
        int day = clock.GetDays((long) clock.currentGameTime);
        int hour = clock.GetHours((long) clock.currentGameTime);
        int minutes = clock.GetMinutes((long) clock.currentGameTime);

        Action action;
        action = GetActionWithTag(Constants.Actions.WRITE_THESIS);
        SetAction(action, !(day == (int) ClockManager.DayOfWeek.Friday && hour >= 18));
        
        action = GetActionWithTag(Constants.Actions.SUBMIT_THESIS);
        SetAction(action, (day == (int) ClockManager.DayOfWeek.Friday && hour >= 18));

        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(UpdateInteractions());
    }

    void SetAction(Action action, bool active)
    {
        if(action != null)
        {
            action.active = active;
        }
    }

    public IEnumerator EndGame(bool submitted)
    {
        ScreenFader fader = GameObject.FindObjectOfType<ScreenFader>();
        PlayerPrefs.SetInt(Constants.Prefs.THESIS_SUBMITTED, submitted ? 1 : 0);
        PlayerPrefs.SetFloat(Constants.Prefs.PLAYER_STATUS, status.value);

        yield return fader.FadeToColor(Constants.Colors.FADE, .5f);
        SceneManager.LoadScene(Constants.Levels.CREDITS);

        yield return null;
    }

    public bool CanExecuteAction(Action action)
    {
        float lastTime = PlayerPrefs.GetFloat(action.tag, 0);

        if(lastTime == 0)
        {
            if(action.tag != Constants.Actions.SLEEP_NIGHT && action.tag != Constants.Actions.TAKE_NAP)
            {
                return true;
            }
        }

        int lastTimeInMinutes = clock.GetTotalMinutes((long) lastTime);
        int currentTimeInMinutes = clock.GetTotalMinutes((long) clock.currentGameTime);

        int minutesDiff = currentTimeInMinutes - lastTimeInMinutes;

        int hour = clock.GetHours((long) clock.currentGameTime);

        switch(action.tag)
        {
            case Constants.Actions.EAT_MEAL:
                return minutesDiff > 4 * 60;
            case Constants.Actions.EAT_SNACK:
                return minutesDiff > 2 * 60;
            case Constants.Actions.LOSE_TIME:
                return minutesDiff > 60;
            case Constants.Actions.SLEEP_NIGHT:
                return hour >= 23 || hour < 3;
            case Constants.Actions.TAKE_NAP:
                return hour >= 13 && minutesDiff > 8 * 60;
            case Constants.Actions.USE_BATHROOM:
                return minutesDiff > 3 * 60;
            case Constants.Actions.WATCH_MOVIE:
                return minutesDiff > 1 * 60;
            case Constants.Actions.WRITE_THESIS:
                return minutesDiff > 1 * 60;
        }

        return true;
    }
}
