using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Represents one of the players that can be picked in the player selection.
	/// </summary>
	public class BadgeBoardPlayerItem : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler, ISubmitHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The player that is currently assigned to this player item.
		/// </summary>
		public Player CurrentPlayer { get; private set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[TabGroup("Player", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The selectable for this player item.
		/// </summary>
		[TabGroup("Player", "Scene References"), SerializeField]
		private Selectable selectable;
		/// <summary>
		/// The image that shows the headshot of the player.
		/// </summary>
		[TabGroup("Player", "Scene References"), SerializeField]
		private Image playerHeadshotImage;
		/// <summary>
		/// The image that should be turned on when the item is highlighted.
		/// </summary>
		[TabGroup("Player", "Scene References"), SerializeField]
		private Image highlightImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this item.
		/// </summary>
		public void ResetState() {
			this.CurrentPlayer = null;
			this.allObjects.SetActive(false);
			this.highlightImage.gameObject.SetActive(false);
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the player item so that the user may select weapons associated with that player.
		/// </summary>
		/// <param name="player"></param>
		public void BuildPlayerItem(Player player) {
			
			// Turn the objects back on.
			this.allObjects.SetActive(true);
			
			// Save a reference to the player.
			this.CurrentPlayer = player;
			
			// Get the headshot sprite for this player.
			this.playerHeadshotImage.overrideSprite = player.playerTemplate.badgeBoardHeadshotSprite;

		}
		#endregion
		
		#region EVENT SYSTEM
		public void OnSelect(BaseEventData eventData) {
			
			// Set the highlight image on.
			this.highlightImage.gameObject.SetActive(true);
			
			Debug.Log("PLAYER WEAPON: " + this.CurrentPlayer.Weapon.PrimaryString);
			
			// When selecing this player, preview what their grid looks like.
			BadgeBoardController.Instance.BadgeBoard.BuildBoard(
				badgeGrid: this.CurrentPlayer.Weapon.BadgeGrid);
			
		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.highlightImage.gameObject.SetActive(false);
		}
		public void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Select);
			BadgeBoardController.Instance.UpdateCurrentPlayer(player: this.CurrentPlayer);
		}
		public void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			BadgeBoardController.Instance.TriggerEvent(eventName: "Back");
		}
		#endregion
	}
}