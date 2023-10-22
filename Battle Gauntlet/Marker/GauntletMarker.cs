using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.Gauntlet.Nodes;

namespace Grawly.Gauntlet {

	/// <summary>
	/// A visual representation of the player on the gauntlet screen.
	/// </summary>
	public abstract class GauntletMarker : MonoBehaviour {

		public static GauntletMarker instance;

		#region UNITY CALLS
		/// <summary>
		/// Assigns the Instance to be this marker on awake.
		/// </summary>
		protected virtual void Awake() {
			instance = this;
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Moves the marker over to the specified node.
		/// </summary>
		/// <param name="node">The node to move to.</param>
		public abstract void MoveToNode(GauntletNode node);
		#endregion

	}


}