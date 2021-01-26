using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreation : MonoBehaviour
{
    public class Planet
    {
        public GameObject m_body;
        public List<Planet> m_moons = new List<Planet>();
    }

    int m_numPlanets = 4;
    int m_numMoons = 3;

    List<Planet> m_planets = new List<Planet>();


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_numPlanets; i++)
        {
            m_planets.Add(CreatePlanet(transform, 1, (i + 1) * 2));

            float t_planetRadius = m_planets[i].m_body.transform.localScale.x * 0.5f;
            float t_offsetFromPlanet = t_planetRadius;
            float t_moonRadius = 0.25f;
            float t_separatorSize = 0.1f;

            float t_distanceToPrevious = 1;
            if (i > 0)
            {
                t_distanceToPrevious = Vector3.Distance(m_planets[i - 1].m_body.transform.position, m_planets[i].m_body.transform.position);
            }

            for (int j = 0; j < m_numMoons; j++)
            {
                float t_posXOffset = (j + 1) * t_moonRadius;

                t_offsetFromPlanet += t_moonRadius + t_separatorSize;

                m_planets[i].m_moons.Add(CreatePlanet(m_planets[i].m_body.transform, t_moonRadius, t_offsetFromPlanet));
            }
        }
    }

    Planet CreatePlanet(Transform parent, float scale, float posX)
    {
        Planet t_planet = new Planet();

        t_planet.m_body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        t_planet.m_body.transform.parent = parent;
        t_planet.m_body.transform.localScale = Vector3.one * scale;

        t_planet.m_body.transform.localPosition = new Vector3(posX, 0, 0);

        return t_planet;
    }
}
