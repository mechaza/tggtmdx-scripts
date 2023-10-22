using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;
using Grawly.Dungeon.UI;
using Grawly.Menus;

namespace Grawly.Battle.Intros.Standard {

	/// <summary>
	/// The intro to use when launching a battle from the battle test screen.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The intro to use when launching a battle from the battle test screen.")]
	public class BattleTestIntro : BattleIntro {

		#region FIELDS - SETTINGS
		/// <summary>
		/// Should the battle cameras copy the settings from the player cam?
		/// Helpful when like, adjusting for the background culling or whatever.
		/// </summary>
		[SerializeField]
		private bool copyPlayerCamSettings = true;
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

			NotificationController.Instance?.Dismiss();
			// Dismiss the minimap if its in the scene.
			// MiniMapDX.Instance?.DismissMap();

			// If this camera should take its values from the player cam, do so.
			if (this.copyPlayerCamSettings == true) {
				BattleCameraController.Instance.CopyCameraSettings(cam: BattleTestController.Instance.MenuCamera);
			}

			// Do some things to have a little intro effect
			AudioController.instance.PlaySFX(type: SFXType.EnemyEncounter, scale: 1f);
			

			// Remember the AudioController's pause time for the dungeon music and reset the playback time
			// Note: I'm sorta taking a bit more specific control over the timing here. Just roll with it.
			if (template.PlaysBattleMusic == true) {
				AudioController.instance.PauseMusic(track: 0, fade: 0f);
			}
			
			// This takes about two seconds I believe.
			// GameController.Instance.StartCoroutine(DungeonPlayer.Instance.PlayBattleTransitionEffectRoutine());
			BattleTestController.Instance.TransitionFlasherImage.DOKill(complete: true);
			BattleTestController.Instance.TransitionFlasherImage.DOFade(endValue: 1f, duration: 0.5f).SetEase(Ease.Linear);
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
			// NotificationController.Instance?.Dismiss();
			
			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);

			// Spawn the enemies stored in the enemy that the player just collided with
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);

			// Turn on the battle arena.
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);

			// Fade the player cam and the battle cam
			// DungeonPlayer.playerModel.SetActive(false);
			BattleTestController.Instance.AllObjects.SetActive(false);
			BattleTestController.Instance.MenuCamera.GetComponent<AudioListener>().enabled = false;
			
			// Perform the transition routine.
			BattleTestController.Instance.TransitionFlasherImage.DOKill(complete: true);
			BattleTestController.Instance.TransitionFlasherImage.DOFade(endValue: 0f, duration: 0.25f).SetEase(Ease.Linear);
			yield return BattleCameraController.Instance.TransitionToBattle(playerCamera: BattleTestController.Instance.MenuCamera);
			/*yield return BattleCameraController.Instance.SoftTransitionToBattle(
				playerCamera: BattleTestController.Instance.MenuCamera, 
				delayTime: 2.95f);*/

			// Initialize the battle
			BattleController.Instance.OnIntroCompleted();

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

	}


}