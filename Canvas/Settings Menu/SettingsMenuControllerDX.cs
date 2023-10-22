using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.UI {

	/// <summary>
	/// Controls the settings menu and how to access it.
	/// </summary>
	public class SettingsMenuControllerDX : MonoBehaviour {

		public static SettingsMenuControllerDX instance;

		#region FIELDS - STATE
		/// <summary>
		/// The GameToggleSet that was provided to this controller.
		/// This is what is cloned.
		/// </summary>
		private GameToggleSetDX originalToggleSet;
		/// <summary>
		/// The GameToggleSet that was cloned.
		/// This is what gets worked with, so that I may discard changes as needed.
		/// </summary>
		private GameToggleSetDX clonedToggleSet;
		/// <summary>
		/// Were any changes made?
		/// </summary>
		private bool ChangesMade { get; set; } = false;
		/// <summary>
		/// The callback to run upon exiting.
		/// </summary>
		private Action onExitCallback;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The SettingsMenuListDX that is on the right hand side of the screen.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private SettingsMenuListDX settingsMenuList;
		/// <summary>
		/// Chat borders. I made them for the chat, but I imagine they'd work good here too!
		/// </summary>
		[SerializeField]
		private ChatBorders chatBorders;
		/// <summary>
		/// The category buttons. 
		/// When one of these is selected, it's toggles should be previewed in the settings list.
		/// </summary>
		[SerializeField]
		private List<SettingsMenuDXTopLevelButton> topLevelButtons = new List<SettingsMenuDXTopLevelButton>();
		/// <summary>
		/// The CanvasGroup that controls whether the top buttons can be selected or not.
		/// </summary>
		[SerializeField]
		private CanvasGroup topLevelButtonCanvasGroup;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
				this.topLevelButtons.Select(b => b.GetComponent<Selectable>()).ToList().ForEach(s => s.interactable = false);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// On start, totally reset the controller.
			this.ResetState();
		}
		#endregion

		#region GLOBAL ACCESS
		/// <summary>
		/// A point of entry that allows access to the settings from anywhere.
		/// Will automatically load in the setings scene if it is not present.
		/// </summary>
		public static void GlobalOpen() {
			if (instance == null) {
				SceneController.instance.AddScene("SettingsMenuDX", callback: delegate () {
					throw new System.Exception("I kind of don't want to do this because I'd prefer " +
						"if the settings menu were loaded at all times.Animations get weird otherwise.");
					instance.Open();
				});
			} else {
				instance.Open();
			}
		}
		#endregion

		#region OPENING/CLOSING
		/// <summary>
		/// The actual routine I use for opening the settings menu.
		/// This uses the standard open method but passes in a default toggle set.
		/// </summary>
		[Button, HideInEditorMode]
		public void Open() {
			// Call the normal function with an empty delegate.
			this.Open(onExitCallback: delegate {
				
			});
		}
		/// <summary>
		/// The actual routine I use for opening the settings menu.
		/// This uses the standard open method but passes in a default toggle set.
		/// </summary>
		public void Open(Action onExitCallback) {

			// Reset the state.
			this.ResetState();

			// Open the menu with the toggle set saved in the SaveController.
			// It will get cloned anyway so whatever.
			this.Open(gameToggleSet: SaveController.CurrentGameToggleSet, onExitCallback: onExitCallback);

		}

		/// <summary>
		/// The actual routine I use for opening the settings menu.
		/// </summary>
		/// <param name="gameToggleSet">The toggle set to present in the controller.</param>
		private void Open(GameToggleSetDX gameToggleSet) {
			// Call the open routine with the empty delegate.
			this.Open(gameToggleSet: gameToggleSet, onExitCallback: delegate {
				
			});
		}
		/// <summary>
		/// The actual routine I use for opening the settings menu.
		/// </summary>
		/// <param name="gameToggleSet">The toggle set to present in the controller.</param>
		private void Open(GameToggleSetDX gameToggleSet, Action onExitCallback) {

			// Save a reference to the toggle set provided and save a clone.
			this.originalToggleSet = gameToggleSet;
			this.clonedToggleSet = gameToggleSet.Clone();

			// Also save the exit callback.
			this.onExitCallback = onExitCallback;
			
			// Turn on the menu list. I do this outside of the animation for logic reasons.
			this.settingsMenuList.gameObject.SetActive(true);
			this.topLevelButtonCanvasGroup.interactable = true;
			
			// Build the settings menu list with the first button's category type.
			this.PreviewToggles(categoryType: this.topLevelButtons.First().CategoryType);

			// Play the opening animation.
			this.OpenAnimation(onCompleteCallback: delegate {
				// Turn on all of the selectables.
				// Pass the first button over to the event system for selection.
				EventSystem.current.SetSelectedGameObject(this.topLevelButtons.First().gameObject);
			});

		}
		/// <summary>
		/// Closes out the settings when things are done and set.
		/// ASSUME THAT THE SETTINGS MENU STATE CAN EXIST IN ANY WAY WHEN CALLING THIS.
		/// </summary>
		public void Close() {
			
			// I guess just toally turn them off.
			this.settingsMenuList.gameObject.SetActive(false);
			this.topLevelButtonCanvasGroup.interactable = false;
			// Null out the sets so that I don't have them anymore.
			this.originalToggleSet = null;
			this.clonedToggleSet = null;

			// Invoke the on exit callback and null it out.
			this.onExitCallback?.Invoke();
			this.onExitCallback = null;
			
			// Tell the pause menu controller we're done. If it is here.
			PauseMenuController.instance?.FSM.SendEvent("Back");
			
		}
		/// <summary>
		/// A version of close that should be used with caution.
		/// Mostly used with the ReturnToTitle toggle.
		/// </summary>
		public void ForceClose() {
			PauseMenuController.instance?.Close();
			// Play the clsoing animation.
			this.CloseAnimation(onCompleteCallback: delegate {
				// WHen its done, actually call close.
				this.settingsMenuList.gameObject.SetActive(false);
				this.originalToggleSet = null;
				this.clonedToggleSet = null;
				this.onExitCallback = null;
			});
		}
		#endregion

		#region MENU CONTROL - GENERAL
		/// <summary>
		/// Previews the toggles of the specified category in the list on the right of the screen.
		/// This happens when a category button on the left is highlighted.
		/// </summary>
		/// <param name="categoryType">The category of toggles to preview.</param>
		public void PreviewToggles(GameToggleCategoryType categoryType) {
			// Grab the toggles from the cloned set and pass them into the settings menu list.
			this.settingsMenuList.PrepareMenuList(
				allMenuables: this.clonedToggleSet.GetToggles(categoryType: categoryType).Cast<IMenuable>().ToList(), 
				startIndex: 0);
		}
		/// <summary>
		/// Gets called from a top level button when a category type is submitted.
		/// </summary>
		/// <param name="categoryType">The category that was picked by the user.</param>
		public void SubmittedToggleCategoryButton(GameToggleCategoryType categoryType) {
			// The list SHOULD be built by now anyway but build it again.
			this.settingsMenuList.PrepareMenuList(
				allMenuables: this.clonedToggleSet.GetToggles(categoryType: categoryType).Cast<IMenuable>().ToList(),
				startIndex: 0);
			// Select the first one.
			this.settingsMenuList.SelectFirstMenuListItem();

		}
		/// <summary>
		/// Cancelled from a menu list item and going to go back to a category button.
		/// </summary>
		/// <param name="categoryType">The category that needs to be reselected.</param>
		public void CancelledMenuListItem(GameToggleCategoryType categoryType) {
			// Reset the list again I fucking guess.
			this.settingsMenuList.PrepareMenuList(
				allMenuables: this.clonedToggleSet.GetToggles(categoryType: categoryType).Cast<IMenuable>().ToList(),
				startIndex: 0);
			// Tell the event system to re-select the button that we came from originally.
			EventSystem.current.SetSelectedGameObject(this.topLevelButtons.First(b => b.CategoryType == categoryType).gameObject);
		}
		/// <summary>
		/// Gets called when one of the top level buttons hits cancelled.
		/// </summary>
		public void TopLevelCancelled() {

			AudioController.instance?.PlaySFX(type: SFXType.Close);
			// Deselect the current top level option.
			GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
			EventSystem.current.SetSelectedGameObject(null);

			// Do a quick hack and just apply the changes.
			this.ApplyChangesButtonHit();
			
			/*// Pop open the option picker for confirmation.
			OptionPicker.Instance.Display(
				prompt: "Close without applying changes?",
				options: new List<string>() { "Yes", "No" },
				callback1: delegate {					
					// Play the clsoing animation.
					this.CloseAnimation(onCompleteCallback: delegate {
						// WHen its done, actually call close.
						this.Close();
					});
				},
				callback2: delegate {
					// If not actually closing, reselect the previous game object.
					EventSystem.current.SetSelectedGameObject(currentSelected);
				},
				reselectOnDone: false);*/

			
		}
		#endregion

		#region MENU CONTROL - SPECIFIC BUTTONS
		/// <summary>
		/// The method that should run when the restore defaults button is hit.
		/// </summary>
		public void RestoreDefaultsButtonHit() {
			// Use the option picker to prompt the user what they want to do.
			OptionPicker.instance.Display(
				prompt: "Revert settings to their default values?", 
				options: new List<string>() { "Yes","No" }, 
				callback1: delegate {
					// If reverting,
					Debug.Log("Reverting settings.");
					// Grab the default toggles and reassign them. Note that I don't overwrite the original.
					this.clonedToggleSet = this.clonedToggleSet.GetDefaultSet();
				}, 
				callback2: delegate {
					Debug.Log("Restore cancelled.");
				});
			// Debug.LogError("RESTORE DEFAULTS HIT BUT IM NOT DOING ANYTHING YET");
			// throw new System.NotImplementedException("maybe ask the option picker and if the player syas yes restore defaults if not dont");
		}
		/// <summary>
		/// The method that should run when the apply changes button is hit.
		/// </summary>
		public void ApplyChangesButtonHit() {
			// Write the configuration out to disk.
			SaveController.WriteConfig(gameToggleSet: this.clonedToggleSet);
			// Play the clsoing animation.
			this.CloseAnimation(onCompleteCallback: delegate {
				// WHen its done, actually call close.
				this.Close();
			});
		}
		#endregion

		#region STATE QUERIES
		/// <summary>
		/// Gets the number of toggles for the specified category type from the cloned set.
		/// </summary>
		/// <param name="categoryType">The category of toggles being queried.</param>
		/// <returns>How many toggles are in the specified category.</returns>
		public int GetToggleCount(GameToggleCategoryType categoryType) {
			return this.clonedToggleSet.GetToggles(categoryType: categoryType).Count;
			// return this.clonedToggleSet.GetToggleCount(categoryType: categoryType);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Totally resets the positions of all the top level buttons and the menu/chat borders.
		/// </summary>
		private void ResetState() {
			// Reset the changes flag.
			this.ChangesMade = false;
			// Reset the borders.
			this.chatBorders.ResetState();
			// Reset the settings menu list.
			this.settingsMenuList.ResetState();
			// Reset each top level button.
			this.topLevelButtons.ForEach(b => b.ResetState());
			// Remove the exit callback.
			this.onExitCallback = null;
		}
		/// <summary>
		/// The animation that plays when presenting the settings menu.
		/// </summary>
		/// <param name="onCompleteCallback">The callback to run when its complete.</param>
		private void OpenAnimation(TweenCallback onCompleteCallback) {

			// Present the chat borders.
			this.chatBorders.PresentBorders(borderParams: new ChatBorderParams());
			// Present the settings menu list.
			this.settingsMenuList.Present();

			// Present each button one at a time.
			Sequence seq = DOTween.Sequence();
			// Do this by going through each button and adding its present plus a split second delay.
			foreach (SettingsMenuDXTopLevelButton b in this.topLevelButtons) {
				seq.AppendCallback(delegate { b.Present(); });
				seq.AppendInterval(0.1f);
			}
			// Add the onCompleteCallback (which is presumably the selection of the first button.)
			seq.OnComplete(action: onCompleteCallback);
			// Play it out.
			seq.Play();
		}
		/// <summary>
		/// The animation that plays when closing out the settings menu.
		/// </summary>
		private void CloseAnimation(TweenCallback onCompleteCallback) {
			// Dismiss the other shit.
			this.chatBorders.DismissBorders(borderParams: new ChatBorderParams());
			this.settingsMenuList.Dismiss();

			// Present each button one at a time.
			Sequence seq = DOTween.Sequence();

			// Make a clone of the top level buttons and reverse their order.
			List<SettingsMenuDXTopLevelButton> topLevelButtonsInReverse = new List<SettingsMenuDXTopLevelButton>(this.topLevelButtons);
			topLevelButtonsInReverse.Reverse();

			// Do this by going through each button and adding its present plus a split second delay.
			foreach (SettingsMenuDXTopLevelButton b in topLevelButtonsInReverse) {
				seq.AppendCallback(delegate { b.Dismiss(); });
				seq.AppendInterval(0.1f);
			}
			// Add the onCompleteCallback (which is presumably the selection of the first button.)
			seq.OnComplete(action: onCompleteCallback);
			// Play it out.
			seq.Play();

		}
		#endregion

	}


}