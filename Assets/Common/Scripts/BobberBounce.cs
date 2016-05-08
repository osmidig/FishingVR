using UnityEngine;
using System.Collections;

public class BobberBounce : MonoBehaviour
{
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private float m_multiplier = 1f;

    public float m_BobRange = 0.5f;

    private Transform m_transform;
    private Transform m_parent;
    private Vector3 m_Offset;

    void Awake()
    {
        m_transform = transform;
        m_parent = m_transform.parent;
    }

    void Update()
    {
        m_transform.up = Vector3.up; //always face up!
    }

    public void DoBounce()
    {
        //tasty bouncing
        m_Offset.y = Mathf.Lerp(-m_BobRange, m_BobRange, (Mathf.Sin(Time.time * Mathf.PI * 0.5f * m_speed) + 1.0f) * 0.5f);
        m_transform.position = m_parent.position + m_Offset;
    }
}
