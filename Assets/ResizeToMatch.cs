using UnityEngine;
using System.Collections;

public class ResizeToMatch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Camera>().orthographicSize = 4.3f * (float)Screen.height / Screen.width;
	}
}
