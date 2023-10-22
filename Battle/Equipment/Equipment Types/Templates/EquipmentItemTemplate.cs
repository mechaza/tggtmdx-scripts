using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.UI.MenuLists;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment {
	
	/// <summary>
	/// Contains the data that should be used to construct and define an EquipmentItem.
	/// </summary>
	public abstract class EquipmentItemTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The name that this item should be called.
		/// </summary>
		[Title("Toggles"), OdinSerialize]
		public string ItemName { get; protected set; } = "";
		/// <summary>
		/// The description for this item.
		/// </summary>
		[OdinSerialize]
		public string ItemDescription { get; protected set; } = "";
		#endregion

		

		
	}
}