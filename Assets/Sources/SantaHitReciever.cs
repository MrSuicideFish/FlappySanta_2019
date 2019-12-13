using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaHitReciever : MonoBehaviour {
	public void OnCollisionEnter( Collision collision ) {
		if(collision.gameObject.layer != LayerMask.NameToLayer( "Santa" )) {
			SFXManager.inst.PlayHitSound();
			GameController.inst.EndGame();
		}
	}
}