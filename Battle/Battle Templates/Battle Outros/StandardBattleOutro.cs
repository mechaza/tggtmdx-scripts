using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;
using Grawly.Dungeon.UI;
using Grawly.UI;
using Grawly.Battle.Modifiers;
using Grawly.Battle.Results;

namespace Grawly.Battle.Outros.Standard {

	/// <summary>
	/// The way in which I'll regularly be handling battle completions. Replaces what used to be FadeToResults in the dungeon controller.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The standard battle outro. This is what will be used the majority of the time. Replaces what used to be FadeToResults in the DungeonController.")]
	public class StandardBattleOutro : BattleOutro {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the playback lock on the AudioController be forced to an unlocked state upon leaving battle?
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("Should the playback lock on the AudioController be forced to an unlocked state upon leaving battle?")]
		private bool forceMusicPlaybackUnlock = false;
		#endregion
		
		#region COMPLETION
		/// <summary>
		/// The routine to run when the battle has been completed.
		/// </summary>
		/// <param name="template"></param>
		public override void PlayOutro(BattleTemplate template, BattleParams battleParams, BattleResultsSet battleResultsSet) {
			// Upon completion, go to the results screen.
			GameController.Instance.StartCoroutine(this.FadeToResults(
				template: template,
				battleParams: battleParams, 
				battleResultsSet: battleResultsSet));
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

			// If the toggle to force release the playback lock on the audiocontroller is set, do so.
			if (this.forceMusicPlaybackUnlock == true) {
				AudioController.instance.SetMusicPlaybackLock(playbackLockType: MusicPlaybackLockType.Unlocked);
			}
			
			yield return new WaitForSeconds(2f);

			if (template.PlaysBattleMusic == true) {
				AudioController.instance.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance.PlayMusic(type: MusicType.BattleResults, track: 1, fade: 0.5f, delay: 0.7f);
			}

			Grawly.UI.Legacy.Flasher.instance.Fade(color: Color.white, fadeOut: 0.5f, fadeIn: 0.5f, interlude: 0.5f);
			yield return new WaitForSeconds(1f);

			// Remove the remaining enemies, if there are any (e.x., escaped the battle or didnt kill everyone)
			BattleArenaControllerDX.instance.RemoveWorldEnemies(enemiesToRemove: BattleController.Instance.Enemies, battleParams: battleParams);
			
			// Turn the battle controller off
			BattleController.Instance.gameObject.SetActive(false);
			// Turn the player's bust up off.
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);
			// Go to the battle results.
			BattleResultsControllerDX.Instance.Present(battleResultsSet: battleResultsSet);
		}
		/// <summary>
		/// This also used to be located in DungeonController, but is Here now.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		private IEnumerator BackToDungeonRoutine(BattleTemplate template, BattleParams battleParams) {
			
			// Rebuild the statuses on the players.
			GameController.Instance.Variables.Players.ForEach(p => p.PlayerStatusDX.QuickRebuild());

			// Fade the screen to black.
			Grawly.UI.Legacy.Flasher.instance.Fade(color: Color.black);

			if (template.PlaysBattleMusic == true) {
				AudioController.instance.StopMusic(track: 1, fade: 0.5f);
				// AudioController.Instance.ResumeMusic(track: 0, fade: 0.5f);
			}

			yield return new WaitForSeconds(0.5f);

			// Collect Garbage
			Grawly.Utils.GrawlyGarbageCollection.Collect();

			// Clear out the combatant analysis. (might have been set from if a persona leveled up)
			Grawly.UI.Legacy.CombatantAnalysisCanvas.instance.Clear();

			// Turn off the camera controller's battle camera
			BattleCameraController.Instance.BackToDungeon();
			BattleResultsControllerDX.Instance.TotalHide();
			// Tween the calendar UI back in.
			Grawly.UI.CalendarDateUI.instance?.Tween(status: true);
			NotificationController.Instance?.Present();

			// Show the minimap, if its in the scene.
			MiniMapDX.instance?.DisplayMap();

			// reactivate the player, turn the audio abck on
			DungeonPlayer.playerCamera.gameObject.SetActive(true);
			DungeonPlayer.playerCamera.GetComponent<AudioListener>().enabled = true;
			DungeonPlayer.playerModel.SetActive(true);
			
			// Fire off any back to dungeon events.
			GameController.Instance.GetPersistentModifiers<IOnBackToDungeon>().ForEach(m => m.OnBackToDungeon());
			
			// Free the player if the flag has been set to do so.
			DungeonPlayer.Instance.SetFSMState(state: 
				battleParams.FreePlayerOnReturn == true 
					? DungeonPlayerStateType.Free 
					: DungeonPlayerStateType.Wait);
			
			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: false);

			if (template.PlaysBattleMusic == true) {
				yield return new WaitForSeconds(0.5f);
				// AudioController.Instance.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance.ResumeMusic(track: 0, fade: 0.5f);
			}

			// Invoke the returnal callback if it exists.
			battleParams.OnBattleReturn?.Invoke();
			
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