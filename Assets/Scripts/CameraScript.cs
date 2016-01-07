using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public int orthographicSize = 8;
	
	void Update()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        Camera.main.orthographicSize = orthographicSize;
	}
}
