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
        daysText.text = "Days remaining: " + clock.GetRemainingDays().ToString();
       
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
        yield return StartCoroutine(screenFader.FadeToColor(Color.clear, 0.5f));
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
