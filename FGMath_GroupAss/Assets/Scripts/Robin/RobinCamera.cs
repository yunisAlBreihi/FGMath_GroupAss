using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinCamera : MonoBehaviour
{
    [SerializeField] private GameObject m_Orrery;
    [SerializeField] private Vector3 m_CameraOffset = new Vector3(0.0f, 3.0f, -2.0f);
    [SerializeField] private Vector3 m_CameraRotationEuler = new Vector3(90.0f, 0.0f, 0.0f);

    void Start()
    {
        Vector3 startPos = m_Orrery.transform.position + m_CameraOffset;

        transform.position = startPos;
        transform.rotation = Quaternion.Euler(m_CameraRotationEuler);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = m_Orrery.transform.position + m_CameraOffset;

        transform.position = newPos;
    }

    private void OnValidate()
    {
        Vector3 newPos = m_Orrery.transform.position + m_CameraOffset;

        transform.position = newPos;
        transform.rotation = Quaternion.Euler(m_CameraRotationEuler);
    }
}
