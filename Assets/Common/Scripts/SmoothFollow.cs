using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform m_follow;
    [SerializeField] private float m_followSpeed = 10f;

    private Vector3 m_previousPosition;
    private Transform m_transform;

    void Awake()
    {
        m_transform = transform;
        m_previousPosition = m_follow.position;
    }

    void LateUpdate()
    {
        m_transform.position = Vector3.Lerp(m_previousPosition, m_follow.position, Time.deltaTime * m_followSpeed);
        m_previousPosition = m_transform.position;
    }
}
