using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Grawly.Toggles;
using Grawly.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Grawly.Toggles.Display;
using UnityEngine.SceneManagement;
using System.Linq;
using Grawly.Console;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using Sirenix.OdinInspector;

namespace Grawly {
	
	/// <summary>
	/// The script that drives the credits scene.
	/// </summary>
	public class CreditsController : MonoBehaviour {

		
		//
		//
		//	The code below is copy/pasted from the disclaimer screen
		//	because I just want to prototype it for right now.
		//
		//
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name of the scene that should be loaded after the disclaimer is done playing.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField, Title("General")]
		private string nextSceneName = "Initialization";
		#endregion
		
		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to wait before displaying the disclaimer message.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField, Title("Timing")]
		private float disclaimerStartDelayTime = 2f;
		/// <summary>
		/// The amount of time to fade the disclaimer message in.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField]
		private float disclaimerFadeInTime = 1f;
		/// <summary>
		/// The amount of time that the disclaimer message should be displayed on screen.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField]
		private float disclaimerDisplayTime = 5f;
		/// <summary>
		/// The amount of time to fade the disclaimer message out.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField]
		private float disclaimerFadeOutTime = 1f;
		/// <summary>
		/// The amount of time to wait after fading the disclaimer message out.
		/// </summary>
		[TabGroup("Disclaimer", "Toggles"), SerializeField]
		private float disclaimerFinishUpTime = 2f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image that should fade in/out to show the disclaimer.
		/// </summary>
		[TabGroup("Disclaimer", "Scene References"), SerializeField]
		private Image disclaimerFadeImage;
		#endregion

		#region UNITY CALLS
		private IEnumerator Start() {
			
			// Wait for the specified delay time.
			yield return new WaitForSeconds(this.disclaimerStartDelayTime);
			
			// Fade the disclaimer in.
			this.disclaimerFadeImage.DOColor(
				endValue: Color.clear,
				duration: this.disclaimerFadeInTime);
			yield return new WaitForSeconds(this.disclaimerFadeInTime);
			
			// Wait a moment so it can be read by the player.
			yield return new WaitForSeconds(this.disclaimerDisplayTime);

			// Fade the disclaimer out.
			this.disclaimerFadeImage.DOColor(
				endValue: Color.black,
				duration: this.disclaimerFadeOutTime);
			
			// Fade out, and stop the music while doing so.
			yield return new WaitForSeconds(this.disclaimerFadeOutTime);
			AudioController.instance?.StopMusic(track: 0, fade: 0.5f);
			
			// Wait a brief moment before loading into the next scene.
			yield return new WaitForSeconds(this.disclaimerFinishUpTime);			
			
			// Load up the next scene.
			SceneManager.LoadScene(sceneName: this.nextSceneName);

		}
		#endregion
		
	}
	
	
}