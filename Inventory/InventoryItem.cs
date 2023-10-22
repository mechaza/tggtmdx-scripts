using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI;
using Grawly.Battle;
using Grawly.UI.MenuLists;

namespace Grawly {

	/// <summary>
	/// Basically just a way to represent a key/value pair of a battle behavior and its count so I can use it in a menu.
	/// I mighr repurpose it for something else later but. I don't know.
	/// </summary>
	public class InventoryItem : IMenuable {


		#region FIELDS - STATE
		/// <summary>
		/// The behavior that should be assocaited with this inventory item.
		/// </summary>
		public BattleBehavior behavior { get; private set; }
		/// <summary>
		/// The variables to reference when pulling the count from.
		/// </summary>
		private GameVariables variables;
		/// <summary>
		/// The number of those behaviors left.
		/// </summary>
		public int Count {
			get {
				// Return the value thats stored in the dictionary that was passed along with this inventory item.
				return this.variables.Items[this.behavior];
			} set {
				Debug.Log("PAUSE MENU: Count for InventoryItem with behavior " + this.behavior.behaviorName + " is now set to " + value + ". This will also set the value in the Variables passed in. In production, this means the GameController's variables were modified.");
				this.variables.Items[this.behavior] = value;
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Basically just a way to represent a key/value pair of a battle behavior and its count so I can use it in a menu.
		/// I mighr repurpose it for something else later but. I don't know.
		/// </summary>
		public InventoryItem(BattleBehavior behavior, GameVariables variables) {
			this.behavior = behavior;
			this.variables = variables;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString {
			get {
				return this.behavior.behaviorName;
			}
		}
		public string QuantityString {
			get {
				return "x" + this.Count.ToString();
			}
		}
		public string DescriptionString {
			get {
				return this.behavior.description;
			}
		}
		public Sprite Icon {
			get {
				return this.behavior.Icon;
			}
		}
		#endregion


	}
}