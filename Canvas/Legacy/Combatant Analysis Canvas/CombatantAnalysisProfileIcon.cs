using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grawly.Battle;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using Grawly.Battle.BattleMenu;

namespace Grawly.UI.Legacy {
	public class CombatantAnalysisProfileIcon : MonoBehaviour, ISelectHandler, ISubmitHandler, IDeselectHandler, ICancelHandler {

		#region FIELDS - LAST SELECTED
		/// <summary>
		/// A reference so I can remember what the last selected profile icon was. Helpful for moving up from a move item but need to re-select the "correct" one.
		/// </summary>
		public static CombatantAnalysisProfileIcon lastSelected;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The background for the given icon.
		/// </summary>
		[SerializeField]
		private Image iconBackground;
		/// <summary>
		/// The combatant's sprite.
		/// </summary>
		[SerializeField]
		private Image combatantProfile;
		/// <summary>
		/// The selectable component that lets this icon be selected.
		/// </summary>
		private Selectable selectable;
		#endregion
		#region FIELDS - MISC
		/// <summary>
		/// The slot for the given icon.
		/// </summary>
		public int slot = -1;
		/// <summary>
		/// The combatant associated with this icon.
		/// </summary>
		private Combatant combatant;
		/// <summary>
		/// The context for which this icon is being built in.
		/// </summary>
		private CombatantAnalysisCanvas.ContextType context;
		#endregion
		#region FIELDS - COMPUTED
		/// <summary>
		/// Depending on what kind of combatant this is, the icon may need to be faded by default.
		/// (i.e., persona that is in use.)
		/// </summary>
		private Color FinalColor {
			get {
				// Make sure the profile icon can be seen.
				if (combatant.GetType() == typeof(Persona)) {
					if (((Persona)combatant).InUse) {
						return Color.gray;
					}
				}
				return Color.white;
			}
		}
		/// <summary>
		/// Can this icon be selected? 
		/// (the only case where it can be is if its a persona thats not in use tbh)
		/// </summary>
		private bool CanSelect {
			get {
				// Make sure the profile icon can be seen.
				if (combatant.GetType() == typeof(Persona)) {
					if (((Persona)combatant).InUse == false) {
						return true;
					}
				}
				return false;
			}
		}
		#endregion

		private void Awake() {
			selectable = GetComponent<Selectable>();
			selectable.enabled = false;
		}

		/// <summary>
		/// Builds up this icon with a given combatant.
		/// </summary>
		/// <param name="combatant"></param>
		public void Build(Combatant combatant, CombatantAnalysisCanvas.ContextType context) {
			this.combatant = combatant;
			this.combatantProfile.sprite = combatant.Icon;
			this.combatantProfile.preserveAspect = true;
			// Set the color to the "Final Color"
			this.combatantProfile.CrossFadeColor(FinalColor, 0f, true, true);
			// Fade the background to white.
			iconBackground.color = Color.white;

			// Remember the context
			this.context = context;
			// Turn on the icon.
			SetSelectable(true);
		}
		/// <summary>
		/// Clears out this profile icon.
		/// </summary>
		public void Clear() {
			this.combatant = null;
			// Hide the icon and make the background black.
			this.combatantProfile.CrossFadeColor(Color.clear, 0f, true, true);
			iconBackground.color = Color.black;
			SetSelectable(false);
		}
		/// <summary>
		/// Sets whether or not this icon can be selected or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetSelectable(bool status) {
			selectable.enabled = status;
		}

		public void OnSelect(BaseEventData eventData) {
			// Make the background red.
			iconBackground.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
			// Tell the analysis canvas to show this combatant's info
			CombatantAnalysisCanvas.instance.DisplayCombatantDetail(this.combatant, this.context);

			// This TRULY is shoehorning at this point. When selected, instruct the top two menu items to set their "up" directions to here.
			CombatantAnalysisCanvas.instance.TopMoveItems
				.Select(m => m.GetComponent<Selectable>())
				.ToList()
				.ForEach(s => {
					// Debug.Log("hey");
					Navigation itemNavigation = s.navigation;
					itemNavigation.selectOnUp = this.selectable;
					s.navigation = itemNavigation;
				});

		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance.PlaySFX(SFXType.Hover);
			iconBackground.color = Color.white;
		}
		public void OnCancel(BaseEventData eventData) {
			iconBackground.color = Color.white;
			CombatantAnalysisCanvas.instance.ResetAndClose();
		}
		public void OnSubmit(BaseEventData eventData) {
			switch (context) {
				case CombatantAnalysisCanvas.ContextType.ChangePersona:
					if (CanSelect == true) {
						// This reference gets used later. Might remove it but also whatever.
						CombatantAnalysisCanvas.instance.selectedCombatant = this.combatant;
						
						Debug.Log("NOTE: This is where the battle canvas and the player status canvas used to get turned back on");
						// CanvasController.Instance.SetBattleCanvas(true); // I need to set the battle canvas back here because otherwise the next line fails.
						// CanvasController.Instance.SetPlayerStatusCanvasGroup(true);
						BattleController.Instance.PrepareBehaviorEvaluation(
							source: BattleController.Instance.CurrentCombatant, 
							targets: null, 
							behavior: BattleMenuControllerDX.instance.CurrentBattleBehavior);		// If the top level selection was Masque, this should have been set.

						// LegacyBattleMenuController.Instance.SendMessage("PrepareBehaviorEvaluation", ht);
					} else {
						AudioController.instance.PlaySFX(SFXType.Invalid);
						// Tell the combatant analysis canvas to flash the bust up of who is currently using this persona.
						CombatantAnalysisCanvas.instance.FlashCurrentPersonaUserVisuals();
					}
					break;
				default:
					Debug.LogWarning("Context not available!");
					AudioController.instance.PlaySFX(SFXType.Invalid);
					break;
			}
		}

	}
}