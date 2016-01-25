using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Action> actions;

    private ClockManager clock;
    private PlayerController player;

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();

        player = GameObject.FindObjectOfType<PlayerController>();
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

    private Action GetActionWithTag(string tag)
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
        action = GetActionWithTag(Constants.Actions.SLEEP_NIGHT);
        SetAction(action, (hour >= 23 && hour < 3));
        
        action = GetActionWithTag(Constants.Actions.TAKE_NAP);
        SetAction(action, (hour >= 14 && hour < 19));
        
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
}
