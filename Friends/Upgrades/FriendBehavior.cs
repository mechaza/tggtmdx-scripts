using System.Collections;
using System.Collections.Generic;
using Grawly.Battle.Modifiers;
using UnityEngine;

namespace Grawly.Friends.Behaviors {
	
	/// <summary>
	/// An upgrade to apply to the whole game.
	/// </summary>
	public abstract class FriendBehavior {

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The name of the behavior.
		/// </summary>
		public abstract string BehaviorName { get; }
		/// <summary>
		/// The description of the behavior.
		/// </summary>
		public abstract string BehaviorDescription { get; }
		#endregion
		
		#region CLONING
		/// <summary>
		/// Creates a clone of this friend upgrade.
		/// </summary>
		/// <returns></returns>
		public FriendBehavior Clone() {
			return (FriendBehavior)this.MemberwiseClone();
		}
		#endregion
		
	}

}