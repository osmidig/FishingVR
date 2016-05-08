using UnityEngine;
using System.Collections;

[RequireComponent( typeof( AudioSource ) )]
public class VinylPlayerInteractable : InteractableItemBase {

    private AudioSource m_audioSource;
    public Transform m_VinylSnapTransform;

    private Vinyl m_currentVinyl;

	protected override void Awake ()
    {
        base.Awake();
        m_audioSource = GetComponent<AudioSource>();
	}

	protected override void Update () {
        base.Update();

        if ( m_currentVinyl != null && m_currentVinyl.IsAttached() )
        {
            m_audioSource.Stop();
            m_audioSource.clip = null;
            m_currentVinyl.GetComponent<Collider>().isTrigger = false;
            m_currentVinyl = null;
        }
	}

    void OnCollisionEnter( Collision c )
    {
        Vinyl v = c.collider.gameObject.GetComponent<Vinyl>();

        if( v != null && m_currentVinyl == null && !v.IsAttached() )
        {
            PlayNewVinyl( v );
        }
    }

    private void PlayNewVinyl( Vinyl v )
    {
        m_audioSource.Stop();
        m_audioSource.clip = v.m_clip;
        m_audioSource.Play();

        v.GetComponent<Collider>().isTrigger = true;
        v.GetComponent<Rigidbody>().isKinematic = true;

        v.transform.SetParent( transform );
        v.transform.position = m_VinylSnapTransform.position;
        v.transform.rotation = m_VinylSnapTransform.rotation;

        m_currentVinyl = v;
    }

    public void TogglePlayback()
    {
        if( m_audioSource.isPlaying )
        {
            m_audioSource.Stop();
        }
        else
        {
            m_audioSource.Play();
        }
    }
}
