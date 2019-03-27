using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Globe : ARInteractable {

	public new AugmentedImage Image;

	public override void OnPointerDown (PointerEventData eventData) {
		// Debug.Log ("Globe OnPointerDown: " + eventData.selectedObject.gameObject.name);
	}

	public override void OnPointerUp (PointerEventData eventData) {
		// Debug.Log ("Globe OnPointerUp: " + eventData.selectedObject.gameObject.name);
	}

	public override void OnPointerClick (PointerEventData eventData) {
		// Debug.Log ("Globe OnPointerClick: " + eventData.selectedObject.gameObject.name);
	}

	public void OnRightButtonClick()
	{
		Debug.Log("Right Button Clicked");
	}

	public void OnLeftButtonClick()
	{
		Debug.Log("Left Button Clicked");
	}
}