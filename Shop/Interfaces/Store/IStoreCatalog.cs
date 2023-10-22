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
	/// Provides functions/data on how a store should operate.
	/// </summary>
	public interface IStoreCatalog {

		/// <summary>
		/// The items that should be displayed in a shop.
		/// </summary>
		public List<ShopItem> ShopItems { get; }

	}
}