using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	public class WeaponSelectionListItem : MenuItem {
	
		#region PROPERTIES - STATE
		/// <summary>
		/// The weapon assigned to this list item.
		/// </summary>
		public Weapon AssignedWeapon { get; set; }
		/// <summary>
		/// This shit usable? lol
		/// </summary>
		protected override bool IsUsable {
			get {
				return true;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM used for this move's name.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private SuperTextMesh weaponNameLabel;
		/// <summary>
		/// The image used to represent the behavior's icon.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image weaponElementalIconImage;
		/// <summary>
		/// The highlight for the icon itself. Yes I have two highlights is there a fucking problem?
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image weaponElementalIconHighlightImage;
		/// <summary>
		/// The image that is used as a backing for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image weaponElementalIconBackingFrontImage;
		/// <summary>
		/// The image that is used as a backing's dropshadow for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image weaponElementalIconBackingDropshadowFrontImage;
		/// <summary>
		/// The GameObject that serves as a sort of Highlight.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private GameObject weaponHighlightBarGameObject;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Anyone here smoke weed?
		/// </summary>
		/// <param name="item"></param>
		public override void BuildMenuItem(IMenuable item) {
			
			// Save a reference to the weapon. 
			this.AssignedWeapon = (Weapon) item;
			
			// Call Dehighlight. This effectively builds the strings.
			this.Dehighlight(item: item);

			// Set the sprite on the elemental icon and its highlight
			this.weaponElementalIconImage.overrideSprite = item.Icon;
			this.weaponElementalIconHighlightImage.overrideSprite = item.Icon;
			
		}
		#endregion

		#region PRESENTATION
		protected internal override void Highlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.weaponNameLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + item.PrimaryString;

			// Also update the elemental icon images
			this.weaponElementalIconHighlightImage.gameObject.SetActive(true);
			this.weaponElementalIconBackingFrontImage.color = Color.red;
			this.weaponElementalIconBackingDropshadowFrontImage.color = Color.black;

			// Turn off the highlight bar.
			this.weaponHighlightBarGameObject.SetActive(true);
			
		}
		protected internal override void Dehighlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.weaponNameLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + item.PrimaryString;
			
			// Also update the elemental icon images
			this.weaponElementalIconHighlightImage.gameObject.SetActive(false);
			this.weaponElementalIconBackingFrontImage.color = Color.black;
			this.weaponElementalIconBackingDropshadowFrontImage.color = Color.clear;

			// Turn off the highlight bar.
			this.weaponHighlightBarGameObject.SetActive(false);
			
		}
		#endregion
		
		#region MENU EVENTS
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.AssignedWeapon);
			
			// Update the current weapon on the board params.
			// BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon = this.AssignedWeapon;
			BadgeBoardController.Instance.UpdateCurrentWeapon(weapon: this.AssignedWeapon);
			
			// Preview what the board looks like when this weapon is selected.
			BadgeBoardController.Instance.BadgeBoard.BuildBoard(boardParams: BadgeBoardController.Instance.CurrentBoardParams);
			
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.AssignedWeapon);
			AudioController.instance?.PlaySFX(SFXType.Hover);
		}
		public override void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Select);
			BadgeBoardController.Instance.TriggerEvent("Weapon Selected");
		}
		public override void OnCancel(BaseEventData eventData) {
			this.Dehighlight(item: this.AssignedWeapon);
			AudioController.instance?.PlaySFX(SFXType.Close);
			BadgeBoardController.Instance.TriggerEvent("Back");
		}
		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {
			base.OnHorizontalMove(moveDir);
		}
		#endregion
		
		
	}
}