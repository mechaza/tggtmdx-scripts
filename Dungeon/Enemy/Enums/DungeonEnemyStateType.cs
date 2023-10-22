using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon.Enemy {
	
	/// <summary>
	/// Defines the different states a DungeonEnemyDX should exhibit, across all kinds of behaviors.
	/// </summary>
	public enum DungeonEnemyStateType {
		None	= 0,
		Idle	= 1,	// just hanging out
		Patrol	= 2,	// just walking around
		Chase	= 3,	// kill
	}
}