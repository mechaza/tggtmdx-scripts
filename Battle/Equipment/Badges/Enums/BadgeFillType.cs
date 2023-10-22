using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Battle.Equipment.Badges {
	
	/// <summary>
	/// The different ways a badge can "fill" in a grid.
	/// </summary>
	public enum BadgeFillType {
		Empty		= 0,	// Empty space that doesn't interfere when being put in a grid.
		Filled		= 1,	// Space that defines the shape of this badge.
	}
	
}