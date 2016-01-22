using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("Time", 0f);
        PlayerPrefs.SetFloat("Pointer", 50f);
    }

    public void StartGame()
    {

        ResetPlayerPrefs();

        SceneManager.LoadScene("Level 0");
    }

}
