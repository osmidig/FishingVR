using UnityEngine;
using System.Collections;

public class FishInteractable : InteractableItemBase
{
    public float m_MinScale = 0.5f;
    public float m_MaxScale = 1.3f;

    protected override void Awake()
    {
        base.Awake();

        float newScale = Random.Range( m_MinScale, m_MaxScale);

        m_minHookedTension = Mathf.Clamp( m_minHookedTension * newScale, 0.1f, 1.9f );
        m_maxHookedTension = Mathf.Clamp( m_maxHookedTension * newScale, 0.1f, 1.9f );

        transform.localScale = transform.localScale * newScale;
    }
}
