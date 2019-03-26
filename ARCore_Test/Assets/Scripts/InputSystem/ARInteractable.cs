using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

    public AugmentedImage Image;

    public virtual void OnPointerClick (PointerEventData eventData) { }

    public virtual void OnPointerUp (PointerEventData eventData) { }

    public virtual void OnPointerDown (PointerEventData eventData) { }
}