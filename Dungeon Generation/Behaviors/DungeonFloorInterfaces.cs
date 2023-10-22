using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// An interface that gets called when the player arrives at this floor for the first time.
	/// </summary>
	public interface IOnFloorFirstTime {
		/// <summary>
		/// A function to run when the player arrives to the floor for the first time.
		/// </summary>
		void OnFloorFirstTime();
	}


}