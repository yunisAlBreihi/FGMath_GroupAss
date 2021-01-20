using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Planet
{
    public GameObject m_GameObject;
    public float m_Radius;
    public float m_Scale;
}


public class RobinTest : MonoBehaviour
{
    GameObject m_Sun = null;
    List<Planet> m_Planets = new List<Planet>();
    int m_NumberOfPlanet = 10;

    private void Awake()
    {
        m_Sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_Sun.transform.parent = transform;
        m_Sun.transform.localScale = new Vector3(40.0f, 40.0f, 40.0f);
        m_Sun.name = "Sun";

        Planet lastPlanet = null;

        for (int i = 0; i < m_NumberOfPlanet; i++)
        {
            Planet planet = new Planet();
            planet.m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.m_GameObject.name = $"Planet {i + 1}";
            planet.m_GameObject.transform.parent = transform;
            planet.m_Radius = lastPlanet == null ? Random.Range(60, 120) : lastPlanet.m_Radius + Random.Range(60,120);
            planet.m_Scale = Random.Range(10, 25);
            planet.m_GameObject.transform.localScale = new Vector3(planet.m_Scale, planet.m_Scale, planet.m_Scale);

            float newPlanetX = planet.m_Radius;
            planet.m_GameObject.transform.position = new Vector3(newPlanetX, planet.m_GameObject.transform.position.y, planet.m_GameObject.transform.position.z);

            lastPlanet = planet;
        }
    }
}
