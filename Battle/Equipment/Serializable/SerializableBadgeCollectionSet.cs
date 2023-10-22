using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A way to save out a badge collection set to disk.
	/// </summary>
	[System.Serializable]
	public class SerializableBadgeCollectionSet {

		#region FIELDS - STATE
		/// <summary>
		/// A list of serializable badges that can be saved out to disk.
		/// </summary>
		[SerializeField]
		public List<SerializableBadge> allSerializableBadges = new List<SerializableBadge>();
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// This constructor is usually used when creating a gamesave.
		/// </summary>
		public SerializableBadgeCollectionSet() {
			this.allSerializableBadges = new List<SerializableBadge>();
		}
		/// <summary>
		/// Create a new serializable badge collection set from one that already exists.
		/// </summary>
		/// <param name="badgeCollectionSet"></param>
		public SerializableBadgeCollectionSet(BadgeCollectionSet badgeCollectionSet) {
			// Transform the badges inside the collection set into a list of serializable badges.
			this.allSerializableBadges = badgeCollectionSet.AllBadges
				.Select(b => new SerializableBadge(b))
				.ToList();
		}
		#endregion
		
	}
}