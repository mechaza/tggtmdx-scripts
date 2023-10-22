using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;
using Object = System.Object;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// A data structure to make it easier to build shit for the auxiliary item.
	/// </summary>
	public class AuxiliaryItemParams {

		#region FIELDS - TOGGLES : RESOURCES
		/// <summary>
		/// The object that is passed around with these params.
		/// Note that this is private so I can unpack it on its way out.
		/// </summary>
		private Object ItemObject { get; set; } = null;
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Do these params have an item stored with them that can be retrieved?
		/// </summary>
		public bool HasItem {
			get {
				return this.ItemObject != null;
			}
		}
		#endregion
		
		#region FIELDS - TOGGLES : STRINGS
		/// <summary>
		/// The string that should be used on the primary label.
		/// </summary>
		public string ItemName { get; set; } = "";
		/// <summary>
		/// The string that should be used for the quantity/cost.
		/// </summary>
		public string ItemQuantity { get; set; } = "";
		/// <summary>
		/// The string that should be used for the description.
		/// </summary>
		public string ItemDescription { get; set; } = "";
		/// <summary>
		/// The string that should be used for the target level.
		/// (I.e., the label after the "NEXT" text.)
		/// </summary>
		public string ItemTargetLevel { get; set; } = "";
		#endregion

		#region FIELDS - TOGGLES : GRAPHICS
		/// <summary>
		/// The sprite to use for the item's icon.
		/// </summary>
		public Sprite ItemIconSprite { get; set; }
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates parameters that can be used to build the auxiliary item.
		/// </summary>
		/// <param name="itemObject"></param>
		public AuxiliaryItemParams(Object itemObject) {
			this.ItemObject = itemObject;
		}
		#endregion
		
		#region GETTERS/SETTERS
		/// <summary>
		/// Returns the resource used to create these params as the specified data type.
		/// </summary>
		/// <typeparam name="T">The type of object to retrieve.</typeparam>
		/// <returns>The object itself.</returns>
		public T GetItem<T>() {
			return (T)this.ItemObject;
		}
		/// <summary>
		/// Saves the item along with these params so it can be passed around.
		/// </summary>
		/// <param name="itemObject"></param>
		/// <typeparam name="T">The type of item being saved.</typeparam>
		public void SetItem<T>(T itemObject) {
			this.ItemObject = itemObject;
		}
		#endregion
		
	}

	
	
}