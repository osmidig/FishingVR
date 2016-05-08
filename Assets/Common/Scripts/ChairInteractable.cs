using UnityEngine;
using System.Collections;

public class ChairInteractable : InteractableItemBase {
    
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();

        transform.rotation = Quaternion.Euler( 0, 90, 0 );
	}
}
