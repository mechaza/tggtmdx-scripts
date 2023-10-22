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
using Sirenix.Serialization;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Encapsulates the badge selection list and allows for more precise control over which badges are displayed.
	/// </summary>
	public class BadgeSelectionController : SerializedMonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu list that this controller will be manipulating.
		/// </summary>
		[TabGroup("Selection","Scene References"), OdinSerialize]
		private BadgeSelectionMenuList BadgeSelectionMenuList { get; set; }
		/// <summary>
		/// The bar that displays the current category of badge being selected from.
		/// </summary>
		[TabGroup("Selection","Scene References"), OdinSerialize]
		private BadgeCategoryBar BadgeCategoryBar { get; set; }
		/// <summary>
		/// Displays information about the currently highlighted badge.
		/// </summary>
		[TabGroup("Selection","Scene References"), OdinSerialize]
		private BadgeInfo BadgeInfo { get; set; }
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			// Reset the state on the individual elements.
			this.BadgeSelectionMenuList.ResetState();
			this.BadgeCategoryBar.ResetState();
			this.BadgeInfo.ResetState();
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the board list onto the screen.
		/// </summary>
		/// <param name="boardParams"></param>
		public void Present(BadgeBoardParams boardParams) {
			this.BadgeSelectionMenuList.Present(boardParams: boardParams);
			// this.BadgeCategoryBar.Present(boardParams: boardParams);
			this.BadgeInfo.Present(boardParams: boardParams);
		}
		/// <summary>
		/// Dismisses the board list from the screen.
		/// </summary>
		/// <param name="boardParams"></param>
		public void Dismiss(BadgeBoardParams boardParams) {
			this.BadgeSelectionMenuList.Dismiss(boardParams: boardParams);
			this.BadgeCategoryBar.Dismiss(boardParams: boardParams);
			this.BadgeInfo.Dismiss(boardParams: boardParams);
		}
		#endregion

		#region BUILDING - LIST
		/// <summary>
		/// Builds the badge selection with the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters keeping track of the board's status.</param>
		/// <param name="elementType">The element that should be filtered when building the menu.</param>
		public void BuildBadgeSelection(BadgeBoardParams boardParams, ElementType elementType) {
			// When an element is provided for this function, get the badges that fit that element specifically.
			this.BuildBadgeSelection(
				boardParams: boardParams,
				badgesToDisplay: boardParams.CurrentVariables.BadgeCollectionSet.GetBadges(elementType: elementType));
		}
		/// <summary>
		/// Builds the badge selection with the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters keeping track of the board's status.</param>
		/// <param name="badgesToDisplay">The actual badges to display.</param>
		public void BuildBadgeSelection(BadgeBoardParams boardParams, List<Badge> badgesToDisplay) {

			// If this category has zero badges, create a dummy list.
			if (badgesToDisplay.Count == 0) {
				throw new NotImplementedException("Implement the ability to handle empty categories/lists!");
			}
			
			// Prep the menu list with the badges.
			this.BadgeSelectionMenuList.PrepareMenuList(
				allMenuables: badgesToDisplay.Cast<IMenuable>().ToList(), 
				startIndex: 0);
			
		}
		/// <summary>
		/// Refreshes the selection list when returning from the crane.
		/// Use cases include when a list item needs to signal it was just equipped.
		/// </summary>
		public void RefreshBadgeSelection() {
			this.BadgeSelectionMenuList.RebuildMenuList();
		}
		#endregion

		#region BUILDING - OTHER
		/// <summary>
		/// Builds the info for the provided badge.
		/// </summary>
		/// <param name="badge">The badge to display the info for.</param>
		public void BuildBadgeInfo(Badge badge) {
			this.BadgeInfo.BuildBadgeInfo(badge: badge);
		}
		#endregion
		
		#region EVENT SYSTEM HELPERS
		/// <summary>
		/// Selects the first available action item.
		/// </summary>
		public void SelectFirstBadgeItem() {
			// This is a function that belongs to the MenuList, which is private.
			this.BadgeSelectionMenuList.SelectFirstMenuListItem();
		}
		#endregion
		
	}
}