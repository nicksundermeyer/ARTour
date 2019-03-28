//-----------------------------------------------------------------------
// <copyright file="AugmentedImageExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleARCore;
using GoogleARCore.Examples.AugmentedImage;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controller for AugmentedImage example.
/// </summary>
public class ThesisController : MonoBehaviour {

            public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;


    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject DetectedPlanePrefab;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject AndyPlanePrefab;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a feature point.
    /// </summary>
    public GameObject AndyPointPrefab;

    /// <summary>
    /// A game object parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;

    /// <summary>
    /// The rotation in degrees need to apply to model when the Andy model is placed.
    /// </summary>
    private const float k_ModelRotation = 180.0f;

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane> ();

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    // Creating custom serializable version of dictionary
    [Serializable]
    public class PrefabDictionary : SerializableDictionary<string, ARInteractable> { }

    // Dictionary of strings (image names from database) with prefabs
    [SerializeField]
    public PrefabDictionary Prefabs;

    private ARInteractable currentObj;

    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    public GameObject FitToScanOverlay;

    private Dictionary<int, ARInteractable> m_Visualizers = new Dictionary<int, ARInteractable> ();
    // private Dictionary<int, AugmentedImageVisualizer> m_Visualizers = new Dictionary<int, AugmentedImageVisualizer> ();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage> ();

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update () {
        _UpdateApplicationLifecycle ();

        // Check that motion tracking is tracking.
        if (Session.Status != SessionStatus.Tracking) {
            return;
        }

        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage> (m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // create objects on each augmented image
        foreach (var image in m_TempAugmentedImages) {
            ARInteractable visualizer = null;
            m_Visualizers.TryGetValue (image.DatabaseIndex, out visualizer);
            if (image.TrackingState == TrackingState.Tracking && visualizer == null) {
                // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                Anchor anchor = image.CreateAnchor (image.CenterPose);
                Prefabs.TryGetValue (image.Name, out currentObj);
                visualizer = (ARInteractable) Instantiate (currentObj, anchor.transform);
                visualizer.transform.rotation = Quaternion.Euler(0, 0, 0);
                visualizer.Image = image;
                m_Visualizers.Add (image.DatabaseIndex, visualizer);
            } else if (image.TrackingState == TrackingState.Stopped && visualizer != null) {
                m_Visualizers.Remove (image.DatabaseIndex);
                GameObject.Destroy (visualizer.gameObject);
            }
        }

        // Show the fit-to-scan overlay if there are no images that are Tracking.
        foreach (var visualizer in m_Visualizers.Values) {
            if (visualizer.Image.TrackingState == TrackingState.Tracking) {
                FitToScanOverlay.SetActive (false);
                return;
            }
        }

        FitToScanOverlay.SetActive (true);
    }
    private void _UpdateApplicationLifecycle () {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey (KeyCode.Escape)) {
            Application.Quit ();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking) {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        } else {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting) {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            _ShowAndroidToastMessage ("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke ("_DoQuit", 0.5f);
        } else if (Session.Status.IsError ()) {
            _ShowAndroidToastMessage ("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke ("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit () {
        Application.Quit ();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage (string message) {
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");

        if (unityActivity != null) {
            AndroidJavaClass toastClass = new AndroidJavaClass ("android.widget.Toast");
            unityActivity.Call ("runOnUiThread", new AndroidJavaRunnable (() => {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject> ("makeText", unityActivity,
                    message, 0);
                toastObject.Call ("show");
            }));
        }
    }
}