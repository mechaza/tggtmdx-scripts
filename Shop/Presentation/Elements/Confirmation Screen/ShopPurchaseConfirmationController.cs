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
	/// Helps manage the elements on the confirmation screen of the shop menu.
	/// </summary>
	public class ShopPurchaseConfirmationController : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// yes
		/// </summary>
		[SerializeField, TabGroup("Confirmation", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The purchase summary for the top item. (i.e., the 'before' item)
		/// </summary>
		[SerializeField, TabGroup("Confirmation", "Scene References")]
		private ShopPurchaseItemSummary topPurchaseSummary;
		/// <summary>
		/// The purchase summary for the bottom item. (i.e., the 'after' item)
		/// </summary>
		[SerializeField, TabGroup("Confirmation", "Scene References")]
		private ShopPurchaseItemSummary bottomPurchaseSummary;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
		
			// Reset the top and bottom summaries.
			this.topPurchaseSummary.ResetState();
			this.bottomPurchaseSummary.ResetState();
			
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