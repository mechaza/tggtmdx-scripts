using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// Contains the data required for building a shop and what items are available within.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shop/Store Template")]
	public class ShopStoreTemplate : SerializedScriptableObject, IStoreCatalog {

		#region INTERFACE PROPERTIES - ISTORECATALOG
		/// <summary>
		/// The items that should be available in a store.
		/// </summary>
		public List<ShopItem> ShopItems {
			get {
				throw new NotImplementedException("ADD THIS");
			}
		}
		#endregion

	}
}