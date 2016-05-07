using UnityEngine;
using System.Collections;

public class Spinnybit : InteractableItemBase
{
    private float m_currentDeltaAngle;

    private Transform m_attachedHand;

    protected override void Update()
    {
        base.Update();

        if(m_attachedHand != null)
        {
            // Remove if distance
            if(Vector3.Distance(m_attachedHand.position, m_transform.position) > 0.5f)
            {
                m_handObject.DetachItem();
                return;
            }

            float angleBefore = m_transform.localRotation.eulerAngles.z;

            Quaternion rotation = m_transform.rotation;

            Vector3 deltaPos = m_attachedHand.position - m_transform.position;

            Vector3 projectedPosition = Vector3.ProjectOnPlane(deltaPos, m_transform.forward);

            rotation = Quaternion.LookRotation(m_transform.forward, -projectedPosition.normalized);

            m_transform.rotation = rotation;

            float angleAfter = m_transform.localRotation.eulerAngles.z;
            float deltaAngle = Mathf.DeltaAngle(angleBefore, angleAfter);
            m_currentDeltaAngle = Mathf.Abs(deltaAngle);
        }
    }

    public override bool Attach(Transform parent, InteractableHand hand, int deviceIndex, bool worldPositionStays)
    {
        if (m_attached) return false;

        m_attached = true;

#if !UNITY_EDITOR_OSX
        m_device = SteamVR_Controller.Input(deviceIndex);

        m_attachedHand = parent;
        m_handObject = hand;

        if (!worldPositionStays)
        {
            m_transform.localPosition = Vector3.zero;
            m_transform.localRotation = Quaternion.identity;
        }

        if (m_rigidbody != null) m_rigidbody.isKinematic = true;
#endif

        return true;
    }

    public override void Detach()
    {
        m_attached = false;
        m_device = null;
        m_handObject = null;
        m_attachedHand = null;
    }
}
