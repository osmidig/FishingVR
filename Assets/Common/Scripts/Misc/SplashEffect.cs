using UnityEngine;
using System.Collections;

public class SplashEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_splashParticle;
    [SerializeField] private AudioSource m_splashAudio;
    [SerializeField] private AudioClip[] m_splashSounds;

    private bool m_splashed = false;

    void Awake()
    {
        m_splashAudio.playOnAwake = false;
        m_splashAudio.loop = false;
    }

	public void Splash()
    {
        m_splashAudio.clip = m_splashSounds[Random.Range(0, m_splashSounds.Length)];

        m_splashParticle.Play();
        m_splashAudio.Play();

        m_splashed = true;
    }

    void Update()
    {
        if(m_splashed && !m_splashAudio.isPlaying && m_splashParticle.isPlaying)
        {
            Destroy(gameObject, 0.5f);
        }
    }
}
