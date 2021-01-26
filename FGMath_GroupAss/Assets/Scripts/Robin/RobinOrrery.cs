using System.Collections.Generic;
using UnityEngine;

public class RobinOrrery : MonoBehaviour
{
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
    [SerializeField] float m_ArmThickness = 0.03f;
    [SerializeField] float m_BaseArmLengthMultiplier = 0.8f;
    [SerializeField] float m_UpperArmLengthMultiplier = 0.3f;
    [SerializeField] bool m_DrawDebugIKLines = true;

    [Header("Planet settings")]
    [SerializeField] float m_PlanetMinScale = 0.01f;
    [SerializeField] float m_PlanetMaxScale = 0.04f;
    [SerializeField] float m_PlanetMinRadiusShift = 0.09f;
    [SerializeField] float m_PlanetMaxRadiusShift = 0.05f;
    [SerializeField] float m_PlanetMinSpeed = 10.0f;
    [SerializeField] float m_PlanetMaxSpeed = 20.0f;

    GameObject m_Sun = null;
    GameObject m_SunPedestal = null;

    List<RobinPlanet> m_Planets = new List<RobinPlanet>();
    List<RobinOrreryArm> m_OrreryArms = new List<RobinOrreryArm>();

    float inputX = 0.0f;
    float inputY = 0.0f;
    bool inputRotateArmRight = false;
    bool inputRotateArmLeft = false;


    #region Unity Methods
    private void Awake()
    {
        GenerateOrrarySystem();
    }

    private void Update()
    {
        UpdateInput();
        MoveOrrery();
        MovePlanets();
        MoveArms();
    }
    #endregion Unity Methods

    #region Input and Movement
    private void UpdateInput()
    {
        inputX = Input.GetAxis("Horizontal") * Time.deltaTime;
        inputY = Input.GetAxis("Vertical") * Time.deltaTime;
        inputRotateArmRight = Input.GetAxis("Fire1") > 0.0f;
        inputRotateArmLeft = Input.GetAxis("Fire2") > 0.0f;
    }

    private void MoveOrrery()
    {
        transform.localPosition = new Vector3(transform.localPosition.x + inputX, transform.localPosition.y, transform.localPosition.z + inputY);
    }

    private void MovePlanets()
    {
        foreach (RobinPlanet planet in m_Planets)
        {
            planet.MovePlanet(inputRotateArmRight ? m_FireRotationMultiplier : inputRotateArmLeft ? -m_FireRotationMultiplier : 0.0f);
        }
    }

    private void MoveArms()
    {
        foreach (RobinOrreryArm arm in m_OrreryArms)
        {
            arm.MoveArm(m_DrawDebugIKLines);
        }
    }
    #endregion Input and Movement

    #region Orrery Generation
    public void GenerateOrrarySystem()
    {
        DeleteCurrentOrreryParts();
        GenerateSun();
        GenerateSunPedestal();
        GenerateSunPedestalFoot();
        GeneratePlanets();
        GenerateNewOrreryArms();
    }

    private void DeleteCurrentOrreryParts()
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
    }

    private void GenerateSun()
    {
        m_Sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_Sun.transform.parent = transform;
        m_Sun.transform.localScale = new Vector3(m_SunScale, m_SunScale, m_SunScale);
        m_Sun.transform.localPosition = Vector3.zero;
        m_Sun.name = "Sun";
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

        // Delete capsule collider and replace with a box collider for the foot
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

        RobinPlanet lastPlanet = null;

        for (int i = 0; i < m_NumberOfPlanet; ++i)
        {
            RobinPlanet planet = new RobinPlanet();
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
            planetTransform.localPosition = new Vector3(newPlanetX, planetTransform.localPosition.y, planetTransform.localPosition.z);

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

        // Create only for first (for testing)
        // int i = 0;

        for (int i = 0; i < m_Planets.Count; ++i)
        {
            RobinPlanet planet = m_Planets[i];
            RobinOrreryArm arm = new RobinOrreryArm();
            float localY = (m_ArmStartPosition + step * i);

            // Set planet
            arm.m_Planet = planet;
            arm.orreryTransform = transform;

            // Create a joint
            arm.m_Joint = new GameObject(); // Create empty later, sphere for debugging.
            arm.m_Joint.name = $"Joint for arm {i + 1}";
            arm.m_Joint.transform.parent = armParent.transform;

            Vector3 orreryWorldPlanetX = transform.TransformPoint((planet.m_Radius * 0.8f), 0.0f, 0.0f);
            arm.m_Joint.transform.position = orreryWorldPlanetX;

            Vector3 jointPosition = arm.m_Joint.transform.localPosition;
            jointPosition.y = localY;
            arm.m_Joint.transform.localPosition = jointPosition;

            // Save arm start position
            arm.m_ArmStartPosition = new Vector3(0.0f, localY, 0.0f);

            // Create a start joint
            arm.m_StartJoint = new GameObject();
            arm.m_StartJoint.name = $"Start joint for arm {i + 1}";
            arm.m_StartJoint.transform.parent = armParent.transform;
            arm.m_StartJoint.transform.localPosition = arm.m_ArmStartPosition;

            // Set arm lengths
            arm.m_BaseArmLength = planet.m_Radius * m_BaseArmLengthMultiplier;
            arm.m_UpperArmLength = planet.m_Radius * m_UpperArmLengthMultiplier;

            // Create arms
            arm.m_BaseArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.m_BaseArm.transform.parent = armParent.transform;
            arm.m_BaseArm.name = $"Base arm for arm {i + 1}";
            Vector3 baseArmScale = arm.m_BaseArm.transform.localScale;
            baseArmScale.x = 0.15f;
            baseArmScale.y = 0.15f;
            baseArmScale.z *= arm.m_BaseArmLength;
            arm.m_BaseArm.transform.localScale = baseArmScale;

            arm.m_UpperArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            arm.m_UpperArm.transform.parent = armParent.transform;
            arm.m_UpperArm.name = $"Upper arm for arm {i + 1}";
            Vector3 upperArmScale = arm.m_UpperArm.transform.localScale;
            upperArmScale.x = 0.15f;
            upperArmScale.y = 0.15f;
            upperArmScale.z *= arm.m_UpperArmLength;
            arm.m_UpperArm.transform.localScale = upperArmScale;

            arm.UpdateArmPositions();

            // Add to arm list
            m_OrreryArms.Add(arm);
        }
    }
    #endregion Orrery Generation
}