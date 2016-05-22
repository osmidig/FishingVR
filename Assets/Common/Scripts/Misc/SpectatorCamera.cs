using UnityEngine;
using System.Collections;

public class SpectatorCamera : MonoBehaviour
{
    [SerializeField] private Vector3 m_offsetPosition = Vector3.zero;

    private Transform m_transform;

    void Awake()
    {
        m_transform = transform;
    }

    public void MoveCamera(Vector3 position, Quaternion rotation)
    {
        m_transform.position = position + m_offsetPosition;
        m_transform.rotation = rotation;
    }
}
