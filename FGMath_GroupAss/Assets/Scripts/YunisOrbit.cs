using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class YunisOrbit : MonoBehaviour
{
    public class Planet
    {
        public GameObject m_body = null;
        public List<Planet> m_moons = new List<Planet>();
        public float m_speed = 0.0f;
        public float m_radius = 0.0f;
        public float m_angle = 0.0f;
    }

    GameObject m_sun = null;
    List<Planet> m_planets = new List<Planet>();

    int m_radius = 10;

    private void Awake()
    {
        m_sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_sun.transform.parent = transform;
        m_sun.name = "Sun";

        int t_numberOfPlanets = Random.Range(1, 10);

        CreatePlanets(t_numberOfPlanets);
    }

    private void Update()
    {
        //foreach (var t_planet in m_planets)
        //{
        //    MovePlanet(t_planet, m_sun.transform.position);
        //}
    }

    void MovePlanet(Planet planet, Vector3 center)
    {
        planet.m_angle += Time.deltaTime * planet.m_speed;
        planet.m_body.transform.position = getPositionInRadius(center, planet.m_radius, planet.m_angle);

        foreach (var t_moon in planet.m_moons)
        {
            MovePlanet(planet, planet.m_body.transform.position);
        } 
    }

    void CreatePlanets(int numPlanets)
    {
        for (int i = 0; i < numPlanets; ++i)
        {
            Planet t_planet = CreatePlanet(0.75f, Random.Range(0, 4), m_radius);
            m_planets.Add(t_planet);
        }
    }

    Planet CreatePlanet(float planetSize, int numMoons, int radius)
    {
        GameObject t_body = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        int t_randPosX = Random.Range(2, radius);
        float t_randAngle = Random.Range(0, 360);

        t_body.transform.position = new Vector3(t_randPosX, 0.0f, 0.0f);
        t_body.transform.position = getPositionInRadius(m_sun.transform.position, t_randPosX, t_randAngle);
        t_body.transform.localScale *= planetSize;

        Planet t_planet = new Planet();
        t_planet.m_body = t_body;
        t_planet.m_speed = Random.Range(1, 20);
        t_planet.m_radius = t_randPosX;
        t_planet.m_angle = t_randAngle;
        for (int i = 0; i < numMoons; i++)
        {
            t_planet.m_moons.Add(CreatePlanet(Random.Range(0.05f, planetSize / 0.5f), 0, 0));
        }
        return t_planet;
    }

    private Vector3 getPositionInRadius(Vector3 center, float radius, float angle)
    {
        float t_posX = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float t_posZ = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector3(t_posX, 0.0f, t_posZ);
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (var t_planet in m_planets)
    //    {
    //        Gizmos.DrawWireSphere(m_sun.transform.position, t_planet.m_radius);
    //    }
    //}
}
