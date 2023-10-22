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
	/// The list in which a player can select a weapon from their inventory to perform a number of actions on.
	/// </summary>
	public class WeaponSelectionMenuList : MenuList {

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Selection", "Tweening"), SerializeField, Title("Timing")]
		private float backingFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Selection", "Tweening"), SerializeField]
		private float backingFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color to use for the backing's front when displayed.
		/// </summary>
		[TabGroup("Selection", "Tweening"), SerializeField, Title("Colors")]
		private Color backingFrontDisplayColor = Color.black;
		/// <summary>
		/// The color to use for the backing's dropshadow when displayed.
		/// </summary>
		[TabGroup("Selection", "Tweening"), SerializeField]
		private Color backingDropshadowDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image to use for the backing's front.
		/// </summary>
		[TabGroup("Selection", "Scene References"), SerializeField]
		private Image backingFrontImage;
		/// <summary>
		/// The image to use for the backing's dropshadow.
		/// </summary>
		[TabGroup("Selection", "Scene References"), SerializeField]
		private Image backingDropshadowImage;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on the selection menu list.
		/// </summary>
		private void KillAllTweens() {
			this.backingFrontImage.DOKill(complete: true);
			this.backingDropshadowImage.DOKill(complete: true);
		}
		/// <summary>
		/// Resets the state of this menu list.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens that might be ongoing.
			this.KillAllTweens();
			
			// Snap the colors for both images to be clear.
			this.backingFrontImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;
			
			// Clear the menu list.
			this.ClearMenuList();
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the element using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this element should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill any outstanding tweens.
			this.KillAllTweens();
			
			// Tween the images to their display colors.
			this.backingFrontImage.DOColor(
					endValue: this.backingFrontDisplayColor,
					duration: this.backingFadeInTime)
				.SetEase(Ease.Linear);
			this.backingDropshadowImage.DOColor(
					endValue: this.backingDropshadowDisplayColor,
					duration: this.backingFadeInTime)
				.SetEase(Ease.Linear);
			
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			// Kill any outstanding tweens.
			this.KillAllTweens();
			
			// Tween the images to their display colors.
			this.backingFrontImage.DOColor(
					endValue: Color.clear, 
					duration: this.backingFadeOutTime)
				.SetEase(Ease.Linear);
			this.backingDropshadowImage.DOColor(
					endValue: Color.clear, 
					duration: this.backingFadeOutTime)
				.SetEase(Ease.Linear);
			
		}
		#endregion
		
	}
}