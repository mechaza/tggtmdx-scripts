using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.Battle.Equipment.Badges;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Equipment {

	/// <summary>
	/// The class from which other modifiers should inherit from.
	/// </summary>
	public interface IEquipmentComponent {
		
	}
	
	/// <summary>
	/// Anything that has a badge grid needs to implement this.
	/// </summary>
	public interface IBadgeGridHaver : IEquipmentComponent {

		/// <summary>
		/// The badge grid that the object should have.
		/// </summary>
		BadgeGrid BadgeGrid { get; set; }
		
	}
	
	#region GENERAL
	
	#endregion

	#region BADGES
	
	#endregion
	
}