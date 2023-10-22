using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.BattleMenu;
using Grawly.Calendar;
using Grawly.UI;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Returns the player to the designated area.
	/// </summary>
	[System.Serializable]
	public class Gohom : PauseBehaviorFunction {

		/*#region FIELDS - TOGGLES
		/// <summary>
		/// The scene that should be teleported to.
		/// </summary>
		[SerializeField]
		private string destinationScene = "";
		#endregion*/

		#region FIELDS - TOGGLES
		/// <summary>
		/// The scene that should be teleported to.
		/// </summary>
		[SerializeField]
		private LocationType targetLocation = LocationType.DungeonLobby;
		#endregion
		
		#region FIELDS - INHERITED
		/// <summary>
		/// This is asynchronous because I don't want to fuck up the pause menu.
		/// </summary>
		public override bool IsAsynchronous {
			get {
				return true;
			}
		}
		#endregion

		#region FUNCTION
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior self) {
			Debug.Log("Sending ABORT event to the pause menu.");
			PauseMenuController.instance.AbortPauseMenu();
			PauseMenuController.instance.Close();
			GameController.Instance.WaitThenRun(timeToWait: 0.1f, action: () => {
				SceneController.instance.BasicLoadSceneWithFade(locationType: this.targetLocation);
			});
		}
		#endregion

		#region INSPECTOR STUFF
		private static string descriptionText = "Returns the player to the designated area.";
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

	}


}