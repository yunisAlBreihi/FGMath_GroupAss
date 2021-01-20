using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Planet
{
    public GameObject m_GameObject;
    public float m_Radius;
    public float m_Scale;
}

class OrreryArm
{
    public GameObject m_GameObject;
    public Planet m_Planet;
}

public class RobinOrrery : MonoBehaviour
{
    GameObject m_Sun = null;
    List<Planet> m_Planets = new List<Planet>();
    List<OrreryArm> m_OrreryArms = new List<OrreryArm>();

    GameObject m_SunPedestal = null;

    [Header("General settings")]
    [SerializeField] int m_NumberOfPlanet = 10;

    [Header("Sun settings")]
    [SerializeField] float m_SunScale = 0.19f;
    [SerializeField] float m_PlanetSunOffset = 0.12f;

    [Header("Sun pedestal settings")]
    [SerializeField] float m_PedestalToSunScale = 6.0f;

    [Header("Orrery Arm settings")]
    [SerializeField] float m_ArmStartPosition = 0.8f;
    [SerializeField] float m_ArmEndPosition = 2.5f;
    [SerializeField] float m_ArmToPlanetRadiusSizeScalar = 13.0f;
    [SerializeField] float m_ArmThickness = 0.03f;

    [Header("Planet settings")]
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
        // Delete current solar system
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
        m_Planets.Clear();

        m_Sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_Sun.transform.parent = transform;
        m_Sun.transform.localScale = new Vector3(m_SunScale, m_SunScale, m_SunScale);
        m_Sun.name = "Sun";

        GenerateSunPedistal();
        GeneratePlanets();
        GenerateOrreryArms();
    }

    private void GenerateSunPedistal()
    {
        m_SunPedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_SunPedestal.name = "Sun Pedestal";
        m_SunPedestal.transform.SetParent(transform);

        float pedistalYScale = (1.0f / m_NumberOfPlanet);
        float pedistalXZScale = m_SunScale / m_PedestalToSunScale;

        m_SunPedestal.transform.localScale = new Vector3(pedistalXZScale, pedistalYScale, pedistalXZScale);
        m_SunPedestal.transform.position = new Vector3(0.0f, -(m_SunScale * 0.98f), 0.0f);
    }

    private void GeneratePlanets()
    {

        Planet lastPlanet = null;

        for (int i = 0; i < m_NumberOfPlanet; ++i)
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

            // Push planet into list of planets
            m_Planets.Add(planet);

            // Update last planet
            lastPlanet = planet;
        }
    }

    private void GenerateOrreryArms()
    {
        // Get the amount of planets in frames between two numbers
        float step = (m_ArmStartPosition - m_ArmEndPosition) / (m_Planets.Count - 1);

        for (int i = 0; i < m_Planets.Count; ++i)
        {
            Planet planet = m_Planets[i];
            OrreryArm orreryArm = new OrreryArm();

            float localY = (m_ArmStartPosition + step * i);

            orreryArm.m_Planet = planet;
            orreryArm.m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            orreryArm.m_GameObject.transform.parent = m_SunPedestal.transform;

            orreryArm.m_GameObject.transform.localPosition = new Vector3(planet.m_Radius * m_ArmToPlanetRadiusSizeScalar, localY, 0.0f);
            orreryArm.m_GameObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            orreryArm.m_GameObject.transform.localScale = new Vector3(m_ArmThickness, 
                                                                      planet.m_Radius * m_ArmToPlanetRadiusSizeScalar, 
                                                                      m_ArmThickness);
        }
    }
}
