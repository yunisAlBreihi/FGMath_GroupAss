using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Planet
{
    public GameObject m_GameObject;
    public float m_Radius;
    public float m_Scale;
    public float m_Angle;
    public float m_Speed = 3.0f;

    public void MovePlanet(float rotationMultiplier = 1.0f)
    {
        m_Angle += Time.deltaTime * m_Speed * rotationMultiplier;
        m_GameObject.transform.localPosition = GetPositionInRadius(m_Radius, m_Angle);
    }

    private Vector3 GetPositionInRadius(float radius, float angle)
    {
        float posX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float posZ = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        return new Vector3(posX, m_GameObject.transform.localPosition.y, posZ);
    }
}

class OrreryArm
{
    public GameObject m_GameObject;
    public GameObject m_ChildArm;
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
    [SerializeField] float m_FireRotationMultiplier = 15.0f;

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
    [SerializeField] float m_PlanetMinSpeed = 10.0f;
    [SerializeField] float m_PlanetMaxSpeed = 20.0f;

    private void Awake()
    {
        GenerateOrrarySystem();
    }

    private void Update()
    {
        float newX = Input.GetAxis("Horizontal") * Time.deltaTime;
        float newY = Input.GetAxis("Vertical") * Time.deltaTime;
        bool fire1 = Input.GetAxis("Fire1") > 0.0f;
        bool fire2 = Input.GetAxis("Fire2") > 0.0f;

        transform.localPosition = new Vector3(transform.localPosition.x + newX, transform.localPosition.y, transform.localPosition.z + newY);

        foreach (Planet planet in m_Planets)
        {
            planet.MovePlanet(fire1 ? m_FireRotationMultiplier :  fire2 ? -m_FireRotationMultiplier : 0.0f);
        }
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
        m_Sun.transform.localPosition = Vector3.zero;
        m_Sun.name = "Sun";

        GenerateSunPedestal();
        GenerateSunPedestalFoot();
        GeneratePlanets();
    }

    private void GenerateSunPedestal()
    {
        m_SunPedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_SunPedestal.name = "Sun Pedestal";
        m_SunPedestal.transform.SetParent(transform);

        float pedistalYScale = (1.0f / m_NumberOfPlanet);
        float pedistalXZScale = m_SunScale / m_PedestalToSunScale;

        m_SunPedestal.transform.localScale = new Vector3(pedistalXZScale, pedistalYScale, pedistalXZScale);
        m_SunPedestal.transform.localPosition = new Vector3(0.0f, -(m_SunScale * 0.98f), 0.0f);
    }

