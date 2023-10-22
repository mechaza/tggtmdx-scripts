using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;
using Grawly.Dungeon.UI;

namespace Grawly.Battle.Intros.Standard {

	/// <summary>
	/// This effectively replaces what the old routine in DungeonController was.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard battle intro. Will get used the majority of the time. Replaces what was previously hard coded in the DungeonController.")]
	public class StandardBattleIntro : BattleIntro {

		#region FIELDS - SETTINGS
		/// <summary>
		/// Should the battle cameras copy the settings from the player cam?
		/// Helpful when like, adjusting for the background culling or whatever.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private bool copyPlayerCamSettings = true;
		/// <summary>
		/// The amount of time to wait before actually playing the battle music.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private float battleMusicDelay = 1.5f;
		/// <summary>
		/// Should the playback lock on the AudioController be forced to a locked state upon starting battle?
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("Should the playback lock on the AudioController be forced to a locked state upon starting battle?")]
		private bool forceMusicPlaybackLocked = false;
		#endregion

		#region BATTLE PREPARATION
		/// <summary>
		/// Preps the battle for use. Replaces what was the old routine in DungeonController.
		/// </summary>
		/// <param name="template"></param>
		public override void PlayIntro(BattleTemplate template, BattleParams battleParams) {
			GameController.Instance.StartCoroutine(this.StartBattleRoutine(template: template, battleParams: battleParams));
		}
		/// <summary>
		/// This is the main driver behind transitioning between the dungeon scene and the battle scene
		/// </summary>
		/// <returns></returns>
		private IEnumerator StartBattleRoutine(BattleTemplate template, BattleParams battleParams) {
			Debug.Log("Start Battle Routine is now running.");

			// Dismiss the minimap if its in the scene.
			MiniMapDX.instance?.DismissMap();

			// If this camera should take its values from the player cam, do so.
			if (this.copyPlayerCamSettings == true) {
				BattleCameraController.Instance.CopyCameraSettings(cam: DungeonPlayer.playerCamera);
			}

			// Do some things to have a little intro effect
			AudioController.instance.PlaySFX(type: SFXType.EnemyEncounter, scale: 1f);
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);

			// Remember the AudioController's pause time for the dungeon music and reset the playback time
			// Note: I'm sorta taking a bit more specific control over the timing here. Just roll with it.
			if (template.PlaysBattleMusic == true) {
				AudioController.instance.PauseMusic(track: 0, fade: 0f);
			}

			// This takes about two seconds I believe.
			GameController.Instance.StartCoroutine(DungeonPlayer.Instance.PlayBattleTransitionEffectRoutine());
			yield return new WaitForSeconds(0.5f);

			// Start playing the music if that's what the setting was.
			if (template.PlaysBattleMusic == true) {
				if (template.battleMusicType == BattleTemplate.BattleMusicType.Default) {
					AudioController.instance.PlayMusic(type: MusicType.Battle, track: 1, fade: 0f, delay: this.battleMusicDelay);
				} else if (template.battleMusicType == BattleTemplate.BattleMusicType.Override) {
					AudioController.instance.PlayMusic(audio: template.CustomBattleMusic, track: 1, fade: 0f, delay: this.battleMusicDelay);
				} else {
					throw new System.NotImplementedException("Could not figure out what kind of music to play!");
				}
			}

			// Filling up the remainder of whats above.
			yield return new WaitForSeconds(1.5f);

			// Tween the calendar UI out.
			CalendarDateUI.instance?.Tween(status: false);
			NotificationController.Instance?.Dismiss();
			
			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

			// Spawn the enemies stored in the enemy that the player just collided with
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

			// Turn on the battle arena.
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);

			// Fade the player cam and the battle cam
			DungeonPlayer.playerModel.SetActive(false);
			DungeonPlayer.playerCamera.GetComponent<AudioListener>().enabled = false;

			// If this battle has an advantage type...
			if (battleParams.HasAdvantageType == true) {
				// ...present the appropriate alert.
				CombatantAdvantageAlertController.Instance.DisplayAlert(advantageType: battleParams.BattleAdvantageType);
			}
			
			// Perform the transition routine.
			yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: DungeonPlayer.playerCamera);

			// If specified to do so, set the lock on the AudioController which locks music playback.
			if (this.forceMusicPlaybackLocked == true) {
				AudioController.instance.SetMusicPlaybackLock(playbackLockType: MusicPlaybackLockType.Locked);
			}
			
			// Initialize the battle
			BattleController.Instance.OnIntroCompleted();

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		private string FoldoutGroupTitle {
			get {
				return "Advanced Toggles";
			}
		}
		#endregion
		
	}


}