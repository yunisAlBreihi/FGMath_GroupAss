using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RobinOrreryArm
{
    public Transform orreryTransform;

    public GameObject m_BaseArm;
    public GameObject m_UpperArm;

    public float m_BaseArmLength;
    public float m_UpperArmLength;

    public Vector3 m_ArmStartPosition;
    public GameObject m_Joint;
    public GameObject m_StartJoint;

    public RobinPlanet m_Planet;

    public void MoveArm(bool drawDebugLines = false)
    {
        // Move from target (ie planet) to start | joint toward planet, start toward joint
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

        UpdateArmMesh(m_BaseArm.transform, m_StartJoint.transform.position, m_Joint.transform.position);
        UpdateArmMesh(m_UpperArm.transform, m_Joint.transform.position, m_Joint.transform.position - (dir * m_UpperArmLength));
    }

    private void SolveIK(Transform start, Transform target, float armLength)
    {
        Vector3 dir = target.position - start.position;

        dir = dir.normalized;

        start.position = target.position - (dir * armLength);
    }

    void UpdateArmMesh(Transform transform, Vector3 start, Vector3 end)
    {
        Vector3 middlePoint = (start + end) * 0.5f;

        transform.LookAt(end);
        transform.position = middlePoint;
    }
}
