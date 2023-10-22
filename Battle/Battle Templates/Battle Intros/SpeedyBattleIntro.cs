using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Intros.Special {

	/// <summary>
	/// Transitions seamlessly from whatever camera is currently active to the ones needed for battle.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Intended for use with auto battle and debugging. Does not look very good.")]
	public class SpeedyBattleIntro : BattleIntro {

		#region BATTLE PREPARATION
		/// <summary>
		/// Preps the battle for use. Replaces what was the old routine in DungeonController.
		/// </summary>
		/// <param name="template"></param>
		public override void PlayIntro(BattleTemplate template, BattleParams battleParams) {
			// throw new System.NotImplementedException();
			GameController.Instance.StartCoroutine(this.StartBattleRoutine(template: template, battleParams: battleParams));
		}
		/// <summary>
		/// This is the main driver behind transitioning between the dungeon scene and the battle scene
		/// </summary>
		/// <returns></returns>
		private IEnumerator StartBattleRoutine(BattleTemplate template, BattleParams battleParams) {
			Debug.Log("Start Battle Routine is now running.");

			// Set the isBattling variable so that anything that accesses will now that the BattleController is in the middle of a battle.
			// BattleController.Instance.IsBattling = true;

			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

			// Spawn the enemies stored in the enemy that the player just collided with
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

			// Instruct the battle camera controller to update the new group targets
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);

			// Set up the results for the battle.
			// BattleController.Instance.SetBattleResults(new List<EnemySpawnTemplate>(BattleController.Instance.battleTemplateQueue));
			// (This gets done in the battle controller itself now.)

			// Fade the player cam and the battle cam
			DungeonPlayer.playerModel.SetActive(false);
			DungeonPlayer.playerCamera.GetComponent<AudioListener>().enabled = false;

			// yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: DungeonPlayer.playerCamera);
			// yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: Gauntlet.Legacy.LegacyBattleGauntletLevelSelectController.Instance.mainCam);
			yield return BattleCameraController.Instance.TransitionToBattle(
				playerCamera: DungeonPlayer.playerCamera,
				delayTime: 0.05f);

			// Initialize the battle
			BattleController.Instance.OnIntroCompleted();

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

		/*#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Transitions seamlessly from whatever camera is currently active to the ones needed for battle.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion*/

	}


}