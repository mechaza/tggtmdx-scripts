using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// Anything that needs to show what it does on the BadgeInfo should implement this.
	/// </summary>
	public interface IBadgeFactHaver {
		
		/// <summary>
		/// The BadgeFacts for whatever implements this.
		/// </summary>
		List<BadgeFact> BadgeFacts { get; }
		
	}
}