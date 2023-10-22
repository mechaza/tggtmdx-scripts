using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Grawly.Menus.NameEntry;
using System.Linq;
using DG.Tweening;
using Grawly.Chat;
using Grawly.Playstyle;
using Grawly.UI;
using UnityEngine.EventSystems;

namespace Grawly.Menus {
	
	/// <summary>
	/// Controls the screen in which the player can select between modes like Standard or 3-Strike.
	/// </summary>
	public class PlaystyleSelectScreenController : MonoBehaviour {
		
		public static PlaystyleSelectScreenController Instance { get; private set; }

		#region FIELDS - TOGGLES
		/// <summary>
		/// The PlaystyleTemplates to use in constructing the buttons to be presented.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private List<PlaystyleTemplate> allPlaystyleTemplates = new List<PlaystyleTemplate>();
		/// <summary>
		/// The tutorial template that elaborates on how playstyles work.
		/// This is presented before a player picks a playstyle.
		/// </summary>
		[SerializeField]
		private TutorialTemplate playstyleSelectionTutorialTemplate;
		/// <summary>
		/// The script to run when confirming whether or not the player wants to use the selected play style.
		/// </summary>
		[SerializeField]
		private TextAsset selectionConfirmationChatScript;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The script to run when confirming whether or not the player wants to use the selected play style.
		/// </summary>
		public TextAsset SelectionConfirmationChatScript => this.selectionConfirmationChatScript;
		#endregion
		
