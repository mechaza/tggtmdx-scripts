using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// Used to show what the currently highlighted player has equipped in the shop.
	/// </summary>
	public class ShopPlayerEquipmentSummary : MonoBehaviour {

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time it should take for the foreground diamond to tween in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening"), Title("Timing")]
		private float foregroundDiamondTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time it should take for the background diamond to tween in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float backgroundDiamondTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time it should take for the foreground diamond to tween out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float foregroundDiamondTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time it should take for the background diamond to tween out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float backgroundDiamondTweenOutTime = 0.2f;
		/// <summary>
		/// The amount of time that should be delayed before tweening the foreground diamond in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float foregroundDiamondDelayInTime = 0.05f;
		/// <summary>
		/// The amount of time that should be delayed before tweening the background diamond in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float backgroundDiamondDelayInTime = 0.1f;
		#endregion
		
		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the foreground diamond in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening"), Title("Easing")]
		private Ease foregroundDiamondEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the background diamond in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private Ease backgroundDiamondEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the foreground diamond out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private Ease foregroundDiamondEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the background diamond out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private Ease backgroundDiamondEaseOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - TWEENING : SPEED
		/// <summary>
		/// The speed at which the foreground diamond should rotate at.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float foregroundDiamondRotationSpeed = 5f;
		/// <summary>
		/// The speed at which the background diamond should rotate at.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float backgroundDiamondRotationSpeed = 10f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the objects as children.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The image for the diamond graphic foreground.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private Image foregroundDiamondImage;
		/// <summary>
		/// The image for the diamond graphic background.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private Image backgroundDiamondImage;
		/// <summary>
		/// The RectTransform used to manipulate the foreground diamond.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private RectTransform foregroundDiamondRectTransform;
		/// <summary>
		/// The RectTransform used to manipulate the foreground diamond.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private RectTransform backgroundDiamondRectTransform;
		/// <summary>
		/// A list of the different equipment summary details that display the equipment of the current selected player.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private List<ShopPlayerEquipmentSummaryDetail> equipmentSummaryDetails = new List<ShopPlayerEquipmentSummaryDetail>();
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this object.
		/// </summary>
		private void KillAllTweens() {
			// Kill the tweens on the diamond's rect transforms.
			this.foregroundDiamondRectTransform.DOKill(complete: true);
			this.backgroundDiamondRectTransform.DOKill(complete: true);
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens.
			this.KillAllTweens();
			
			// Go through the details and reset those as well.
			this.equipmentSummaryDetails.ForEach(d => d.ResetState());
			
			// Snap the scale of both diamonds to zero.
			this.foregroundDiamondRectTransform.localScale = Vector3.zero;
			this.backgroundDiamondRectTransform.localScale = Vector3.zero;
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		/// <summary>
		/// Preps this object to be used with the shop menu params specified.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Prepare(ShopMenuParams shopMenuParams) {
			
			// Set the colors on the diamonds.
			this.foregroundDiamondImage.color = shopMenuParams.ShopThemeTemplate.EquipmentSummaryForegroundDiamondColor;
			this.backgroundDiamondImage.color = shopMenuParams.ShopThemeTemplate.EquipmentSummaryBackgroundDiamondColor;
			
			// Also go through each of the details and prepare those as well.
			this.equipmentSummaryDetails.ForEach(sd => sd.Prepare(shopMenuParams));
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the equipment summary with the parameters specified.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Present(ShopMenuParams shopMenuParams) {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
	}
}