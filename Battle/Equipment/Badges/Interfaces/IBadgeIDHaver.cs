using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// A way to identify badge-related classes that should be able to provide their ID.
	/// </summary>
	public interface IBadgeIDHaver {
		/// <summary>
		/// The ID that should be generated from a class that implements this interface.
		/// </summary>
		BadgeID BadgeID { get; }
	}
}