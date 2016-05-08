using UnityEngine;
using System.Collections;

public class InteractableHead : MonoBehaviour
{

	void OnTriggerEnter(Collider other)
    {
        InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();

        if(item != null)
        {
            item.OnMouth = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        InteractableItemBase item = other.GetComponentInParent<InteractableItemBase>();

        if (item != null)
        {
            item.OnMouth = false;
        }
    }
}
