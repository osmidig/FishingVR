using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishInteractable : InteractableItemBase
{
    public float m_ScaleRange = 0.3f;

    public MeshRenderer m_FishMesh;

    public List<MaterialOverride> m_MaterialOverrides;

    [System.Serializable]
    public class MaterialOverride
    {
        public bool doOverride;
        public Color color;
        public string name;
    };

    protected override void Awake()
    {
        base.Awake();

        float newScale = Random.Range( 1.0f - m_ScaleRange, 1.0f + m_ScaleRange );

        m_minHookedTension = Mathf.Clamp( m_minHookedTension * newScale, 0.1f, 1.9f );
        m_maxHookedTension = Mathf.Clamp( m_maxHookedTension * newScale, 0.1f, 1.9f );

        transform.localScale = transform.localScale * newScale;

        if( m_FishMesh != null )
        {
            //this triggers materials to be instanced, and their order to be changed. Grr.
            string n = m_FishMesh.materials[ 0 ].name;
            int overrideIndex = -1;

            for( int i = 0; i < m_FishMesh.materials.Length; ++i )
            {
                overrideIndex = -1;

                for( int x = 0; x < m_MaterialOverrides.Count; ++x )
                {
                    if( m_FishMesh.materials[ i ].name.Contains( m_MaterialOverrides[ x ].name ) )
                    {
                        overrideIndex = x;
                        break;
                    }

                }

                if( overrideIndex == -1 )
                {
                    Debug.LogWarning( "Override Material not found on Fish." );
                    continue;
                }

                m_FishMesh.materials[ i ].color = m_MaterialOverrides[ overrideIndex ].doOverride ? 
                                                        m_MaterialOverrides[ overrideIndex ].color : 
                                                        m_FishMesh.materials[ i ].color;

            }
        }
    }
}
