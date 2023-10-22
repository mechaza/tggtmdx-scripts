using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;
using Grawly.Dungeon.UI;
using Grawly.DungeonCrawler;
using Sirenix.Serialization;

namespace Grawly.Battle.Intros {
	
	/// <summary>
	/// An EasyBattleIntro can be inherited from to quickly create intros that don't require a lot of granular control.
	/// </summary>
	public abstract class EasyBattleIntro : BattleIntro {
		
		#region FIELDS - SETTINGS
		/// <summary>
		/// Should the battle cameras copy the settings from the player cam?
		/// Helpful when like, adjusting for the background culling or whatever.
		/// </summary>
		[OdinSerialize]
		protected bool copyPlayerCamSettings = true;
		/// <summary>
		/// The amount of time to wait before actually playing the battle music.
		/// </summary>
		[OdinSerialize]
		protected float battleMusicDelay = 1.5f;
		#endregion
		
		#region BATTLE PREPARATION
		public override void PlayIntro(BattleTemplate template, BattleParams battleParams) {
			GameController.Instance.StartCoroutine(this.StartBattleRoutine(template: template, battleParams: battleParams));
		}
		/// <summary>
		/// This can be used as a boilerplate for any kind of intro that inherits this one, really.
		/// </summary>
		/// <returns></returns>
		protected IEnumerator StartBattleRoutine(BattleTemplate template, BattleParams battleParams) {
			
			this.TweenMenus(status: false);
			this.SetPlayerState(status: false);

			if (this.copyPlayerCamSettings == true) {
				this.CopyPlayerCameraSettings();
			}
			
			AudioController.instance.PlaySFX(type: SFXType.EnemyEncounter, scale: 1f);
			
			if (template.PlaysBattleMusic == true) {
				this.PauseMusic();
			}

			this.PlayBattleTransitionEffect();
			yield return new WaitForSeconds(0.5f);

			if (template.PlaysBattleMusic == true) {
				this.PlayBattleMusic(battleTemplate: template, delay: this.battleMusicDelay);
			}
			
			yield return new WaitForSeconds(1.5f);
			this.PrepareBattleArena(battleTemplate: template, battleParams: battleParams);
			yield return this.StandardCameraTransition();
			BattleController.Instance.OnIntroCompleted();
			PlayerStatusDXController.instance.TweenSize(big: true);

		}
		#endregion

		#region COMMON CALLS - PRIOR STATE
		/// <summary>
		/// Sets whether the player should be locked/unlocked.
		/// </summary>
		/// <param name="status"></param>
		protected void SetPlayerState(bool status) {
			
			if (status == true) {
				DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Free);
				CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Free);
				
			} else {
				DungeonPlayer.Instance?.SetFSMState(DungeonPlayerStateType.Wait);
				CrawlerPlayer.Instance?.SetState(CrawlerPlayerState.Wait);
			}
			
		}
		/// <summary>
		/// Copies the settings currently on the player camera over to the battle cameras.
		/// </summary>
		protected void CopyPlayerCameraSettings() {
			BattleCameraController.Instance.CopyCameraSettings(cam: DungeonPlayer.playerCamera);
		}
		#endregion

		#region COMMON CALLS - ANIMATIONS
		/// <summary>
		/// Tweens many common menus in/out depending on the value passed in.
		/// </summary>
		/// <param name="status"></param>
		protected void TweenMenus(bool status) {
			
			if (status == true) {
				MiniMapDX.instance?.DisplayMap();
				CrawlerMiniMap.Instance?.DisplayMap();
				EnemyRadar.Instance?.Display();
				NotificationController.Instance?.Present();
			} else {
				MiniMapDX.instance?.DismissMap();
				CrawlerMiniMap.Instance?.DismissMap();
				EnemyRadar.Instance?.Dismiss();
				NotificationController.Instance?.Dismiss();
			}
			
			CalendarDateUI.instance?.Tween(status: status);
		}
		/// <summary>
		/// Plays the effect that blurs out the camera and junk.
		/// </summary>
		protected void PlayBattleTransitionEffect() {
			GameController.Instance.StartCoroutine(DungeonPlayer.Instance.PlayBattleTransitionEffectRoutine());
		}
		#endregion

		#region COMMON CALLS - AUDIO
		/// <summary>
		/// Just a quick way of pausing the music.
		/// </summary>
		/// <param name="fade">The amount of time to take to fade out the music.</param>
		protected void PauseMusic(float fade = 0f) {
			AudioController.instance.PauseMusic(track: 0, fade: fade);
		}
		/// <summary>
		/// Plays the music for this battle.
		/// </summary>
		/// <param name="battleTemplate">The template that has the type/reference to the music to play.</param>
		/// <param name="fade">The amount to fade in by.</param>
		/// <param name="delay">The amount to delay before starting.</param>
		protected void PlayBattleMusic(BattleTemplate battleTemplate, float fade = 0f, float delay = 1.5f) {
			
			if (battleTemplate.battleMusicType == BattleTemplate.BattleMusicType.Default) {
				AudioController.instance.PlayMusic(type: MusicType.Battle, track: 1, fade: fade, delay: delay);
				
			} else if (battleTemplate.battleMusicType == BattleTemplate.BattleMusicType.Override) {
				AudioController.instance.PlayMusic(audio: battleTemplate.CustomBattleMusic, track: 1, fade: fade, delay: delay);
				
			} else {
				throw new System.Exception("Could not figure out what kind of music to play!");
			}
			
		}
		#endregion
		
		#region COMMON CALLS - BATTLE ARENA
		/// <summary>
		/// Contains the general functions that prepare the battle arena.
		/// Note this is where the camera's audio listeners get manipulated.
		/// </summary>
		/// <param name="battleTemplate"></param>
		/// <param name="battleParams"></param>
		protected void PrepareBattleArena(BattleTemplate battleTemplate, BattleParams battleParams) {
			
			// Enable the battle controller
			BattleController.Instance.gameObject.SetActive(true);
			
			// Spawn the enemies stored in the enemy that the player just collided with
			BattleArenaControllerDX.instance.PrepareBattleArena(battleTemplate: battleTemplate, battleParams: battleParams);
			
			// Turn on the battle arena.
			BattleCameraController.Instance.UpdateCinemachineTargetGroup(worldEnemies: BattleArenaControllerDX.instance.ActiveWorldEnemyDXs);
			
			// Tweak the DungeonPlayer a bit.
			if (DungeonPlayer.Instance != null) {
				DungeonPlayer.playerModel.SetActive(false);
				DungeonPlayer.playerCamera.GetComponent<AudioListener>().enabled = false;
			}

			// Also tweak the crawler player.
			if (CrawlerPlayer.Instance != null) {
				CrawlerPlayer.Instance.PlayerCamera.GetComponent<AudioListener>().enabled = false;
			}
		
		}
		#endregion
		
		#region COMMON CALLS - CAMERA MANIPULATION
		/// <summary>
		/// Just plays the transition animation.
		/// </summary>
		/// <returns></returns>
		protected IEnumerator StandardCameraTransition() {
			Camera mainCam = DungeonPlayer.playerCamera ?? CrawlerPlayer.Instance?.PlayerCamera ?? Camera.main;
			yield return BattleCameraController.Instance.TransitionToBattle(playerCamera:mainCam);
			// yield return BattleCameraController.instance.TransitionToBattle(playerCamera: DungeonPlayer.playerCamera);
		}
		#endregion
		
	}
}