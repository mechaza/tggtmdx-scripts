using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Battle.Analysis {
	
	
	/// <summary>
	/// Handles the preparation and display of the elemental icons on the analysis screen.
	/// </summary>
	public class CombatantAnalysisDXElementIcon : MonoBehaviour {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The element type to associate with this icon.
		/// </summary>
		[SerializeField, TabGroup("Icon","Toggles")]
		private ElementType elementType = ElementType.None;
		#endregion

		#region FIELDS - TWEENING : POSITION
		/// <summary>
		/// The position this icon should be when hiding.
		/// </summary>
		[Title("Position")]
		[SerializeField, TabGroup("Icon","Tweening")]
		private Vector2Int iconHidingPos = new Vector2Int();
		/// <summary>
		/// The position this icon should be when displayed.
		/// </summary>
		[SerializeField, TabGroup("Icon","Tweening")]
		private Vector2Int iconDisplayPos = new Vector2Int();
		#endregion
		
		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening this icon in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Icon", "Tweening")]
		private float iconTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening this icon out.
		/// </summary>
		[SerializeField, TabGroup("Icon","Tweening")]
		private float iconTweenOutTime = 0.2f;
		#endregion
		
		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The kind of ease to use when tweening in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Icon", "Tweening")]
		private Ease iconEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The kind of ease to use when tweening out.
		/// </summary>
		[SerializeField, TabGroup("Icon", "Tweening")]
		private Ease iconEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The RectTransform that should be manipulated for movement/tweens.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The image showing the actual element to the player.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private Image elementIconImage;
		/// <summary>
		/// The backing image for the icon.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private Image backingFrontImage;
		/// <summary>
		/// The dropshadow image for the icon.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private Image backingDropshadowImage;
		/// <summary>
		/// The label that shows the resistance of the element when it is known.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private SuperTextMesh knownResistanceLabel;
		/// <summary>
		/// The label that is shown when the resistance is not known.
		/// This gets used in conjunction with the masking image.
		/// </summary>
		[SerializeField, TabGroup("Icon","Scene References")]
		private SuperTextMesh unknownResistanceLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this icon.
		/// </summary>
		public void ResetState() {
			// Kill the tweens.
			this.KillTweens();
			// Also reset the position.
			this.mainPivotRectTransform.anchoredPosition = this.iconHidingPos;
			// Make the labels blank just in case.
			this.knownResistanceLabel.Text = "";
			this.unknownResistanceLabel.Text = "";
			
		}
		/// <summary>
		/// Kill any tweens operating on this icon.
		/// </summary>
		public void KillTweens() {
			this.mainPivotRectTransform.DOKill(complete: true);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Slides the icon in.
		/// </summary>
		public void TweenIn() {
			// Do the tween but like, also return it.
			this.mainPivotRectTransform.DOAnchorPos(
					endValue: this.iconDisplayPos, 
					duration: this.iconTweenInTime, 
					snapping: true)
				.SetEase(ease: this.iconEaseInType);
		}
		/// <summary>
		/// Slides the icon out.
		/// </summary>
		public void TweenOut() {
			this.mainPivotRectTransform.DOAnchorPos(
					endValue: this.iconHidingPos, 
					duration: this.iconTweenOutTime, 
					snapping: true)
				.SetEase(ease: this.iconEaseOutType);
		}
		#endregion
		
		#region BUILDING
		/// <summary>
		/// Rebuilds this icon with the specified parameters.
		/// </summary>
		/// <param name="analysisParams">The parameters that should be used to build this icon.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {

			// Get the sprite to use.
			Sprite iconSprite = DataController.GetDefaultElementalIcon(elementType: this.elementType, useLargeIcon: true);
			// Assign it to the image.
			this.elementIconImage.overrideSprite = iconSprite;
			
			// Set the default colors on the front/dropshadow images.
			this.backingFrontImage.color = Color.black;
			this.backingDropshadowImage.color = Color.white;

			// Get the resistance type associated with this icon's element.
			// Note that the result depends on the current combatant as well as the screen's category type.
			ResistanceType? knownResistanceType = analysisParams.GetDynamicResistance(this.elementType);
			
			if (knownResistanceType.HasValue == false) {
				// If there is no value (resistance unknown to player,) communicate as such. 
				this.unknownResistanceLabel.gameObject.SetActive(true);
				this.knownResistanceLabel.gameObject.SetActive(false);
			} else if (knownResistanceType.Value == ResistanceType.Nm) {
				// If there IS a value but its normal, just show the icon normally.
				this.unknownResistanceLabel.gameObject.SetActive(false);
				this.knownResistanceLabel.gameObject.SetActive(false);
			} else {
				// If the value is known and not normal, set it up as much.
				this.unknownResistanceLabel.gameObject.SetActive(false);
				this.knownResistanceLabel.gameObject.SetActive(true);
				this.knownResistanceLabel.Text = knownResistanceType.Value.ToString();
			}

		}
		#endregion
		
	}
	
}