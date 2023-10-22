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
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	public class BadgeSelectionListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The badge that has been assigned to this list item.
		/// </summary>
		public Badge AssignedBadge { get; private set; }
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// This shit usable? lol
		/// </summary>
		protected override bool IsUsable {
			get {
				return true;
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES : OBJECTS
		/// <summary>
		/// Contains all relevant objects as children.
		/// </summary>
		[SerializeField,  TabGroup("Menu List Item", "Scene References"), Title("Objects")]
		private GameObject allObjects;
		/// <summary>
		/// The GameObject that serves as a sort of Highlight.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private GameObject  badgeHighlightBarGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// The component that is used to help display information relating to the badge's experience/level.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References"), Title("Components")]
		private BadgeListItemExperienceInfo badgeExperienceInfo;
		/// <summary>
		/// The icon which represents if the badge is being used in the current grid or not.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private BadgeListItemUsageIcon badgeUsageIcon;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : LABELS
		/// <summary>
		/// The label used for this badge's name.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References"), Title("Labels")]
		private SuperTextMesh badgeNameLabel;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image used to represent the  badge's icon.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References"), Title("Images")]
		private Image badgeElementalIconImage;
		/// <summary>
		/// The highlight for the icon itself. Yes I have two highlights is there a fucking problem?
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image badgeElementalIconHighlightImage;
		/// <summary>
		/// The image that is used as a backing for the  badge's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image badgeElementalIconBackingFrontImage;
		/// <summary>
		/// The image that is used as a backing's dropshadow for the badge's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image  badgeElementalIconBackingDropshadowFrontImage;
		/// <summary>
		/// The image actually used to display the highlight bar.
		/// This can be used to manipulate the color and whatnot.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image highlightBarImage;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Anyone here smoke weed?
		/// </summary>
		/// <param name="item"></param>
		public override void BuildMenuItem(IMenuable item) {
			
			// Save the badge being used.
			this.AssignedBadge = (Badge) item;
			
			// Set the icons.
			this.badgeElementalIconImage.overrideSprite = item.Icon;
			this.badgeElementalIconHighlightImage.overrideSprite = item.Icon;
			
			// Build the usage icon based on if the badge is being used in this grid or anywhere else.
			this.badgeUsageIcon.BuildUsageIcon(
				badgeToCheck: (Badge)item,
				currentBadgeGrid: BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid,
				gameVariables: BadgeBoardController.Instance.CurrentBoardParams.CurrentVariables);
			
			// Dehighlight. This actually sets a lot of the visuals.
			this.Dehighlight(item: item);
			
		}
		#endregion

		#region PRESENTATION
		protected internal override void Highlight(IMenuable item) {
			
			// Set the colors on the elemental icon images.
			// Note that the icon highlight has a special script on it which necessitates activating/deactivating.
			this.badgeElementalIconImage.color = Color.white;
			this.badgeElementalIconHighlightImage.gameObject.SetActive(true);
			this.badgeElementalIconBackingFrontImage.color = GrawlyColors.Red;
			this.badgeElementalIconBackingDropshadowFrontImage.color = Color.white;
			
			// Rebuild the labels with the appropriate strings.
			this.badgeNameLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + item.PrimaryString;
			
			// Set the visuals on the experience info.
			this.badgeExperienceInfo.Highlight(badge: (Badge)item);
			
			// Turn the highlight bar on.
			this.badgeHighlightBarGameObject.SetActive(true);
			
		}
		protected internal override void Dehighlight(IMenuable item) {
			
			// Set the colors on the elemental icon images.
			// Note that the icon highlight has a special script on it which necessitates activating/deactivating.
			this.badgeElementalIconImage.color = Color.white;
			this.badgeElementalIconHighlightImage.gameObject.SetActive(false);
			this.badgeElementalIconBackingFrontImage.color = GrawlyColors.Black;
			this.badgeElementalIconBackingDropshadowFrontImage.color = Color.clear;
			
			// Rebuild the labels with the appropriate strings.
			this.badgeNameLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + item.PrimaryString;
			
			// Set the visuals on the experience info.
			this.badgeExperienceInfo.Dehighlight(badge: (Badge)item);
			
			// Turn the highlight bar off.
			this.badgeHighlightBarGameObject.SetActive(false);
			
		}
		#endregion
		
		#region MENU EVENTS
		public override void OnSelect(BaseEventData eventData) {
			
			// Highlight this list item in particular.
			this.Highlight(item: this.AssignedBadge);
			
			// Tell the badge board it should highlight the board piece assigned with this badge if its already placed.
			BadgeBoardController.Instance.BadgeBoard.UpdatePulsingBoardPiece(currentBadge: this.AssignedBadge);
			
			// Build the info.
			BadgeBoardController.Instance.BadgeSelectionController.BuildBadgeInfo(badge: this.AssignedBadge);
			
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.AssignedBadge);
			AudioController.instance?.PlaySFX(SFXType.Hover);
		}
		public override void OnSubmit(BaseEventData eventData) {

			// Check if any other grids are using this badge.
			bool otherGridsUsingBadge = this.OtherGridsUseBadge(badge: this.AssignedBadge);
			
			// If another grid IS using this badge, reject the submission.
			if (otherGridsUsingBadge == true) {
				AudioController.instance?.PlaySFX(SFXType.Invalid);
			} else {
				// If this grid (or for that matter, no grid) is using this badge, select it.
				AudioController.instance?.PlaySFX(SFXType.Select);
				// Pass the badge associated with this item to the board params.
				BadgeBoardController.Instance.CurrentBoardParams.CurrentSelectedBadge = this.AssignedBadge;
				// Send the FSM event.
				BadgeBoardController.Instance.TriggerEvent("Badge Selected");
			}	
			
		}
		public override void OnCancel(BaseEventData eventData) {
			this.Dehighlight(item: this.AssignedBadge);
			AudioController.instance?.PlaySFX(SFXType.Close);
			BadgeBoardController.Instance.TriggerEvent("Back");
		}
		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMove(moveDir);
		}
		#endregion

		#region PRIVATE HELPERS
		/// <summary>
		/// Checks if the specified badge is being used in the current grid and, if not, if its used in any others.
		/// </summary>
		/// <param name="badge"></param>
		/// <returns></returns>
		private bool OtherGridsUseBadge(Badge badge) {
			// Cascade down using the current information available.
			return this.OtherGridsUseBadge(
				badge: badge,
				currentGrid: BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid, 
				currentVariables: BadgeBoardController.Instance.CurrentBoardParams.CurrentVariables);
		}
		/// <summary>
		/// Checks if the specified badge is being used in the current grid and, if not, if its used in any others.
		/// </summary>
		/// <param name="badge"></param>
		/// <param name="currentGrid"></param>
		/// <param name="currentVariables"></param>
		/// <returns></returns>
		private bool OtherGridsUseBadge(Badge badge, BadgeGrid currentGrid, GameVariables currentVariables) {
			
			// Grab all the badge grids from the weapons in the game variables.
			// FYI, the removal should NEVER fail.
			// The currentBadgeGrid technically should exist in the weapons grids somehwere too.
			List<BadgeGrid> otherBadgeGrids = currentVariables.WeaponCollectionSet.AllWeapons.Select(w => w.BadgeGrid).ToList();
			bool result = otherBadgeGrids.Remove(currentGrid);
			Debug.Assert(result == true);
			
			// Cascade down.
			return this.OtherGridsUseBadge(
				badge: badge, 
				currentGrid: currentGrid, 
				otherBadgeGrids: otherBadgeGrids);
		}
		/// <summary>
		/// Checks if the specified badge is being used in the current grid and, if not, if its used in any others.
		/// </summary>
		/// <param name="badge"></param>
		/// <param name="currentGrid"></param>
		/// <param name="otherBadgeGrids"></param>
		/// <returns></returns>
		private bool OtherGridsUseBadge(Badge badge, BadgeGrid currentGrid, List<BadgeGrid> otherBadgeGrids) {
			
			// The list of "other" grids should NOT contain the one currently on display on the board screen.
			Debug.Assert(otherBadgeGrids.Contains(currentGrid) == false);

			// If any of the other grids contain the specified badge, return true.
			return otherBadgeGrids.Any(bg => bg.HasBadge(badge)) == true;

			/*// Check if the current grid has the badge to check against inside of it.
			if (currentGrid.HasBadge(badge: badge) == true) {
				this.usedInCurrentGridVisuals.SetActive(true);
			} else if (otherBadgeGrids.Any(bg => bg.HasBadge(badgeToCheck)) == true) {
				// If the badge isnt in the current grid, check if its in any of the others.
				this.usedInOtherGridVisuals.SetActive(true);
			}*/
		}
		#endregion
		
	}
}