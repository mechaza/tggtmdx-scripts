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
	/// Provides access to the visuals that help display/animate the cursor attached to a board piece.
	/// </summary>
	public class DummyPieceCursor : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Provides access to all the other visuals for this cursor.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Scene References")]
		private GameObject allObjects;
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Enables the cursor's visuals.
		/// </summary>
		public void EnableCursor() {
			// For right now, just toggle the allObjects object.
			this.allObjects.SetActive(true);
		}
		/// <summary>
		/// Disables the cursor's visuals.
		/// </summary>
		public void DisableCursor() {
			// For right now, just toggle the allObjects object.
			this.allObjects.SetActive(false);
		}
		#endregion
		
	}
}