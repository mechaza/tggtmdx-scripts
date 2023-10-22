using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle {

	/// <summary>
	/// The different kinds of moods that may contribute to a combatant's behaviors.
	/// </summary>
	public enum CombatantMoodType {
		Scared = 0,
		Angry = 1,
		Eager = 2,
		Happy = 3,
	}

	/// <summary>
	/// How filled up the specified mood is.
	/// </summary>
	public enum CombatantMoodSeverityType {
		Stage1 = 0,
		Stage2 = 1,
		Stage3 = 2,
		Stage4 = 3,
	}

}