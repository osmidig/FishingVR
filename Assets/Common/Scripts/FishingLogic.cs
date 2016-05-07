using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingLogic : MonoBehaviour {

    public FishingRodInteractable m_FishingRod;
    public GameObject m_Bobber;

    public List<InteractableItemBase> m_HookableObjects;
    public List<InteractableItemBase> m_PostGameHookableObjects;

    public float m_MinBiteTime = 8.0f;
    public float m_MaxBiteTime = 25.0f;

    private float m_timeTillBite;

    private Vector3 m_initialBiteLoc;
    private float m_biteDistanceToHook = 0.5f;
    private float m_biteTime;
    private float m_biteGracePeriod = 1.0f; //time to react before losing fish
    private bool m_CurrentlyHooked;
    private InteractableItemBase m_HookedObj;

    protected float m_targetFishTension;
    protected float m_lastFishTensionChangeTime;
    private float m_ActualFishTension;
    private float m_tensionToBreakFree = 0.85f;
    private float m_minTensionHapticValue = 500;
    private float m_maxTensionHapticValue = 3999;


	// Use this for initialization
	void Start () 
    {
        ResetBite();
        m_CurrentlyHooked = false;
        m_lastFishTensionChangeTime = 0;
        m_ActualFishTension = 0.0f;
	}

    private void DoHooked()
    {
        if( m_lastFishTensionChangeTime == 0 )
        {
            m_lastFishTensionChangeTime = m_HookedObj.GetRandomFishTensionChangeTime();
            m_targetFishTension = m_HookedObj.GetRandomFishTension();
        }
        else
        {
            m_lastFishTensionChangeTime -= Time.deltaTime;
            m_ActualFishTension = Mathf.Lerp( m_ActualFishTension, m_targetFishTension, 0.1f ) * Time.deltaTime;

            float totalTension = ( m_ActualFishTension + m_FishingRod.GetReelTension() ) * 0.5f;

            if( totalTension > m_tensionToBreakFree )
            {
                GameObject.Destroy( m_HookedObj ); //we didnt get it, destroy it
                ResetBite();
            }
            else
            {
                //haptic value on a Coserp scale
                float hapticVal = Mathf.Lerp(m_minTensionHapticValue, m_maxTensionHapticValue, 1.0f - Mathf.Cos( (totalTension / m_tensionToBreakFree ) * Mathf.PI * 0.5f));

                //DO HAPTICS HERE
            }
        }
    }

    private void WaitingForHook()
    {
        //Have we hooked?
        float biteDist = Vector3.Distance( m_initialBiteLoc, m_Bobber.transform.position );
        if( m_biteTime != 0  && biteDist >= m_biteDistanceToHook )
        {
            Hook();
        }
        else if( m_biteTime != 0 && Time.timeSinceLevelLoad - m_biteTime > m_biteGracePeriod ) //lost the fish
        {
            ResetBite();
        }
    }

    private void ResetBite()
    {
        m_timeTillBite = Random.Range( m_MinBiteTime, m_MaxBiteTime );
        m_biteTime = 0;
        m_ActualFishTension = 0;
        m_targetFishTension = 0;
        m_CurrentlyHooked = false;
        m_HookedObj = null;
    }

    private void TriggerBite()
    {
        m_biteTime = Time.timeSinceLevelLoad;
        m_initialBiteLoc = m_Bobber.transform.position;

        //TODO HAPTICS?
    }

    private void Hook()
    {
        if(m_HookableObjects.Count > 0 )
        {
            m_CurrentlyHooked = true;
            m_HookedObj = (InteractableItemBase)GameObject.Instantiate( m_HookableObjects[ 0 ], Vector3.zero, Quaternion.identity );
            m_FishingRod.HookObject( m_HookedObj );
            m_biteTime = 0;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if( collision.collider.gameObject == m_Bobber )
        {
            if( !m_CurrentlyHooked && m_timeTillBite > 0 && m_HookableObjects.Count > 0 )
            {
                m_timeTillBite -= Time.fixedDeltaTime;

                if( m_timeTillBite <= 0 )
                {
                    m_timeTillBite = 0;
                    TriggerBite();
                }
            }
            else if( !m_CurrentlyHooked && m_timeTillBite == 0 )
            {
                WaitingForHook();
            }
            else if( m_CurrentlyHooked )
            {
                if( m_HookedObj.IsAttached() ) // has player grabbed it?
                {
                    m_CurrentlyHooked = false;

                    //remove this catch from the list
                    if( m_HookableObjects.Count > 0 )
                    {
                        m_HookableObjects.RemoveAt( 0 );
                    }

                    //if we've caught everything, add a random "extra"
                    if( m_HookableObjects.Count == 0 )
                    {
                        m_HookableObjects.Add( m_PostGameHookableObjects[ Random.Range( 0, m_PostGameHookableObjects.Count ) ] );
                    }

                    ResetBite();
                }
                else
                {
                    DoHooked();
                }
            }
        }
    }
}
