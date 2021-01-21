using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class YunisOrbit : MonoBehaviour
{
    public class Planet
    {
        public GameObject m_body = null;
        public List<Planet> m_moons = new List<Planet>();
        public Transform m_parent = null;
        public float m_speed = 0.0f;
        public float m_radius = 0.0f;
        public float m_angle = 0.0f;
    }

    GameObject m_sun = null;
    List<Planet> m_planets = new List<Planet>();

    int m_radius = 20;

    private void Awake()
    {
        m_sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_sun.transform.parent = transform;
        m_sun.name = "Sun";
        m_sun.transform.localScale = new Vector3(3, 3, 3);

        int t_numberOfPlanets = Random.Range(5, 10);

        CreatePlanets(t_numberOfPlanets);
    }

    private void Update()
    {
        foreach (var t_planet in m_planets)
        {
            MovePlanet(t_planet);
        }
    }

    void MovePlanet(Planet planet)
    {
        planet.m_angle += Time.deltaTime * planet.m_speed;
        planet.m_body.transform.position = getPositionInRadius(planet.m_parent.position, planet.m_radius, planet.m_angle);

        foreach (var t_moon in planet.m_moons)
        {
            MovePlanet(t_moon);
        }
    }

    void CreatePlanets(int numPlanets)
    {
        for (int i = 0; i < numPlanets; ++i)
        {
            Planet t_planet = CreatePlanet(m_sun.transform, 0.75f, m_sun.transform.position, Random.Range(1, 4), new Vector2(4, m_radius), "Planet");
            m_planets.Add(t_planet);
        }
    }

    Planet CreatePlanet(Transform parent, float planetSize, Vector3 center, int numMoons, Vector2 radiusRange, string name)
    {
        GameObject t_body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        t_body.name = name;
        t_body.transform.parent = parent;

        float t_randPosX = Random.Range(radiusRange.x, radiusRange.y);
        float t_randAngle = Random.Range(0, 360);

        t_body.transform.position = new Vector3(t_randPosX, 0.0f, 0.0f);
        t_body.transform.position = getPositionInRadius(center, t_randPosX, t_randAngle);
        t_body.transform.localScale *= planetSize;

        Planet t_planet = new Planet();
        t_planet.m_body = t_body;
        t_planet.m_parent = parent;
        t_planet.m_speed = Random.Range(1, 20);
        t_planet.m_radius = t_randPosX;
        t_planet.m_angle = t_randAngle;
        for (int i = 0; i < numMoons; i++)
        {
            t_planet.m_moons.Add(CreatePlanet(t_body.transform, Random.Range(0.05f, planetSize / 2.0f), t_body.transform.position, 0, new Vector2(1, 3), "Moon"));
        }
        return t_planet;
    }

    private Vector3 getPositionInRadius(Vector3 center, float radius, float angle)
    {
        float t_posX = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float t_posZ = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector3(t_posX, 0.0f, t_posZ);
    }
}
