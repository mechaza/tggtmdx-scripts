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
	/// Displays information on the currently highlighted item in the prototype shop.
	/// </summary>
	public class PrototypeShopItemDescription : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM that displays the discription of whatever is currently highlighted.
		/// </summary>
		[SerializeField, TabGroup("Description", "Scene References")]
		private SuperTextMesh descriptionLabel;
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Displays the information about the provided ShopItem.
		/// </summary>
		/// <param name="shopItem"></param>
		public void BuildItemDescription(ShopItem shopItem) {
			// Update the description label with the description text.
			this.descriptionLabel.Text = shopItem.DescriptionString;
		}
		#endregion
		
	}
}