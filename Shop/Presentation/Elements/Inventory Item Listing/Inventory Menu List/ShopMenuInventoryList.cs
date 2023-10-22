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
	/// The MenuList showing the inventory of available purchases.
	/// </summary>
	public class ShopMenuInventoryList : MenuList {
		
		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the Inventory list should be at when hiding.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening"), Title("Positions")]
		private Vector2Int inventoryListHidingPos = new Vector2Int();
		/// <summary>
		/// The position the Inventory list should be at when on display.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening")]
		private Vector2Int inventoryListDisplayPos = new Vector2Int();
		/// <summary>
		/// The position the Inventory list should be at when on standby
		/// (i.e., later in the chain of menus).
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening")]
		private Vector2Int inventoryListStandbyPos = new Vector2Int();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the Inventory list in.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening"), Title("Timing")]
		private float inventoryListTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the Inventory list out.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening")]
		private float inventoryListTweenOutTime = 0.2f;
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the Inventory list in.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening"), Title("Easing")]
		private Ease inventoryListEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the Inventory list out.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Tweening")]
		private Ease inventoryListEaseOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects. As children. Fucked up.
		/// </summary>
		[SerializeField, TabGroup("Inventory List", "Scene References")]
		private GameObject allObjects;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform to be used for positional tweens.
		/// </summary>
		private RectTransform MainPositionPivotRectTransform => this.allObjects.GetComponent<RectTransform>();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Kills all tweens operating on this object.
		/// </summary>
		private void KillAllTweens() {
		
			// Stop any tweens on the position pivot.
			this.MainPositionPivotRectTransform.DOKill(complete: true);
			
		}
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
		
			// Kill all tweens.
			this.KillAllTweens();
			
			// Snap the positional pivot to the hiding position.
			this.MainPositionPivotRectTransform.anchoredPosition = this.inventoryListHidingPos;
			
			// Clear the menu list itself.
			this.ClearMenuList();
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		/// <summary>
		/// Preps this object to be used with the shop menu params specified.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Prepare(ShopMenuParams shopMenuParams) {
			throw new NotImplementedException("SET THE DEFAULT COLORS");
		}
		#endregion
		
	}
}