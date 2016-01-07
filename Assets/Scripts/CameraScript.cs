using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {


    private GameObject cam;
    public int OrthographicSize = 8;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.GetComponent<Camera>().orthographicSize = OrthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
	}
}
