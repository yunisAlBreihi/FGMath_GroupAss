using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YunisIK : MonoBehaviour
{
    public GameObject m_targetLocation = null;
    public GameObject m_IKOrigin = null;

    float m_upperLength = 2.0f;
    float m_lowerLength = 2.0f;

    private GameObject m_endPoint = null;

    private GameObject m_linearArrowParent = null;
    private GameObject m_linearArrow = null;

    private GameObject m_verticalMarkerParent = null;
    private GameObject m_verticalMarker = null;

    private GameObject m_joint = null;

    private float ArrowLength = 0.0f;

    private void Awake()
    {
        m_targetLocation = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_targetLocation.transform.parent = transform;
        m_targetLocation.name = "Target_Location";

        m_IKOrigin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_IKOrigin.transform.parent = transform;
        m_IKOrigin.name = "IK_Origin";

        m_endPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_endPoint.transform.parent = transform;
        m_endPoint.name = "End_Point";

        m_linearArrowParent = new GameObject();
        m_linearArrowParent.transform.parent = transform;
        m_linearArrowParent.name = "Linear_Arrow_Parent";
        m_linearArrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_linearArrow.transform.parent = m_linearArrowParent.transform;
        m_linearArrow.transform.Rotate(Vector3.right, 90.0f);
        m_linearArrow.transform.Translate(new Vector3(0, -1.0f, 0.0f));
        m_linearArrow.name = "Linear_Arrow";

        m_verticalMarkerParent = new GameObject();
        m_verticalMarkerParent.transform.parent = transform;
        m_verticalMarkerParent.name = "Vertical_Marker_Parent";
        m_verticalMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_verticalMarker.transform.parent = m_verticalMarkerParent.transform;
        m_verticalMarker.transform.Rotate(Vector3.right, 90.0f);
        m_verticalMarker.transform.Translate(new Vector3(0, -1.0f, 0.0f));
        m_verticalMarker.name = "Vertical_Marker";

        m_joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_joint.transform.parent = transform;
        m_joint.name = "Joint";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FirstLookAtSecondPoint();
        FirstArmScale();

        TrigSolution t_trigSolution = createTrigSolution();

        m_verticalMarkerParent.transform.localPosition = m_IKOrigin.transform.position + t_trigSolution.m_verticalMarkerLocation;
        m_verticalMarkerParent.transform.localRotation = t_trigSolution.m_verticalMarkerRotation;

        m_joint.transform.localPosition = m_IKOrigin.transform.position + t_trigSolution.m_segmentJointPosition;

        ArrowLength = t_trigSolution.m_markerHeight;
        m_verticalMarkerParent.transform.localScale = new Vector3(1, 1, ArrowLength);
    }

    private void FirstLookAtSecondPoint()
    {
        m_endPoint.transform.localPosition = m_IKOrigin.transform.position + FindLinearEndpoint();
        m_linearArrowParent.transform.localPosition = m_IKOrigin.transform.position;
        m_linearArrowParent.transform.localRotation = Quaternion.LookRotation(m_IKOrigin.transform.position - IKOffsetSolve(), Vector3.forward);
    }

    private void FirstArmScale()
    {
        ArrowLength = FindLinearEndpoint().magnitude / 2.0f;
        m_linearArrowParent.transform.localScale = new Vector3(1, 1, ArrowLength);
    }


    public struct TrigSolution 
    {
        public Vector3 m_verticalMarkerLocation;
        public Quaternion m_verticalMarkerRotation;
        public float m_markerHeight;
        public Vector3 m_segmentJointPosition;
    }

    private TrigSolution createTrigSolution()
    {
        TrigSolution t_trigSolution = new TrigSolution();

        float t_lengthMiddlePoint = m_upperLength / (m_upperLength + m_lowerLength);
        float t_lengthOfAdjacent = FindLinearEndpoint().magnitude * t_lengthMiddlePoint;
        Vector3 t_verticalMarkerLocation = FindLinearEndpoint() * t_lengthOfAdjacent;
        t_trigSolution.m_verticalMarkerLocation = t_verticalMarkerLocation;

        float t_opposite = Mathf.Sqrt(Mathf.Pow(m_upperLength, 2) - Mathf.Pow(t_lengthOfAdjacent, 2));
        t_trigSolution.m_markerHeight = t_opposite;

        Vector3 t_rotationUpVector =  Quaternion.LookRotation(FindLinearEndpoint(), -Vector3.up) * Vector3.up;
        Quaternion t_verticalMarkerRotation = Quaternion.LookRotation(t_rotationUpVector);
        t_trigSolution.m_verticalMarkerRotation = t_verticalMarkerRotation;

        //Important!
        Vector3 t_segmentJointPosition = t_verticalMarkerLocation + (t_rotationUpVector * t_opposite);
        t_trigSolution.m_segmentJointPosition = t_segmentJointPosition;

        return t_trigSolution;
    }

    private Vector3 FindLinearEndpoint()
    {
        float t_IKOffsetLength = IKOffsetSolve().magnitude;
        float t_totalLength = m_upperLength + m_lowerLength;
        float t_IKOffsetPercent = Mathf.Clamp(t_IKOffsetLength / t_totalLength, 0.0f, 1.0f);
        float t_lengthPercent = t_totalLength * t_IKOffsetPercent;
        return IKOffsetSolveNormalized() * t_lengthPercent;
    }

    Vector3 IKOffsetSolveNormalized()
    {
        return IKOffsetSolve().normalized;
    }

    Vector3 IKOffsetSolve()
    {
        return m_targetLocation.transform.position - m_IKOrigin.transform.position;
    }
}
