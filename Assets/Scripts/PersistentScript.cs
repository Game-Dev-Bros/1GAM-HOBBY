using UnityEngine;
using System.Collections;

public class PersistentScript : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Awake () {
        if (GameObject.Find("PersistentDataObject") == null)
        {
            var o = Instantiate(prefab);
            o.name = "PersistentDataObject";
        }
	}

}
