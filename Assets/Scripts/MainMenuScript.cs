using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{


    public void StartGame()
    {
        SceneManager.LoadScene("Level 0");
    }

}
