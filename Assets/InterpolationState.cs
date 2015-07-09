using UnityEngine;
using System.Collections;
using System;

public class InterpolationState : EulerState {


    private int playDirection = 1;

	// Use this for initialization
	void Start () {
        neighbors = FindObjectsOfType<InterpolationState>();
        ShowRings(false);
	}


    float GetVScale()
    {
        return (float)(Screen.height-60) / 700f;
    }

	// Update is called once per frame
	void Update () {
        if (autoPlay)
        {
            autoAt = Mathf.Repeat(autoAt + Time.deltaTime * 0.5f, 2f * Mathf.PI);
            interpolationAt = Mathf.Sin(autoAt) * 0.5f + 0.5f;
            float r = Mathf.Abs(0.5f - interpolationAt) * 2f;
            //float speed = (1f - Mathf.Pow(Mathf.SmoothStep(0f, 1f, r),2f)) * 0.9f + 0.1f;
            //interpolationAt = interpolationAt + Time.deltaTime * 0.7f * playDirection * speed;
            //if (interpolationAt >= 1f)
            //{
            //    playDirection = -1;
            //    interpolationAt = 1f;
            //}
            //else
            //    if (interpolationAt <= 0f)
            //    {
            //        playDirection = 1;
            //        interpolationAt = 0f;
            //    }
        }
        transform.position = Vector3.Lerp(interpolateFrom.transform.position, interpolateTo.transform.position, interpolationAt * 0.5f + 0.25f);
        transform.position = transform.position + new Vector3(0f, GetVOffset() * GetVScale(), 0f);


        Vector3 state0, state1;

        switch (interpolate)
        {
            case Interpolation.Euler:
                state0 = interpolateFrom.angles;
                    state1 = interpolateTo.angles;
                Vector3 delta = state1 - state0;
                delta.x = Mathf.Repeat(delta.x + 180f, 360f) - 180f;
                delta.y = Mathf.Repeat(delta.y + 180f, 360f) - 180f;
                delta.z = Mathf.Repeat(delta.z + 180f, 360f) - 180f;

                angles = state0 + delta * interpolationAt;
                quaternionState = Quaternion.Euler(angles);
                break;
            case Interpolation.Momentum:
                {
                    state0 = interpolateFrom.angles;
                    state1 = interpolateTo.angles;
                    Quaternion q0 = Quaternion.Euler(state0);
                    Quaternion q1 = Quaternion.Euler(state1);
                    Vector3 a0,a1;
                    float angle0,angle1;
                    q0.ToAngleAxis(out angle0, out a0);
                    q1.ToAngleAxis(out angle1, out a1);
                    Vector3 m0 = a0 * angle0;
                    Vector3 m1 = a1 * angle1;
                    Vector3 d = m1 - m0;
                    //Debug.Log("a0=" + angle0 + ", a1=" + angle1);
                    Vector3 m11 = a1 * -(360f - angle1);
                    Vector3 m01 = a0 * -(360f - angle0);
                    if ((m11 - m0).sqrMagnitude < d.sqrMagnitude)
                        d = m11 - m0;
                    if ((m1 - m01).sqrMagnitude < d.sqrMagnitude)
                    {
                        d = m1 - m01;
                        m0 = m01;
                    }
                    if ((m11 - m0).sqrMagnitude < d.sqrMagnitude)
                        d = m11 - m0;
                    //Debug.Log("mag=" + d.magnitude);
                    Vector3 m = m0 + d * interpolationAt;
                    //Vector3 m = Vector3.Lerp(a0, a1, interpolationAt);
                    float angle = m.magnitude;
                    Vector3 axis = angle > 0f ? m / angle : Vector3.left;
                    Quaternion q = Quaternion.AngleAxis(angle, axis);
                    angles = q.eulerAngles;
                    quaternionState = q;
                }
                break;
            case Interpolation.QuaternionLerp:
                {
                    state0 = interpolateFrom.angles;
                    state1 = interpolateTo.angles;
                    Quaternion q0 = Quaternion.Euler(state0);
                    Quaternion q1 = Quaternion.Euler(state1);
                    Quaternion q = Quaternion.Lerp(q0, q1, interpolationAt);

                    angles = q.eulerAngles;
                    quaternionState = q;
                }
                break;
            case Interpolation.QuaternionSLerp:
                {
                    state0 = interpolateFrom.angles;
                    state1 = interpolateTo.angles;
                    Quaternion q0 = Quaternion.Euler(state0);
                    Quaternion q1 = Quaternion.Euler(state1);
                    Quaternion q = Quaternion.Slerp(q0, q1, interpolationAt);

                    angles = q.eulerAngles;
                    quaternionState = q;
                }
                break;
        }

	}

    bool autoPlay = false;

    InterpolationState[] neighbors;

    public enum Interpolation
    {
        Euler,
        Momentum,
        QuaternionLerp,
        QuaternionSLerp
    };

    public Interpolation interpolate = Interpolation.Euler;
    public EulerState interpolateFrom,
                        interpolateTo;
    static float interpolationAt = 0f, autoAt = 0f;


    public Quaternion quaternionState = Quaternion.identity;

    void OnGUI()
    {
        Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Camera.main.transform.right * 0.5f);


        int labelX = (int)screen.x;
        int labelY = Screen.height - (int)screen.y;
        GUI.Label(new Rect(labelX, labelY, 150, 20), interpolate.ToString());


        if (interpolate == Interpolation.Euler)
        {
            float old = interpolationAt;
            interpolationAt = GUI.HorizontalSlider(new Rect(85, Screen.height - 50, Screen.width - 110, 20), interpolationAt, 0f, 1f);
            if (interpolationAt != old)
                autoPlay = false;
            if (GUI.Button(new Rect(25, Screen.height - 50, 50, 20), "Play"))
            {
                autoPlay = !autoPlay;
                if (autoPlay)
                {
                    autoAt = Mathf.Asin(interpolationAt * 2f - 1f);
                }
            }
        }

        int w = 150;
        int x = Screen.width / 2 + Mathf.FloorToInt(GetVOffset()) * w;

        bool newRings = GUI.Toggle(new Rect(x, Screen.height - 70, w, 20), showRings, interpolate.ToString() + ".Rings");

        //if (newRings)
        //{
        //    foreach (var n in neighbors)
        //        if (n != this)
        //            n.ShowRings(false);
        //}
        ShowRings(newRings);

    }

    float GetVOffset()
    {
        return (float)interpolate - Enum.GetNames(typeof(Interpolation)).Length / 2 + 0.5f;
    }



}
