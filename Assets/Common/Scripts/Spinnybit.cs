using UnityEngine;
using System.Collections;

public class Spinnybit : InteractableItemBase
{
    [SerializeField] private int m_numberOfSnaps = 10;
    [SerializeField] private ushort m_hapticDuration = 500;
    [SerializeField] private AudioClip[] m_clickSounds;

    private float m_currentDeltaAngle;
    private float m_targetDeltaAngle;

    private float m_spinnyBuildup = 0f;

    private bool m_firstAttached = true;
    private Transform m_attachedHand;

    private AudioSource m_audio;

    public float GetCurrentDeltaAngle()
    {

#if UNITY_EDITOR_OSX
        return Mathf.Abs( Input.mouseScrollDelta.y );
#else
        return m_currentDeltaAngle;
#endif
    }

    protected override void Awake()
    {
        base.Awake();
        m_audio = GetComponent<AudioSource>();
    }

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

            if(m_firstAttached)
            {
                m_currentDeltaAngle = 0f;
                m_targetDeltaAngle = 0f;
                m_firstAttached = false;
            }
            else
            {
                m_targetDeltaAngle = Mathf.Abs(deltaAngle);
                m_currentDeltaAngle = Mathf.Lerp(m_currentDeltaAngle, m_targetDeltaAngle, Time.deltaTime * 5f);
            }

            m_spinnyBuildup += m_currentDeltaAngle;

            float snapAngle = 360f / m_numberOfSnaps;
            if(m_spinnyBuildup >= snapAngle)
            {
                m_spinnyBuildup -= snapAngle;
                //m_device.TriggerHapticPulse(500, Valve.VR.EVRButtonId.k_EButton_Axis0);
                m_device.TriggerHapticPulse(m_hapticDuration, Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
                m_audio.PlayOneShot(m_clickSounds[Random.Range(0, m_clickSounds.Length)]);
            }
        }
        else
        {
            m_firstAttached = true;
            m_currentDeltaAngle = 0f;
            m_targetDeltaAngle = 0f;
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
