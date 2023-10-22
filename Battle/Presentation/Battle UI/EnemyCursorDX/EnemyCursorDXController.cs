using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle.WorldEnemies;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// Maintains control of the enemy cursors in the scene and provides access to ways to show the cursors on screen.
	/// </summary>
	public class EnemyCursorDXController : MonoBehaviour {

		public static EnemyCursorDXController instance;

		#region FIELDS - STATE
		/// <summary>
		/// A reference to the world enemies that may be selected when a move with target "all enemies" is hit.
		/// This is only relevant when the hidden object is being used.
		/// </summary>
		private List<WorldEnemyDX> currentlySelectedWorldEnemies;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// An "invisible" gameobject that is selected in the event that the move that was chosen is one that targets all enemies.
		/// An event trigger should be hooked up to it which will events detailed below.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private GameObject hiddenAllEnemiesGameObject;
		/// <summary>
		/// The GameObject that contains all of the EnemyCursorDX's.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private GameObject enemyCursorDXsGameObject;
		/// <summary>
		/// The cursors that are in the scene.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private List<EnemyCursorDX> enemyCursors = new List<EnemyCursorDX>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region ENEMY SELECTION
		/// <summary>
		/// Activates the cursors for the specified world enemies.
		/// </summary>
		/// <param name="worldEnemies">The enemies that should have their icons being shown.</param>
		/// <param name="currentBehavior">The behavior that was selected by the player prior.</param>
		public void BuildEnemyCursorsWithSelectables(List<WorldEnemyDX> worldEnemies, BattleBehavior currentBehavior) {

			// Disable all enemy cursors. This allows me to just use only the ones I need.
			this.DisableEnemyCursors();

			// Determine whether or not a single enemy should be targeted or not.
			bool targetsSingleEnemy = currentBehavior.targetType == TargetType.OneAliveEnemy;
			
			// Go through each world enemy and assign it a cursor.
			for (int i = 0; i < worldEnemies.Count; i++) {
				this.enemyCursors[i].BuildEnemyCursorDX(
					worldEnemyDX: worldEnemies[i],                                              // The indicies of the cursor/enemy should match.
					currentBattleBehavior: currentBehavior,                                     // Obviously, it just needs the behavior the player picked.
					isSelectable: targetsSingleEnemy);											// Its selectability was determined earlier in the function. (This also sets its visibility.)			
			}

			if (targetsSingleEnemy == true) {
				// If the cursors are selectable, select the first cursor that was used.
				EventSystem.current.SetSelectedGameObject(this.enemyCursors[0].gameObject);
			} else {
				// Save a reference to the world enemies.
				this.currentlySelectedWorldEnemies = worldEnemies;
				// If it is targeting more than one enemy, only select the 'hidden' object. 
				EventSystem.current.SetSelectedGameObject(this.hiddenAllEnemiesGameObject);
			}

		}
		/// <summary>
		/// Builds the enemy cursor from a damage calculation.
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX who was part of this attack.</param>
		/// <param name="damageCalculation">The damage calculation itself.</param>
		public void BuildEnemyCursorFromDamageCalculation(WorldEnemyDX worldEnemyDX, DamageCalculation damageCalculation) {
			// Find the first enemy cursor that is not active in the hierarchy and build it.
			EnemyCursorDX nextAvailableCursor = this.enemyCursors.First(ec => ec.gameObject.activeInHierarchy == false);
			nextAvailableCursor.BuildEnemyCursorDX(worldEnemyDX: worldEnemyDX, damageCalculation: damageCalculation);

			// Wait two seconds and then turn it off.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				nextAvailableCursor.gameObject.SetActive(false);
			});

		}
		/// <summary>
		/// Builds the enemy cursor from a simple number and resource type.
		/// Good for afflictions which don't really use DamageCalculations.
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX that needs to have its cursor built.</param>
		/// <param name="damageAmount">The amount of damage to give to this enemy.</param>
		/// <param name="resourceType">The resource to target. Affects the color of the bar.</param>
		public void BuildEnemyCursorFromSimpleDamage(WorldEnemyDX worldEnemyDX, int damageAmount, BehaviorCostType resourceType) {
			// Find the first enemy cursor that is not active in the hierarchy and build it.
			EnemyCursorDX nextAvailableCursor = this.enemyCursors.First(ec => ec.gameObject.activeInHierarchy == false);
			// Build it.
			nextAvailableCursor.BuildEnemyCursorDX(
				worldEnemyDX: worldEnemyDX,
				damageAmount: damageAmount,
				resourceType: resourceType);

			// Wait two seconds and then turn it off.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				nextAvailableCursor.gameObject.SetActive(false);
			});

		}
		/// <summary>
		/// Disables all of the enemy cursors currently on screen.
		/// </summary>
		public void DisableEnemyCursors() {
			this.enemyCursors.ForEach(ec => ec.gameObject.SetActive(false));
		}
		#endregion

		#region EVENT TRIGGERS : HIDDEN SELECTABLE
		/// <summary>
		/// An event that is called from the 'hidden' object that is meant to be used when targeting all enemies.
		/// </summary>
		/// <param name="submit">Was the event a submit or a cancel? Yes I know this is kinda vague.</param>
		public void HiddenAllEnemiesGameObjectEvent(bool submit) {
			// Null out the currently selected object. It can still accept events otherwise.
			EventSystem.current.SetSelectedGameObject(null);

			if (submit == true) {
				// If submit was hit, tell the battle menu controller to proceed with the saved world enemies.
				BattleMenuControllerDX.instance.SetCurrentTargetCombatants(combatants: this.currentlySelectedWorldEnemies.Select(we => we.Enemy).Cast<Combatant>().ToList());
			} else {
				AudioController.instance.PlaySFX(SFXType.Close);
				BattleMenuControllerDX.instance.CancelCombatantSelection();
			}
			// Null out the reference to the list. I don't want to remember it.
			this.currentlySelectedWorldEnemies = null;
			
		}
		#endregion

	}


}