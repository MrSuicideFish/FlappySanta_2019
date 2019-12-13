using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PointCounter : MonoBehaviour{

	private Text pointTxt;
	private void Update() {

		if (GameController.inst == null) {
			return;
		}

		if(pointTxt == null) {
			pointTxt = this.GetComponent<Text>();
		}

		pointTxt.text = GameController.inst.points.ToString();
	}
}