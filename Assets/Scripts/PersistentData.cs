using UnityEngine;
using System.Collections;

public class PersistentData : MonoBehaviour
{

    public bool hasChangedFloors = false;


    // Use this for initialization
    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject); //juggernaut, bitch!
    }

}
