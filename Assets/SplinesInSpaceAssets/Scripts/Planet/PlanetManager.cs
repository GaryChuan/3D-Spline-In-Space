using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    [SerializeField]
    Planet planet;

    // Start is called before the first frame update
    void Awake()
    {
        planet.GeneratePlanet();

        for(int i = 1; i < 10; ++i)
        {
            Planet newPlanet = GameObject.Instantiate(planet);
            newPlanet.transform.position += new Vector3(i * 5.0f, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
