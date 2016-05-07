using UnityEngine;
using System.Collections;

public class RenderLine : MonoBehaviour
{
    [SerializeField] private Transform m_tip;
    [SerializeField] private Transform m_bobber;
    [SerializeField] private Transform m_hook;
    [SerializeField] private GameObject m_linePrefab;

    private Transform[] m_lineCylinders;

	void Awake()
    {
        m_lineCylinders = new Transform[2];
        for (int i = 0; i < 2; ++i)
        {
            GameObject inst = Instantiate(m_linePrefab) as GameObject;
            m_lineCylinders[i] = inst.transform;
        }

        UpdatePositions();
    }

    void FixedUpdate()
    {
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        SetTransform(m_lineCylinders[0], m_tip.position, m_bobber.position);
        SetTransform(m_lineCylinders[1], m_bobber.position, m_hook.position);
    }

    private void SetTransform(Transform trans, Vector3 from, Vector3 to)
    {
        Vector3 delta = to - from;
        float distance = delta.magnitude;

        Vector3 scale = trans.localScale;
        scale.z = distance;
        trans.localScale = scale;

        trans.position = from;

        trans.LookAt(to);
    }
}
