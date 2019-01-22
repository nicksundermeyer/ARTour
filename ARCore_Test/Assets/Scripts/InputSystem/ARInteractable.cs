using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GoogleARCore;

public class ARInteractable : MonoBehaviour {

    public AugmentedImage Image;

	public virtual void OnPointerClick (PointerEventData eventData) { }

    public virtual void OnPointerUp (PointerEventData eventData) { }

    public virtual void OnPointerDown (PointerEventData eventData) { }
}
