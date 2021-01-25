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
    public Transform orreryTransform;

    public GameObject m_BaseArm;
    public GameObject m_UpperArm;

    public float m_BaseArmLength;
    public float m_UpperArmLength;

    public Vector3 m_ArmStartPosition;
    public GameObject m_Joint;
    public GameObject m_StartJoint;

    public Planet m_Planet;

    public void MoveArm(bool drawDebugLines = false)
    {
        // Move from target (ie planet) to start | joint toward planet, start toward joint
        if (m_Joint == null)
        {
            Debug.Log("Joint is null");
        }
        if (m_StartJoint == null)
        {
            Debug.Log("Startjoint is null");
        }
        if (m_Planet == null)
        {
            Debug.Log("planet is null");
        }
        SolveIK(m_Joint.transform, m_Planet.m_GameObject.transform, m_UpperArmLength);
        SolveIK(m_StartJoint.transform, m_Joint.transform, m_BaseArmLength);

        // Set start back to it's original pos
        m_StartJoint.transform.localPosition = m_ArmStartPosition;

        // Move from start to end | joint toward start
        SolveIK(m_Joint.transform, m_StartJoint.transform, m_BaseArmLength);

        UpdateArmPositions();

        if (drawDebugLines)
        {

            // Calculate normalized vector between joint and planet
            Vector3 dir = m_Joint.transform.position - m_Planet.m_GameObject.transform.position;
            dir = dir.normalized;

            // Start to joint
            Debug.DrawLine(m_StartJoint.transform.position, m_Joint.transform.position, Color.green);
            // Joint to planet
            Debug.DrawLine(m_Joint.transform.position, m_Joint.transform.position - (dir * m_UpperArmLength), Color.red);
        }
    }

    public void UpdateArmPositions()
    {
        // Calculate normalized vector between joint and planet
        Vector3 dir = m_Joint.transform.position - m_Planet.m_GameObject.transform.position;
        dir = dir.normalized;

        SetPos(m_BaseArm.transform, m_StartJoint.transform.position, m_Joint.transform.position);
        SetPos(m_UpperArm.transform, m_Joint.transform.position, m_Joint.transform.position - (dir * m_UpperArmLength));
    }

    private void SolveIK(Transform start, Transform target, float armLength)
    {
        Vector3 dir = target.position - start.position;

        dir = dir.normalized;

        start.position = target.position - (dir * armLength);
    }

    void SetPos(Transform transform, Vector3 start, Vector3 end)
    {
        Vector3 middlePoint = (start + end) * 0.5f;

        transform.LookAt(end);
        transform.position = middlePoint;


        // transform.rotation = Quaternion.LookRotation(m_Joint.transform.localPosition - transform.localPosition, Vector3.up);
        Debug.DrawLine(start, (start + end) * 0.5f, Color.blue);
    }
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

        foreach (OrreryArm arm in m_OrreryArms)
        {
            arm.MoveArm(m_DrawDebugIKLines);
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
        GenerateNewOrreryArms();
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
            Planet planet = m_Planets[i];
            OrreryArm arm = new OrreryArm();
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
}
