using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// The behavior that manages how a shuffle time card behaves at runtime.
	/// </summary>
	public abstract class ShuffleCardBehavior {

		#region CLONING
		public ShuffleCardBehavior Clone() {
			return this.MemberwiseClone() as ShuffleCardBehavior;
		}
		#endregion

	}


}