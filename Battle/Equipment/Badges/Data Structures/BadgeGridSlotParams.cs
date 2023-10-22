using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle.Equipment.Badges.Behaviors;
using Sirenix.Utilities;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// An atomic piece of data to help with calculations.
	/// </summary>
	public class BadgeGridSlotParams {

		#region FIELDS
		/// <summary>
		/// The XPosition of this slot in global space.
		/// </summary>
		public int XPos { get; }
		/// <summary>
		/// The YPosition of this slot in global space.
		/// </summary>
		public int YPos { get; }
		/// <summary>
		/// The BadgePlacement that this slot params is associated with.
		/// May be null.
		/// </summary>
		public BadgePlacement ParentBadgePlacement { get; }
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Is the position represented by these params occupied?
		/// </summary>
		public bool IsOccupied {
			get {
				// If theres a badge placement in here, it means its being occupied.
				return this.ParentBadgePlacement != null;
			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates these slot params with an empty badge parent.
		/// This is meant to represent an empty slot on the grid.
		/// </summary>
		/// <param name="xPos">The x-position of this slot on the grid.</param>
		/// <param name="yPos">The y-position of this slot on the grid.</param>
		public BadgeGridSlotParams(int xPos, int yPos) {
			this.XPos = xPos;
			this.YPos = yPos;
			this.ParentBadgePlacement = null;
		}
		/// <summary>
		/// Creates these slot params with an empty badge parent.
		/// This is meant to represent a filled slot on the grid.
		/// </summary>
		/// <param name="xPos">The x-position of this slot on the grid.</param>
		/// <param name="yPos">The y-position of this slot on the grid.</param>
		/// <param name="badgePlacement">The BadgePlacement that generated these params.</param>
		public BadgeGridSlotParams(int xPos, int yPos, BadgePlacement badgePlacement) {
			this.XPos = xPos;
			this.YPos = yPos;
			this.ParentBadgePlacement = badgePlacement;
		}
		#endregion
		
	}
	
}