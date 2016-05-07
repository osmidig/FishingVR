using UnityEngine;
using System.Collections;

public class FishingRodInteractable : InteractableItemBase {

    public Bounds m_MouseMovementBounds;
    public float m_MouseSensitivity = 10.0f;

    public GameObject m_FishingRodTip;
    public GameObject m_Hook;
    public GameObject m_Bobber;

    private Vector3 m_DebugRodPos;
    public GameObject yourmum;

	// Use this for initialization
	void Start () 
    {
        m_Hook.transform.parent = null;
        m_Bobber.transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () 
    {
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
                    Attach( null, 0, true );
                }
            }
        }

        if( IsAttached() && Input.GetMouseButtonDown( 1 ) )
        {
            Detach();
        }            
#endif

        if( IsAttached() )
        {
            MoveRod();
        }

        Debug.Log( yourmum.GetComponent<SkinnedMeshRenderer>().bounds.extents );


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

        m_DebugRodPos.y = 0.75f;

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
