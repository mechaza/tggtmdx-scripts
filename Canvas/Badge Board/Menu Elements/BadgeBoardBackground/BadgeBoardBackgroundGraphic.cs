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
	/// Controls the background imagery of the badge board controller.
	/// </summary>
	public class BadgeBoardBackgroundGraphic : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The color to use when in hiding.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private Color backgroundHidingColor = Color.clear;
		/// <summary>
		/// The color to use when displayed opn screen.
		/// </summary>
		[SerializeField]
		private Color backgroundDisplayColor = Color.white;
		/// <summary>
		/// The color to use when the scrolling background is on screen.
		/// </summary>
		[SerializeField]
		private Color scrollingDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The actual image to fade in/out.
		/// </summary>
		[SerializeField]
		private Image backgroundImage;
		/// <summary>
		/// The image that contains the scrolling effect.
		/// </summary>
		[SerializeField]
		private Image scrollingImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this background.
		/// </summary>
		private void KillAllTweens() {
			this.backgroundImage.DOKill(complete: true);
			this.scrollingImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of the background.
		/// </summary>
		public void ResetState() {
			// Kill all tweens.
			this.KillAllTweens();
			// Set the color to be hiding.
			this.backgroundImage.color = this.backgroundHidingColor;
			this.scrollingImage.color = this.backgroundHidingColor;
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the background graphic onto the screen.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this background should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Tween the color in appropriately.
			this.backgroundImage.DOColor(
					endValue: this.backgroundDisplayColor,
					duration: 0.5f)
				.SetEase(Ease.Linear);
			this.scrollingImage.DOColor(
					endValue: this.scrollingDisplayColor,
					duration: 0.5f)
				.SetEase(Ease.Linear);
			
		}
		/// <summary>
		/// Dismisses the background graphic from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Tween the color out appropriately.
			this.backgroundImage.DOColor(
					endValue: this.backgroundHidingColor,
					duration: 0.5f)
				.SetEase(Ease.Linear);
			this.scrollingImage.DOColor(
					endValue: this.backgroundHidingColor,
					duration: 0.5f)
				.SetEase(Ease.Linear);
			
		}
		#endregion
		
	}
}