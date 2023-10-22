using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle {

	/// <summary>
	/// I was getting sick of having situations where a BattleBehavior was assumed but extending functionality to where I would need to get a BattleBehavior.
	/// I don't want to use strings, so I'm going to define them strictly here.
	/// Most common use will be as like. Keys in dictionaries for the DataController.
	/// </summary>
	public enum CommonBattleBehaviorType  {
		Attack = 0,
	}

}