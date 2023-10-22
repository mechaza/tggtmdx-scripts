using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {

	/// <summary>
	/// Attached to the player's body and pulses periodically to check if a battle should ensue.
	/// </summary>
	public class DungeonPlayerBodyTrigger : MonoBehaviour {

		public static DungeonPlayerBodyTrigger Instance { get; private set; }

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void OnTriggerEnter(Collider other) {
			// If the other collider has a collision handler, invoke the enter method.
			foreach (IPlayerCollisionHandler handler in other.GetComponents<IPlayerCollisionHandler>()) {
				handler.OnPlayerCollisionEnter(dungeonPlayer: DungeonPlayer.Instance);
			}
			// other.GetComponent<IPlayerCollisionHandler>()?.OnPlayerCollisionEnter();
		}
		private void OnTriggerExit(Collider other) {
			// If the other collider has a collision handler, invoke the exit method.
			foreach (IPlayerCollisionHandler handler in other.GetComponents<IPlayerCollisionHandler>()) {
				handler.OnPlayerCollisionExit(dungeonPlayer: DungeonPlayer.Instance);
			}
			// other.GetComponent<IPlayerCollisionHandler>()?.OnPlayerCollisionExit();
		}
		private void OnTriggerStay(Collider other) {
			// If the other collider has a collision handler, invoke the stay method.
			foreach (IPlayerCollisionHandler handler in other.GetComponents<IPlayerCollisionHandler>()) {
				handler.OnPlayerCollisionStay(dungeonPlayer: DungeonPlayer.Instance);
			}
			// other.GetComponent<IPlayerCollisionHandler>()?.OnPlayerCollisionStay();
		}
		#endregion

	}


}