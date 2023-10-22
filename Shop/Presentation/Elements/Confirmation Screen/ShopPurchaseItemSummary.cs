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
using Grawly.Menus;
using Grawly.UI.MenuLists;

namespace Grawly.Shop.UI {
	
	/// <summary>
	/// Displays the information about an item that will be purchased or replaced upon purchase.
	/// </summary>
	public class ShopPurchaseItemSummary : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of summary this is.
		/// Used to help prepare the graphics on the confirmation screen appropriately.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Toggles")]
		private ShopPurchaseSummaryType purchaseSummaryType = ShopPurchaseSummaryType.None;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The type of summary this is.
		/// Used to help prepare the graphics on the confirmation screen appropriately.
		/// </summary>
		public ShopPurchaseSummaryType PurchaseSummaryType => this.purchaseSummaryType;
		#endregion

		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position this summary should be at when hiding.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening"), Title("Positions")]
		private Vector2Int summaryHidingPos = new Vector2Int();
		/// <summary>
		/// The position this summary should be at when displayed.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private Vector2Int summaryDisplayPos = new Vector2Int();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the summary in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening"), Title("Timing")]
		private float summaryTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the summary out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private float summaryTweenOutTime = 0.2f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the summary in.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening"), Title("Easing")]
		private Ease summaryEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the summary out.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Tweening")]
		private Ease summaryEaseOutType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// Contains all the objects for this thingamajic.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private GameObject allObjects;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The front image for the item's icon.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References"), Title("Images")]
		private Image itemIconFrontImage;
		/// <summary>
		/// The dropshadow image for the item's icon.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private Image itemIconDropshadowImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The label that should be used to show off the primary text.
		/// Usually will be the item's name.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References"), Title("Labels")]
		private SuperTextMesh itemPrimaryLabel;
		/// <summary>
		/// The label that should display the description of the item.
		/// Used in situations where stats are not displayed to the player.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References")]
		private SuperTextMesh itemDescriptionLabel;
		#endregion

		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// Contains the objects which can be used to display stats of an item.
		/// Used in situations where a text description is not displayed to the player.
		/// </summary>
		[SerializeField, TabGroup("Summary", "Scene References"), Title("Other")]
		private List<ShopItemStat> shopItemStats = new List<ShopItemStat>();
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform that should be manipulated to adjust the position of the summary.
		/// </summary>
		private RectTransform MainPositionPivotRectTransform => this.allObjects.GetComponent<RectTransform>();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this item summary.
		/// </summary>
		private void KillAllTweens() {
			
			// Kill any tweens on the positional pivot.
			this.MainPositionPivotRectTransform.DOKill(complete: true);

		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			
			// Kill all tweens that might be happening.
			this.KillAllTweens();
			
			// Snap the positional pivot to its hiding position.
			this.MainPositionPivotRectTransform.anchoredPosition = this.summaryHidingPos;
			
			// Turn off all the visuals.
			this.allObjects.SetActive(false);
			
		}
		#endregion
		
	}
}