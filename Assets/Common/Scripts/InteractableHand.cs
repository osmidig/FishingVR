using UnityEngine;
using System.Collections;

public class InteractableHand : MonoBehaviour
{
    [SerializeField] private GameObject m_trackedModel;

    private InteractableItemBase m_heldObject;

    private bool m_itemAttached = false;

    private int m_deviceIndex = -1;
    private SteamVR_Controller.Device m_device;

    void SetDeviceIndex(int index)
    {
        m_deviceIndex = index;
        m_device = SteamVR_Controller.Input(index);
    }

    void Update()
    {
        if(m_heldObject != null)
        {
            Vector2 triggerInput = m_device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

            if (triggerInput.x < 0.5f && !m_itemAttached)
            {
                DetachItem();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        Vector2 triggerInput = m_device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        if (m_device != null && m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            if (m_heldObject == null)
            {
                InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();
                if (item != null && item.Attachable)
                {
                    AttachItem(item);
                }
            }
            else
            {
                DetachItem();
            }
        }
        else if (triggerInput.x >= 0.5f)
        {
            if (m_heldObject == null)
            {
                InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();
                if (item != null)
                {
                    PickupItem(item);
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
        }
    }

    private void AttachItem(InteractableItemBase item)
    {
        if (item.Attach(transform, this, m_deviceIndex, false))
        {
            m_heldObject = item;
            m_trackedModel.SetActive(false);
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
