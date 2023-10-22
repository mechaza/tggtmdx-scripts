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
using Sirenix.Serialization;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Encapsulates all the information regarding whatever badge is highlighted.
	/// </summary>
	public class BadgeInfo : SerializedMonoBehaviour {

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Info", "Tweening"), SerializeField, Title("Timing")]
		private float backingFadeInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading the backing in.
		/// </summary>
		[TabGroup("Info", "Tweening"), SerializeField]
		private float backingFadeOutTime = 0.5f;
		#endregion

		#region FIELDS - TWEENING : COLORS
		/// <summary>
		/// The color to use for the backing's front when displayed.
		/// </summary>
		[TabGroup("Info", "Tweening"), SerializeField, Title("Colors")]
		private Color backingFrontDisplayColor = Color.black;
		/// <summary>
		/// The color to use for the backing's dropshadow when displayed.
		/// </summary>
		[TabGroup("Info", "Tweening"), SerializeField]
		private Color backingDropshadowDisplayColor = Color.white;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image to use for the backing's front.
		/// </summary>
		[TabGroup("Info", "Scene References"), SerializeField, Title("Images")]
		private Image backingFrontImage;
		/// <summary>
		/// The image to use for the backing's dropshadow.
		/// </summary>
		[TabGroup("Info", "Scene References"), SerializeField]
		private Image backingDropshadowImage;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : COMPONENTS
		/// <summary>
		/// The representation of how big this badge will be when placed on the grid.
		/// </summary>
		[TabGroup("Info", "Scene References"), OdinSerialize, Title("Components")]
		private BadgeSizeInfo BadgeSizeInfo { get; set; }
		/// <summary>
		/// A list to populate with the various details describing a badges functionality.
		/// </summary>
		[TabGroup("Info", "Scene References"), OdinSerialize]
		private List<BadgeInfoDetail> BadgeInfoDetails { get; set; } = new List<BadgeInfoDetail>();
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
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill any tweens operating on the info.
			this.KillAllTweens();
			
			// Snap the colors for both images to be clear.
			this.backingFrontImage.color = Color.clear;
			this.backingDropshadowImage.color = Color.clear;
			
			// Reset the state of the size info.
			this.BadgeSizeInfo.ResetState();
			
			// Go through each of the info details and reset those too.
			foreach (BadgeInfoDetail badgeInfoDetail in this.BadgeInfoDetails) {
				badgeInfoDetail.ResetState();
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
			
			// Present the size info.
			this.BadgeSizeInfo.Present(boardParams: boardParams);
			
			// Also present the info details.
			foreach (BadgeInfoDetail badgeInfoDetail in this.BadgeInfoDetails) {
				badgeInfoDetail.Present(boardParams: boardParams);
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
			
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Displays information about the badge specified.
		/// </summary>
		/// <param name="badge">The badge to write out information for.</param>
		public void BuildBadgeInfo(Badge badge) {
			
			// Update the size.
			this.BadgeSizeInfo.BuildSizeInfo(badge: badge);
			
			// Reset the state on every badge detail in preparation for the ones that are about to be built.
			this.BadgeInfoDetails.ForEach(bi => bi.ResetState());

			// Get the facts from the badge and iterate through them to build the details.
			List<BadgeFact> badgeFacts = badge.BadgeFacts;
			Debug.Log("Facts count: " + badgeFacts.Count);
			for (int i = 0; i < badgeFacts.Count; i++) {
				this.BadgeInfoDetails[i].BuildInfoDetail(badgeFact: badgeFacts[i]);
			}
			
		}
		#endregion
		
	}
}