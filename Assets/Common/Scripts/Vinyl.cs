using UnityEngine;
using System.Collections;

public class Vinyl : InteractableItemBase
{
    private Collider m_collider;
    public AudioClip m_clip;

    protected override void Awake()
    {
        base.Awake();
        m_collider = GetComponent<Collider>();
    }

    public override bool Attach(Transform parent, InteractableHand hand, int deviceIndex, bool worldPositionStays)
    {
        bool attach = base.Attach(parent, hand, deviceIndex, worldPositionStays);
        m_collider.isTrigger = true;
        return attach;
    }

    public override void Detach()
    {
        base.Detach();
        m_collider.isTrigger = false;
    }
}
