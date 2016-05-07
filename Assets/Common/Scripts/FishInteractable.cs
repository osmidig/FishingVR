using UnityEngine;
using System.Collections;

public class FishInteractable : InteractableItemBase
{
    public float m_ScaleRange = 0.3f;

    protected override void Awake()
    {
        base.Awake();

        float newScale = Random.Range( 1.0f - m_ScaleRange, 1.0f + m_ScaleRange );

        m_minHookedTension = Mathf.Clamp( m_minHookedTension * newScale, 0.1f, 1.9f );
        m_maxHookedTension = Mathf.Clamp( m_maxHookedTension * newScale, 0.1f, 1.9f );

        transform.localScale = transform.localScale * newScale;
    }
}
