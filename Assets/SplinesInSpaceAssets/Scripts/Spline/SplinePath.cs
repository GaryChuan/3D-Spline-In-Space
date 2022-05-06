using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePath : MonoBehaviour
{
    public bool autoUpdate = true;

    [HideInInspector]
    public bool splineSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    SplineGenerator splineGenerator = new SplineGenerator();

    public SplineSettings splineSettings;
    public SplineColourSettings colourSettings;

    LineRenderer lineRenderer;

    Spline spline;

    public List<float> times;

    public Spline Spline { get { return spline; } }

    public void OnSplineSettingsUpdated()
    {
        if(!autoUpdate)
        {
            return;
        }

        GenerateSpline();
    }

    public void OnColourSettingsUpdated()
    {
        if(!autoUpdate)
        {
            return;
        }

        GenerateColour();
    }

    void Initialize()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        if(lineRenderer == null )
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        splineGenerator.UpdateSettings(splineSettings);
    }

    public int GetIndex(float t)
    {
        int counter = 0;

        foreach(float time in times)
        {
            if(t >= time)
            {
                ++counter;
            }
        }

        return counter - 1;
    }

    public float GetTime(int index)
    {
        return times[index];
    }

    void GenerateLines()
    {
        if(spline == null)
        {
            return;
        }

        List<Vector3> points = new List<Vector3>(spline.Points.Count * 100);

        for(float t = 0; t < (spline.Points.Count - 1); t += 0.01f)
        {
            points.Add(spline.GeneratePoint(t));
        }

        points.Add(spline.Points[0]);

        // arclength
        List<float> arclengths = new List<float>(spline.Points.Count);
        
        Vector3 previousPt = new Vector3();

        // 0 ~ 6
        for(int i = 0; i < spline.Points.Count - 1; ++i)
        {
            arclengths.Add(0);
            previousPt = spline.GeneratePoint(i);

            for(float t = i; t < i + 1; t += 0.005f)
            {
                Vector3 newPt = spline.GeneratePoint(t);
                arclengths[i] += Vector3.Distance(previousPt, newPt);
                previousPt = newPt;
            }

            arclengths[i] += Vector3.Distance(spline.Points[(i + 1) % spline.Points.Count], previousPt);
        }
        
        // v = s / t;
        // t = s / v - time between each point p(0) = 0, p(1) = s[0] / v
        
        times = new List<float>();
        times.Add(0);

        float previousTime = 0;
        for(int i = 0; i < arclengths.Count; ++i)
        {
            float interval = arclengths[i] / Vector3.Magnitude(splineSettings.initialVelocity);
            times.Add(previousTime + interval);
            previousTime = previousTime + interval;    
        }

        lineRenderer.startWidth = colourSettings.lineThickness;
        lineRenderer.endWidth = colourSettings.lineThickness;
        lineRenderer.gameObject.SetActive(true);
        lineRenderer.positionCount = points.Count; 
        lineRenderer.SetPositions(points.ToArray());

        // if(spline == null)
        // {
        //     return;
        // }

        // int totalLines = spline.Points.Count - 1;
        // int linesToAdd  = totalLines - lineRenderer;
        
        // for(int i = 0; i < linesToAdd; ++i)
        // {
        //     GameObject lineObj = new GameObject("line");
        //     LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        //     lineObj.transform.parent = transform;
        // }

        // lineRenderers = transform.GetComponentsInChildren<LineRenderer>();

        // for(int i = 0; i < totalLines; ++i)
        // {
        //     List<Vector3> points = new List<Vector3>();

        //     for(float t = 0; t <= 1; t += 0.01f)
        //     {
        //         points.Add(spline.GeneratePoint(i + t));
        //     }

        //     LineRenderer lineRenderer = lineRenderers[i];

        //     lineRenderer.startWidth = colourSettings.lineThickness;
        //     lineRenderer.endWidth = colourSettings.lineThickness;
        //     lineRenderer.gameObject.SetActive(true);
        //     lineRenderer.positionCount = points.Count; 
        //     lineRenderer.SetPositions(points.ToArray());
        // }

        // for(int i = totalLines; i < lineRenderers.Length; ++i)
        // {
        //     lineRenderers[i].gameObject.SetActive(false);
        // }
    }

    public void GenerateSpline()
    {
        Initialize();
        spline = splineGenerator.GenerateSpline();
        GenerateLines();
    }

    public void GenerateColour()
    {
        Initialize();

        lineRenderer.startWidth = colourSettings.lineThickness;
        lineRenderer.endWidth = colourSettings.lineThickness;
        lineRenderer.material = colourSettings.material;
        // for(int i = 0; i < splineSettings.points.Count - 1; ++i)
        // {
        //     LineRenderer lineRenderer = lineRenderers[i];
        //     lineRenderer.startWidth = colourSettings.lineThickness;
        //     lineRenderer.endWidth = colourSettings.lineThickness;
        //     lineRenderer.material = colourSettings.material;
        // }
    }
}
