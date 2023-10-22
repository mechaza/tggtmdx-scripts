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
	/// A quick way to define how much EXP it takes to level up a given badge.
	/// </summary>
	public enum BadgeMasteryType {
		None		= 0,
		VeryEasy	= 1,
		Easy		= 2,
		Medium		= 3,
		Hard		= 4,
		VeryHard	= 5,
		Master		= 6,		
	}
}