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

	private void Awake() {
		float aspectRatio = (float) vp.clip.width / (float) vp.clip.height;

        Vector3 scale = plane.transform.localScale;
		scale.z = scale.x / aspectRatio;

        plane.transform.localScale = scale;
	}
}