		#region FIELDS - TWEENING
		/// <summary>
		/// The amount of time to wait between each button's presentation.
		/// Helps it feel a bit more fluid.
		/// </summary>
		[SerializeField, Title("Tweening")]
		private float buttonIntervalTime = 0.2f;
		/// <summary>
		/// The amount of time to wait before finalizing the presentation routine on the buttons.
		/// </summary>
		[SerializeField]
		private float presentationFinalizeTime = 0.5f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Every one of the playstyle buttons that are available, regardless of which list they belong to.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private List<PlaystyleSelectButtonList> allPlaystyleButtonLists = new List<PlaystyleSelectButtonList>();
		/// <summary>
		/// The borders that are around the screen! wow.
		/// </summary>
		[SerializeField]
		private StandardChatBorders chatBorders;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of the screen.
		/// </summary>
		private void ResetState() {
			
			// Go through all of the lists of buttons and reset each and every button.
			this.allPlaystyleButtonLists
				.SelectMany(pbs => pbs.PlaystyleButtons)
				.ToList()
				.ForEach(pb => pb.ResetState());
			
			// Reset the chat borders.
			this.chatBorders.ResetState();

		}
		/// <summary>
		/// Prepares the selection buttons with the provided playstyle templates and returns the selection buttons to use.
		/// This will also deactivate any selection buttons that will go unused.
		/// </summary>
		/// <param name="playstyleTemplates">The playstyle templates to use for the selection buttons.</param>
		/// <param name="allButtonLists">All of the lists which also contain the buttons that relate to the number.. f   agih  sdig</param>
		/// <returns>An appropriately sized list of the active selection buttons.</returns>
		private List<PlaystyleSelectButton> PrepareSelectionButtons(List<PlaystyleTemplate> playstyleTemplates, List<PlaystyleSelectButtonList> allButtonLists) {
			
			// Find the button list that has the same number of buttons as the number of templates passed in.
			PlaystyleSelectButtonList targetButtonList = allButtonLists.First(bl => bl.ButtonCount == playstyleTemplates.Count);

			// Grab the buttons themselves.
			List<PlaystyleSelectButton> targetButtons = targetButtonList.PlaystyleButtons;

			// Cascade down to the version of this function that takes the buttons themselves.
			return this.PrepareSelectionButtons(playstyleTemplates: playstyleTemplates, playstyleButtons: targetButtons);

		}
		/// <summary>
		/// Prepares the selection buttons passed in with the provided templates.
		/// </summary>
		/// <param name="playstyleTemplates">The templates to populate the selection buttons with.</param>
		/// <param name="playstyleButtons">The buttons to actually prepare.</param>
		/// <returns>The buttons passed in, but prepared.</returns>
		private List<PlaystyleSelectButton> PrepareSelectionButtons(List<PlaystyleTemplate> playstyleTemplates, List<PlaystyleSelectButton> playstyleButtons) {
			
			// Just like. Make sure the lists actually match in count.
			Debug.Assert(playstyleTemplates.Count == playstyleButtons.Count);
			
			// Go through the buttons that should actually be prepared and... prepare them.
			for (int i = 0; i < playstyleTemplates.Count; i++) {
				PlaystyleTemplate targetTemplate = playstyleTemplates[i];
				PlaystyleSelectButton targetButton = playstyleButtons[i];
				targetButton.Prepare(playstyleTemplate: targetTemplate);
			}
			
			// Return the same list, now that the buttons are prepared.
			return playstyleButtons;

		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the playstyle buttons to the player to select from.
		/// </summary>
		/// <param name="playstyleButtons"></param>
		private void Present(List<PlaystyleSelectButton> playstyleButtons) {
			
			// Start the coroutine that actually handles all this.
			GameController.Instance.StartCoroutine(this.PresentRoutine(
				playstyleButtons: playstyleButtons,
				intervalTime: this.buttonIntervalTime, 
				finalizeWaitTime: this.presentationFinalizeTime));
			
		}
		/// <summary>
		/// The actual routine that handles presenting the buttons.
		/// </summary>
		/// <param name="playstyleButtons">The playstyle buttons to present to the player.</param>
		/// <param name="intervalTime">The amount of time to wait between each button's tween in.</param>
		/// <param name="finalizeWaitTime">The amount of time to wait after the last button has been tweened in..</param>
		/// <returns></returns>
		private IEnumerator PresentRoutine(List<PlaystyleSelectButton> playstyleButtons, float intervalTime, float finalizeWaitTime) {
			
			// Go through each playstyle button and present it.
			foreach (var playstyleButton in playstyleButtons) {
				playstyleButton.Present();
				yield return new WaitForSeconds(seconds: intervalTime);
			}

			// Wait a little longer just to make the presentation feel better.
			yield return new WaitForSeconds(seconds: finalizeWaitTime);

			// Select the first button.
			EventSystem.current.SetSelectedGameObject(playstyleButtons[0].ButtonSelectable.gameObject);
			
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// This is what kicks off the process of allowing a player to pick a playstyle.
		/// </summary>
		public void BeginPlaystyleSelection() {
			
			// Reset the state of.. everything.
			this.ResetState();

			// Present the chat borderes.
			this.chatBorders.PresentBorders();
			
			// Prepare the buttons that are about to be presented and figure out what they are.
			List<PlaystyleSelectButton> targetButtons = this.PrepareSelectionButtons(
				playstyleTemplates: this.allPlaystyleTemplates, 
				allButtonLists: this.allPlaystyleButtonLists);

			// Open the tutorial on how playstyles work.
			TutorialScreenController.OpenTutorial(
				tutorialTemplate: this.playstyleSelectionTutorialTemplate, 
				objectToReselectOnClose: null,
				actionOnClose: () => {
					// Upon closing the tutorial, present the selections available.
					this.Present(playstyleButtons: targetButtons);
				});
			
			
			
		}
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// Starts the screen in debug mode wow.
		/// </summary>
		[Button("Debug Start"), HideInEditorMode]
		private void DebugStart() {
			
			this.BeginPlaystyleSelection();
			
			/*// Reset the state of.. everything.
			this.ResetState();

			// Present the chat borderes.
			this.chatBorders.PresentBorders();
			
			// Prepare the buttons that are about to be presented and figure out what they are.
			List<PlaystyleSelectButton> targetButtons = this.PrepareSelectionButtons(
				playstyleTemplates: this.allPlaystyleTemplates, 
				allButtonLists: this.allPlaystyleButtonLists);

			this.Present(playstyleButtons: targetButtons);*/

			/*// Make all of the target buttons snap to their display position.
			targetButtons.ForEach(tb => tb.TweenToPosition(
				stateType: PlaystyleSelectButton.PlaystyleSelectButtonStateType.Displayed, 
				snapToPosition: true));*/

		}
		#endregion
		
	}
}