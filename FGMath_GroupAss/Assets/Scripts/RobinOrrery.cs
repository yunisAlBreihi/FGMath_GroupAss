using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Planet
{
    public GameObject m_GameObject;
    public float m_Radius;
    public float m_Scale;
}


public class RobinOrrery : MonoBehaviour
{
    GameObject m_Sun = null;
    List<Planet> m_Planets = new List<Planet>();

    [SerializeField] int m_NumberOfPlanet = 10;
    [SerializeField] float m_PlanetSunOffset = 0.12f;
    [SerializeField] float m_SunScale = 0.19f;
    [SerializeField] float m_PlanetMinScale = 0.01f;
    [SerializeField] float m_PlanetMaxScale = 0.04f;
    [SerializeField] float m_PlanetMinRadiusShift = 0.09f;
    [SerializeField] float m_PlanetMaxRadiusShift = 0.05f;

    private void Awake()
    {
        GenerateOrrarySystem();
    }

    public void GenerateOrrarySystem()
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        else if (Application.isEditor)
        {
            foreach (Transform child in transform)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (child)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                };
            }
        }

        m_Sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_Sun.transform.parent = transform;
        m_Sun.transform.localScale = new Vector3(m_SunScale, m_SunScale, m_SunScale);
        m_Sun.name = "Sun";

        Planet lastPlanet = null;

        for (int i = 0; i < m_NumberOfPlanet; i++)
        {
            Planet planet = new Planet();
            GameObject planetObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Transform planetTransform = planetObject.transform;

            // Set planet gameobject
            planet.m_GameObject = planetObject;

            // Set planet name
            planetObject.name = $"Planet {i + 1}";
            planetTransform.parent = transform;

            // Get a random radius shift
            float randomShift = Random.Range(m_PlanetMinRadiusShift, m_PlanetMaxRadiusShift);

            // Set the radius of the planet based on the last planet
            planet.m_Radius = lastPlanet == null ? m_PlanetSunOffset + randomShift : lastPlanet.m_Radius + randomShift;

            // Randomize the planet scale
            planet.m_Scale = Random.Range(m_PlanetMinScale, m_PlanetMaxScale);

            float newPlanetX = planet.m_Radius;
            planetTransform.position = new Vector3(newPlanetX, planetTransform.position.y, planetTransform.position.z);

            // Set the scale
            planet.m_GameObject.transform.localScale = new Vector3(planet.m_Scale, planet.m_Scale, planet.m_Scale);

            // Update last planet
            lastPlanet = planet;
        }
    }

    IEnumerator DestroyAfterFrame(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

    private void OnValidate()
    {
        // GenerateOrrarySystem();
    }
}
