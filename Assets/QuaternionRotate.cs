using UnityEngine;
using System.Collections;

public class QuaternionRotate : MonoBehaviour {


    InterpolationState source;

	// Use this for initialization
	void Start () {
        source = GetComponentInParent<InterpolationState>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localRotation = source.quaternionState;
	}
}
