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
	/// Displays the amount of money on hand in the prototype shop.
	/// </summary>
	public class PrototypeMoneyCounter : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label that displays how much cash the player has.
		/// </summary>
		[SerializeField, TabGroup("Counter", "Scene References")]
		private SuperTextMesh moneyCountLabel;
		#endregion

		#region BUILDING
		/// <summary>
		/// Updates the counter to show the amount of funds available to the player.
		/// </summary>
		/// <param name="gameVariables"></param>
		public void UpdateCounter(GameVariables gameVariables) {
			// Cascade down using the funds in the variables.
			this.UpdateCounter(availableFunds: gameVariables.Money);
		}
		/// <summary>
		/// Updates the counter to show the amount of funds available to the player.
		/// </summary>
		/// <param name="availableFunds"></param>
		public void UpdateCounter(int availableFunds) {
			this.moneyCountLabel.Text = availableFunds.ToString();
		}
		#endregion

	}
}