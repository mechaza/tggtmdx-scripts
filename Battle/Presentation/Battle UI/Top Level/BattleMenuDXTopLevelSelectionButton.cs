using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// Basically a selection on the top level of the battle menu.
	/// </summary>
	public class BattleMenuDXTopLevelSelectionButton : MonoBehaviour, ISubmitHandler, IDeselectHandler, ISelectHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The string to use for this selection. 
		/// </summary>
		private string selectionText = "";
		/// <summary>
		/// Should I allow this button to be used? This is mostly for cases like Skill being affected by the fact the player may have a Silence affliction.
		/// </summary>
		private bool allowSubmission = true;
		/// <summary>
		/// Should I allow this button to be used? This is mostly for cases like Skill being affected by the fact the player may have a Silence affliction.
		/// </summary>
		public bool AllowSubmission {
			get {
				return this.allowSubmission;
			} set {
				/*if (value == false) {
					Debug.Log("SETTING ALLOWSUBMISSION ON " + this.gameObject.name + " TO FALSE");
				}*/
				this.allowSubmission = value;
			}
		}
		#endregion

		#region FIELDS - LOGICAL TOGGLES
		/// <summary>
		/// The type of top level selection this button is.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Logical Toggles")]
		private BattleMenuDXTopLevelSelectionType topLevelSelectionType;
		/// <summary>
		/// The type of top level selection this button is.
		/// </summary>
		public BattleMenuDXTopLevelSelectionType TopLevelSelectionType {
			get {
				return this.topLevelSelectionType;
			}
		}
		/// <summary>
		/// Should this top level selection set the battle behavior on the menu controller?
		/// </summary>
		[SerializeField, TabGroup("Selection", "Logical Toggles")]
		private bool setsBattleBehavior = false;
		/// <summary>
		/// The battle behavior to send to the battle menu controller in the event this top level selection is told to do so.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Logical Toggles"), ShowIf("setsBattleBehavior")]
		private BattleBehavior battleBehaviorToSet;
		#endregion

		#region FIELDS - GRAPHICAL TOGGLES
		/// <summary>
		/// For the sake of debugging, how much should the highlight bar be offset when this top level selection is picked?
		/// </summary>
		[SerializeField, TabGroup("Selection", "Graphical Toggles")]
		private float highlightBarVerticalOffset = 0f;
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// The material to apply to the SuperTextMesh when this selection is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Resources")]
		private Material highlightedMaterial;
		/// <summary>
		/// The material to apply to the SuperTextMesh when this selection is dehighlighted.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Resources")]
		private Material dehighlightedMaterial;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The SuperTextMesh to use.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private SuperTextMesh selectionLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// On awake, grab the text that should be used for this selection. 
			this.selectionText = this.selectionLabel.text;
			// Don't ask me why I need this. I don't know either. If I don't have this, baton touch is left blank.
			if (this.topLevelSelectionType == BattleMenuDXTopLevelSelectionType.BatonTouch) {
				this.selectionText = "Baton Touch";
			}
		}
		#endregion

		#region REBUILDING
		/// <summary>
		/// Uh. I guess this rebuilds the text with the appropriate color.... fuckedt up... i only use it in one place.
		/// </summary>
		public void Rebuild() {
			this.selectionLabel.Text = (this.allowSubmission == true ? "<c=black>" : "<c=gray>") + this.selectionText;
		}
		#endregion

		#region UNITY EVENTSYSTEM INTERFACE IMPLEMENTATIONS
		public void OnSubmit(BaseEventData eventData) {

			// If this button is in a state where it cannot be used, back out immediately.
			if (this.allowSubmission == false) {
				AudioController.instance.PlaySFX(SFXType.Invalid);
				return;
			}

			// Play the SFX for selection.
			AudioController.instance?.PlaySFX(type: SFXType.Select);

			// Set the text on the rotating cube on Select.
			RotatingMenuSquare.instance.SetSquareText(topLevelSelection: this.topLevelSelectionType);
			

			// If this type of button should set the battle behavior (e.x., just attacking, nothing fancy,) set it.
			if (this.setsBattleBehavior == true) {
				BattleMenuControllerDX.instance.SetCurrentBattleBehavior(behavior: this.battleBehaviorToSet);
			}

			// Set the top level selection type on the battle menu controller.
			BattleMenuControllerDX.instance.SetCurrentTopLevelSelectionType(battleMenuDXTopLevelSelectionType: this.topLevelSelectionType);

			// Reset the text.
			this.selectionLabel.textMaterial = this.dehighlightedMaterial;
			this.selectionLabel.Text = "<c=black>" + this.selectionText;

			// Hide the description text.
			BattleMenuDXTopLevelDescription.instance.Hide();

		}
		public void OnDeselect(BaseEventData eventData) {
			// On deselect, just change the material and rebuild the text.
			this.selectionLabel.textMaterial = this.dehighlightedMaterial;
			// The color should be different depending on whether submission is allowed or not.
			this.selectionLabel.Text = (this.allowSubmission == true ? "<c=black>" : "<c=gray>") + this.selectionText;
			// Also play the sound effect for moving.
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
		}
		public void OnSelect(BaseEventData eventData) {
			// On select, change the material and rebuild the text.
			this.selectionLabel.textMaterial = this.highlightedMaterial;
			// The color should be different depending on whether submission is allowed or not.
			this.selectionLabel.Text = (this.allowSubmission == true ? "<c=white>" : "<c=gray>") + this.selectionText;
			// Also move the highlight bar over to this location.
			BattleMenuControllerDX.instance.TopLevelHighlightBarRectTransform.anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition + new Vector2(x: 0f, y: this.highlightBarVerticalOffset);
			// Tell the rotating cube that I hovered over this option and it may want to tween the color to the one associated with this top level selection.
			RotatingMenuSquare.instance.TweenSquareColor(topLevelSelection: this.topLevelSelectionType);
			// By default, set the text to be nothing inside the Cube.
			RotatingMenuSquare.instance.SetSquareText("");
			// Also display the description for the top level selection.
			BattleMenuDXTopLevelDescription.instance.Display(topLevelType: this.topLevelSelectionType);
		}
		#endregion

	}

	/// <summary>
	/// The different possible types of selections that could be made from the top level.
	/// </summary>
	public enum BattleMenuDXTopLevelSelectionType {
		Analysis = 0,
		Guard = 1,
		Attack = 2,
		Skill = 3,
		Masque = 4,
		Item = 5,
		Escape = 6,
		BatonTouch = 7,
	}

}