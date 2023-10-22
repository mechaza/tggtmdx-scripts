using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Dungeon {

	/// <summary>
	/// I mostly use this during cinemachine timelines to manually enable/disable the player's FSM state.
	/// I can't figure out how to do this beyond enabling/disabling this object.
	/// </summary>
	public class DungeonPlayerTimelineHelper : MonoBehaviour {

		#region UNITY CALLS
		private void OnEnable() {
			this.GetComponentInParent<DungeonPlayer>().SetFSMState(DungeonPlayerStateType.Free);
		}
		private void OnDisable() {
			this.GetComponentInParent<DungeonPlayer>().SetFSMState(DungeonPlayerStateType.Wait);
		}
		#endregion
	}


}