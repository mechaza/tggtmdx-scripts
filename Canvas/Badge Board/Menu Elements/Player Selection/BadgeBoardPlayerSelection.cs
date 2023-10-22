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
	/// Controls access to selecting a player to build their weapon selection out of.
	/// </summary>
	public class BadgeBoardPlayerSelection : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the other objects as children.
		/// </summary>
		[TabGroup("Players", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The list of players that can be chosen from on the board.
		/// </summary>
		[TabGroup("Players", "Scene References"), SerializeField]
		private List<BadgeBoardPlayerItem> playerItems = new List<BadgeBoardPlayerItem>();
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the player selection.
		/// </summary>
		public void ResetState() {
			// Reset the state on all the other player items.
			this.playerItems.ForEach(pi => pi.ResetState());
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the player selection onto the screen.
		/// </summary>
		/// <param name="boardParams"></param>
		public void Present(BadgeBoardParams boardParams) {
			// Build the item on each player.
			// this.playerItems.ForEach(pi => pi.BuildPlayerItem());
		}
		/// <summary>
		/// Dismisses the player selection.
		/// </summary>
		/// <param name="boardParams"></param>
		public void Dismiss(BadgeBoardParams boardParams) {
			this.ResetState();
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the player items based on the players passed in.
		/// </summary>
		/// <param name="availablePlayers">The players who should have their items built.</param>
		public void BuildPlayerItems(List<Player> availablePlayers) {
			
			// Reset the state on all the player items.
			this.playerItems.ForEach(pi => pi.ResetState());

			// Iterate through the available players and build them
			for (int i = 0; i < availablePlayers.Count; i++) {
				Player player = availablePlayers[i];
				this.playerItems[i].BuildPlayerItem(player: player);
			}
			
		}
		#endregion

		#region EVENT SYSTEM
		/// <summary>
		/// Selects the first available player item.
		/// </summary>
		public void SelectFirstPlayerItem() {
			EventSystem.current.SetSelectedGameObject(this.playerItems.First().gameObject);
		}
		#endregion
		
	}
}