using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleMenu.Legacy;

namespace Grawly.Battle.Functions {

	[System.Serializable]
	public class AllOutAttackBattleBehaviorFunction : StandardBattleBehaviorFunction {

		#region FUNCTION
		/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			BattleController.Instance.StartCoroutine(this.AllOutAttackRoutine(source: source, targets: targets, battleBehavior: battleBehavior));
		}
		protected IEnumerator AllOutAttackRoutine(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			
			// Uh. Get new lists that I can use with the calculations below. :x
			List<Combatant> players = new List<Combatant>();
			List<Combatant> enemies = new List<Combatant>();
			// foreach (Player source in sources) { players.Add(source); }
			foreach (Player player in GameController.Instance.Variables.Players.Where(p => p.IsDead == false)) { players.Add(player); }
			foreach (Enemy target in targets) { enemies.Add(target); }

			DamageCalculationSet dcs = base.GenerateDamageCalculationSet(source: source, targets: enemies, self: battleBehavior);

			Debug.LogWarning("All Out Attack is currently broken. Please fix it to work with the new calculation system.");

			// yield return EffectsController.AllOutAttackAnimation(players, enemies, dcs);
			yield return this.AllOutAttackAnimation(players, enemies, dcs);

			foreach (DamageCalculation damageTuple in dcs.damageCalculations) {
				damageTuple.target.EvaluateDamageCalculation(damageTuple);
			}

			yield return new WaitForSeconds(1.0f);

			BattleController.Instance.AllOutAttackComplete();
		}
		#endregion

		#region GENERAL BUT I SHOULD PROBABLY MAKE A CLASS FOR THIS
		/// <summary>
		/// Handles the dust clouds and whatejver for the All Out Attack
		/// </summary>
		private IEnumerator AllOutAttackAnimation(List<Combatant> sources, List<Combatant> targets, DamageCalculationSet damageCalculationSet) {
			yield return null;

			Debug.Log("NOTE: This is where the battle canvas used to get turned off.");
			// CanvasController.Instance.SetBattleCanvas(false);

			// Play the sfx for the all out attack.
			AudioController.instance.PlaySFX(SFXType.AllOutAttack);
			
			// Turn the orbit camera off.
			BattleCameraController.Instance.OrbitEnemyBattleCamera.gameObject.SetActive(false);

			// Turn the head on camera on.
			BattleCameraController.Instance.ActivateVirtualCamera(BattleCameraController.BattleCameraType.HeadOnCamera);

			// Wait a moment for effect.
			yield return new WaitForSeconds(.5f);

			// Turn on the dynamic head on camera.
			BattleCameraController.Instance.DynamicHeadOnBattleCamera.gameObject.SetActive(true);

			yield return new WaitForSeconds(.4f);
			CanvasController.instance.Flash();
			yield return new WaitForSeconds(.1f);

			// Turn on the all out attack visuals object.
			// LegacyAllOutAttackDXAnimationController.instance.AllOutAttackAnimationVisualsObject.SetActive(true);

			// Play the animation.
			AllOutAttackControllerDX.Instance.PlayAttackAnimation(participatingPlayers: sources.Cast<Player>().ToList());
			
			// Turn Off the dynamic head on camera.
			BattleCameraController.Instance.DynamicHeadOnBattleCamera.gameObject.SetActive(false);

			yield return new WaitForSeconds(1.3f);

			// Turn off the animation visual object.
			// LegacyAllOutAttackDXAnimationController.instance.AllOutAttackAnimationVisualsObject.SetActive(false);
			AllOutAttackControllerDX.Instance.ResetState();
			
			GameObject fightCloud = GameObject.Instantiate<GameObject>(DataController.Instance.GetBFX(BFXType.FightCloud));
			// GameObject fightCloud = GameObject.Instantiate<GameObject>(DataController.GetBFX(BFXType.FightCloud));

			// fightCloud.transform.position = GameObject.FindObjectOfType<LegacyWorldEnemyGroup>().transform.position;
			fightCloud.transform.position = BattleArenaControllerDX.instance.FightCloudPosition;

			yield return new WaitForSeconds(1.7f);
			// CameraController.Instance.RushToPosition(WorldEnemyGroup.Instance.headOnCamera.transform);
			yield return new WaitForSeconds(2.0f);

			Debug.Log("NOTE: This is where the battle canvas/player status canvases used to get turned back on.");
			// CanvasController.Instance.SetBattleCanvas(true);
			// CanvasController.Instance.SetPlayerStatusCanvasGroup(true);

			// Go through each target and play the appropriate animation.
			foreach (DamageCalculation damageCalculation in damageCalculationSet.damageCalculations) {
				damageCalculation.FinalTarget.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation);

				/*// Play the prefab associated with this behavior.
				if (damageCalculation.target.HP - damageCalculation.amountToDeduct <= 0) {
					damageCalculation.FinalTarget.CombatantAnimator.AnimateHarmfulCalculation(damageCalculation);
				} else {
					// Play the prefab associated with this behavior.
					// PlayBattleFX(damageTuple.target, BattleController.Instance.allOutAttack);
					throw new System.Exception("The line below this does not work.");
					// PlayBattleFX(damageTuple.target, DataController.Instance.GetBehavior(behaviorName: "All Out Attack"));
				}
				// Play the bar fill animation for the enemy. This will also mark it as weak/crit/etc.
				throw new System.Exception("line below this doesnt work anymore.");
				// damageTuple.target.AnimateDamageCalculationTarget(damageTuple);*/
			}
		}
		#endregion

		#region CALCULATIONS
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			throw new System.NotImplementedException();
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "HERES OUR CHANCE FOR AN ALL OUT ATTACK";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

	}

}