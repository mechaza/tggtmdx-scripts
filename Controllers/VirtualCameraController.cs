using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly {

	/// <summary>
	/// I use this to help me keep track of what virtual cameras in the scene I need to be able to cut between and reference them by name.
	/// </summary>
	public class VirtualCameraController : SerializedMonoBehaviour {

		public static VirtualCameraController instance;

		#region FIELDS - STATE
		/// <summary>
		/// The name of the camera that is currently active.
		/// </summary>
		private string currentCameraName = "";
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains references to the GameObjects in the scene that should be activated via strings.
		/// </summary>
		[SerializeField]
		private Dictionary<string, GameObject> cameraDict = new Dictionary<string, GameObject>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Enables the camera of the specified name.
		/// </summary>
		/// <param name="cameraName">The name of the camera to enable.</param>
		public void EnableCamera(string cameraName) {
			// Disable the previous camera if it exists.
			if (this.cameraDict.ContainsKey(key: this.currentCameraName)) {
				this.cameraDict[this.currentCameraName].SetActive(false);
			}
			// Turn on the specified camera and save it.
			this.cameraDict[cameraName].SetActive(true);
			this.currentCameraName = cameraName;
		}
		/// <summary>
		/// Disables any custom camera currently in use.
		/// </summary>
		public void DisableCamera() {
			if (this.cameraDict.ContainsKey(this.currentCameraName)) {
				this.cameraDict[this.currentCameraName].SetActive(false);
			}
			this.currentCameraName = "";
		}
		#endregion

	}


}