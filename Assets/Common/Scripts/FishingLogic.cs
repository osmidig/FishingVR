using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingLogic : MonoBehaviour {

    public FishingRodInteractable m_FishingRod;
    public GameObject m_Bobber;

    public List<InteractableItemBase> m_HookableObjects_Medium;
    public List<InteractableItemBase> m_HookableObjects_Long;
    public List<InteractableItemBase> m_PostGameHookableObjects_Medium;
    public List<InteractableItemBase> m_PostGameHookableObjects_Long;

    public BobberBounce m_BobberBounce;
    public ParticleSystem m_BobberEffect;

    public float m_MinBiteTime = 8.0f;
    public float m_MaxBiteTime = 25.0f;
    public float m_MediumFishingRadius = 14.0f;
    public float m_LongFishingRadius = 22.0f;

    public float m_SplashForce = 10.0f;
    public ParticleSystem m_SplashEffect;

    public AudioClip[] m_splashSounds;

    private float m_timeTillBite;

    private Vector3 m_initialBiteLoc;
    private float m_biteDistanceToHook = 0.5f;
    private float m_biteTime;
    private float m_TriggerBiteHaptic;
    private float m_BiteHapticInterval = 0.1f;
    private float m_biteGracePeriod = 1.0f; //time to react before losing fish
    private bool m_CurrentlyHooked;
    private InteractableItemBase m_HookedObj;

    protected float m_targetFishTension;
    protected float m_lastFishTensionChangeTime;
    private float m_ActualFishTension;
    private float m_tensionToBreakFree = 0.7f;
    private float m_tensionWarningZone = 0.15f;
    private float m_minTensionHapticValue = 500;
    private float m_maxTensionHapticValue_PreWarning = 1000;
    private float m_maxTensionHapticValue_PostWarning = 3999;

    private float m_bobberEffectTimer = 0;
    private float m_bobberEffectDelay = 1.0f;

    private List<InteractableItemBase> m_currentBucket;


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
        if( m_lastFishTensionChangeTime <= 0 )
        {
            m_lastFishTensionChangeTime = m_HookedObj.GetRandomFishTensionChangeTime();
            m_targetFishTension = m_HookedObj.GetRandomFishTension();
        }        
        
        m_lastFishTensionChangeTime -= Time.deltaTime;
        m_ActualFishTension = Mathf.Lerp( m_ActualFishTension, m_targetFishTension, 0.8f );

        float totalTension = ( m_ActualFishTension + m_FishingRod.GetReelTension() ) * 0.5f;

        if( totalTension > m_tensionToBreakFree )
        {
            GameObject.Destroy( m_HookedObj.gameObject ); //we didnt get it, destroy it
            ResetBite();
            m_currentBucket = null;
        }
        else
        {
            //haptic value on a Coserp scale
            float hapticVal = m_maxTensionHapticValue_PostWarning;

            if( totalTension < m_tensionToBreakFree - m_tensionWarningZone)
            {
                hapticVal = Mathf.Lerp(m_minTensionHapticValue, m_maxTensionHapticValue_PreWarning, 1.0f - Mathf.Cos((totalTension / m_tensionToBreakFree) * Mathf.PI * 0.5f));
            }            

            SteamVR_Controller.Device device = m_FishingRod.Device;
            if (device != null)
            {
                device.TriggerHapticPulse((ushort)hapticVal);
            }

            //DO HAPTICS HERE
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
        m_TriggerBiteHaptic -= Time.deltaTime;

        if ( !m_CurrentlyHooked && m_TriggerBiteHaptic <= 0 )
        {
            m_TriggerBiteHaptic = m_BiteHapticInterval;

            SteamVR_Controller.Device device = m_FishingRod.Device;
            if (device != null)
            {
                device.TriggerHapticPulse(2000);
            }
        }
    }

    private List<InteractableItemBase> GetFishBucket()
    {
        if( m_Bobber.transform.position.sqrMagnitude > Mathf.Pow( m_LongFishingRadius, 2.0f ) )
        {
            return m_HookableObjects_Long;
        }
        else if( m_Bobber.transform.position.sqrMagnitude > Mathf.Pow( m_MediumFishingRadius, 2.0f ) )
        {
            return m_HookableObjects_Medium;
        }

        return null;
    }

    private void Hook()
    {
        List<InteractableItemBase> bucket = GetFishBucket();

        if( bucket != null && bucket.Count > 0 )
        {
            m_CurrentlyHooked = true;
            m_HookedObj = (InteractableItemBase)GameObject.Instantiate( bucket[ 0 ], Vector3.zero, Quaternion.identity );
            m_FishingRod.HookObject( m_HookedObj );
            m_biteTime = 0;

            m_currentBucket = bucket;
        }
    }

    void Update()
    {
        if (m_CurrentlyHooked)
        {
            if (m_HookedObj.IsAttached()) // has player grabbed it?
            {
                m_CurrentlyHooked = false;

                List<InteractableItemBase> postGameBucket = m_currentBucket == m_HookableObjects_Long ? m_PostGameHookableObjects_Long : m_PostGameHookableObjects_Medium;

                //remove this catch from the list
                if (m_currentBucket.Count > 0)
                {
                    m_currentBucket.RemoveAt(0);
                }

                //if we've caught everything, add a random "extra"
                if (m_currentBucket.Count == 0)
                {
                    m_currentBucket.Add(postGameBucket[Random.Range(0, postGameBucket.Count)]);
                }

                m_currentBucket = null;

                ResetBite();
            }
        }
        else if (!m_CurrentlyHooked && m_timeTillBite == 0)
        {
            WaitingForHook();
        }

        if(!m_CurrentlyHooked && m_Bobber.transform.position.y < transform.position.y + 0.05f)
        {
            m_BobberBounce.DoBounce();
        }

        m_bobberEffectTimer = Mathf.Max( 0, m_bobberEffectTimer - Time.deltaTime );
    }

    void OnCollisionEnter(Collision collision)
    {        
        if( collision.collider.gameObject == m_Bobber )
        {
            if( m_bobberEffectTimer == 0 )
            {
                if (collision.relativeVelocity.sqrMagnitude > Mathf.Pow(m_SplashForce, 2.0f))
                {
                    m_SplashEffect.transform.position = collision.transform.position;
                    m_SplashEffect.Play();
                    AudioSource.PlayClipAtPoint(m_splashSounds[Random.Range(0, m_splashSounds.Length)], collision.transform.position);
                }

                m_BobberEffect.transform.position = collision.transform.position;
                m_BobberEffect.Play();

                m_bobberEffectTimer = m_bobberEffectDelay;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        //Bites and hooking can only happen beyond minimum fishing radius
        bool IsBeyondMinFishing = collision.transform.position.sqrMagnitude > Mathf.Pow( m_MediumFishingRadius, 2.0f );
        if( collision.collider.gameObject == m_Bobber && IsBeyondMinFishing )
        {
            List<InteractableItemBase> bucket = GetFishBucket();

            if( !m_CurrentlyHooked && m_timeTillBite > 0 && bucket.Count > 0 )
            {
                m_timeTillBite -= Time.fixedDeltaTime;

                if( m_timeTillBite <= 0 )
                {
                    m_timeTillBite = 0;
                    
                    m_biteTime = Time.timeSinceLevelLoad;
                    m_initialBiteLoc = m_Bobber.transform.position;

                    m_TriggerBiteHaptic = 0;
                }
            }
            else if(!m_CurrentlyHooked && m_biteTime != 0)
            {
                TriggerBite();
            }
            else if(m_CurrentlyHooked && !m_HookedObj.IsAttached())
            {
                DoHooked();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere( Vector3.zero, m_MediumFishingRadius );
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere( Vector3.zero, m_LongFishingRadius );
    }
}
