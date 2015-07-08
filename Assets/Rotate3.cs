using UnityEngine;
using System.Collections;

public class Rotate3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        euler = transform.localEulerAngles;
	}

    Vector3 euler = Vector3.zero;

    public Vector3 factor = Vector3.one;

	
	// Update is called once per frame
	void Update () {
        float a = Time.deltaTime * 10f;
    //    transform.localEulerAngles = transform.localEulerAngles - new Vector3(Time.deltaTime * 10f, Time.deltaTime * 10f, Time.deltaTime * 10f);
        euler += new Vector3(a * factor.x, a * factor.y, a * factor.z);
        //euler += new Vector3(0f,0f,a);
        transform.localEulerAngles = euler;
	
	}
}
