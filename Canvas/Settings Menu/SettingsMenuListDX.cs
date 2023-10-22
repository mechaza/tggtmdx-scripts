using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.UI {

	/// <summary>
	/// What populates the list on the right of the screen of the settings menu.
	/// </summary>
	public class SettingsMenuListDX : MenuList {

		#region FIELDS - TOGGLES : TWEENING
		/// <summary>
		/// The amount of time to take when tweening the menu list in.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class"), Title("Tweening")]
		private float tweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the menu list out.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class")]
		private float tweenOutTime = 0.5f;
		/// <summary>
		/// The position where all the objects should be when resting.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class")]
		private Vector2Int restingPosition = new Vector2Int();
		/// <summary>
		/// The position where all the objects should be when active.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class")]
		private Vector2Int activePosition = new Vector2Int();
		/// <summary>
		/// The tweening to use when easing in.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class")]
		private Ease easeInType = Ease.InOutCirc;
		/// <summary>
		/// The tweening to use when easing out.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class")]
		private Ease easeOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The rect transform for all the other objects.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Sub Class"), Title("Scene References")]
		private RectTransform allObjects;
		#endregion

		#region ADDITIONAL ANIMATIONS
		/// <summary>
		/// Resets the settings menu list on the right of the screen to a hiding position.
		/// </summary>
		public void ResetState() {
			// Clear the menu list.
			this.ClearMenuList();
			// Snap it to the resting position.
			this.allObjects.anchoredPosition = this.restingPosition;
		}
		/// <summary>
		/// Presents this settings menu list onto the screen.
		/// </summary>
		public void Present() {
			// Kill any animations that are currently acting on the all visuals transform.
			this.allObjects.DOKill(complete: true);
			// Tween that shit in.
			this.allObjects.DOAnchorPos(
				endValue: this.activePosition, 
				duration: this.tweenInTime, 
				snapping: true)
				.SetEase(ease: this.easeInType);
		}
		/// <summary>
		/// Dismisses this settings menu list from the screen.
		/// </summary>
		public void Dismiss() {
			// Kill any animations that are currently acting on the all visuals transform.
			this.allObjects.DOKill(complete: true);
			// Tween that shit out.
			this.allObjects.DOAnchorPos(
				endValue: this.restingPosition,
				duration: this.tweenOutTime,
				snapping: true)
				.SetEase(ease: this.easeOutType);
		}
		#endregion

	}


}