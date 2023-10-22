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

namespace Grawly.Battle.Intros.Standard {

	/// <summary>
	/// This effectively replaces what the old routine in DungeonController was.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard battle intro. Will get used the majority of the time. Replaces what was previously hard coded in the DungeonController.")]
	public class GauntletBattleIntro : BattleIntro {


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

			GauntletController.instance.SetFSMState(GauntletStateType.Battle);
			
			GauntletController.instance.GauntletNodes.Select(n => n.GetComponent<Selectable>()).ToList().ForEach(s => s.enabled = false);
			// BattleController.Instance.ResetBattleControllerState(battleTemplate: template);

			// Hide the title.
			GauntletMenuController.instance.NodeTitle.SetVisualsActive(false);

			// Do some things to have a little intro effect
			AudioController.instance.PlaySFX(type: SFXType.EnemyEncounter, scale: 1f);
			
			// Remember the AudioController's pause time for the dungeon music and reset the playback time
			// Note: I'm sorta taking a bit more specific control over the timing here. Just roll with it.
			if (template.PlaysBattleMusic == true) {
				AudioController.instance.PauseMusic(track: 0, fade: 0f);
			}

			// This takes about two seconds I believe.
			yield return new WaitForSeconds(0.5f);

			// Start playing the music if that's what the setting was.
			if (template.PlaysBattleMusic == true) {
				if (template.battleMusicType == BattleTemplate.BattleMusicType.Default) {
					AudioController.instance.PlayMusic(type: MusicType.Battle, track: 1, fade: 0f, delay: 1.5f);
				} else if (template.battleMusicType == BattleTemplate.BattleMusicType.Override) {
					AudioController.instance.PlayMusic(audio: template.CustomBattleMusic, track: 1, fade: 0f, delay: 1.5f);
				} else {
					throw new System.NotImplementedException("Could not figure out what kind of music to play!");
				}
			}

			// Filling up the remainder of whats above.
			yield return new WaitForSeconds(1.5f);

			// Tween the calendar UI out.
			CalendarDateUI.instance?.Tween(status: false);

			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

			// Spawn the enemies stored in the enemy that the player just collided with
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

			// Instruct the battle camera controller to update the new group targets
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);


			GauntletController.instance.MainCamera.GetComponent<AudioListener>().enabled = false;

			yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: GauntletController.instance.MainCamera);

			BattleController.Instance.OnIntroCompleted();

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

	}


}