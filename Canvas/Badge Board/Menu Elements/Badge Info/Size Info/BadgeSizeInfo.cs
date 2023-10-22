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
	/// A class to help encapsulate the details regarding a badge's size in the info box.
	/// </summary>
	public class BadgeSizeInfo : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the visual objects as children.
		/// </summary>
		[SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// A list of slots for each of the fills on the info box.
		/// I'm drunk right now.
		/// </summary>
		[SerializeField]
		private List<BadgeSizeInfoSlot> fillSlots = new List<BadgeSizeInfoSlot>();
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			foreach (BadgeSizeInfoSlot fillSlot in this.fillSlots) {
				fillSlot.ResetState();
			}
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Present(BadgeBoardParams boardParams) {
			/*foreach (BadgeSizeInfoSlot fillSlot in this.fillSlots) {
				fillSlot.Present(boardParams: boardParams);
			}*/
		}
		/// <summary>
		/// Dismisses this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			/*foreach (BadgeSizeInfoSlot fillSlot in this.fillSlots) {
				fillSlot.Dismiss(boardParams: boardParams);
			}*/
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the size info based on the badge passed in.
		/// </summary>
		/// <param name="badge"></param>
		public void BuildSizeInfo(Badge badge) {
			
			// Reset the state of this size info.
			this.ResetState();
			
			// Get the fill coordinates that the badge contains.
			List<Vector2Int> badgeFillCoordinates = badge.BaseLocalFillPositions;

			// Get the size info slots that match the fill positions in the badge.
			List<BadgeSizeInfoSlot> matchingSlots = this.fillSlots
				.Where(fs => badgeFillCoordinates.Contains(fs.LocalCoordinate))
				.ToList();

			// Show the slot.
			foreach (BadgeSizeInfoSlot fillSlot in matchingSlots) {
				fillSlot.ShowSlot();
			}
			
		}
		#endregion
		
	}
}