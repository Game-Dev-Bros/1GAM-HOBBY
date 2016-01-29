using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayCounterScript : MonoBehaviour {

    private ScreenFader screenFader;
    private Text daysText;
    private Image fadeImage;
    private ClockManager clock;
    public bool isRunning;

	void Awake ()
    {
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
            daysText.text = remainingDays + " day";
            if(remainingDays != 1)
            {
                daysText.text += "s";
            }
        }
        else
        {
            daysText.text = "Last day";
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
        isRunning = true;

        Color color = daysText.color;
        float step = 1f / steps;
        float alpha = color.a;

        for (int i = 0; i < steps; i++)
        {
            alpha += step;
            daysText.color = new Color(color.r, color.g, color.b, alpha);
            yield return new WaitForSeconds(seconds / steps);
        }
    }

    IEnumerator FadeOutDays(float seconds, int steps = 60)
    {
        Color color = daysText.color;
        float step = 1f / steps;
        float alpha = color.a;

        for (int i = 0; i < steps; i++)
        {
            alpha -= step;
            daysText.color = new Color(color.r, color.g, color.b, alpha);
            yield return new WaitForSeconds(seconds / steps);
        }

        isRunning = false;
    }
}
