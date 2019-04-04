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

	bool dragStarted = false;
	public float minDragDistance;

	// Update is called once per frame
	void Update () {
		int touchCount = Input.touchCount;

		if (touchCount == 0) {
			if (dragStarted) {
				TouchDragEnd (_lastTouch.position);
				dragStarted = false;
			}

			if (_lastCount == 1) {
				TouchUp (_lastTouch.position);

				if (Time.time - _touchStartTime < _clickThreshold) {
					TouchClick (_lastTouch.position);
				}
			}

			_lastCount = 0;
		}
		else if (touchCount == 1) {
			if (_lastCount == 1) {
				if (dragStarted) {
					TouchDrag (_lastTouch.position);
				} else {
					float dragDist = (_lastTouch.position - Input.touches[0].position).magnitude;
					if (dragDist > minDragDistance) {
						TouchDragStart (_lastTouch.position);
						dragStarted = true;
					}
				}
			} else if (_lastCount == 0) {
				_touchStartTime = Time.time;
				TouchDown (_lastTouch.position);
			}

			_lastTouch = Input.touches[0];
			_lastCount = 1;
		}
	}

	public void TouchDown (Vector2 pos) {

		if (RaycastGUI<IPointerDownHandler> (pos)) {
			return;
		}
		PointerEventData pData = Raycast (pos);
		if (pData != null) {
			currentObject = pData.selectedObject;
			currentObject.GetComponentInParent<ARInteractable> ().OnPointerDown (pData);
			// DebugWithoutRepeats ("TouchDown: " + currentObject.name);
		}
	}

	public void TouchUp (Vector2 pos) {
		// DebugWithoutRepeats ("TouchUp: " + currentObject.name);

		if (RaycastGUI<IPointerUpHandler> (pos)) {
			return;
		}
		PointerEventData pData = Raycast (pos);
		if (pData != null && currentObject != null) {
			currentObject.GetComponentInParent<ARInteractable> ().OnPointerUp (pData);
			currentObject = null;
		}
	}

	public void TouchClick (Vector2 pos) {
		// DebugWithoutRepeats ("TouchClick: " + currentObject.name);

		if (RaycastGUI<IPointerClickHandler> (pos)) {
			return;
		}
		PointerEventData pData = Raycast (pos);
		if (pData != null) {
			currentObject = pData.selectedObject;
			currentObject.GetComponentInParent<ARInteractable> ().OnPointerClick (pData);
		}
	}

	public void TouchDragStart (Vector2 pos) {
		if(RaycastGUI<IBeginDragHandler>(pos)) {
			return;
		}
		PointerEventData pData = Raycast (pos);
		if (pData != null && currentObject != null) {
			currentObject = pData.selectedObject;
			currentObject.GetComponentInParent<ARInteractable> ().OnBeginDrag (pData);
			// DebugWithoutRepeats ("DragStart: " + currentObject.name);
		}
	}

	public void TouchDrag (Vector2 pos) {
		if(RaycastGUI<IDragHandler>(pos)) {
			return;
		}
		PointerEventData pData = new PointerEventData (EventSystem.current);
		pData.position = pos;

		if(currentObject != null)
		{
			currentObject.GetComponentInParent<ARInteractable> ().OnDrag (pData);
		}
	}

	public void TouchDragEnd (Vector2 pos) {
		if(RaycastGUI<IEndDragHandler>(pos)) {
			return;
		}
		PointerEventData pData = new PointerEventData (EventSystem.current);
		pData.position = pos;

		if(currentObject != null)
		{
			currentObject.GetComponentInParent<ARInteractable> ().OnEndDrag (pData);
			currentObject = null;
		}
	}

	public bool RaycastGUI<T> (Vector2 pos) where T : IEventSystemHandler {
		// Debug.Log("RaycastGUI");
		// raycast to GUI first
		PointerEventData pointerData = new PointerEventData (EventSystem.current);
		pointerData.position = pos;

		List<RaycastResult> results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (pointerData, results);

		if (results.Count > 0) {
			foreach (RaycastResult result in results) {
				if (ExecuteEvents.CanHandleEvent<T> (result.gameObject)) {
					if (typeof (T) == typeof (IPointerClickHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
					} else if (typeof (T) == typeof (IPointerDownHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
					} else if (typeof (T) == typeof (IPointerUpHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
					} else if (typeof (T) == typeof (IBeginDragHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.beginDragHandler);
					} else if (typeof (T) == typeof (IDragHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.dragHandler);
					} else if (typeof (T) == typeof (IEndDragHandler)) {
						ExecuteEvents.Execute (result.gameObject, pointerData, ExecuteEvents.endDragHandler);
					}
					return true;
				}
			}
		}

		return false;
	}

	public PointerEventData Raycast (Vector2 pos) {
		// then raycast for objects
		Ray ray = Camera.main.ScreenPointToRay (pos);
		Debug.DrawRay(ray.origin, ray.direction, Color.green, 5.0f);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, float.MaxValue)) {
			GameObject comp = hit.collider.gameObject;

			// setting current controlled object if hit
			if (comp.GetComponentInParent<ARInteractable> () != null) {
				PointerEventData pData = new PointerEventData (EventSystem.current);
				pData.position = pos;
				pData.selectedObject = comp;
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