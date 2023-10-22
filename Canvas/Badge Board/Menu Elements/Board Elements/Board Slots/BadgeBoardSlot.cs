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
	/// One of the pieces that will be placed on the board. Very cool!
	/// </summary>
	public class BadgeBoardSlot : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The coordinates of this slot in particular.
		/// </summary>
		[Title("Toggles"), SerializeField]
		private Vector2Int slotCoordinates = new Vector2Int();
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The coordinates of this slot in particular.
		/// </summary>
		public Vector2Int SlotCoordinates {
			get {
				return this.slotCoordinates;
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The object which contains all of the other objects as children.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image that displays the corners of the board slot.
		/// </summary>
		[SerializeField]
		private Image slotCornerImage;
		/// <summary>
		/// The image that displays the ghost piece on this slot.
		/// </summary>
		[SerializeField]
		private Image slotGhostPieceImage;
		/// <summary>
		/// The RectTransform that contains the anchored position any board pieces should snap to when placed.
		/// </summary>
		[SerializeField]
		private RectTransform slotPivotTargetRectTransform;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The RectTransform representing the anchored position where any potential pieces should be placed on.
		/// </summary>
		public RectTransform TargetRectTransform => this.slotPivotTargetRectTransform;
		#endregion
		
		#region DISPLAY
		/// <summary>
		/// Hides the slot from the board.
		/// </summary>
		public void HideSlot() {
			this.allObjects.SetActive(false);
		}
		/// <summary>
		/// Shows the slot on the board.
		/// </summary>
		public void ShowSlot() {
			this.allObjects.SetActive(true);
		}
		#endregion
		
	}
	
}