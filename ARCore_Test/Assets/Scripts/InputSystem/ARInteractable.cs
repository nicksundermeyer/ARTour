using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public AugmentedImage Image;

    public virtual void OnPointerClick (PointerEventData eventData) { }

    public virtual void OnPointerUp (PointerEventData eventData) { }

    public virtual void OnPointerDown (PointerEventData eventData) { }
    
    public virtual void OnBeginDrag (PointerEventData eventData) { }

    public virtual void OnEndDrag (PointerEventData eventData) { }

    public virtual void OnDrag (PointerEventData eventData) { }
}