﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    private Text creditsText;
    private Text namesText;
    private GameObject content;
    private Text grade;
    private MusicPlayer mplayer;
    public float shakeIntensity = 3;
    public float scrollDuration = 3;
    private Image fadeImage;

    void Start ()
    {
        mplayer = GameObject.Find(Constants.Game.PERSISTENT_OBJECT).GetComponent<MusicPlayer>();
        content = GameObject.Find("Content");
        namesText = GameObject.Find("Names").GetComponent<Text>();
        creditsText = GetComponent<Text>();
        grade = GameObject.Find("Grade").GetComponent<Text>();
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        fadeImage.color = Color.clear;

        float playerStatus = PlayerPrefs.GetFloat(Constants.Prefs.PLAYER_STATUS, 0);
        float offset = Mathf.Abs(playerStatus - 50);
        float playerGrade = 100 - (offset * 2);

        grade.text = "[" + LetterGradeFromNumber(playerGrade) + "]";

        if(PlayerPrefs.GetInt(Constants.Prefs.THESIS_SUBMITTED, Constants.Prefs.Defaults.THESIS_DELIVERED) == 1)
        {
            creditsText.text = Constants.Credits.SUCCESS_GREETING;
        }
        else
        {
            creditsText.text = Constants.Credits.UNDELIVERED_GREETING;
        }

        if(grade.text[1].CompareTo('C') >= 0) // less than B-
        {
            creditsText.text += Constants.Credits.TIP_GREETING;
        }

        namesText.text = Constants.Credits.CREDITS_STRING;

        PlayerPrefs.SetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME);
        PlayerPrefs.Save();

        StartCoroutine(Play());
	}

    IEnumerator Play()
    {
        yield return StartCoroutine(RollCredits(scrollDuration));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(AddStamp(.2f));
        yield return StartCoroutine(ShakeScreen(0.2f, 20));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(FadeInNames(2));
        yield return StartCoroutine(FadeToMainMenu(3));
    }

    IEnumerator RollCredits(float speed, int steps = 60)
    {
        var step = (Screen.height / steps)/speed;
        while (transform.position.y < 650)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);
            yield return null;
        }
    }

    IEnumerator AddStamp(float duration)
    {
        float minScale = 0.8f;
        float maxScale = 10;

        grade.enabled = true;
        float time = 0;
        grade.transform.localScale = Vector3.one * maxScale;
        while(time < duration)
        {
            time += Time.deltaTime;
            grade.transform.localScale = Vector3.one * Mathf.Lerp(maxScale, minScale, time / duration);
            yield return new WaitForEndOfFrame();
        }

        grade.transform.localScale = Vector3.one * minScale;
    }

    IEnumerator ShakeScreen(float duration, int steps = 60)
    {
        grade.enabled = true;
        GetComponent<AudioSource>().Play();
        Vector3 initialPos = content.transform.position;
        for (int i = 0; i < steps; i++)
        {
            content.transform.position = initialPos + (Vector3)Random.insideUnitCircle * shakeIntensity;
            yield return new WaitForSeconds(duration / steps);
        }
        content.transform.position = initialPos;
    }


    IEnumerator FadeInNames(float seconds, int steps = 60)
    {
        var c = namesText.color;
        var step = 1f / steps;
        var newA = c.a;
        for (int i = 0; i < steps; i++)
        {
            newA += step;
            namesText.color = new Color(c.r,c.g,c.b,newA);
            yield return new WaitForSeconds(seconds/steps);
        }
    }

    IEnumerator FadeToMainMenu(float duration)
    {
        Color startingColor = fadeImage.color;
        float startingVolume = AudioListener.volume;
        float time = 0;

        float steps = 100;
        for (int i = 0; i < steps; i++)
        {
            time += duration / steps;
            fadeImage.color = Color.Lerp(startingColor, Constants.Colors.FADE, time/duration);
            AudioListener.volume = Mathf.Lerp(startingVolume, 0, time / duration);
            yield return new WaitForSeconds(duration/steps);
        }

        Destroy(GameObject.Find("PersistentDataObject"));
        SceneManager.LoadScene(Constants.Levels.START_MENU);
        yield return null;
    }

    private string LetterGradeFromNumber(float numberGrade) 
    {
	    if(numberGrade >= 98) 
	    {
		    return "A+";
	    }
	    if(numberGrade >= 95) 
	    {
		    return "A";
	    }
	    if(numberGrade >= 90) 
	    {
		    return "A-";
	    }
	    if(numberGrade >= 85) 
	    {
		    return "B+";
	    }
	    if(numberGrade >= 80) 
	    {
		    return "B";
	    }
	    if(numberGrade >= 75) 
	    {
		    return "B-";
	    }
	    if(numberGrade >= 68) 
	    {
		    return "C+";
	    }
	    if(numberGrade >= 59) 
	    {
		    return "C";
	    }
	    if(numberGrade >= 49) 
	    {
		    return "C-";
	    }
	    if(numberGrade >= 38) 
	    {
		    return "D+";
	    }
	    if(numberGrade >= 26) 
	    {
		    return "D";
	    }
	    if(numberGrade >= 12) 
	    {
		    return "D-";
	    }

	    return "F";
    }
}
