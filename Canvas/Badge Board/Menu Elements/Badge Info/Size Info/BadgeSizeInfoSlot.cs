using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Just a simple slot representing one block on the size info.
	/// This might be overkill. but fuck it.
	/// </summary>
	public class BadgeSizeInfoSlot : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The coordinates that this slot is meant to represent.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private Vector2Int localSlotCoordinate = new Vector2Int();
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The coordinates that this slot is meant to represent.
		/// </summary>
		public Vector2Int LocalCoordinate => this.localSlotCoordinate;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the other objects as children.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this info slot.
		/// </summary>
		public void ResetState() {
			this.HideSlot();
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Sets this slot to be turned on.
		/// </summary>
		public void ShowSlot() {
			this.allObjects.SetActive(true);
		}
		/// <summary>
		/// Sets this slot to be turned off.
		/// </summary>
		public void HideSlot() {
			this.allObjects.SetActive(false);
		}
		#endregion
		
		/*#region PRESENTATION
		/// <summary>
		/// Presents this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Present(BadgeBoardParams boardParams) {
			this.allObjects.SetActive(true);
		}
		/// <summary>
		/// Dismisses this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			this.allObjects.SetActive(false);
		}
		#endregion*/
		
	}
}