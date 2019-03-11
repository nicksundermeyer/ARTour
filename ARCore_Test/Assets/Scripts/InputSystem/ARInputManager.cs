using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

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
	private ARInteractable currentObject;

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
		// DebugWithoutRepeats("TouchDown" + pos);

		// raycast to check object being touched
		if(Raycast (pos))
		{
			// create event data and send to object
			PointerEventData pData = new PointerEventData(EventSystem.current);
			pData.position = pos;
			currentObject.TouchDown(pData);
		}
	}

	public void TouchUp (Vector2 pos) {
		// DebugWithoutRepeats ("TouchUp" + pos);
	}

	public void TouchClick (Vector2 pos) {
		// DebugWithoutRepeats ("TouchClick" + pos);
	}

	public bool Raycast (Vector2 pos) {
		Ray ray = Camera.main.ScreenPointToRay (pos);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, float.MaxValue)) {
			ARInteractable comp = hit.collider.gameObject.GetComponentInParent<ARInteractable> ();

			// setting current controlled object if hit
			if (comp != null) {
				currentObject = comp;
				return true;
			}
		}
		return false;
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