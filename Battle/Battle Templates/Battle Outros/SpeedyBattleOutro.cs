using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.Results;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Outros.Special {

	/// <summary>
	/// The way in which I'll regularly be handling battle completions. Replaces what used to be FadeToResults in the dungeon controller.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Intended for use with auto battle and debugging. Does not look very good.")]
	public class SpeedyBattleOutro : BattleOutro {

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
			yield return new WaitForSeconds(0.05f);
			// Tell the audio controller to fade to the battle results. I can override this if needed.

			BattleController.Instance.gameObject.SetActive(false);

			// Remove the remaining enemies, if there are any (e.x., escaped the battle or didnt kill everyone)
			BattleArenaControllerDX.instance.RemoveWorldEnemies(enemiesToRemove: BattleController.Instance.Enemies, battleParams: battleParams);

			// Start the next battle.
			AutoBattleTerminal.instance.StartNextBattle();

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
			// BattleResultsController.instance.SetActive(false);

			// Tween the calendar UI back in.
			Grawly.UI.CalendarDateUI.instance?.Tween(status: true);

			// reactivate the player, turn the audio abck on
			DungeonPlayer.playerCamera.gameObject.SetActive(true);
			DungeonPlayer.playerCamera.GetComponent<AudioListener>().enabled = true;
			DungeonPlayer.playerModel.SetActive(true);
			// Free the player.
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);

			// Tween the player statuses back out.
			PlayerStatusDXController.instance.TweenSize(big: false);

			if (template.PlaysBattleMusic == true) {
				yield return new WaitForSeconds(0.5f);
				// AudioController.Instance.StopMusic(track: 1, fade: 0.5f);
				AudioController.instance.ResumeMusic(track: 0, fade: 0.5f);
			}

		}
		#endregion

	}


}