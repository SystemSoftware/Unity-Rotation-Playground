using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tracer : MonoBehaviour {

    GameObject globalTrace;


	// Use this for initialization
	void Start () {
        globalTrace = new GameObject("Global trace");
        filter = globalTrace.AddComponent<MeshFilter>();

        state = GetComponentInParent<InterpolationState>();
        lastVersion0 = state.interpolateFrom.version;
        lastVersion1 = state.interpolateTo.version;


        filter.mesh = new Mesh(); ;

        globalTrace.AddComponent<MeshRenderer>().material = material;


        last = InterpolationState.interpolationAt;
        lastAt = transform.position;


        if (colorMap == null)
        {
            colorMap = new Texture2D(256, 2);
            Color[] image = new Color[colorMap.width*2];
            for (int i = 0; i < colorMap.width; i++)
            {
                //image[i].g = (float)i / (colorMap.width - 1);
                Color c = new Color(0.2f,0.7f,1f);

                //c *= (-Mathf.Cos(Mathf.Repeat((float)i / 64f, 1f) * Mathf.PI)) * 0.25f + 0.75f;

                if ((i / 64) % 2 != 0)
                    c *= 0.75f;
                image[i] = c;


                Color gray = new Color(c.grayscale,c.grayscale,c.grayscale);
                image[i + colorMap.width] = (c - gray) * 0.15f + gray;
                image[i + colorMap.width].a = 1f;
            }
            colorMap.SetPixels(image);
            colorMap.wrapMode = TextureWrapMode.Clamp;
            colorMap.Apply();
            material.mainTexture = colorMap;
        }
	}


    int lastVersion0, lastVersion1, //version counters of interpolation extremes
        lastVersionUpdated = 0;

    MeshFilter filter;

    public Material material;

    InterpolationState state;

    const int resolution = 100;

    Vector3[] route = new Vector3[resolution];
    public int routeBegin = 0, routeEnd = 0;

    float last;
    Vector3 lastAt;


    int ToSlot(float at)
    {
        return Mathf.RoundToInt( ToSlotf(at));
    }
    float ToSlotf(float at)
    {
        return ( at * (resolution-1));
    }


    static Texture2D colorMap;


    bool collapsed = true,
            hasLast = false;

	// Update is called once per frame
	void LateUpdate () {

        int newState0 = state.interpolateFrom.version;
        int newState1 = state.interpolateTo.version;
        if ((lastVersion0 != newState0 || lastVersion1 != newState1) )
        {
            //if (Time.frameCount - lastVersionUpdated > 2)
            {
                lastVersion0 = newState0;
                lastVersion1 = newState1;
                lastVersionUpdated = Time.frameCount;


                collapsed = true;
                hasLast = false;
            }
        }


        if (!hasLast)
        {
            last = InterpolationState.interpolationAt;
            lastAt = transform.position;
            hasLast = true;
            return;
        }



        int at1 = ToSlot(InterpolationState.interpolationAt);
        int at0 = ToSlot( last  );

        if (at1 != at0)
        {
            float fat0 = ToSlotf(last);
            float fat1 = ToSlotf(InterpolationState.interpolationAt);
            Vector3 vat0 = lastAt;
            Vector3 vat1 = transform.position;
            if (at0 > at1)
            {
                int at = at0;
                at0 = at1;
                at1 = at;
                //at0++;
                //at1++;


                float fat = fat0;
                fat0 = fat1;
                fat1 = fat;
                Vector3 vat = vat0;
                vat0 = vat1;
                vat1 = vat;
            }



            for (int i = at0; i < at1; i++)
            {
                float edge = ((float)i+0.5f) / (float)(resolution - 1);
                float fi = (float)(i)-fat0 / (float)(fat1-fat0);
                Vector3 at = Vector3.Lerp(vat0, vat1, fi);

                route[i] = at;
                if (collapsed)
                {
                    routeBegin = i;
                    routeEnd = i + 1;
                    collapsed = false;
                }
                else
                {
                    routeEnd = Mathf.Max(routeEnd, i + 1);
                    routeBegin = Mathf.Min(routeBegin, i);
                }
            }







            if (routeEnd - routeBegin > 1)
            {
                const int radialResolution = 8;

                int routeLen = routeEnd - routeBegin;

                Vector3[] vertices = new Vector3[routeLen * radialResolution];
                int[] indices = new int[routeLen * 3 * 2 * radialResolution];
                Vector3[] path = route,
                        dir = new Vector3[routeLen],
                        normals = new Vector3[routeLen * radialResolution];
                Vector2[] uv = new Vector2[routeLen * radialResolution];
                for (int i = 1; i + 1 < routeLen; i++)
                    dir[i] = (path[routeBegin + i + 1] - path[routeBegin + i - 1]).normalized;
                dir[0] = (path[routeBegin + 1] - path[routeBegin]).normalized;
                dir[routeLen - 1] = (path[routeEnd - 1] - path[routeEnd - 2]).normalized;


                //Matrix4x4 sys;
                for (int i = 0; i < routeLen; i++)
                {
                    Vector3 y1 = Vector3.Cross(Vector3.up,dir[i]).normalized;
                    Vector3 x1 = Vector3.Cross(dir[i],y1).normalized;
                    Vector3 x = x1 * 0.03f;
                    Vector3 y = y1 * 0.03f;
                    Vector3 center = path[routeBegin + i];
                    float fog = 1f - (center.x - state.interpolateFrom.transform.position.x + 0.5f);
                    for (int j = 0; j < radialResolution; j++)
                    {
                        float a = (float)j / (float)radialResolution * Mathf.PI * 2f;
                        vertices[i*radialResolution+j] = center + x * Mathf.Cos(a) + y * Mathf.Sin(a);
                        normals[i*radialResolution+j] = x1 * Mathf.Cos(a) + y1 * Mathf.Sin(a);
                        uv[i * radialResolution + j] = new Vector2((float)(i+routeBegin) / (resolution - 1), fog);
                        if (i+1 < routeLen)
                        {
                            indices[i * radialResolution * 3 * 2 + 3 * 2 * j + 1] = i * radialResolution + j;
                            indices[i*radialResolution*3*2 + 3 * 2 * j] = i*radialResolution+(j+1)%radialResolution;
                            indices[i*radialResolution*3*2 + 3 * 2 * j+2] = (i+1)*radialResolution+(j+1)%radialResolution;

                            indices[i*radialResolution*3*2 + 3 * 2 * j+4] = i*radialResolution+j;
                            indices[i*radialResolution*3*2 + 3 * 2 * j+3] = (i+1)*radialResolution+(j+1)%radialResolution;
                            indices[i*radialResolution*3*2 + 3 * 2 * j+5] = (i+1)*radialResolution+(j);
                        }
                    }
                }

                filter.mesh.Clear();
	            filter.mesh.vertices = vertices;
	            filter.mesh.normals = normals;
                filter.mesh.uv = uv;
	            //filter.mesh.triangles = indices;
                filter.mesh.SetTriangles(indices, 0);
                filter.mesh.RecalculateBounds();
                //filter.mesh.

                globalTrace.GetComponent<MeshRenderer>().enabled = showTrace;
            }
        }
        last = InterpolationState.interpolationAt;
        lastAt = transform.position;




        //else
        //    if (tubes.Count > 0)
        //    {
        //        tubes[tubes.Count - 1].transform.LookAt(transform.position);
        //        float len = Vector3.Distance(tubes[tubes.Count - 1].transform.position, transform.position) * 0.5f + 0.05f;
        //        tubes[tubes.Count - 1].transform.localScale = new Vector3(0.1f, 0.1f, len);


        //    }

	}

    bool showTrace = true;

    internal bool TraceIsVisible()
    {
        return showTrace;;
    }

    internal void SetTraceVisible(bool enabled)
    {
        globalTrace.GetComponent<MeshRenderer>().enabled = showTrace = enabled;
    }
}
