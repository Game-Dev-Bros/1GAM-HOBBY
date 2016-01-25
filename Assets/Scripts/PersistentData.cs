using UnityEngine;
using System.Collections;

public class PersistentData : MonoBehaviour
{
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject); //juggernaut, bitch!
    }

    void OnLevelWasLoaded()
    {
        GetComponent<GameManager>().Reset();
    }
}
