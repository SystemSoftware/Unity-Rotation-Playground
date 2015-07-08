using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EulerState : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}


    public Vector3 angles = Vector3.zero;


    

    public enum Slider
    {
        Left,
        Right,
        None

    };


    public Slider slider = Slider.None;
    protected bool showRings = true;

    void OnGUI()
    {
        bool newRings = showRings;
        {
            int left = 25;
            int w = 200;
            int tw = 20;
            switch (slider)
            {
                case Slider.Left:
                    break;
                case Slider.Right:
                    left = Screen.width - 25 - w - tw;
                    break;
                case Slider.None:
                    return;
            }
            int h = 20;
            angles.x = GUI.HorizontalSlider(new Rect(left + tw, 25, w, h), angles.x, 0f, 360f);
            angles.y = GUI.HorizontalSlider(new Rect(left + tw, 25 + h, w, h), angles.y, 0f, 360f);
            angles.z = GUI.HorizontalSlider(new Rect(left + tw, 25 + 2 * h, w, h), angles.z, 0f, 360f);
            GUI.Label(new Rect(left, 20, tw, h), "x");
            GUI.Label(new Rect(left, 20 + h, tw, h), "y");
            GUI.Label(new Rect(left, 20 + h * 2, tw, h), "z");

            if (GUI.Button(new Rect(left, 25 + 3 * h, tw + w, h), "Random"))
            {
                angles.x = UnityEngine.Random.Range(0f, 360f);
                angles.y = UnityEngine.Random.Range(0f, 360f);
                angles.z = UnityEngine.Random.Range(0f, 360f);
            }

            newRings = GUI.Toggle(new Rect(left, 25 + 4 * h, tw + w, h), showRings, "Show Rings");
        }
        ShowRings(newRings);
    }

    protected void ShowRings(bool newRings)
    {
        if (newRings != showRings)
        {
            showRings = newRings;
            foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
                if (renderer.tag != "Persistent")
                    renderer.enabled = showRings;
        }
    }


	
	// Update is called once per frame
	void Update () {

        Vector3 change = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
            change.x++;
        if (Input.GetKey(KeyCode.UpArrow))
            change.y++;
        if (Input.GetKey(KeyCode.RightArrow))
            change.z++;

        angles += change * Time.deltaTime * 20f;

        //if (interpolate != Interpolation.Disabled)
        //{
        //    bool newRings = interpolateFrom.showRings || interpolateTo.showRings;
        //    if (newRings != showRings)
        //    {
        //        showRings = newRings;
        //        foreach (var renderer in transform.GetComponentsInChildren<MeshRenderer>())
        //            if (renderer.tag != "Persistent")
        //                renderer.enabled = showRings;

        //    }
        //}
        //angles.x = Mathf.Repeat(angles.x, 360f);
        //angles.y = Mathf.Repeat(angles.y, 360f);
        //angles.z = Mathf.Repeat(angles.z, 360f);
    }
}
