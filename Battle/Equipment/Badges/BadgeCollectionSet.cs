using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Equipment.Badges  {
	
	/// <summary>
	/// Encapsulates the badges that the party will be carrying at any given time.
	/// </summary>
	public class BadgeCollectionSet {

		#region PROPERTIES - STATE
		/// <summary>
		/// Does this set actually have badges in it?
		/// </summary>
		public bool HasBadges => this.AllBadges.Count > 0;
		#endregion
		
		#region FIELDS - BADGES
		/// <summary>
		/// All of the badges that belong inside this set.
		/// </summary>
		public List<Badge> AllBadges { get; private set; } = new List<Badge>();
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a blank collection set.
		/// </summary>
		public BadgeCollectionSet() {
			this.AllBadges = new List<Badge>();
		}
		/// <summary>
		/// Creates a collection set from the template specified.
		/// </summary>
		/// <param name="collectionSetTemplate">The template that should be used in creating this set.</param>
		public BadgeCollectionSet(BadgeCollectionSetTemplate collectionSetTemplate) {
			// Transform the badge templates in the set template into usable badges.
			this.AllBadges = collectionSetTemplate.BadgeTemplates
				.Select(bt => new Badge(bt))
				.ToList();
		}
		/// <summary>
		/// Creates a collection set from one that was previously saved out to disk.
		/// </summary>
		/// <param name="serializableCollectionSet">The serialized data to construct this set from.</param>
		public BadgeCollectionSet(SerializableBadgeCollectionSet serializableCollectionSet) {
			// Transform the list of serializable badges into one that can actually be used in this set.
			this.AllBadges = serializableCollectionSet.allSerializableBadges
				.Select(sb => new Badge(sb))
				.ToList();
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the badge associated with the provided ID.
		/// </summary>
		/// <param name="badgeID">The ID identifying the requested badge.</param>
		/// <returns>The badge whose ID matches the one passed as a parameter.</returns>
		public Badge GetBadge(BadgeID badgeID) {
			// Return the first badge whos ID matches the one passed in.
			// I want this to fail if no badge exists.
			return this.AllBadges.First(b => b.BadgeID.Equals(badgeID));
		}
		/// <summary>
		/// Gets all the badges in this set of the provided elemental type.
		/// </summary>
		/// <param name="elementType">The element type of the desired badges.</param>
		/// <returns>All badges in this set which have the specified element.</returns>
		public List<Badge> GetBadges(ElementType elementType) {
			// Filter the badges to find the ones of the provided element type.
			return this.AllBadges
				.Where(b => b.ElementType == elementType)
				.ToList();
		}
		#endregion
		
	}
}