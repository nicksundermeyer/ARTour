using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoInstallation : ARInteractable {

	public new AugmentedImage Image;

	public VideoPlayer vp;
	public GameObject plane;
	public float width;

	[Header ("Controls")]
	public float skipTime;
	public Button playButton;
	public Button pauseButton;
	public Button nextButton;
	public Button previousButton;
	public Slider progressBar;

	// button click boolean events
	private bool playClicked = false;
	private bool pauseClicked = false;
	private bool nextClicked = false;
	private bool previousClicked = false;

	private void Awake () {
		float aspectRatio = (float) vp.clip.width / (float) vp.clip.height;

		Vector3 scale = plane.transform.localScale;
		scale.z = scale.x / aspectRatio;

		plane.transform.localScale = scale;
	}

	private void Update () {
		progressBar.value = (float) vp.frame / (float) vp.frameCount;
	}

	public void PlayButton () {
		if (!playClicked) {
			vp.Play ();
			playButton.gameObject.SetActive (false);
			pauseButton.gameObject.SetActive (true);
		}
		playClicked = !playClicked;
	}

	public void PauseButton () {
		if (!pauseClicked) {
			vp.Pause ();
			playButton.gameObject.SetActive (true);
			pauseButton.gameObject.SetActive (false);
		}
		pauseClicked = !pauseClicked;
	}

	public void NextButton () {
		if (!nextClicked)
			vp.time += skipTime;
		nextClicked = !nextClicked;
	}

	public void PreviousButton () {
		if (!previousClicked)
			vp.time -= skipTime;
		previousClicked = !previousClicked;
	}
}