using UnityEngine;
using System.Collections;

public class FishingRodInteractable : InteractableItemBase
{
    public Bounds m_MouseMovementBounds;
    public float m_MouseSensitivity = 10.0f;
    public float m_ReelSensitivity = 1.0f;

    public GameObject m_FishingRodTip;
    public GameObject m_Hook;
    public GameObject m_Bobber;

    public Spinnybit m_SpinnyBit;

    private Vector3 m_DebugRodPos;

    private bool m_spoolLocked;
    private bool m_spoolPreviouslyLocked;
    private SpringJoint m_BobberJoint;

	// Use this for initialization
	void Start () 
    {
        m_Hook.transform.parent = null;
        m_Bobber.transform.parent = null;

        m_BobberJoint = m_Bobber.GetComponent< SpringJoint >();
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();

#if UNITY_EDITOR_OSX
        if( !IsAttached() && Input.GetMouseButtonDown( 0 ) )
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            if( Physics.Raycast( ray, out hit, 100.0f ) )
            {
                if( hit.collider.gameObject == gameObject )
                {
                    m_DebugRodPos = transform.position;
                    Attach( null, null, 0, true );
                }
            }
        }

        if( IsAttached() && Input.GetMouseButtonDown( 1 ) )
        {
            Detach();
        } 

        if( IsAttached() && Input.GetMouseButtonDown( 0 ) )
        {
            m_BobberJoint.maxDistance = Vector3.Distance( m_Bobber.transform.position, m_FishingRodTip.transform.position );
        } 

        if( IsAttached() )
        {
            m_spoolLocked = Input.GetMouseButton( 0 );
        }

#else

        if (IsAttached() || m_SpinnyBit.IsAttached())
        {
            m_spoolLocked = m_SpinnyBit.IsAttached() || m_device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        if (m_spoolLocked && m_spoolLocked != m_spoolPreviouslyLocked)
        {
            m_BobberJoint.maxDistance = Vector3.Distance(m_Bobber.transform.position, m_FishingRodTip.transform.position);
        }
#endif

        if ( IsAttached() )
        {
            MoveRod();
        }

        m_BobberJoint.tolerance = m_spoolLocked ? 0.0025f : 100000.0f;

        if( m_SpinnyBit.GetCurrentDeltaAngle() != 0 )
        {
            m_BobberJoint.maxDistance = Mathf.Max( m_BobberJoint.minDistance + 0.00001f, m_BobberJoint.maxDistance - m_SpinnyBit.GetCurrentDeltaAngle() * m_ReelSensitivity );
        }
       
        m_spoolPreviouslyLocked = m_spoolLocked;
	}

    private void MoveRod()
    {
#if UNITY_EDITOR_OSX
        float inputX = -Input.GetAxis( "Mouse Y" );
        float inputY = Input.GetAxis( "Mouse X" );

        m_DebugRodPos.x += inputX * m_MouseSensitivity * Time.deltaTime;
        m_DebugRodPos.z += inputY * m_MouseSensitivity * Time.deltaTime;

        m_DebugRodPos.x = Mathf.Clamp( m_DebugRodPos.x, m_MouseMovementBounds.min.x, m_MouseMovementBounds.max.x );
        m_DebugRodPos.z = Mathf.Clamp( m_DebugRodPos.z, m_MouseMovementBounds.min.z, m_MouseMovementBounds.max.z );

        m_DebugRodPos.y = 0.00f;

        transform.position = m_DebugRodPos;
        transform.rotation = Quaternion.identity;

#else
        //ADD ANY VR MOVEMENT CODE HERE, IF NECESSARY
#endif
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube( m_MouseMovementBounds.center, m_MouseMovementBounds.size );

        Gizmos.DrawLine( m_FishingRodTip.transform.position, m_Bobber.transform.position );
        Gizmos.DrawLine( m_Bobber.transform.position, m_Hook.transform.position );
    }
}
