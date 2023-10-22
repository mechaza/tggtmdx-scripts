using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.Modifiers;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// Makes enemies stronger.
	/// </summary>
	[InfoBox("Makes you explode after the battle.")]
	[System.Serializable]
	public class Explode : ShuffleCardBehavior, IOnBackToDungeon {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The particle effect to spawn after the battle.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private GameObject explosionParticleEffect;
		#endregion

		#region INTERFACE IMPLEMENTATION
		public void OnBackToDungeon() {
			Dungeon.DungeonPlayer.Instance.SetFSMState(Dungeon.DungeonPlayerStateType.Wait);
			GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
				GameObject particles = GameObject.Instantiate(this.explosionParticleEffect);
				particles.transform.position = Dungeon.DungeonPlayer.Instance.transform.position + new Vector3(x: 0f, y: 1f);
				Dungeon.DungeonPlayer.Instance.nodeLabel.HideLabel();
				Dungeon.DungeonPlayer.playerModel.SetActive(false);
				AudioController.instance.PlaySFX(SFXType.DefaultAttack);
				GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
					Grawly.UI.Legacy.Flasher.instance.FadeOut(color: Color.white, fadeTime: 1.5f);
					// Grawly.UI.Legacy.Flasher.Instance.Fade(color: Color.white, fadeOut: 2f, fadeIn: 1f, interlude: 7f, showLoadingText: false);
					AudioController.instance.StopMusic(track: 0, fade: 2f);
				});

				GameController.Instance.WaitThenRun(timeToWait: 5f, action: delegate {
					SceneManager.LoadScene("GameOver");
				});
			});
		}
		#endregion

	}


}