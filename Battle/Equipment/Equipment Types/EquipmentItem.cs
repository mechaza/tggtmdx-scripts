using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Equipment {
	
	/// <summary>
	/// The class that should be used at runtime that defines a piece of equipment on a Combatant.
	/// </summary>
	public abstract class EquipmentItem<T, U> where T : EquipmentItemTemplate where U : SerializableEquipmentItem {

		#region FIELDS - STATE
		/// <summary>
		/// The template that was used to build this item.
		/// </summary>
		public T ItemTemplate { get; protected set; }
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// The base constructor for an equipment item.
		/// </summary>
		/// <param name="itemTemplate">The template to build this item from.</param>
		protected EquipmentItem(T itemTemplate) {
			// son of a bitch everything is real
			this.ItemTemplate = itemTemplate;
		}
		#endregion

		
	}
	
	
}