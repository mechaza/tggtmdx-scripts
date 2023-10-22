using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Grawly.Menus.NameEntry;
using System.Linq;
using DG.Tweening;

namespace Grawly.Menus {

	/// <summary>
	/// Controls the screen where the players' names are entered.
	/// </summary>
	public class NameEntryController : MonoBehaviour {

		public static NameEntryController instance;

		#region FIELDS - STATE : NAMES
		/// <summary>
		/// The names that should be used for the players.
		/// </summary>
		private List<string> EnteredNames = new List<string>() { "", "", "", "" };
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// Have names been entered for all characters?
		/// </summary>
		public bool AllNamesEntered {
			get {
				// Return true only if the number of empty strings is zero.
				return this.EnteredNames.Count(s => s == "") == 0;
			}
		}
		/// <summary>
		/// The index of the next unentered name.
		/// </summary>
		public int NextUnenteredNameIndex {
			get {
				try {
					return this.EnteredNames.FindIndex(s => s == "");
				} catch (System.Exception e) {
					Debug.LogError("Couldn't find an index of an unentered name!");
					return -1;
				}
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : GAMEOBJECTS
		/// <summary>
		/// The GameObject that has the state machine for the controller.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject nameEntryStateMachineGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : INPUT
		/// <summary>
		/// The input box that sends letters over to the entry field.
		/// </summary>
		[SerializeField]
		private StandardInputBox inputBox;
		/// <summary>
		/// The input box that sends letters over to the entry field.
		/// </summary>
		public StandardInputBox InputBox {
			get {
				return this.inputBox;
			}
		}
		/// <summary>
		/// The field where input from the box should be sent.
		/// </summary>
		[SerializeField]
		private StandardEntryField entryField;
		/// <summary>
		/// The field where input from the box should be sent.
		/// </summary>
		public StandardEntryField EntryField {
			get {
				return this.entryField;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : DECORATION
		/// <summary>
		/// The border bar controller that. Controls the borders.
		/// </summary>
		[SerializeField]
		private BorderBarController borderBarController;
		/// <summary>
		/// The border bar controller that. Controls the borders.
		/// </summary>
		public BorderBarController BorderBarController {
			get {
				return this.borderBarController;
			}
		}
		/// <summary>
		/// The text that fades in/out with Mysterious Messages.
		/// </summary>
		[SerializeField]
		private NameEntryDramaticText dramaticText;
		/// <summary>
		/// The text that fades in/out with Mysterious Messages.
		/// </summary>
		public NameEntryDramaticText DramaticText {
			get {
				return this.dramaticText;
			}
		}
		/// <summary>
		/// The NameEntryBustUp that shows the current player receiving a name.
		/// </summary>
		[SerializeField]
		private NameEntryBustUp nameEntryBustUp;
		/// <summary>
		/// The NameEntryBustUp that shows the current player receiving a name.
		/// </summary>
		public NameEntryBustUp NameEntryBustUp {
			get {
				return this.nameEntryBustUp;
			}
		}
		/// <summary>
		/// The canvas group containing the visuals for the name prompt.
		/// </summary>
		[SerializeField]
		private CanvasGroup nameBoxPromptVisualsGroup;
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// A gag resource that should be used if the character is named Sans.
		/// </summary>
		[Title("Resources"), SerializeField]
		private Sprite sansSprite;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Triggers an event on the state machine.
		/// </summary>
		/// <param name="eventName"></param>
		public void TriggerMachineEvent(string eventName) {
			Unity.VisualScripting.CustomEvent.Trigger(target: this.nameEntryStateMachineGameObject, name: eventName, args: new object[] { });
		}
		/// <summary>
		/// Enables the input box and prompts the user for a name.
		/// </summary>
		[Button]
		public void PromptForName() {
			// Fade the prompt label in.
			this.nameBoxPromptVisualsGroup.DOFade(endValue: 1f, duration: 1f).SetEase(Ease.Linear);
			// Present the entry field.
			this.entryField.PresentEntryField();
			// Clear the entry field out. Also signal that the first entry letter should be highlighted.
			this.entryField.Clear(highlightFirstLetter: true);
			// Present the input box.
			this.inputBox.PresentInputBox();
			// Request input from the input box.
			this.inputBox.RequestInput();
		}
		/// <summary>
		/// Confirms whether the name is good or not.
		/// </summary>
		/// <param name="inputText">The text that was entered in the entry field.</param>
		public void ConfirmName(string inputText) {

			// If the string is only whitespace or has nothing at all, do not allow it.
			if (inputText.Replace(oldValue: " ", newValue: "").Length == 0) {
				Debug.LogError("Name is empty! Ask again!");
				return;
			}

			// TODO: Add ability to "go back"

			// Shoehorning this in. If this is Dorothy and the input is sans, start the routine.
			if (this.NextUnenteredNameIndex == 0 && inputText == "Sans") {
				this.SansSpriteSetter(inputText);
				return;
			}
			
			// Put this name into the list.
			this.EnteredNames[this.NextUnenteredNameIndex] = inputText;
			
			// Dismiss the entry field.
			this.entryField.DismissEntryField();
			// Dismiss the input box.
			this.inputBox.DismissInputBox();
			// Dismiss the name box visuals.
			this.nameBoxPromptVisualsGroup.DOFade(endValue: 0f, duration: 0.1f).SetEase(Ease.Linear);
			// Also finish input.
			this.inputBox.FinishInput();

			// Tell the state machine that input was received.
			Unity.VisualScripting.CustomEvent.Trigger(target: this.nameEntryStateMachineGameObject, name: "RECEIVEDINPUT", args: new object[] { });

			Debug.Log("NAME: " + inputText);
		}
		/// <summary>
		/// Completes the NameEntry process by setting the CharacterIDMap in the GameController's variables.
		/// </summary>
		public void CompleteNameEntry() {
			
			GameController.Instance.Variables.UpdateCharacterNames(nameTuples: new List<(CharacterIDType idType, string name)>() {
				(idType: CharacterIDType.Dorothy, name: this.EnteredNames[0]),
				(idType: CharacterIDType.Rose, name: this.EnteredNames[1]),
				(idType: CharacterIDType.Blanche, name: this.EnteredNames[2]),
				(idType: CharacterIDType.Sophia, name: this.EnteredNames[3]),
			});
			
			/*try {
				GameController.Instance.Variables.UpdateCharacterNames(nameTuples: new List<(CharacterIDType idType, string name)>() {
					(idType: CharacterIDType.Dorothy, name: this.EnteredNames[0]),
					(idType: CharacterIDType.Rose, name: this.EnteredNames[1]),
					(idType: CharacterIDType.Blanche, name: this.EnteredNames[2]),
					(idType: CharacterIDType.Sophia, name: this.EnteredNames[3]),
				});
			} catch (System.Exception e) {
				Debug.LogError("Couldn't complete name entry! Reason: " + e.Message);
			}*/
		}
		#endregion

		#region GAG ROUTINES
		/// <summary>
		/// A badly written routine that sets dorothy's sprite to be sans if the input is set to that.'
		/// </summary>
		/// <param name="inputText"></param>
		private void SansSpriteSetter(string inputText) {
			
			this.NameEntryBustUp.ChangeBustUpSprite(this.sansSprite);
			
			GameController.Instance.WaitThenRun(timeToWait: 1f, () => {
				// Put this name into the list.
				this.EnteredNames[this.NextUnenteredNameIndex] = inputText;
			
				// Dismiss the entry field.
				this.entryField.DismissEntryField();
				// Dismiss the input box.
				this.inputBox.DismissInputBox();
				// Dismiss the name box visuals.
				this.nameBoxPromptVisualsGroup.DOFade(endValue: 0f, duration: 0.1f).SetEase(Ease.Linear);
				// Also finish input.
				this.inputBox.FinishInput();

				// Tell the state machine that input was received.
				Unity.VisualScripting.CustomEvent.Trigger(target: this.nameEntryStateMachineGameObject, name: "RECEIVEDINPUT", args: new object[] { });
			});
		}

		#endregion
		
	}


}