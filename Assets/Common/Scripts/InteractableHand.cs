using UnityEngine;
using System.Collections;

public class InteractableHand : MonoBehaviour
{
    [SerializeField] private GameObject m_trackedModel;

    private InteractableItemBase m_heldObject;

    private int m_deviceIndex = -1;
    private SteamVR_Controller.Device m_device;

    public void Detach()
    {
        m_heldObject = null;
        m_trackedModel.SetActive(true);
    }

    void SetDeviceIndex(int index)
    {
        m_deviceIndex = index;
        m_device = SteamVR_Controller.Input(index);
    }

    void OnTriggerStay(Collider other)
    {
        if (m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            if (m_heldObject == null)
            {
                InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();
                if (item != null)
                {
                    if (item.Attach(transform, m_deviceIndex))
                    {
                        m_heldObject = item;
                        m_trackedModel.SetActive(false);
                    }
                }
            }
            else
            {
                m_heldObject.Detach();
                m_heldObject = null;
                m_trackedModel.SetActive(true);
            }
        }
    }
}
