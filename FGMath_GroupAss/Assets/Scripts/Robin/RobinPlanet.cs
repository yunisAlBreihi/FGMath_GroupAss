using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinPlanet
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
