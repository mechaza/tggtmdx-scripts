using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleMenu;
using Grawly.DungeonCrawler;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Grawly.UI;
using Grawly.Battle.Modifiers;
using Grawly.Battle.Results;
using Grawly.MiniGames.ShuffleTime;

namespace Grawly.Battle.Outros.Standard {

	/// <summary>
	/// The way in which I'll regularly be handling battle completions. Replaces what used to be FadeToResults in the dungeon controller.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard battle outro. This is what will be used the majority of the time. Replaces what used to be FadeToResults in the DungeonController.")]
	public class CrawlerBattleOutro : BattleOutro {

		#region DEBUG FILEDS - STATE
		/// <summary>
		/// The culling mask that was saved when this outro was begun.
		/// Helpful for shuffle time transitions.
		/// </summary>
		private int savedCullingMask;
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the results screen be skipped?
		/// </summary>
		[SerializeField, TabGroup("Outro", "Toggles")]
		private bool skipResults = false;
		#endregion

		#region COMPLETION
		/// <summary>
		/// The routine to run when the battle has been completed.
		/// </summary>
		/// <param name="template"></param>
		public override void PlayOutro(BattleTemplate template, BattleParams battleParams, BattleResultsSet battleResultsSet) {
			// Upon completion, go to shuffle time if enabled.
			if (template.EnableShuffleTime == true) {
				throw new NotImplementedException("Change this call to shuffle time so that it takes the battle results set.");
				GameController.Instance.StartCoroutine(this.GoToShuffleTime(template: template, battleParams: battleParams));
			} else {
				// Otherwise, just go to the results.
				GameController.Instance.StartCoroutine(this.FadeToResults(
					template: template, 
					battleParams: battleParams,
					battleResultsSet: battleResultsSet));
			}
		}
		/// <summary>
		/// The routine to run when the results screen has been finished.
		/// </summary>
		/// <param name="template"></param>
		public override void ReturnToCaller(BattleTemplate template, BattleParams battleParams) {
			// Once the results screen has been called, go back to the dungeon.
			GameController.Instance.StartCoroutine(this.BackToDungeonRoutine(template: template, battleParams: battleParams));
		}
		#endregion

		#region ROUTINES
		/// <summary>
		/// This used to be in Dungeon Controller. Now it's located here. Whoa.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		private IEnumerator FadeToResults(BattleTemplate template, BattleParams battleParams, BattleResultsSet battleResultsSet) {

			yield return new WaitForSeconds(2f);
			
			// If the template was instructed to use its own battle music, fade it out.
			if (template.PlaysBattleMusic == true) {
				AudioController.instance.StopMusic(track: 1, fade: 0.5f);
				// If not skipping results, play the results theme.
				if (this.skipResults == false) {
					AudioController.instance.PlayMusic(type: MusicType.BattleResults, track: 2, fade: 0.5f, delay: 0.7f);
				} 
			}

			Grawly.UI.Legacy.Flasher.instance.Fade(color: Color.white, fadeOut: 0.5f, fadeIn: 0.5f, interlude: 0.5f);
			yield return new WaitForSeconds(1f);

			// Remove the remaining enemies, if there are any (e.x., escaped the battle or didnt kill everyone)
			BattleArenaControllerDX.instance.RemoveWorldEnemies(
				enemiesToRemove: BattleController.Instance.Enemies, 
				battleParams: battleParams);
			
			// Turn the battle controller off
			BattleController.Instance.gameObject.SetActive(false);
			// Turn the player's bust up off.
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);

