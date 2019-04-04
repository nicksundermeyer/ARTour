using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Globe : ARInteractable {

	public new AugmentedImage Image;

	// variables for rotation
	private Vector2 dragLast;
	public float rotationSpeed = 0.5f;

	public override void OnPointerDown (PointerEventData eventData) {
		Debug.Log ("Globe OnPointerDown: " + eventData.selectedObject.gameObject.name);
	}

	public override void OnPointerUp (PointerEventData eventData) {
		Debug.Log ("Globe OnPointerUp: " + eventData.selectedObject.gameObject.name);
	}

	public override void OnPointerClick (PointerEventData eventData) {
		Debug.Log ("Globe OnPointerClick: " + eventData.selectedObject.gameObject.name);
	}

	public override void OnBeginDrag(PointerEventData eventData) {
		Debug.Log ("Globe TouchDragStart: " + eventData.position);

		dragLast = eventData.position;
	}

	public override void OnEndDrag(PointerEventData eventData) {
		Debug.Log ("Globe TouchDragEnd: " + eventData.position);
	}

	public override void OnDrag(PointerEventData eventData) {
		Debug.Log ("Globe TouchDrag: " + eventData.position);

		// rotating globe
		transform.Rotate(new Vector3(0, (dragLast.x - eventData.position.x)*rotationSpeed, 0), Space.World);
		dragLast = eventData.position;
	}
}