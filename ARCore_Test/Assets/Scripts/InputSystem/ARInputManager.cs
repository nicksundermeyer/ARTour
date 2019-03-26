using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARInputManager : MonoBehaviour {

	// debug helpers
	[SerializeField]
	private bool _showEvents;
	private string _lastDebug;

	private float _touchStartTime;

	[SerializeField]
	private float _clickThreshold;

	// previous number of touches
	private int _lastCount;

	// previous location of touch
	private Touch _lastTouch;

	// currently controlled object
	private GameObject currentObject;

	// Update is called once per frame
	void Update () {
		int touchCount = Input.touchCount;

		if (touchCount == 0) {
			if (_lastCount == 1) {
				TouchUp (_lastTouch.position);

				if (Time.time - _touchStartTime < _clickThreshold) {
					TouchClick (_lastTouch.position);
				}
			}

			_lastCount = 0;
		}

		if (touchCount == 1) {
			if (_lastCount == 0) {
				_touchStartTime = Time.time;
				TouchDown (_lastTouch.position);
			}

			_lastTouch = Input.touches[0];
			_lastCount = 1;
		}
	}

	public void TouchDown (Vector2 pos) {
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchDown" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerDown (pData);
		}
	}

	public void TouchUp (Vector2 pos) {
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchUp" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerUp (pData);
			currentObject = null;
		}
	}

	public void TouchClick (Vector2 pos) {
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchClick" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerClick (pData);
		}
	}

	public PointerEventData Raycast (Vector2 pos) {
		// raycast to GUI first
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = pos;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);

		if (results.Count > 0) {
			foreach(RaycastResult result in results)
			{
				if(result.gameObject.GetComponentInParent<ARInteractable>() != null)
				{
					pointerData.selectedObject = result.gameObject;
					currentObject = result.gameObject;
					return pointerData;
				}
			}
		}

		// then raycast for objects
		Ray ray = Camera.main.ScreenPointToRay (pos);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, float.MaxValue)) {
			GameObject comp = hit.collider.gameObject;

			// setting current controlled object if hit
			if (comp.GetComponentInParent<ARInteractable>() != null) {
				pointerData.selectedObject = comp;
				currentObject = comp;
				return pointerData;
			}
		}
		return null;
	}

	public void DebugWithoutRepeats (string logMessage) {
		if (_showEvents) {
			if (_lastDebug != logMessage) {
				Debug.Log (logMessage);
			}
			_lastDebug = logMessage;
		}
	}
}