			// If skipping the results, send an event to the Gauntlet FSM to proceed. 
			if (this.skipResults == true) {
				// Show the mini map and radar.
				CrawlerMiniMap.Instance?.DisplayMap();
				EnemyRadar.Instance?.Show(CrawlerController.Instance.CurrentDangerLevel);
				NotificationController.Instance?.Present();
				CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);
			} else {
				// Otherwise, turn the results controller on.		
				// BattleResultsController.instance.SetActive(true);
				BattleResultsControllerDX.Instance.Present(battleResultsSet: battleResultsSet);
			}
		}
		/// <summary>
		/// This also used to be located in DungeonController, but is Here now.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		private IEnumerator BackToDungeonRoutine(BattleTemplate template, BattleParams battleParams) {

			// Rebuild the statuses on the players.
			GameController.Instance.Variables.Players.ForEach(p => p.PlayerStatusDX.QuickRebuild());

			// Fade the screen to black. This is only relevant if the results were shown.
			if (this.skipResults == false) {
				Grawly.UI.Legacy.Flasher.instance.Fade(color: Color.black);

				// If the battle had its own music, also turn that off here.
				if (template.PlaysBattleMusic == true) {
					AudioController.instance.StopMusic(track: 2, fade: 0.5f);
				}

				yield return new WaitForSeconds(0.5f);

			}


			

			// Collect Garbage
			Grawly.Utils.GrawlyGarbageCollection.Collect();

			// Clear out the combatant analysis. (might have been set from if a persona leveled up)
			Grawly.UI.Legacy.CombatantAnalysisCanvas.instance.Clear();

			// Turn off the camera controller's battle camera
			
			BattleCameraController.Instance.BackToCrawler();
			BattleResultsControllerDX.Instance.TotalHide();
			// BattleResultsController.instance.SetActive(false);
			
			// Tween the calendar UI back in.
			Grawly.UI.CalendarDateUI.instance?.Tween(status: true);

			// reactivate the player, turn the audio abck on
			CrawlerPlayer.Instance.PlayerCamera.gameObject.SetActive(true);
			CrawlerPlayer.Instance.PlayerCamera.GetComponent<AudioListener>().enabled = true;
			
			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: false);

			if (template.PlaysBattleMusic == true) {
				yield return new WaitForSeconds(0.5f);
				// AudioController.Instance.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance.ResumeMusic(track: 0, fade: 0.5f);
			}

			// Show the mini map and radar.
			CrawlerMiniMap.Instance?.DisplayMap();
			NotificationController.Instance?.Present();
			EnemyRadar.Instance?.Show(CrawlerController.Instance.CurrentDangerLevel);

			// Fire off any back to dungeon events.
			GameController.Instance.GetPersistentModifiers<IOnBackToDungeon>().ForEach(m => m.OnBackToDungeon());
			
			CrawlerPlayer.Instance.SetState(CrawlerPlayerState.Free);

		}
		#endregion

		#region DEBUGGING

		/// <summary>
		/// This used to be in Dungeon Controller. Now it's located here. Whoa.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		private IEnumerator GoToShuffleTime(BattleTemplate template, BattleParams battleParams) {

			Debug.LogWarning("GOING TO SHUFFLE TIME");
			
			// Play the shuffle time intro.
			ShuffleTimeIntroAnimation.Instance.PlayAnimation();

			// Tween the player statuses and bust up out.
			// BattleMenuControllerDX.Instance.TweenPlayerBustUp(visible: false, snap: false);
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);
			Debug.LogWarning("tween te player bust up out!");
			PlayerStatusDXController.instance.TweenVisible(status: false, inBattle: true);
			Chat.ChatControllerDX.Instance.ChatBorders.PresentBorders();

			yield return new WaitForSeconds(2f);

			if (template.PlaysBattleMusic == true) {
				AudioController.instance.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance.PlayMusic(type: MusicType.ShuffleTime, track: 2, fade: 0.5f, delay: 0.7f);
			}

			Grawly.UI.Legacy.Flasher.instance.Fade(color: Color.white, fadeOut: 0.5f, fadeIn: 0.5f, interlude: 1.5f);
			yield return new WaitForSeconds(0.5f);

			Chat.ChatControllerDX.StandardInstance.ChatBorders.DismissBorders(new Chat.ChatBorderParams());

			yield return new WaitForSeconds(0.5f);

			// Grab the current culling parameters. These will be needed in a bit.
			// this.savedCullingMask = BattleArenaControllerDX.Instance.CurrentCameraRig.MainCamera.cullingMask;
			Camera currentBattleCamera = BattleCameraController.Instance.MainCamera;
			this.savedCullingMask = currentBattleCamera.cullingMask;
			currentBattleCamera.cullingMask = 1 << LayerMask.NameToLayer("Shuffle Time");

			// Remove the remaining enemies, if there are any (e.x., escaped the battle or didnt kill everyone)
			BattleArenaControllerDX.instance.RemoveWorldEnemies(
				enemiesToRemove: BattleController.Instance.Enemies,
				battleParams: battleParams);

			// Present the shuffle time controller itself.
			ShuffleTimeController.Instance.Prepare(battleTemplate: template, onComplete: delegate {
				// Enable the battle results controller.
				// BattleResultsController.instance.SetActive(true);
				throw new NotImplementedException("You need to pass the results set along with the call to this method so that the new results controller can be presented.");
				// Restore the previous culling mask on the camera.
				currentBattleCamera.cullingMask = this.savedCullingMask;
			});

			

			// Turn the battle controller off
			BattleController.Instance.gameObject.SetActive(false);

		}

		#endregion

	}


}