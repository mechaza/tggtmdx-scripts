using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Gets displayed on the side of a badge list item in the selection list to communicate if its being used or not.
	/// </summary>
	public class BadgeListItemUsageIcon : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES : OBJECTS
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[SerializeField, TabGroup("Icon", "Scene References"), Title("Objects")]
		private GameObject allObjects;
		/// <summary>
		/// The gameobject containing components used to display if a badge is being used in the current grid.
		/// </summary>
		[SerializeField, TabGroup("Icon", "Scene References")]
		private GameObject usedInCurrentGridVisuals;
		/// <summary>
		/// The gameobject containing components used to display if a badge is being used in a separate grid.
		/// </summary>
		[SerializeField, TabGroup("Icon", "Scene References")]
		private GameObject usedInOtherGridVisuals;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this icon.
		/// </summary>
		public void ResetState() {
			// Man just turn it all off.
			// this.allObjects.SetActive(false);
			this.usedInCurrentGridVisuals.SetActive(false);
			this.usedInOtherGridVisuals.SetActive(false);
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Checks whether or not a badge is being used in the grid currently on display and builds the icon accordingly.
		/// </summary>
		/// <param name="badgeToCheck">The badge that this icon is being associated with.</param>
		/// <param name="currentBadgeGrid">The badge grid currently on display.</param>
		/// <param name="gameVariables">The GameVariables that may contain records of the badge being used elsewhere..</param>
		public void BuildUsageIcon(Badge badgeToCheck, BadgeGrid currentBadgeGrid, GameVariables gameVariables) {

			// Grab all the badge grids from the weapons in the game variables.
			// FYI, the removal should NEVER fail.
			// The currentBadgeGrid technically should exist in the weapons grids somehwere too.
			List<BadgeGrid> otherBadgeGrids = gameVariables.WeaponCollectionSet.AllWeapons.Select(w => w.BadgeGrid).ToList();
			bool result = otherBadgeGrids.Remove(currentBadgeGrid);
			Debug.Assert(result == true);
			
			// Cascade down.
			this.BuildUsageIcon(
				badgeToCheck: badgeToCheck, 
				currentBadgeGrid: currentBadgeGrid, 
				otherBadgeGrids: otherBadgeGrids);
			
		}
		/// <summary>
		/// Checks whether or not a badge is being used in the grid currently on display and builds the icon accordingly.
		/// </summary>
		/// <param name="badgeToCheck">The badge that this icon is being associated with.</param>
		/// <param name="currentBadgeGrid">The badge grid currently on display.</param>
		/// <param name="otherBadgeGrids">A list of all other badge grids. The current grid is not included.</param>
		private void BuildUsageIcon(Badge badgeToCheck, BadgeGrid currentBadgeGrid, List<BadgeGrid> otherBadgeGrids) {
			
			// The list of "other" grids should NOT contain the one currently on display on the board screen.
			Debug.Assert(otherBadgeGrids.Contains(currentBadgeGrid) == false);
			
			// Begin by turning everything off.
			this.ResetState();

			// Check if the current grid has the badge to check against inside of it.
			if (currentBadgeGrid.HasBadge(badge: badgeToCheck) == true) {
				this.usedInCurrentGridVisuals.SetActive(true);
			} else if (otherBadgeGrids.Any(bg => bg.HasBadge(badgeToCheck)) == true) {
				// If the badge isnt in the current grid, check if its in any of the others.
				this.usedInOtherGridVisuals.SetActive(true);
			}
			
		}
		#endregion
		
	}
}