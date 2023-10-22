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
using Grawly.DungeonCrawler;

namespace Grawly.Battle.Intros.Standard {

	/// <summary>
	/// This effectively replaces what the old routine in DungeonController was.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard battle intro. Will get used the majority of the time. Replaces what was previously hard coded in the DungeonController.")]
	public class CrawlerBattleIntro : BattleIntro {


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
			
			// Dismiss the prompt. This should work regardless of it was shown or not.
			// TODO: Make it re-display the prompt upon dismissing, if one existed.
			CrawlerActionPrompt.Instance?.Dismiss();
			
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Wait);
			
			// Do some things to have a little intro effect
			AudioController.instance.PlaySFX(type: SFXType.EnemyEncounter, scale: 1f);
			
			// Remember the AudioController's pause time for the dungeon music and reset the playback time
			// Note: I'm sorta taking a bit more specific control over the timing here. Just roll with it.
			if (template.PlaysBattleMusic == true) {
				AudioController.instance.PauseMusic(track: 0, fade: 0f);
			}

			// This takes about two seconds I believe.
			GameController.Instance.StartCoroutine(CrawlerPlayer.Instance.PlayBattleTransitionEffectRoutine());
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

			// Tween the UI out.
			CalendarDateUI.instance?.Tween(status: false);
			CrawlerMiniMap.Instance?.DismissMap();
			EnemyRadar.Instance?.Dismiss();
			NotificationController.Instance?.Dismiss();
			
			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

		
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

		
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);

			CrawlerPlayer.Instance.PlayerCamera.GetComponent<AudioListener>().enabled = false;
			
			// If this battle has an advantage type...
			if (battleParams.HasAdvantageType == true) {
				// ...present the appropriate alert.
				CombatantAdvantageAlertController.Instance.DisplayAlert(advantageType: battleParams.BattleAdvantageType);
			}
			
			yield return BattleCameraController.Instance.TransitionToBattle(playerCamera:CrawlerPlayer.Instance.PlayerCamera);

			BattleController.Instance.OnIntroCompleted();

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

	}


}