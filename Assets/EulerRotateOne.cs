using UnityEngine;
using System.Collections;

public class EulerRotateOne : MonoBehaviour {

	// Use this for initialization
	void Start () {
        state = transform.localEulerAngles;
	}

    Vector3 state;
    public int axis = 0;
    
	
	// Update is called once per frame
	void Update () {
        EulerState state = transform.GetComponentInParent<EulerState>();

        Vector3 set = this.state;
        set.z += state.angles[axis];
        transform.localEulerAngles = set;
	}

}
