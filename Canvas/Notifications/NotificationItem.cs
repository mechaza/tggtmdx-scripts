using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Grawly.UI {
	
	/// <summary>
	/// An item that contains all of the elements for a notification on the screen.
	/// </summary>
	public class NotificationItem : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The parameters that were used to build this item.
		/// </summary>
		public NotificationParams CurrentParams { get; private set; }
		#endregion
		
		#region FIELDS - TOGGLES : POSITIONS
		/// <summary>
		/// The position for the primaryLabel.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Item", "Toggles")]
		private Vector2 baseLabelPosition;
		#endregion

		#region FIELDS - TOGGLES : OFFSETS
		/// <summary>
		/// The amount of offset to have when using a secondary label.
		/// </summary>
		[Title("Offsets")]
		[SerializeField, TabGroup("Item", "Toggles")]
		private Vector2 labelOffset;
		#endregion
		
		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to take when tweening the label elements.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Item", "Toggles")]
		private float labelTweenTime = 0.2f;
		#endregion
		
		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease to use when tweening the label elements.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Item", "Toggles")]
		private Ease labelEase = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects inside this notification item.
		/// </summary>
		[Title("Anchors and Objects")]
		[SerializeField, TabGroup("Item", "Scene References")]
		private GameObject AllObjects;
		/// <summary>
		/// The rect transform for the elements inside of the notification item.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private RectTransform labelElementsRectTransform;
		/// <summary>
		/// The image for the front of the notification.
		/// </summary>
		[Title("UI Elements")]
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image barImageFront;
		/// <summary>
		/// The image for the front of the icon.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image iconFrontImage;
		/// <summary>
		/// The image for the dropshadow of the icon.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image iconDropshadowImage;
		/// <summary>
		/// The label for the notification.
		/// </summary>
		[FormerlySerializedAs("notificationLabel")]
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh primaryLabel;
		/// <summary>
		/// The label for secondary text.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh secondaryLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Totally resets the state of this notification item.
		/// </summary>
		public void ResetState() {

			// Kill any tweens on the label elements RT and snap it back to position.
			this.labelElementsRectTransform.DOKill(complete: true);
			this.labelElementsRectTransform.anchoredPosition = this.baseLabelPosition;
			
			// Reset the visuals.
			this.barImageFront.color = Color.white;
			this.primaryLabel.Text = "";
			this.secondaryLabel.Text = "";
			this.iconFrontImage.overrideSprite = null;
			this.iconDropshadowImage.overrideSprite = null;
			
		}
		/// <summary>
		/// Builds this item with the params specified.
		/// </summary>
		/// <param name="notificationParams"></param>
		public void Build(NotificationParams notificationParams) {

			// Remember the params.
			this.CurrentParams = notificationParams;
			
			// Set the color of the bar.
			this.barImageFront.color = notificationParams.color;
			
			// Set the icon sprite.
			this.iconFrontImage.overrideSprite = notificationParams.icon;
			this.iconDropshadowImage.overrideSprite = notificationParams.icon;

			// Set the text.
			this.primaryLabel.Text = notificationParams.primaryText;
			this.secondaryLabel.Text = notificationParams.secondaryText;
			
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tween the visuals to the secondary label.
		/// </summary>
		public void TweenSecondaryLabel() {
			this.labelElementsRectTransform.DOAnchorPos(
					endValue: this.baseLabelPosition + this.labelOffset, 
					duration: this.labelTweenTime, 
					snapping: true)
				.SetEase(this.labelEase);
		}
		#endregion
		
	}

	
}