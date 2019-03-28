using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaryReed : ARInteractable {

	public new AugmentedImage Image;

	// variables for image carousel
	public Image imageCarousel;
	public Sprite[] sprites;
	private int currentImage = 0;

	public void OnNextClicked () {
		Debug.Log("Next Clicked");
		currentImage++;
		if(currentImage >= sprites.Length)
		{
			currentImage = 0;
		}

		imageCarousel.sprite = sprites[currentImage];
	}

	public void OnPreviousClicked () {
		Debug.Log("Previous Clicked");
		currentImage--;
		if(currentImage < 0)
		{
			currentImage = sprites.Length-1;
		}

		imageCarousel.sprite = sprites[currentImage];
	}
}