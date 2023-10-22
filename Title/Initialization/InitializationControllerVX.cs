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
	
	public class InitializationControllerVX : MonoBehaviour {

		public static InitializationControllerVX Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// Was the SRDebugger setup?
		/// </summary>
		private static bool SRDebuggerPrepared = false;
		/// <summary>
		/// This one is a little tricky but it's for use with the delegate
		/// that controls whether the player should be freed upon the console closing.
		/// </summary>
		private static bool playerWasFree = false;
		/// <summary>
		/// The game object that was selected last before the menu closed.
		/// </summary>
		private static GameObject lastSelectedGameObject;
		/// <summary>
		/// The index to use for the intro sequence and title screen music.
		/// </summary>
		public static int IntroMediaIndex { get; private set; } = 0;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// A list of scenes to use randomly for the intro.
		/// </summary>
		[SerializeField]
		private List<string> introSceneNames = new List<string>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private IEnumerator Start() {

			// Reset the state variables set above.
			playerWasFree = false;
			lastSelectedGameObject = null;
			
			// If the debugger was not set up...
			if (SRDebuggerPrepared == false && Application.isEditor == false) {
				// ...prepare it.
				this.PrepareDebugConsole();
				// Make sure to set the flag as true so this routine doesn't run again.
				SRDebuggerPrepared = true;
			}

			// Check whether the game toggles set has been initialized. If not, this is the first boot of the game.
			// This check needs to happen BEFORE preparation, since file creation will happen there regardless.
			bool isFirstBoot = SaveController.GameToggleSetHasInitialized == false;
			
			// Prepare the save controller so it's ready to provide access.
			SaveController.PrepareSaveController();
			
			// Just wanna give some delay.
			yield return new WaitForSeconds(1f);
			
			// Make sure to load up the settings!
			yield return SceneManager.LoadSceneAsync(sceneName: "Chat Controller DX", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "SettingsMenuDX", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "BottomInfoBar", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "SocialLinkUI", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "Shuffle Time", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "CombatantAnalysisDX", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "BattleResultsControllerDX", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "Badge Grid Screen", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "AllOutAttackDX", mode: LoadSceneMode.Additive);
			yield return SceneManager.LoadSceneAsync(sceneName: "PrototypeShop", mode: LoadSceneMode.Additive);
			
			// Process the boot toggles. I will probably always want to do this on initialzation.
			ToggleController.ProcessBootToggles();
			
			// Using the check from a few lines up, branch based on if this is the first boot of the game.
			// This way, the player can adjust settings before going to the intro.
			if (isFirstBoot == true) {
				// If this is the first boot, load the FirstBootSettingsController scene.
				SceneManager.LoadScene(sceneName: "FirstBootSettings");
			} else {
				// If this is not the first boot,
				// Generate a number thats within the range of available intro scenes and set the static variable.
				InitializationControllerVX.IntroMediaIndex = this.introSceneNames.RandomIndex();
				string introScene = this.introSceneNames[InitializationControllerVX.IntroMediaIndex];
				SceneManager.LoadScene(sceneName: introScene);
			}
			
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// The routine for prepping the debug console.
		/// </summary>
		private void PrepareDebugConsole() {
		
			// Assert that the debugger has not been prepared yet.
			Debug.Assert(SRDebuggerPrepared == false);
			
			// Initialize it.
			SRDebug.Init();
			
			// Set a delegate to run when the panel is opened/closed.
			SRDebug.Instance.PanelVisibilityChanged += delegate(bool visible) {

				if (visible == true) {

					// Determine if the player was free when the console was brought up and remember it.
					if (DungeonPlayer.Instance != null) {
						playerWasFree = DungeonPlayer.Instance.GetFSMState() == DungeonPlayerStateType.Free;
						DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
					} else if (CrawlerPlayer.Instance != null) {
						playerWasFree = CrawlerPlayer.Instance.PlayerState == CrawlerPlayerState.Free;
						CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
					} else {
						playerWasFree = false;
					}
					
					// Do some quick tweaks.
					FindObjectOfType<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = true;
					lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
					EventSystem.current.SetSelectedGameObject(null);

					
				} else {
					
					// Revert the state.
					EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
					lastSelectedGameObject = null;
					FindObjectOfType<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = false;

					// If the player was meant to be free, free em up.
					if (playerWasFree == true) {
						DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Free);
						CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Free);
					}
					
				}
				
			};
		}
		#endregion
		
	}

	
}