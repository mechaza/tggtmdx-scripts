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

namespace Grawly {
	
	/// <summary>
	/// Controls access to the Settings menu upon first boot of the game.
	/// </summary>
	public class FirstBootSettingsController : MonoBehaviour {

		public static FirstBootSettingsController Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// The scene to load upon exiting the screen.
		/// </summary>
		[SerializeField]
		private string sceneToLoad = "";
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private IEnumerator Start() {
			
			// Wait for a brief moment.
			yield return new WaitForSeconds(1f);
			
			// Open the settings menu. Upon closing, a random intro should be loaded.
			SettingsMenuControllerDX.instance.Open(onExitCallback: () => {
				
				// Null out the currently selected game object so it cant be picked again.
				GameController.Instance.RunEndOfFrame(action: () => {
					EventSystem.current.SetSelectedGameObject(null);
				});
				
				// Wait a moment, then load the next scene.
				GameController.Instance.WaitThenRun(timeToWait: 1f, action: () => {
					SceneManager.LoadScene(sceneName: this.sceneToLoad);
				});
				
			});
			
			
			
		}
		#endregion
		
	}
}