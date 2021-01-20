using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrreryRod : MonoBehaviour
{
    GameObject m_planetCylinder = null;
    GameObject m_sunCylinder = null;

    public OrreryRod(Vector3 downFromSun, Vector3 planetPosition)
    {
        m_planetCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);



        m_sunCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_sunCylinder.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        m_sunCylinder.transform.position = planetPosition / 2 - downFromSun;
        m_sunCylinder.transform.localScale = new Vector3(1.0f, planetPosition.x / 2, 1.0f);
    }

}
