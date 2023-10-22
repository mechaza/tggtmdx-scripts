using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle.Equipment.Badges.Behaviors;
using Sirenix.Utilities;

namespace Grawly.Battle.Equipment.Badges  {
	
	/// <summary>
	/// A very basic data structure for use only within BadgeGridTemplates for the sake of adding default badges.
	/// </summary>
	public struct DefaultBadgeCoordinates {

		#region FIELDS
		public int xPos;
		public int yPos;
		[SerializeField]
		private BadgeTemplate badgeTemplate;
		#endregion

		#region PROPERTIES
		public BadgeID BadgeID => badgeTemplate.BadgeID;
		#endregion

	}
}