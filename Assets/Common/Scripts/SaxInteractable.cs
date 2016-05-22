using UnityEngine;
using System.Collections;

public class SaxInteractable : InteractableItemBase
{
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private AudioClip[] m_saxSounds;

    protected override void Awake()
    {
        base.Awake();
        m_audio.loop = false;
        m_audio.playOnAwake = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!m_attached || !OnMouth) return;

        if(!m_audio.isPlaying)
        {
            if (m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) || m_device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
            {
                m_audio.clip = m_saxSounds[Random.Range(0, m_saxSounds.Length)];
                m_audio.Play();
            }
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
}
