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
	/// The menu that displays different actions the player can do on a weapon currently selected.
	/// </summary>
	public class WeaponActionMenu : MonoBehaviour {

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Actions", "Tweening"), SerializeField, Title("Timing")]
		private float backingFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Actions", "Tweening"), SerializeField]
		private float backingFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color to use for the backing's front when displayed.
		/// </summary>
		[TabGroup("Actions", "Tweening"), SerializeField, Title("Colors")]
		private Color backingFrontDisplayColor = Color.black;
		/// <summary>
		/// The color to use for the backing's dropshadow when displayed.
		/// </summary>
		[TabGroup("Actions", "Tweening"), SerializeField]
		private Color backingDropshadowDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image to use for the backing's front.
		/// </summary>
		[TabGroup("Actions", "Scene References"), SerializeField, Title("Images")]
		private Image backingFrontImage;
		/// <summary>
		/// The image to use for the backing's dropshadow.
		/// </summary>
		[TabGroup("Actions", "Scene References"), SerializeField]
		private Image backingDropshadowImage;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// The action items containing the differnt things the player can do.
		/// </summary>
		[TabGroup("Actions", "Scene References"), SerializeField, Title("Other")]
		private List<WeaponActionItem> weaponActionItems = new List<WeaponActionItem>();
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens currently operating on this menu.
		/// </summary>
		private void KillAllTweens() {
			this.backingFrontImage.DOKill(complete: true);
			this.backingDropshadowImage.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens operating on this menu.
			this.KillAllTweens();
			
			// Snap the colors for both images to be clear.
			this.backingFrontImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;
			
			// Reset the state on all the children as well.
			foreach (WeaponActionItem actionItem in this.weaponActionItems) {
				actionItem.ResetState();
			}
			
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the element using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this element should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			
			// Kill any tweens currently ongoing.
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

		#region BUILDING
		/// <summary>
		/// Builds the action menu to fit the requirements of the parameters passed in.
		/// </summary>
		/// <param name="boardParams">The parameters dictating how this shit works.</param>
		public void BuildActionMenu(BadgeBoardParams boardParams) {
			// Turn on each action item.
			// TODO: FIX THIS SO IT CAN BE MORE SPECIFIC IF IT NEEDS TO
			foreach (WeaponActionItem actionItem in this.weaponActionItems) {
				actionItem.BuildActionItem(boardParams: boardParams);
			}
		}
		#endregion

		#region EVENT SYSTEM HELPERS
		/// <summary>
		/// Selects the first available action item.
		/// </summary>
		public void SelectFirstActionItem() {
			// This is pretty straightforward.
			EventSystem.current.SetSelectedGameObject(this.weaponActionItems.First().gameObject);
		}
		#endregion
		
	}
}