    private void GenerateSunPedestalFoot()
    {
        GameObject pedestalFoot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestalFoot.name = "SunPedestal Foot";
        pedestalFoot.transform.SetParent(transform);
        pedestalFoot.transform.localPosition = Vector3.zero;
        pedestalFoot.transform.localScale = new Vector3(m_SunScale * 1.5f, 0.01f, m_SunScale * 1.5f);

        pedestalFoot.transform.localPosition = new Vector3(0.0f, -m_SunScale - m_SunPedestal.transform.localScale.y, 0.0f);

        CapsuleCollider capsuleCollider = pedestalFoot.GetComponent<CapsuleCollider>();


        if (capsuleCollider != null)
        {
            if (Application.isPlaying)
            {
                Destroy(capsuleCollider);
            }
            else if (capsuleCollider)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(capsuleCollider);
                };
            }
        }

        pedestalFoot.AddComponent<BoxCollider>();
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
            planetTransform.localPosition = Vector3.zero;

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

            // Set planet speed
            planet.m_Speed = Random.Range(m_PlanetMinSpeed, m_PlanetMaxSpeed);
            planet.m_Angle = Random.Range(0.0f, 360.0f);

            // Push planet into list of planets
            m_Planets.Add(planet);

            // Update last planet
            lastPlanet = planet;
        }
    }
    private void GenerateNewOrreryArms()
    {
        m_OrreryArms.Clear();
        // Get the amount of planets in frames between two numbers
        float step = (m_ArmStartPosition - m_ArmEndPosition) / (m_Planets.Count - 1);

        // Since unity sets the rotation relative to the parents scale we need to add
        // an empty parent with a scale of 1
        GameObject armParent = new GameObject("ArmParent");
        armParent.transform.parent = transform;
        armParent.transform.localScale = new Vector3(m_ArmThickness, m_ArmThickness, m_ArmThickness);
        armParent.transform.localPosition = m_SunPedestal.transform.localPosition;

        for (int i = 0; i < m_Planets.Count; ++i)
        {
            Planet planet = m_Planets[i];
            OrreryArm orreryArm = new OrreryArm();

            float localY = (m_ArmStartPosition + step * i);

            orreryArm.m_Planet = planet;
            orreryArm.m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            orreryArm.m_GameObject.name = $"Main arm for planet {i + 1}";
            float x = Vector3.Distance(m_Sun.transform.position, planet.m_GameObject.transform.position);
            Vector3 newScale = new Vector3(x,
                                           1f,
                                           1f);

            orreryArm.m_GameObject.transform.localScale = newScale;
            orreryArm.m_GameObject.transform.parent = armParent.transform;
            orreryArm.m_GameObject.transform.localScale = new Vector3(
                    orreryArm.m_GameObject.transform.localScale.x,
                    0.3f,
                    0.3f
                );
            orreryArm.m_GameObject.transform.localPosition = new Vector3(orreryArm.m_GameObject.transform.localScale.x * 0.5f, localY, 0.0f);

            float y = Vector3.Distance(m_Sun.transform.position, m_SunPedestal.transform.TransformPoint(0.0f, localY, 0.0f));

            orreryArm.m_ChildArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            orreryArm.m_ChildArm.name = $"Child arm for planet {i + 1}";
            orreryArm.m_ChildArm.transform.localScale = new Vector3(1.0f, y, 1.0f);
            orreryArm.m_ChildArm.transform.parent = armParent.transform;
            orreryArm.m_ChildArm.transform.localScale = new Vector3(
                    0.3f,
                    orreryArm.m_ChildArm.transform.localScale.y,
                    0.3f
                );
            orreryArm.m_GameObject.transform.localPosition = new Vector3(orreryArm.m_GameObject.transform.localScale.x * 0.5f, localY, 0.0f);
        }
    }
     
    private void GenerateCubeOrreryArms()
    {
        // Get the amount of planets in frames between two numbers
        float step = (m_ArmStartPosition - m_ArmEndPosition) / (m_Planets.Count - 1);

        // Since unity sets the rotation relative to the parents scale we need to add
        // an empty parent with a scale of 1
        GameObject armParent = new GameObject("ArmParent");
        armParent.transform.parent = transform;
        armParent.transform.localScale = new Vector3(m_ArmThickness, m_ArmThickness, m_ArmThickness);
        armParent.transform.localPosition = m_SunPedestal.transform.localPosition;

        for (int i = 0; i < m_Planets.Count; ++i)
        {
            Planet planet = m_Planets[i];
            OrreryArm orreryArm = new OrreryArm();

            float localY = (m_ArmStartPosition + step * i);

            orreryArm.m_Planet = planet;
            orreryArm.m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            orreryArm.m_GameObject.transform.parent = armParent.transform;
            orreryArm.m_GameObject.name = $"Main arm for planet {i + 1}";
            Vector3 newScale = new Vector3(planet.m_Radius * m_ArmToPlanetRadiusSizeScalar,
                                           0.3f,
                                           0.3f);
            orreryArm.m_GameObject.transform.localScale = newScale;
            orreryArm.m_GameObject.transform.localPosition = new Vector3(newScale.x * 0.5f, localY, 0.0f);

            Vector3 localStartPos = orreryArm.m_GameObject.transform.localPosition;
            Vector3 armStartPos = orreryArm.m_GameObject.transform.TransformPoint(localStartPos);
            Vector3 IKOffset = planet.m_GameObject.transform.position - armStartPos;
            Debug.DrawLine(armStartPos, IKOffset, Color.red, 4.0f);

            orreryArm.m_ChildArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            orreryArm.m_ChildArm.transform.parent = armParent.transform;
            orreryArm.m_ChildArm.name = $"Child arm for planet {i + 1}";
            orreryArm.m_ChildArm.transform.localScale = new Vector3(5.0f, 0.3f, 0.3f);
            orreryArm.m_ChildArm.transform.localPosition = new Vector3(newScale.x + 1.0f, localY, 0.0f);
        }
    }

    private void GenerateCapsuleOrreryArms()
    {
        m_OrreryArms.Clear();
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
            Vector3 newScale = new Vector3(planet.m_Radius * m_ArmToPlanetRadiusSizeScalar,
                                           m_ArmThickness,
                                           m_ArmThickness);
            orreryArm.m_GameObject.transform.localScale = newScale;

            orreryArm.m_GameObject.transform.localPosition = new Vector3(newScale.x *0.5f, localY, 0.0f);

            m_OrreryArms.Add(orreryArm);

            orreryArm.m_ChildArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            orreryArm.m_ChildArm.transform.parent = orreryArm.m_GameObject.transform;
            orreryArm.m_ChildArm.transform.localPosition = new Vector3(0.5f, planet.m_Radius * m_ArmToPlanetRadiusSizeScalar, 0.0f);
            orreryArm.m_ChildArm.transform.localScale = Vector3.one;
        }
    }
}
