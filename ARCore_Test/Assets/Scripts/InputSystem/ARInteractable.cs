using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GoogleARCore;

public class ARInteractable : MonoBehaviour {

    public AugmentedImage Image;

	public virtual void TouchDown (PointerEventData eventData) { }

    public virtual void TouchUp (PointerEventData eventData) { }

    public virtual void TouchClick (PointerEventData eventData) { }
}
