using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grawly.Battle;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using Grawly.Chat;

namespace Grawly.UI.Legacy {

	public class CombatantAnalysisMoveItem : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler, IDeselectHandler {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label to represent this move.
		/// </summary>
		[SerializeField]
		private SuperTextMesh moveLabel;
		/// <summary>
		/// The image that represents the button for this move.
		/// </summary>
		[SerializeField]
		private Image moveImage;
		/// <summary>
		/// The selectable. Self explainatory.
		/// </summary>
		private Selectable selectable;
		#endregion
		#region FIELDS - COMPUTED
		/// <summary>
		/// The dehighlight color for this move item.
		/// </summary>
		private string DeHighlightString {
			get {
				// Slot 8 is the "next level" button.
				if (slot == 8) {
					return "<c=white>";
				} else {
					return "<c=black>";
				}
			}
		}
		/// <summary>
		/// The dehighlight color for this move item.
		/// </summary>
		private Color DeHighlightColor {
			get {
				// Slot 8 is the "next level" button.
				if (slot == 8) {
					return Color.black;
				} else {
					return Color.white;
				}
			}
		}
		#endregion
		#region FIELDS - MISC
		/// <summary>
		/// The "slot" for this item.
		/// </summary>
		public int slot = -1;
		/// <summary>
		/// The behavior associated with this move item.
		/// </summary>
		private BattleBehavior behavior;
		/// <summary>
		/// The context the button is being built in.
		/// </summary>
		private CombatantAnalysisCanvas.ContextType context;
		#endregion

		private void Awake() {
			selectable = GetComponent<Selectable>();
			SetSelectable(false);
		}

		/// <summary>
		/// Assembles the move item based on the contents of a given behavior.
		/// </summary>
		/// <param name="behavior"></param>
		public void Build(BattleBehavior behavior, CombatantAnalysisCanvas.ContextType context) {
			// Assemble the metadata of the button.
			this.behavior = behavior;
			this.moveLabel.Text = DeHighlightString + behavior.behaviorName;
			// this.moveImage.CrossFadeColor(DeHighlightColor, 0f, true, true);
			this.moveImage.color = this.DeHighlightColor;
			// Remember the context.
			this.context = context;
			// THIS SUCKS PLEASE REFACTOR THE ANALYSIS CANVAS WHHAHFGAH

			if (this.moveLabel.Text != "" && (this.context == CombatantAnalysisCanvas.ContextType.AnalyzeEnemy || this.context == CombatantAnalysisCanvas.ContextType.ChangePersona)) {
				this.SetSelectable(true);
			} else {
				this.SetSelectable(false);
			}
		}
		/// <summary>
		/// Clears out the move item.
		/// </summary>
		public void Clear() {
			moveLabel.Text = "";
			behavior = null;
			SetSelectable(false);
		}
		/// <summary>
		/// Sets whether or not this move item can be selected or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetSelectable(bool status) {

			Debug.Log("SETTING SELECTABLE ON SLOT " + this.slot.ToString() + " TO " + status.ToString());

