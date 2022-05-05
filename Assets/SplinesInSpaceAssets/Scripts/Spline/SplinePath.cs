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

    List<LineRenderer> lineRenderers;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSpline();
    }

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
        if(lineRenderers == null)
        {
            lineRenderers = new List<LineRenderer>();
        }

        splineGenerator.UpdateSettings(splineSettings);
    }

    void GenerateLines()
    {
        int totalLines = (splineSettings.points.Count - 1);

        if(lineRenderers.Count < totalLines)
        {
            int linesToAdd  = splineSettings.points.Count - totalLines;

            for(int i = 0; i < linesToAdd; ++i)
            {
                GameObject lineObj = new GameObject("line");
                LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
                lineObj.transform.parent = transform;

                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderers.Add(lineRenderer);
            }
        }

        for(int i = 0; i < totalLines; ++i)
        {
            List<Vector3> points = new List<Vector3>();

            for(float t = 0; t <= 1; t += 0.01f)
            {
                points.Add(splineGenerator.GeneratePoint(i + t));
            }

            var lineRenderer = lineRenderers[i];

            lineRenderer.gameObject.SetActive(true);
            lineRenderer.positionCount = points.Count; 
            lineRenderer.SetPositions(points.ToArray());
        }

        for(int i = totalLines; i < lineRenderers.Count; ++i)
        {
            lineRenderers[i].gameObject.SetActive(false);
        }
    }

    public void GenerateSpline()
    {
        Initialize();
        splineGenerator.UpdateSpline();
        GenerateLines();
        // splineGenerator.UpdateLines(gameObject.GetComponent<LineRenderer>());
    }

    public void GenerateColour()
    {
        
    }
}
