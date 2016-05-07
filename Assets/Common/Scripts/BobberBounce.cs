using UnityEngine;
using System.Collections;

public class BobberBounce : MonoBehaviour
{
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private float m_multiplier = 1f;

    private Transform m_transform;
    private Transform m_parent;
    private Vector3 m_Offset;

    void Awake()
    {
        m_transform = transform;
        m_parent = m_transform.parent;
    }

    public void DoBounce()
    {
        float time = Mathf.Sin(Time.time * m_speed);
        m_Offset.y = time * m_multiplier;
        m_transform.position = m_parent.position + m_Offset;
    }
}
