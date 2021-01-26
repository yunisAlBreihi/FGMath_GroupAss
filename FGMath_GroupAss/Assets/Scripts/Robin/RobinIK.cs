using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinIK : MonoBehaviour
{
    [SerializeField] private float m_ArmLength = 3.0f;
    [SerializeField] private int m_NumberOfJoints = 3;
    [SerializeField] private Transform m_TargetTransform;

    private GameObject m_IKStart;
    private GameObject m_TargetLocation;
    private List<GameObject> m_Joints = new List<GameObject>();

    private Vector3 m_BasePosition;

    void Awake()
    {
        SetupIK();
        m_BasePosition = m_IKStart.transform.position;
        
        if (m_TargetTransform != null)
        {
            m_TargetLocation.transform.position = (m_TargetTransform.position + Vector3.up);
        }
    }

    void SetupIK()
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
        m_Joints.Clear();

        m_IKStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_IKStart.transform.parent = transform;
        m_IKStart.transform.localPosition = Vector3.zero;
        m_IKStart.name = "IK Start Position";
        m_IKStart.transform.localScale = Vector3.one;

        m_TargetLocation = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_TargetLocation.transform.parent = transform;
        m_TargetLocation.transform.localPosition = Vector3.zero;
        m_TargetLocation.name = "IK Target Location";
        m_TargetLocation.transform.localScale = Vector3.one;

        for (int i = 0; i < m_NumberOfJoints; i++)
        {
            GameObject joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            joint.transform.parent = transform;
            joint.transform.localPosition = Vector3.zero;
            joint.transform.localScale = Vector3.one;
            joint.name = $"Joint {i + 1}";

            m_Joints.Add(joint);
        }

        m_TargetLocation.transform.localPosition = new Vector3(m_ArmLength * m_NumberOfJoints, 0.0f, 0.0f); // Just set it as this for the testing
    }

    private void SolveIK(Transform start, Transform target)
    {
        Vector3 dir = target.position - start.position;

        dir = dir.normalized;

        start.position = target.position - (dir * m_ArmLength);
    }

    void Update()
    {
        if (m_TargetTransform != null)
        {
            m_TargetLocation.transform.position = (m_TargetTransform.position + Vector3.up);
        }

        // First iteration, from target to start
        Transform current = m_Joints[m_NumberOfJoints - 1].transform;
        Transform currentTarget = m_TargetLocation.transform;

        for (int i = m_NumberOfJoints-1; i >= 0; --i)
        {
            SolveIK(current, currentTarget);
            currentTarget = current;

            if (i > 0)
            {
                current = m_Joints[i-1].transform;
            }
        }
        SolveIK(m_IKStart.transform, m_Joints[0].transform);

        // Second iteration, from start to target (To lock the base at it's position

        m_IKStart.transform.position = m_BasePosition;

        current = m_Joints[0].transform;
        currentTarget = m_IKStart.transform;

        for (int i = 0; i < m_NumberOfJoints; ++i)
        {
            SolveIK(current, currentTarget);
            Debug.DrawLine(current.position, currentTarget.position, Color.green);
            currentTarget = current;

            if (i < m_NumberOfJoints - 1)
            {
                current = m_Joints[i + 1].transform;
            }
        }
        
        // Just for debugdrawing
        Vector3 dir = m_Joints[m_NumberOfJoints - 1].transform.position - m_TargetLocation.transform.position;

        dir = dir.normalized;
        /////////////////////////

        Debug.DrawLine(m_Joints[m_NumberOfJoints - 1].transform.position, m_Joints[m_NumberOfJoints - 1].transform.position - (dir * m_ArmLength), Color.red);
    }

    private void OnValidate()
    {
        // SetupIK();
    }
}
