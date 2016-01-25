using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour {

    private Text creditsText;
    private Text namesText;
    private GameObject content;
    private Text grade;
    public float shakeIntensity = 3;
    public float scrollDuration = 3;

    // Use this for initialization
    void Start () {
        content = GameObject.Find("Content");
        namesText = GameObject.Find("Names").GetComponent<Text>();
        creditsText = GetComponent<Text>();
        grade = GameObject.Find("Grade").GetComponent<Text>();

        float playerStatus = PlayerPrefs.GetFloat(Constants.Prefs.PLAYER_STATUS, 0);
        float playerGrade = Mathf.Abs(100 - (50 - playerStatus) * 2);

        grade.text = "[" + LetterGradeFromNumber(playerGrade) + "]";

        if(PlayerPrefs.GetInt(Constants.Prefs.THESIS_DELIVERED, Constants.Prefs.Defaults.THESIS_DELIVERED) == 1)
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

        StartCoroutine(Play());
	}

    IEnumerator Play()
    {
        yield return StartCoroutine(RollCredits(scrollDuration));
        yield return StartCoroutine(AddStamp(.2f));
        yield return StartCoroutine(ShakeScreen(0.2f, 20));
        yield return StartCoroutine(FadeInNames(2));
        yield return StartCoroutine(WaitAndLoad(5f));
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

    IEnumerator WaitAndLoad(float seconds, int steps = 60)
    {
        for(int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(seconds/steps);
        }

        // TODO: remove saved data
        SceneManager.LoadScene(Constants.Levels.START_MENU);
    }

    private string LetterGradeFromNumber(float numberGrade) 
    {
	    if(numberGrade >= 95) 
	    {
		    return "A+";
	    }
	    if(numberGrade >= 93) 
	    {
		    return "A";
	    }
	    if(numberGrade >= 90) 
	    {
		    return "A-";
	    }
	    if(numberGrade >= 87) 
	    {
		    return "B+";
	    }
	    if(numberGrade >= 83) 
	    {
		    return "B";
	    }
	    if(numberGrade >= 80) 
	    {
		    return "B-";
	    }
	    if(numberGrade >= 77) 
	    {
		    return "C+";
	    }
	    if(numberGrade >= 73) 
	    {
		    return "C";
	    }
	    if(numberGrade >= 70) 
	    {
		    return "C-";
	    }
	    if(numberGrade >= 67) 
	    {
		    return "D+";
	    }
	    if(numberGrade >= 63) 
	    {
		    return "D";
	    }
	    if(numberGrade >= 60) 
	    {
		    return "D-";
	    }

	    return "F";
    }
}
