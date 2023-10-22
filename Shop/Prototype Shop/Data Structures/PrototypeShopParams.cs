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
using Grawly.Shop.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// A basic data structure to encapsulate the data required for the prototype shop.
	/// </summary>
	public class PrototypeShopParams {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The items that should be displayed on the prototype shop menu.
		/// </summary>
		public List<PrototypeShopItem> CurrentShopItems { get; private set; } = new List<PrototypeShopItem>();
		/// <summary>
		/// The GameVariables that should be used with these shop params.
		/// </summary>
		public GameVariables CurrentGameVariables { get; private set; }
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a set of parameters for use in the prototype shop menu.
		/// </summary>
		/// <param name="prototypeStoreTemplate"></param>
		/// <param name="gameVariables"></param>
		public PrototypeShopParams(PrototypeShopStoreTemplate prototypeStoreTemplate, GameVariables gameVariables) {
			
			// Generate the shop items that should be used for these parameters.
			this.CurrentShopItems = prototypeStoreTemplate.GeneratePrototypeShopItems();
			
			// Save the variables as well.
			this.CurrentGameVariables = gameVariables;
			
		}
		#endregion
		
	}
}