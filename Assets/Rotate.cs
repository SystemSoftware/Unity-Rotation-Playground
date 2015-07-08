using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public float factor = 1f;
	
	// Update is called once per frame
	void Update () {

        transform.localEulerAngles = transform.localEulerAngles + new Vector3(0f, 0f,Time.deltaTime * 10f * factor);
	}
}
