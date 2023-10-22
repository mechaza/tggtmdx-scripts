using System.Collections;
using System.Collections.Generic;
using Grawly.Toggles.Proto;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Grawly {

	/// <summary>
	/// A class to immediately reset to the start of the game.
	/// </summary>
	public class HotReset : MonoBehaviour {

		private static HotReset instance;

		#region FIELDS - STATE
		/// <summary>
		/// The amount of time the reset hot key has been pressed.
		/// </summary>
		private float holdTimer = 0f;
		/// <summary>
		/// Has the hot reset been triggered?
		/// </summary>
		private bool triggered = false;
		#endregion
		
		#region FIELDS - TOGGLES
		[SerializeField]
		private float resetTriggerTime = 5f;
		#endregion

		private void Awake() {
			if (instance == null) {
				instance = this;
				
				// ResetController.AddToDontDestroy(this.gameObject);
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}

		private void Update() {
			
			// If it was already triggered, back out so i dont accidentally do it again.
			if (this.triggered == true) {
				return;
			}
			
			// If the key is being held,
			if (Input.GetKey(key: KeyCode.Escape)) {
				// Increment the timer.
				this.holdTimer += Time.deltaTime;
			} else {
				// otherwise, reset it.
				this.holdTimer = 0f;
			}

			if (this.holdTimer > this.resetTriggerTime) {
				EventSystem.current.SetSelectedGameObject(null);
				AudioController.instance?.StopMusic(track: 0, fade: 0.5f);
				AudioController.instance?.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance?.StopMusic(track: 2, fade: 0.5f);
				this.triggered = true;
				SceneController.instance.BasicLoadSceneWithFade("TotalReset");
			}
		}

	}


}