using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMessage : MonoBehaviour{

	public RectTransform rect;
	public Text messageText;
	public Animation animComponent;

	public void SetMessage(string msg ) {
		messageText.text = msg;
	}

	public void Play(string anim ) {
		if (!string.IsNullOrEmpty( anim )) {
			animComponent.Play( anim );
		}

		GameObject.Destroy( this.gameObject, 2.0f );
	}
}
