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
using Sirenix.Serialization;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// A template to be used to create an item inside the prototype shop.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shop/Prototype/Prototype Store Template")]
	public class PrototypeShopStoreTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// A list of templates that should be used to create shop items at runtime.
		/// </summary>
		[OdinSerialize, TabGroup("Store", "Toggles")]
		private List<PrototypeShopItemTemplate> PrototypeShopItemTemplates { get; set; } = new List<PrototypeShopItemTemplate>();
		#endregion

		#region GENERATORS
		/// <summary>
		/// Generates a list of shop items to use in the prototype menu.
		/// </summary>
		public List<PrototypeShopItem> GeneratePrototypeShopItems() {
			// Go through the item templates and generate shop items from them.
			return this.PrototypeShopItemTemplates
				.Select(it => it.GeneratePrototypeItem())
				.ToList();
		}
		#endregion
		
	}
}