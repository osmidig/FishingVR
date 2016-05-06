using UnityEngine;
using System.Collections;

public class InteractableItemBase : MonoBehaviour
{

    private bool m_attached = false;
    private SteamVR_Controller.Device m_device;

    private Transform m_origParent;
    private Transform m_transform;

    protected virtual void Awake()
    {
        m_transform = transform;
    }

    public bool Attach(Transform parent, int deviceIndex)
    {
        if (m_attached) return false;

        m_attached = true;
        m_device = SteamVR_Controller.Input(deviceIndex);

        m_origParent = m_transform.parent;
        m_transform.SetParent(parent, false);
        m_transform.localPosition = Vector3.zero;
        m_transform.localRotation = Quaternion.identity;

        return true;
    }

    public void Detach()
    {
        m_attached = false;
        m_device = null;
        m_transform.SetParent(m_origParent, true);
    }
}
