using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// An item inside the weapon action menu to allow a player to decide what to do with the current weapon.
	/// </summary>
	public class WeaponActionItem : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The board params currently cached in this item.
		/// </summary>
		private BadgeBoardParams CurrentBoardParams { get; set; }
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of action item this is associated with.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private WeaponActionItemType actionItemType = WeaponActionItemType.None;
		/// <summary>
		/// The string to use when builidng this action item.
		/// </summary>
		[SerializeField]
		private string actionItemName = "";
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The type of item action this is associated with.
		/// </summary>
		public WeaponActionItemType ActionItemType => this.actionItemType;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that are children of this item.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image to use as the highlight.
		/// </summary>
		[SerializeField]
		private Image highlightImage;
		/// <summary>
		/// The STM used to display this action's name.
		/// </summary>
		[SerializeField]
		private SuperTextMesh actionNameLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this item.
		/// </summary>
		public void ResetState() {
			// Turn the objects off.
			this.allObjects.SetActive(false);
			// Clear out the board params.
			this.CurrentBoardParams = null;
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds this item with the parameters specified.
		/// </summary>
		/// <param name="boardParams"></param>
		public void BuildActionItem(BadgeBoardParams boardParams) {
			
			// Save a reference to the board params in use.
			this.CurrentBoardParams = boardParams;
			
			// Turn on all the objects.
			this.allObjects.SetActive(true);
			
			// Dehighlight the item.
			this.Dehighlight(boardParams: boardParams);
			
		}
		#endregion
		
		#region HIGHLIGHTING
		/// <summary>
		/// Sets this items graphics to be highlighted.
		/// </summary>
		/// <param name="boardParams">The parameters used for building this board.</param>
		private void Highlight(BadgeBoardParams boardParams) {
			
			// Turn the highlight image off.
			this.highlightImage.gameObject.SetActive(true);
			
			// Set the text on the name label.
			this.actionNameLabel.Text = "<c=black>" + this.actionItemName;
		}
		/// <summary>
		/// Sets this items graphics to be dehighlighted.
		/// </summary>
		/// <param name="boardParams">The parameters used for building this board.</param>
		private void Dehighlight(BadgeBoardParams boardParams) {
			
			// Turn the highlight image off.
			this.highlightImage.gameObject.SetActive(false);
			
			// Set the text on the name label.
			this.actionNameLabel.Text = "<c=white>" + this.actionItemName;

		}
		#endregion
		
		#region EVENT HANDLERS
		public void OnSelect(BaseEventData eventData) {
			this.Highlight(boardParams: this.CurrentBoardParams);
		}
		public void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(boardParams: this.CurrentBoardParams);
			AudioController.instance?.PlaySFX(SFXType.Hover);
		}
		public void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Select);
			// Pass this information off to the board controller.
			BadgeBoardController.Instance.UpdateCurrentWeaponAction(weaponActionType: this.ActionItemType);
		}
		public void OnCancel(BaseEventData eventData) {
			this.Dehighlight(boardParams: this.CurrentBoardParams);
			AudioController.instance?.PlaySFX(SFXType.Close);
			BadgeBoardController.Instance.TriggerEvent("Back");
		}
		#endregion
		
	}
}