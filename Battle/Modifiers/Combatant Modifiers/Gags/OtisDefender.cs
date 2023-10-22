using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.WorldEnemies;

namespace Grawly.Battle.Modifiers.Gags {

	/// <summary>
	/// Intercepts a player attack and asks them to Not Kill Them.
	/// </summary>
	public class OtisDefender : CombatantModifier, IOnTurnEnd {

		#region FIELDS
		/// <summary>
		/// The script that gets played when this combatant begs for their fucking life.
		/// </summary>
		[SerializeField]
		private TextAsset script;
		/// <summary>
		/// The audio clip to use when interrupting.
		/// </summary>
		[SerializeField]
		private AudioClip pleaseWaitAudioClip;
		/// <summary>
		/// The sprite to use when angry.
		/// </summary>
		[SerializeField]
		private Sprite evilSprite;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONTURNEND
		public BattleReaction OnTurnEnd() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				
				// If Otis is still alive, execute the next reaction and return.
				if (this.combatantOwner.Allies.Select(c => c.metaData.name).Contains("Otis") == true) {
					battleReactionSequence.ExecuteNextReaction();
					return;
				}
				
				// If otis is dead, run the routine.
				GameController.Instance.StartCoroutine(this.MikuAngryRoutine(battleReactionSequence));
				
			};
		}
		/// <summary>
		/// The routine to run when miku becomes angry.
		/// </summary>
		/// <param name="battleReactionSequence"></param>
		/// <returns></returns>
		private IEnumerator MikuAngryRoutine(BattleReactionSequence battleReactionSequence) {
			// Play an sfx.
			AudioController.instance.PlaySFX(sfx: this.pleaseWaitAudioClip);
					
			// Shake the enemy. 
			(this.combatantOwner as Enemy).WorldEnemyDX.AllVisuals.transform.DOShakePosition(duration: 0.5f, strength: new Vector3(x: 0.5f, y: 0f, z: 0f), vibrato: 50);

			// Cast the target as an enemy and grab its world enemy dx sprite, then ask for its thot bubble and show it.
			((this.combatantOwner as Enemy).WorldEnemyDX as WorldEnemyDXSprite).ThoughtBubble.DisplayThoughtBubble(text: "<j>OTIS!! NO!!", time: 1.5f);
			yield return new WaitForSeconds(1.5f);
			
			// Set the vignette.
			BattleCameraController.Instance.SetVignette(amt: 5, time: 2f);

			// Activate the virtual camera on the enemy.
			BattleCameraController.Instance.ActivateVirtualCamera(((Enemy)this.combatantOwner).WorldEnemyDX.ZoomInCamera);

			// Display the notifier.
			BattleNotifier.DisplayNotifier(text: this.combatantOwner.metaData.name + " fills with rage...", time: 3f);
			yield return new WaitForSeconds(3f);

			// Tween the player statuses out.
			PlayerStatusDXController.instance.TweenVisible(status: false, inBattle: true);
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);

			// Create a stupid fucking variable to maintain the loop until the chat gets closed.
			bool chatActive = true;

			ChatControllerDX.GlobalOpen(
				textAsset: this.script,
				chatOpenedCallback: delegate { },
				chatClosedCallback: delegate {
					PlayerStatusDXController.instance.TweenVisible(status: true, inBattle: true);
					BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(true);
					BattleCameraController.Instance.SetVignette(amt: 0, time: 0.5f);
					((this.combatantOwner as Enemy).WorldEnemyDX as WorldEnemyDXSprite).ChangeEnemySprite(this.evilSprite);
					chatActive = false;
				});
			
			// Loop until the chat gets closed. uhg.
			while (chatActive == true) {
				yield return null;
			}

			
			this.combatantOwner.RemoveModifier(this);
			battleReactionSequence.ExecuteNextReaction();
			
		}
		#endregion
		
		/*#region INTERFACE IMPLEMENTATION - IINTERRUPTPLAYERATTACKANIMATION
		/// <summary>
		/// Intercepts an attack being made on this combatant before it hits.
		/// </summary>
		/// <param name="dcs">The damage calculation set involved with this calculation.</param>
		/// <param name="continueOnComplete">Should the animation continue when this routine completes?</param>
		/// <returns></returns>
		public IEnumerator OnInterceptOpponentAttackAnimation(DamageCalculationSet dcs) {

			// Animate the source (player only, as of writing) attempting to use their move.
			dcs.PrimarySource.CombatantAnimator.AnimateBehaviorUseInterruption(damageCalculationSet: dcs);
			yield return new WaitForSeconds(0.3f);

			// Play an sfx.
			AudioController.Instance.PlaySFX(sfx: this.pleaseWaitAudioClip);

			// Shake the enemy. 
			((dcs.Targets.First() as Enemy).WorldEnemyDX).WorldEnemyDXVisuals.transform.DOShakePosition(duration: 0.5f, strength: new Vector3(x: 0.5f, y: 0f, z: 0f), vibrato: 50);

			// Cast the target as an enemy and grab its world enemy dx sprite, then ask for its thot bubble and show it.
			((dcs.Targets.First() as Enemy).WorldEnemyDX as WorldEnemyDXSprite).ThoughtBubble.DisplayThoughtBubble(text: "<j>PLEASE WAIT!", time: 1.5f);
			yield return new WaitForSeconds(1.5f);

			// Set the vignette.
			BattleCameraController.Instance.SetVignette(amt: 5, time: 2f);

			// Activate the virtual camera on the enemy.
			BattleCameraController.Instance.ActivateVirtualCamera(((Enemy)dcs.damageCalculations[0].target).WorldEnemyDX.ZoomInCamera);

			// Display the notifier.
			BattleNotifier.DisplayNotifier(text: dcs.Targets.First().metaData.name + " begins panicking...", time: 3f);
			yield return new WaitForSeconds(3f);

			// Tween the player statuses out.
			PlayerStatusDXController.Instance.TweenVisible(status: false, inBattle: true);
			BattleMenuControllerDX.Instance.PlayerBustUpGameObject.SetActive(false);

			// Create a stupid fucking variable to maintain the loop until the chat gets closed.
			bool chatActive = true;

			ChatControllerDX.GlobalOpen(
				textAsset: this.script,
				chatOpenedCallback: delegate { },
				chatClosedCallback: delegate {
					PlayerStatusDXController.Instance.TweenVisible(status: true, inBattle: true);
					BattleMenuControllerDX.Instance.PlayerBustUpGameObject.SetActive(true);
					BattleCameraController.Instance.SetVignette(amt: 0, time: 0.5f);
					chatActive = false;
				});
			
			// Loop until the chat gets closed. uhg.
			while (chatActive == true) {
				yield return null;
			}

		}
		#endregion*/

		#region INSPECTOR JUNK
		private string inspectorDescription = "Intercepts a player attack and asks them to Not Kill Them.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion

		
	}


}