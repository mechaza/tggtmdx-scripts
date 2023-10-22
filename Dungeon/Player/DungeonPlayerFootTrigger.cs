using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {

	/// <summary>
	/// Uh I guess this is what serves as a trigger for the player's body instead of what's in front of them like the front trigger.
	/// </summary>
	public class DungeonPlayerFootTrigger : MonoBehaviour {


		#region UNITY CALLS
		private void OnTriggerEnter(Collider other) {
			// I'm mostly just using bolt for this.
			Unity.VisualScripting.CustomEvent.Trigger(target: other.gameObject, name: "PLAYER ENTER");
		}
		private void OnTriggerExit(Collider other) {
			// I'm mostly just using bolt for this.
			Unity.VisualScripting.CustomEvent.Trigger(target: other.gameObject, name: "PLAYER EXIT");
		}
		#endregion

	}


}