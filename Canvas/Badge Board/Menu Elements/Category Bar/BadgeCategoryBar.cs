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
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Manages the different categories of badges that can be picked from in the selection.
	/// </summary>
	public class BadgeCategoryBar : MonoBehaviour {

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
		
		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// The category items that this bar is going to be managing.
		/// </summary>
		[TabGroup("Category", "Scene References"), SerializeField, Title("Items")]
		private List<BadgeCategoryItem> categoryItems = new List<BadgeCategoryItem>();
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image to use for the backing's front.
		/// </summary>
		[TabGroup("Category", "Scene References"), SerializeField, Title("Elements")]
		private Image backingFrontImage;
		/// <summary>
		/// The image to use for the backing's dropshadow.
		/// </summary>
		[TabGroup("Category", "Scene References"), SerializeField]
		private Image backingDropshadowImage;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this element.
		/// </summary>
		private void KillAllTweens() {
			this.backingFrontImage.DOKill(complete: true);
			this.backingDropshadowImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {

			// Kill all tweens.
			this.KillAllTweens();
			
			// Clear out the colors on the backing.
			this.backingFrontImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;
			
			// Reset the state on each category item.
			foreach (BadgeCategoryItem categoryItem in this.categoryItems) {
				categoryItem.ResetState();
			}
			
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
			
			// Go through each category item and present those too I guess.
			foreach (BadgeCategoryItem categoryItem in this.categoryItems) {
				categoryItem.Present(boardParams: boardParams);
			}
			
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
			
			// Go through each category item and dismiss those too I guess.
			foreach (BadgeCategoryItem categoryItem in this.categoryItems) {
				categoryItem.Dismiss(boardParams: boardParams);
			}
			
		}
		#endregion
		
	}
}