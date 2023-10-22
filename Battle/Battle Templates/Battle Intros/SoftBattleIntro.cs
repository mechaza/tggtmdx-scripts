using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Grawly.Gauntlet;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Intros.Special {

	/// <summary>
	/// Transitions seamlessly from whatever camera is currently active to the ones needed for battle.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Transitions seamlessly from whatever camera is currently active to the ones needed for battle.")]
	public class SoftBattleIntro : BattleIntro {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to wait until finally starting the battle.
		/// </summary>
		[SerializeField, TabGroup("Intro", "Toggles")]
		private float delayTime = 4f;
		#endregion

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

			GauntletController.instance?.GauntletNodes.Select(n => n.GetComponent<Selectable>()).ToList().ForEach(s => s.enabled = false);
			// BattleController.Instance.ResetBattleControllerState(battleTemplate: template);

			// Hide the title.
			GauntletMenuController.instance?.NodeTitle.SetVisualsActive(false);


			// Set the isBattling variable so that anything that accesses will now that the BattleController is in the middle of a battle.
			// BattleController.Instance.IsBattling = true;

			// Turn off the Dungeon HUD.
			// CanvasController.Instance.SetDungeonHUDCanvas(false);
			// Debug.Log("NOTE: This is where the dungeon hud canvas used to be turned off.");

			// Tween the calendar UI out.
			// CalendarDateUI.Instance?.Tween(status: false);

			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

			// Spawn the enemies stored in the enemy that the player just collided with
			// BattleController.Instance.worldEnemies = LegacyBattleArenaController.Instance.PrepareWorldEnemySprites(enemyTemplates: template.EnemyTemplates);
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

			// Turn on the battle arena.
			// LegacyBattleArenaController.Instance.SetBattleArenaActive(true);

			// Instruct the battle camera controller to update the new group targets
			// BattleCameraController.Instance.UpdateCinemachineTargetGroup(BattleController.Instance.worldEnemies);
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);


			// Gauntlet.Legacy.LegacyBattleGauntletLevelSelectController.Instance.mainCam.GetComponent<AudioListener>().enabled = false;
			GauntletController.instance.MainCamera.GetComponent<AudioListener>().enabled = false;

			// Set up the results for the battle.
			// BattleController.Instance.SetBattleResults(new List<EnemySpawnTemplate>(BattleController.Instance.battleTemplateQueue));
			// (This gets done in the battle controller itself now.)

			// yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: DungeonPlayer.playerCamera);
			// yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: Gauntlet.Legacy.LegacyBattleGauntletLevelSelectController.Instance.mainCam);
			yield return BattleCameraController.Instance.SoftTransitionToBattle(
				playerCamera: GauntletController.instance.MainCamera,
				delayTime: this.delayTime);

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