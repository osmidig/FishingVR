using UnityEngine;
using System.Collections;

public class CreepyEyeStare : MonoBehaviour
{
    private Transform m_target;
    private Transform m_transform;

    void Awake()
    {
        m_transform = transform;
        var head = FindObjectOfType<InteractableHead>();
        if (head != null) m_target = head.transform;
    }

    void LateUpdate()
    {
        if(m_target != null)
        {
            m_transform.LookAt(m_target);
        }
    }
}
