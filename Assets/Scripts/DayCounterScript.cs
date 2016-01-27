using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCounterScript : MonoBehaviour {

    private ScreenFader screenFader;
    private Text daysText;
    private Image fadeImage;
    private ClockManager clock;

	// Use this for initialization
	void Awake () {
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        daysText = GetComponent<Text>();
        screenFader = GameObject.Find(Constants.Game.FADER_OBJECT).GetComponent<ScreenFader>();
        clock = GameObject.Find("Clock").GetComponent<ClockManager>();
	}
	
    public void ShowRemainingDays()
    {
        int remainingDays = clock.GetRemainingDays();

        if(remainingDays > 0)
        {
            daysText.text = "Only " + remainingDays + " day";
            if(remainingDays != 1)
            {
                daysText.text += "s";
            }
            daysText.text += " remaining. ";
            daysText.text += "I can do this!";
        }
        else
        {
            daysText.text += "Last day! I can't forget to submit it by midnight.";
        }

        fadeImage.enabled = true;
        StopAllCoroutines();
        StartCoroutine(FadeAndShowDays());
    }

    IEnumerator FadeAndShowDays()
    {
        clock.Pause();
        //Set black?
        fadeImage.enabled = true;
        fadeImage.color = Color.black;
        //animate days
        yield return StartCoroutine(FadeInDays(1));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeOutDays(0.5f));
        StartCoroutine(screenFader.FadeToColor(Color.clear, 0.5f));
        clock.Resume();
    }


    IEnumerator FadeInDays(float seconds, int steps = 60)
    {
        var c = daysText.color;
        var step = 1f / steps;
        var newA = c.a;
        for (int i = 0; i < steps; i++)
        {
            newA += step;
            daysText.color = new Color(c.r, c.g, c.b, newA);
            yield return new WaitForSeconds(seconds / steps);
        }
    }

    IEnumerator FadeOutDays(float seconds, int steps = 60)
    {
        var c = daysText.color;
        var step = 1f / steps;
        var newA = c.a;
        for (int i = 0; i < steps; i++)
        {
            newA -= step;
            daysText.color = new Color(c.r, c.g, c.b, newA);
            yield return new WaitForSeconds(seconds / steps);
        }
    }

}
