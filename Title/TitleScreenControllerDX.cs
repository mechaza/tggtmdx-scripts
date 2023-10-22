using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Grawly.UI;
using Grawly.UI.Legacy;

namespace Grawly.Title {
	
	/// <summary>
	/// Controls the title screen.
	/// </summary>
	public class TitleScreenControllerDX : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when fading elements.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private float fadeTime = 1f;
		#endregion
		
		#region FIELDS - RESOURCES
		/// <summary>
		/// A list of potential tracks to use for the intro.
		/// One will get picked based on the index generated in the initialization controller earlier.
		/// This is to help match the intro song with the title song.
		/// </summary>
		[Title("Resources")]
		[SerializeField]
		private List<IntroloopAudio> titleScreenMusicList = new List<IntroloopAudio>();
		/*/// <summary>
		/// The music to use for the intro.
		/// </summary>
		[Title("Resources")]
		[SerializeField]
		private IntroloopAudio titleMusic;*/
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The list of options that can be picked from upon launch.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private List<GameObject> menuSelections = new List<GameObject>();
		/// <summary>
		/// The actual game object showing the prompt text.
		/// </summary>
		[SerializeField]
		private GameObject promptTextObject;
		/// <summary>
		/// The image for the game's logo.
		/// </summary>
		[SerializeField]
		private CanvasGroup logoGroup;
		/// <summary>
		/// The group for the prompt text.
		/// </summary>
		[SerializeField]
		private CanvasGroup promptTextGroup;
		/// <summary>
		/// The canvas group for the menu selections.
		/// </summary>
		[SerializeField]
		private CanvasGroup menuGroup;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
		
		}
		private IEnumerator Start() {
			
			// Grab the title screen music to use and play it.
			int mediaIndex = InitializationControllerVX.IntroMediaIndex;
			bool indexIsValid = (mediaIndex >= 0) && (mediaIndex < this.titleScreenMusicList.Count);
			Debug.Assert(indexIsValid);
			if (indexIsValid == true) {
				IntroloopAudio musicToUse = this.titleScreenMusicList[InitializationControllerVX.IntroMediaIndex];
				AudioController.instance?.PlayMusic(track: 0, audio: musicToUse);
			} else {
				Debug.LogError("Index is not valid! Using random music. " + mediaIndex);
				IntroloopAudio musicToUse = this.titleScreenMusicList.Random();
				AudioController.instance?.PlayMusic(track: 0, audio: musicToUse);
			}
			
			// AudioController.instance?.PlayMusic(track: 0, audio: this.titleMusic);
			
			// Set the different groups to clear.
			this.logoGroup.alpha = 0f;
			this.menuGroup.alpha = 0f;
			this.promptTextGroup.alpha = 0f;
			this.menuGroup.interactable = false;
			this.promptTextGroup.interactable = false;
			this.logoGroup.interactable = false;
	
			// If the save collection has no data,
			if (SaveController.LoadCollection().HasAnySaveData == false) {
				// Turn off the first two objects.
				this.menuSelections[0].SetActive(false);
				this.menuSelections[1].SetActive(false);
			}
		
			// Wait.
			yield return new WaitForSeconds(1f);

			// Fade the logo in.
			this.logoGroup.DOFade(endValue: 1f, duration: this.fadeTime).SetEase(Ease.Linear);

			// Wait some more.
			yield return new WaitForSeconds(2f);
		
			// Fade the prompt text in.
			this.promptTextGroup.DOFade(endValue: 1f, duration: this.fadeTime).SetEase(Ease.Linear);

			this.promptTextGroup.interactable = true;
			EventSystem.current.SetSelectedGameObject(this.promptTextObject);
		}
		#endregion

		#region EVENTS - PROMPT
		public void OnTitlePromptHit() {
			this.promptTextGroup.interactable = false;
			this.promptTextObject.GetComponent<Selectable>().interactable = false;
			this.StartCoroutine(this.OnTitlePromptHitRoutine());
		}
		private IEnumerator OnTitlePromptHitRoutine() {
			
			// Play a selection sfx.
			AudioController.instance.PlaySFX(type: SFXType.Select);
			
			// Fade the graphics out.
			this.logoGroup.DOFade(endValue: 0f, duration: this.fadeTime).SetEase(Ease.Linear);
			this.promptTextGroup.DOFade(endValue: 0f, duration: this.fadeTime).SetEase(Ease.Linear);
			
			// Wait a moment.
			yield return new WaitForSeconds(this.fadeTime + 0.5f);

			// Fade the menu group in and enable it.
			this.menuGroup.DOFade(endValue: 1f, duration: this.fadeTime).SetEase(Ease.Linear);
			
			// Wait a little more.
			yield return new WaitForSeconds(this.fadeTime + 0.1f);
			
			// Enable the menu group and select the first available object.
			menuGroup.interactable = true;			
			GameObject firstItem = this.menuSelections.First(go => go.activeInHierarchy == true);
			EventSystem.current.SetSelectedGameObject(firstItem);
			
		}
		#endregion

		#region EVENTS - MENUS

		public void OnContinueButtonHit(BaseEventData eventData) {
			menuGroup.interactable = false;
			menuGroup.DOFade(endValue: 0f, duration: this.fadeTime).SetEase(Ease.Linear);
			EventSystem.current.SetSelectedGameObject(null);
			Flasher.FadeOut();
			GameController.Instance.WaitThenRun(1f, delegate {
				SaveController.LoadLastSave(incrementLoadCount: true, onSaveLoaded: delegate {
					Flasher.FadeIn();
				});
			});
		}
		public void OnLoadGameButtonHit(BaseEventData eventData) {
			
			GameObject selectedObject = eventData.selectedObject;
			
			
			// Disable the menu.
			this.menuGroup.interactable = false;
			menuGroup.DOFade(endValue: 0f, duration: this.fadeTime).SetEase(Ease.Linear);
			
			// Null out the current selected objet.
			EventSystem.current.SetSelectedGameObject(null);

			
			// Display the load menu.
			SaveScreenController.GlobalShow(SaveScreenType.Load, sourceOfCall: this.gameObject, onCancelCallback: delegate {
				// If the selection was cancelled, re-select this.
				this.menuGroup.interactable = true;
				menuGroup.DOFade(endValue: 1f, duration: this.fadeTime).SetEase(Ease.Linear);
				EventSystem.current.SetSelectedGameObject(selectedObject);
			});
			
		}
		public void OnNewGameButtonHit(BaseEventData eventData) {
			menuGroup.interactable = false;
			EventSystem.current.SetSelectedGameObject(null);
			AudioController.instance?.StopMusic(track: 0, fade: 1f);
			SceneController.instance.BasicLoadSceneWithFade(sceneName: "GamePresetLoader");
		}
		public void OnSettingsButtonHit(BaseEventData eventData) {
			
			// Save the button.
			GameObject selectedObject = eventData.selectedObject;
			// Disable the menu group.
			menuGroup.interactable = false;
			menuGroup.DOFade(endValue: 0f, duration: this.fadeTime).SetEase(Ease.Linear);
			
			SettingsMenuControllerDX.instance.Open(onExitCallback: delegate {
				menuGroup.interactable = true;
				menuGroup.DOFade(endValue: 1f, duration: this.fadeTime).SetEase(Ease.Linear);
				EventSystem.current.SetSelectedGameObject(selectedObject);
			});
			
		}
		public void OnExitButtonHit(BaseEventData eventData) {

			GameObject selectedObject = eventData.selectedObject;
			
			menuGroup.interactable = false;
			
			OptionPicker.instance.Display(
				prompt: "Exit the game?",
				option1: "Yes",
				option2: "No",
				callback1: delegate {
					Application.Quit();
				},
				callback2: delegate {
					menuGroup.interactable = true;
					EventSystem.current.SetSelectedGameObject(selectedObject);
				}, 
				reselectOnDone: false);
		}
		public void OnControlMapButtonHit(BaseEventData eventData) {
			menuGroup.interactable = false;
			EventSystem.current.SetSelectedGameObject(null);
			AudioController.instance?.StopMusic(track: 0, fade: 1f);
			SceneController.instance.BasicLoadSceneWithFade(sceneName: "ControlMapperDX");
		}
		#endregion
		
		
	}

	
}