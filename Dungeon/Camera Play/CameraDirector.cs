using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Dungeon;
using Grawly.Toggles;
using CameraTransitions;
using Grawly.Battle;
using Cinemachine;

namespace Grawly {

	/// <summary>
	/// A specific class to help with managing cameras that are available for cutscenes.
	/// </summary>
	[RequireComponent(typeof(CameraTransitions.CameraTransition))]
	public class CameraDirector : SerializedMonoBehaviour {

		public static CameraDirector Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The current camera in use.
		/// None assumes I'm not in a cutscene and using whatever cameras I normally do.
		/// </summary>
		private CameraTagType CurrentCameraType { get; set; } = CameraTagType.None;
		/// <summary>
		/// The flag that determines which camera brain is active.
		/// </summary>
		private bool cameraSwap = false;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The CameraTransition component that manages... camera transitions.
		/// </summary>
		private CameraTransition CameraTransition { get; set; }
		/// <summary>
		/// The first camera. This will usually be used by default unless switched to cameraBrain2 via a transition.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private Camera cameraOne;
		/// <summary>
		/// The second camera. This is usually used when in transitions.
		/// </summary>
		[SerializeField]
		private Camera cameraTwo;
		/// <summary>
		/// A dictionary of the different virtual cameras in the scene.
		/// </summary>
		[SerializeField]
		private Dictionary<CameraTagType, GameObject> virtualCameras = new Dictionary<CameraTagType, GameObject>();
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The camera that is currently active.
		/// </summary>
		private Camera MainCamera {
			get {
				if (cameraSwap == false) {
					return cameraOne;
				} else {
					return cameraTwo;
				}
			}
		}
		/// <summary>
		/// The "Secondary" camera (i.e., the one that is turned off)
		/// </summary>
		public Camera SecondaryCamera {
			get {
				if (cameraSwap == true) {
					return cameraOne;
				} else {
					return cameraTwo;
				}
			}
		}
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance != null) {
				throw new Exception("The CameraDirector should never be not-null when Awake() is called! Is there more than one in the scene?");
			}
			
			Instance = this;
		
			// Grab a reference to the camera transitions.
			this.CameraTransition = this.GetComponent<CameraTransition>();
			
			// This should not have a key of None.
			Debug.Assert(this.virtualCameras.ContainsKey(CameraTagType.None) == false);
			
			// Pop the camera from the flag controller and swap over to it.
			this.SwapCamera(cameraType: GlobalFlagController.Instance.PopCameraOverride());
			
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Uses the camera asssociated with the given tag type and disables the one in use.
		/// </summary>
		/// <param name="cameraType">The tag of the camera to use.</param>
		public void SwapCamera(CameraTagType cameraType) {
			
			// Note that usually NONE will not have an entry,
			// which is why I do not allow for it to be enabled.

			// If it's the same type, return.
			if (cameraType == this.CurrentCameraType) {
				return;
			}
			
			Debug.Log("Swapping to camera: " + cameraType + " from " + this.CurrentCameraType);
			
			if (cameraType != CameraTagType.None) {
				// Set the desired camera's GameObject on.
				this.virtualCameras[cameraType].SetActive(true);
			}
			
			if (this.CurrentCameraType != CameraTagType.None) {
				// Turn off the one that is currently enabled.
				this.virtualCameras[this.CurrentCameraType].SetActive(false);
			}

			// Save the tag of the camera that was just set.
			this.CurrentCameraType = cameraType;
		}
		/// <summary>
		/// Uses the camera asssociated with the given tag type and disables the one in use.
		/// Also performs a transition effect.
		/// </summary>
		/// <param name="cameraType">The type of camera to transition to.</param>
		/// <param name="transitionEffect">The effect to use when transitioning.</param>
		/// <param name="time">The duration of the transition effect.</param>
		public void SwapCamera(CameraTagType cameraType, CameraTransitionEffects transitionEffect, float time = 0.2f) {
			
			// Assert that the main camera is, in fact, active.
			Debug.Assert(this.MainCamera.gameObject.activeInHierarchy == true);
			Debug.Assert(this.SecondaryCamera.gameObject.activeInHierarchy == false);
			
			// Save the clear flags/culling mask
			CameraClearFlags clearFlags = MainCamera.clearFlags;
			int cullingMask = MainCamera.cullingMask;
			// Give the main camera the secondary camera's info
			MainCamera.clearFlags = SecondaryCamera.clearFlags;
			MainCamera.cullingMask = SecondaryCamera.cullingMask;

			// Reverse the enabled status of the listener.
			MainCamera.GetComponent<AudioListener>().enabled = false;
			SecondaryCamera.GetComponent<AudioListener>().enabled = true;

			// Fix up the secondary camera
			SecondaryCamera.clearFlags = clearFlags;
			SecondaryCamera.cullingMask = cullingMask;

			// Swap the brains. Swap the brains!
			// (Do this because I need to make sure the camera being swapped doesn't follow a target virtual camera.)
			MainCamera.GetComponent<CinemachineBrain>().enabled = false;
			SecondaryCamera.GetComponent<CinemachineBrain>().enabled = true;
			
			this.cameraSwap = !this.cameraSwap;
			this.CameraTransition.DoTransition(
				transition: transitionEffect,
				from: this.SecondaryCamera,
				to: this.MainCamera,
				time: time);
			
			this.SwapCamera(cameraType: cameraType);
			
		}
		/// <summary>
		/// Turns off custom cameras for cutscenes.
		/// </summary>
		public void DismissCamera() {

			// If the current camera type is none, there is no entry to turn off.
			if (this.CurrentCameraType == CameraTagType.None) {
				return;
			}
			
			Debug.Log("Turning off custom cameras.");
			// Access the camera currently in use and turn it off.
			this.virtualCameras[this.CurrentCameraType].SetActive(false);
			// Zero out the current type.
			this.CurrentCameraType = CameraTagType.None;
		}
		#endregion
		
	}


}