using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Dungeon;
using UnityEngine;

namespace Grawly {
	
	/// <summary>
	/// This is primarily used to register flags that aren't necessarily part of the GameVariables,
	/// but still need to be available in a centralized location.
	/// </summary>
	[RequireComponent(typeof(GameController))]
	public class GlobalFlagController : MonoBehaviour {	
		
		public static GlobalFlagController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// A dictionary containing the actual values of the global flags and their current values.
		/// </summary>
		private Dictionary<GlobalFlagType, bool> globalFlagDict = new Dictionary<GlobalFlagType, bool>();
		/// <summary>
		/// The current "override" set for the camera that should be used when a scene is loaded.
		/// This is usually used with the CameraDirector, but I need a centralized place for flags like this.
		/// </summary>
		private CameraTagType cameraOverride = CameraTagType.None;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
			}
		}
		#endregion

		#region GETTERS - FLAGS
		/// <summary>
		/// Gets the value associated with the specified global flag type.
		/// </summary>
		/// <param name="flagType">The type of flag to check.</param>
		/// <returns>The state of the flag.</returns>
		public bool GetFlag(GlobalFlagType flagType) {
			// Check if an entry actually exists. If not, set it to false.
			if (this.globalFlagDict.ContainsKey(flagType) == false) {
				this.SetFlag(flagType: flagType, value: false);
			}

			// Return it.
			return this.globalFlagDict[flagType];
		}
		#endregion

		#region GETTERS - CAMERA
		/// <summary>
		/// Returns the current camera "override" tag and resets it back to type of None.
		/// </summary>
		/// <returns>The current CameraTagType to be used when a scene loads.</returns>
		public CameraTagType PopCameraOverride() {
			Debug.Log("Popping the camera override: " + this.cameraOverride);
			CameraTagType currentCameraOverride = this.cameraOverride;
			this.cameraOverride = CameraTagType.None;
			return currentCameraOverride;
		}
		#endregion
		
		#region SETTERS - FLAGS
		/// <summary>
		/// Sets the flag of the specified type to the provided value.
		/// </summary>
		/// <param name="flagType">The type of flag to set.</param>
		/// <param name="value">The value to set for the flag.</param>
		public void SetFlag(GlobalFlagType flagType, bool value) {
			
			Debug.Log("Setting global flag of type " + flagType.ToString() + " to " + value);
			
			// Check if an entry actually exists. If not, add it.
			if (this.globalFlagDict.ContainsKey(flagType) == false) {
				this.globalFlagDict.Add(key: flagType, value: value);
			} else {
				// If it does actually exist, just set the new value.
				this.globalFlagDict[flagType] = value;
			}
		}
		/// <summary>
		/// Sets the tag on the camera override so that when a CameraDirector is next used,
		/// it uses said tag to activate the associated camera.
		/// </summary>
		/// <param name="cameraTagType">The tag to set for the override.</param>
		public void SetCameraOverride(CameraTagType cameraTagType) {
			Debug.Log("Setting Camera Override to " + cameraTagType.ToString());
			this.cameraOverride = cameraTagType;
		}
		#endregion
		
	}
	
}