using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class Globe : ARInteractable {

	public new AugmentedImage Image;

	public override void TouchDown (PointerEventData eventData) {
		Debug.Log ("Globe TouchDown");
	}

	public override void TouchUp (PointerEventData eventData) {
		Debug.Log ("Globe TouchUp");
	}

	public override void TouchClick (PointerEventData eventData) {
		Debug.Log ("Globe TouchClick");
	}
}