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
	/// Contains a collection of badges that should be added to the player's inventory.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Badges/Badge Collection Set Template")]
	public class BadgeCollectionSetTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The badge templates that should be used for creating the set spawned from this template.
		/// </summary>
		[OdinSerialize]
		public List<BadgeTemplate> BadgeTemplates { get; private set; } = new List<BadgeTemplate>();
		#endregion
		
	}
}