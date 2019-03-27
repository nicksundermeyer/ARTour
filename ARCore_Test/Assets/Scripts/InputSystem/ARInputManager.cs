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
		if(RaycastGUI<IPointerDownHandler>(pos))
		{
			return;
		}
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchDown" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerDown (pData);
		}
	}

	public void TouchUp (Vector2 pos) {
		if(RaycastGUI<IPointerUpHandler>(pos))
		{
			return;
		}
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchUp" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerUp (pData);
			currentObject = null;
		}
	}

	public void TouchClick (Vector2 pos) {
		if(RaycastGUI<IPointerClickHandler>(pos))
		{
			return;
		}
		PointerEventData pData = Raycast(pos);
		if (pData != null) {
			// DebugWithoutRepeats ("TouchClick" + pos);
			currentObject.GetComponentInParent<ARInteractable>().OnPointerClick (pData);
		}
	}

	public bool RaycastGUI<T>(Vector2 pos) where T : IEventSystemHandler {
		// raycast to GUI first
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = pos;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);

		if (results.Count > 0) {
			foreach(RaycastResult result in results)
			{
				if(ExecuteEvents.CanHandleEvent<T>(result.gameObject))
				{
					if(typeof(T) == typeof(IPointerClickHandler))
					{
						ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
					}
					else if(typeof(T) == typeof(IPointerDownHandler)) {
						ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
					}
					return true;
				}
			}
		}

		return false;
	}

	public PointerEventData Raycast(Vector2 pos) {
		// then raycast for objects
		Ray ray = Camera.main.ScreenPointToRay (pos);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, float.MaxValue)) {
			GameObject comp = hit.collider.gameObject;

			// setting current controlled object if hit
			if (comp.GetComponentInParent<ARInteractable>() != null) {
				PointerEventData pData = new PointerEventData(EventSystem.current);
				pData.position = pos;
				pData.selectedObject = comp;

				currentObject = comp;
				return pData;
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