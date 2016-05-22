using UnityEngine;
using System.Collections;

public class InteractableHand : MonoBehaviour
{
    [SerializeField] private GameObject m_trackedModel;

    private InteractableItemBase m_heldObject;

    private bool m_itemAttached = false;
    private bool m_triggerUsed = false;
    private bool m_attachedThisFrame = false;

    private int m_deviceIndex = -1;
    private SteamVR_Controller.Device m_device;

    void SetDeviceIndex(int index)
    {
        m_deviceIndex = index;
        m_device = SteamVR_Controller.Input(index);
    }

    void Update()
    {
        if (m_device == null) return;

        if(m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            SpectatorCamera specCamera = FindObjectOfType<SpectatorCamera>();
            if(specCamera != null)
            {
                specCamera.MoveCamera(transform.position, transform.rotation);
            }
        }

        if(m_heldObject != null)
        {
            if(((m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip) && m_heldObject.Attachable) || (!m_device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) && !m_itemAttached)) && !m_attachedThisFrame)
            {
                DetachItem();
            }
        }
        m_attachedThisFrame = false;
    }

    void FixedUpdate()
    {
        m_triggerUsed = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (m_triggerUsed) return;

        bool inputGripDown = m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
        bool inputTriggerDown = m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        if (m_device != null && (inputGripDown || inputTriggerDown))
        {
            if (m_heldObject == null)
            {
                InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();
                if (item != null)
                {
                    if (!item.IsAttached())
                        m_triggerUsed = true;
                    if (item.Attachable)
                    {
                        AttachItem(item);
                    }
                    else
                    {
                        if(!inputGripDown)
                            PickupItem(item);
                    }
                }
            }
        }
    }

    private void PickupItem(InteractableItemBase item)
    {
        if (item.Attach(transform, this, m_deviceIndex, true))
        {
            m_heldObject = item;
            m_trackedModel.SetActive(false);
            m_attachedThisFrame = true;
        }
    }

    private void AttachItem(InteractableItemBase item)
    {
        if (item.Attach(transform, this, m_deviceIndex, false))
        {
            m_heldObject = item;
            m_trackedModel.SetActive(false);
            m_attachedThisFrame = true;
            m_itemAttached = true;
        }
    }

    public void DetachItem()
    {
        m_heldObject.Detach();
        m_heldObject = null;
        m_trackedModel.SetActive(true);
        m_itemAttached = false;
    }
}
