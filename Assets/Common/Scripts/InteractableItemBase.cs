using UnityEngine;
using System.Collections;

[ RequireComponent ( typeof( Rigidbody ) ) ]
[ RequireComponent ( typeof( Collider ) ) ]
public class InteractableItemBase : MonoBehaviour
{
    [SerializeField] private bool m_attachable = true;

    private bool m_attached = false;
    private SteamVR_Controller.Device m_device;

    private Rigidbody m_rigidbody;

    private Vector3 m_previousPosition;
    private Quaternion m_previousRotation;
    private Vector3 m_previousForward;

    private Transform m_origParent;
    private Transform m_transform;

    private Rigidbody m_rigidbody;
    private Vector3 storedVelocity;

    public bool Attachable
    {
        get { return m_attachable; }
    }

    protected virtual void Awake()
    {
        m_transform = transform;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {

    }

    protected virtual void LateUpdate()
    {
        m_previousPosition = m_transform.position;
        m_previousRotation = m_transform.rotation;
        m_previousForward = m_transform.forward;
    }

    public bool Attach(Transform parent, int deviceIndex, bool worldPositionStays)
    {
        if (m_attached) return false;

        m_attached = true;

#if !UNITY_EDITOR_OSX
        m_device = SteamVR_Controller.Input(deviceIndex);

        m_origParent = m_transform.parent;
        m_transform.SetParent(parent, worldPositionStays);

        if(!worldPositionStays)
        {
            m_transform.localPosition = Vector3.zero;
            m_transform.localRotation = Quaternion.identity;
        }

        if (m_rigidbody != null) m_rigidbody.isKinematic = true;
#endif

        return true;
    }

    public void Detach()
    {
        m_attached = false;
        m_device = null;
        m_transform.SetParent(m_origParent, true);

        if (m_rigidbody != null)
        {
            m_rigidbody.isKinematic = false;

            Vector3 deltaPos = m_transform.position - m_previousPosition;
            Vector3 newVel = deltaPos / Time.deltaTime;

            m_rigidbody.AddForce(newVel, ForceMode.VelocityChange);

            /*
            Quaternion deltaRot = Quaternion.Inverse(m_transform.rotation) * m_previousRotation;
            Vector3 newTorque = deltaRot.eulerAngles / Time.deltaTime;

            m_rigidbody.AddTorque(newTorque, ForceMode.VelocityChange);
            */

            Vector3 x = Vector3.Cross(m_previousForward, m_transform.forward);
            float theta = Mathf.Asin(x.magnitude);
            Vector3 w = x.normalized * theta / Time.fixedDeltaTime;

            Quaternion q = transform.rotation * m_rigidbody.inertiaTensorRotation;
            Vector3 newTorque = q * Vector3.Scale(m_rigidbody.inertiaTensor, (Quaternion.Inverse(q) * w));

            m_rigidbody.AddTorque(newTorque, ForceMode.Impulse);
        }
    }

    public bool IsAttached()
    {
        return m_attached;
    }
   
}
