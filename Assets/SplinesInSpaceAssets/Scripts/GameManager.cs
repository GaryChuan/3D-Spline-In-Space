using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<Planet> planets = new List<Planet>();
    
    [SerializeField] Planet basePlanet;
    [SerializeField] SplinePath path;

    [SerializeField] GameObject spaceship;

    float timer = 0;

    public void Start()
    {
        Debug.Log("Generated spline");
        path.GenerateSpline();
        planets.Add(basePlanet);

        for(int i = 1; i < path.Spline.Points.Count; ++i)
        {
            Planet newPlanet = GameObject.Instantiate(basePlanet);

            newPlanet.GeneratePlanet();
            planets.Add(newPlanet);
        }

        for(int i = 0; i < path.Spline.Points.Count; ++i)
        {
            planets[i].transform.position = path.Spline.Points[i];
        }

        spaceship.transform.position = path.Spline.GeneratePoint(0);
        // spaceship.transform.rotation = path.Spline.GenerateOrientation(0);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        //if(timer >= path.GetTime(path.Spline.Points.Count - 1))
        if(timer >= path.Spline.Points.Count - 1)
        {
            timer = 0;
        }

        // int index = path.GetIndex(timer);
        // float upperTime = path.GetTime(index + 1);
        // float lowerTime = path.GetTime(index);
        
        spaceship.transform.position = path.Spline.GeneratePoint(timer);
        spaceship.transform.rotation = path.Spline.GenerateOrientation(timer);
        // spaceship.transform.position = path.Spline.GeneratePoint(index, timer, upperTime, lowerTime);
        //spaceship.transform.rotation = path.Spline.GenerateOrientation(index, timer, upperTime, lowerTime);
    }
}
