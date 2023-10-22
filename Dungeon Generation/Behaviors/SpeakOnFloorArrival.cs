using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// A kind of behavior that can accept events based on certain criteria.
	/// </summary>
	[System.Serializable]
	public class SpeakOnFloorArrival : DungeonFloorBehavior, IOnFloorFirstTime {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The script to read.
		/// </summary>
		[SerializeField]
		private TextAsset scriptToRead;
		#endregion

		#region FIELDS - INTERFACE IMPLEMENTATION - IONFLOORFIRSTTIME
		/// <summary>
		/// Opens up the chat when the player arrives at a floor for the first time.
		/// </summary>
		public void OnFloorFirstTime() {
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
			ChatControllerDX.GlobalOpen(
				textAsset: this.scriptToRead,
				chatOpenedCallback: delegate { },
				chatClosedCallback: delegate {
					DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
				});
		}
		#endregion

	}


}