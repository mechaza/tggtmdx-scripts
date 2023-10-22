using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A way of saving a badge out to disk.
	/// </summary>
	[System.Serializable]
	public class SerializableBadge : IBadgeIDHaver {

		#region FIELDS - STATE
		/// <summary>
		/// A very basic way of storing the ID of a badge without exposing it.
		/// Gets used in actually constructing the ID.
		/// </summary>
		[SerializeField]
		private int rawBadgeID = 0;
		/// <summary>
		/// The current amount of experience allocated to this badge
		/// </summary>
		[SerializeField]
		public int currentExperience = 0;
		#endregion
		
		#region PROPERTIES - IBADGEIDHAVER
		/// <summary>
		/// The ID that helps to identify the badge being represented by this class.
		/// </summary>
		public BadgeID BadgeID {
			get {
				return new BadgeID() {
					IDNumber = this.rawBadgeID
				};
			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Creates a badge to be saved to disk from one that currently exists.
		/// </summary>
		/// <param name="badge"></param>
		public SerializableBadge(Badge badge) {
			// Save the raw ID as well as the current experience.
			this.rawBadgeID = badge.BadgeID.IDNumber;
			this.currentExperience = badge.BadgeExperience;
		}
		#endregion
		
	}
	
}