			selectable.enabled = status;
			// If trying to turn the selectable off and this is the one thats selected, tell the event system to null it out.
			if (status == false && EventSystem.current.currentSelectedGameObject == selectable.gameObject) {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		#region ANIMATIONS
		/// <summary>
		/// Replaces the behavior for this move item with another one. (Usually in the context of leveling up)
		/// </summary>
		/// <param name="behavior"></param>
		public void FlashNewMoveItem(BattleBehavior behavior) {
			this.behavior = behavior;
			this.moveLabel.Text = behavior.behaviorName;
			Sequence seq = DOTween.Sequence();
			seq.Append(moveImage.DOColor(GrawlyColors.colorDict[GrawlyColorTypes.Red], 0.01f));
			seq.AppendCallback(new TweenCallback(delegate {
				AudioController.instance.PlaySFX(SFXType.PlayerBattleMenu);
			}));
			seq.Append(moveImage.DOColor(this.DeHighlightColor, 1f));
			seq.Play();
		}
		#endregion

		public void OnDeselect(BaseEventData eventData) {
			try {
				AudioController.instance.PlaySFX(SFXType.Hover);
				moveLabel.Text = DeHighlightString + behavior.behaviorName;
				// this.moveImage.CrossFadeColor(DeHighlightColor, 0f, true, true);
				this.moveImage.color = this.DeHighlightColor;
			} catch (System.Exception e) {
				Debug.LogError("ISSUE WITH ITEM " + this.slot.ToString());
			}
			
		}
		/*public void OnMove(AxisEventData eventData) {
			
			// Find the selectable that is directly above this one.
			Selectable selectableAbove = this.selectable.FindSelectableOnUp();
			// If the selectable above this one is a profile icon, select the profile icon that was previously picked.
			// (the one above this might not be the correct one due to how unity's event system works.
			if (selectableAbove.GetComponent<CombatantAnalysisProfileIcon>() != null) {
				EventSystem.current.SetSelectedGameObject(CombatantAnalysisProfileIcon.lastSelected.gameObject);
			}
		}*/
		public void OnSelect(BaseEventData eventData) {
			try {
				moveLabel.Text = "<c=white>" + behavior.behaviorName;
				// moveImage.CrossFadeColor(GrawlyColors.colorDict[GrawlyColorTypes.Red], 0f, true, false);
				this.moveImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				CombatantAnalysisCanvas.instance.DisplayInfoBox(this.behavior.description);
			} catch (System.Exception e) {
				Debug.LogError("ISSUE WITH ITEM " + this.slot.ToString());
			}
		}
		public void OnCancel(BaseEventData eventData) {
			moveLabel.Text = DeHighlightString + behavior.behaviorName;
			// this.moveImage.CrossFadeColor(DeHighlightColor, 0f, true, true);
			this.moveImage.color = this.DeHighlightColor;
			// If leveling up the persona, ask the player if they want to change.
			if (this.context == CombatantAnalysisCanvas.ContextType.PersonaLevelUp) {
				EventSystem.current.SetSelectedGameObject(null);
				CombatantAnalysisCanvas.instance.DisplayInfoBox("");
				CombatantAnalysisCanvas.instance.PromptForCancel(slot: this.slot);
			} else {
				// Otherwise, close out the menu.
				CombatantAnalysisCanvas.instance.ResetAndClose();
			}
			
		}
		public void OnSubmit(BaseEventData eventData) {
			
			// bad
			if (this.context == CombatantAnalysisCanvas.ContextType.AnalyzeEnemy || this.context == CombatantAnalysisCanvas.ContextType.ChangePersona) {
				Debug.Log("Context is currently enemy analyzation or changing persona. Backing out of submission.");
				return;
			}

			
			// bad
			CombatantAnalysisCanvas.instance.slot = this.slot;
			moveLabel.Text = DeHighlightString + behavior.behaviorName;
			// this.moveImage.CrossFadeColor(DeHighlightColor, 0f, true, true);
			this.moveImage.color = this.DeHighlightColor;
			CombatantAnalysisCanvas.instance.DisplayInfoBox("");
			// Check what the context is and perform certain functions from here.
			if (context == CombatantAnalysisCanvas.ContextType.PersonaLevelUp || context == CombatantAnalysisCanvas.ContextType.SkillCardAdd) {

				// bad
				if (this.slot == 8) {				// If this is the level up move itself, treat it as if i hit cancel
					this.OnCancel(eventData);
					return;
				}

				List<ChatDirective> directives = new List<ChatDirective>();
				directives.Add(new DialogueDirective(speakerShorthand: "", dialogue: "Replace " + this.behavior.behaviorName + "?"));
				directives.Add(new OptionDirective(trueString: "Yes", falseString: "No"));

				ChatControllerDX.GlobalOpen(
					chatScript: new PlainChatScript(directives),
					chatOpenedCallback: delegate { CombatantAnalysisCanvas.instance.SetMoveItemSelectables(false, context); },
					chatClosedCallback: CombatantAnalysisCanvas.instance.ConfirmMoveReplacement);

				/*Chat.Legacy.LegacyChatController.Open(
					script: new PlainChatScript(directives),
					chatOpenedCallback: delegate { CombatantAnalysisCanvas.Instance.SetMoveItemSelectables(false, context); },
					chatClosedCallback: CombatantAnalysisCanvas.Instance.ConfirmMoveReplacement);*/



			}
		}

		#region CHAT CLOSED DELEGATES
		/// <summary>
		/// A function used to confirm whether or not the move should be replaced.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="num"></param>
		/// <param name="toggle"></param>
		private void ConfirmMoveReplacement(string str, int num, bool toggle) {
			throw new Exception("DO NOT CALL");
			/*if (toggle == true) {
				Persona p = (Persona)CombatantAnalysisCanvas.Instance.selectedCombatant;
				// If this is the move to be replace, add it.
				BattleResultsController.Instance.PersonaAddLevelUpMove(persona: p, slot: this.slot, behavior: p.levelUpMoves.Peek().behavior);
			} else {
				CombatantAnalysisCanvas.Instance.SetMoveItemSelectables(true, context);
			}*/
		}
		#endregion

	}
}