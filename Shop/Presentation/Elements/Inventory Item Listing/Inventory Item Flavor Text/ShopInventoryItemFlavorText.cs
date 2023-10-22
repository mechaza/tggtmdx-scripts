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
	/// Controls how the flavor text of whatever inventory item is highlighted in the shop should be displayed.
	/// </summary>
	public class ShopInventoryItemFlavorText : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// you know the drill.
		/// </summary>
		[SerializeField, TabGroup("Flavor", "Scene References")]
		private GameObject allObjects;
		#endregion

	}
}