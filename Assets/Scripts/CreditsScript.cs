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
    private bool isShakeRunning = false, namesFading = false;

    // Use this for initialization
    void Start () {
        content = GameObject.Find("Content");
        namesText = GameObject.Find("Names").GetComponent<Text>();
        creditsText = GetComponent<Text>();
        grade = GameObject.Find("Grade").GetComponent<Text>();

        //grade.text = ""; //bota nota
        creditsText.text = Constants.Credits.SUCCESS_GREETING;
        namesText.text = Constants.Credits.CREDITS_STRING;

        StartCoroutine(Play());
	}


    IEnumerator Play()
    {
        yield return StartCoroutine(RollCredits(3));
        yield return StartCoroutine(ShakeScreen(0.2f, 20));
        StartCoroutine(FadeInNames(2));
    }

    IEnumerator RollCredits(float duration, int steps = 60)
    {
        //var h = gameObject.GetComponent<RectTransform>();
        var step = (300 / steps)/duration;
        while (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);
            yield return null;
        }
    }

    IEnumerator ShakeScreen(float duration, int steps = 60)
    {
        isShakeRunning = true;
        grade.enabled = true;
        Vector3 initialPos = content.transform.position;
        for (int i = 0; i < steps; i++)
        {
            content.transform.position = initialPos + (Vector3)Random.insideUnitCircle * shakeIntensity;
            yield return new WaitForSeconds(duration / steps);
            //content.transform.position = initialPos;
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
        StartCoroutine(WaitAndLoad(3f));
    }

    IEnumerator WaitAndLoad(float seconds, int steps = 60)
    {
        for(int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(seconds/steps);
        }
        SceneManager.LoadScene(Constants.Levels.START_MENU);
    }


}
