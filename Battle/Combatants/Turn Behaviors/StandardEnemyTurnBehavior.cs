using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.Modifiers;
using Sirenix.OdinInspector;

namespace Grawly.Battle.TurnBehaviors {

	/// <summary>
	/// The standard way in which enemies pick and choose their moves. Nothing fancy.
	/// </summary>
	[System.Serializable]
	[InfoBox("The standard way in which enemies pick and choose their moves. Nothing fancy.")]
	public class StandardEnemyTurnBehavior : CombatantTurnBehavior {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The number of actions this enemy may take per turn.
		/// </summary>
		[SerializeField]
		private int actionsPerTurn = 1;
		#endregion
		
		#region GETTERS - STATE
		/// <summary>
		/// Does this combatant have more actions on this turn?
		/// </summary>
		/// <param name="turnActionCount">The number of actions the combatant has taken so far.</param>
		/// <returns>Whether or not this combatant has any more actions this turn.</returns>
		public override bool HasMoreTurnActions(int turnActionCount) {
			
			// Theoretically this should always be more than zero.
			Debug.Assert(this.actionsPerTurn > 0);
			
			// If the combatant has taken any actions, they're done.
			if (turnActionCount >= this.actionsPerTurn) {
				return false;
			} else {
				return true;
			}
			
		}
		#endregion
		
		#region TURN START
		/// <summary>
		/// Just pick out a move as you normally would.
		/// </summary>
		public override void ExecuteTurn() {

			// Change up the visuals for the enemy. I need to do that.
			BattleMenuControllerDX.instance.PlayerBustUpGameObject.SetActive(false);
			BattleMenuControllerDX.instance.TopLevelSelectionsGameObject.SetActive(false);

			// If the affliction needs to override the turn behavior, execute that.
			if (this.combatant.Affliction is ITurnBehaviorOverride == true && (this.combatant.Affliction as ITurnBehaviorOverride).TakesPriority == true) {
				(this.combatant.Affliction as ITurnBehaviorOverride).ExecuteTurn();

			} else {

				// Grab only the behaviors the combatant is allowed to use.
				List<BattleBehavior> potentialBehaviors =
					combatant
					.UsableSpecialBehaviors
					.Where(b => this.GetTargets(combatant: this.combatant, behavior: b).Count > 0)	// Make sure the potential behavior has targets!
					.ToList();



				// If there is something to pick from, do that. I might implement better functionality later.
				BattleBehavior behavior = potentialBehaviors.Count > 0 ? potentialBehaviors[Random.Range(0, potentialBehaviors.Count)] : this.combatant.AllBehaviors[BehaviorType.Attack].First();

				// Figure out what targets need to be used for this move.
				List<Combatant> targets = this.GetTargets(combatant: this.combatant, behavior: behavior);


				// Pass this information to the cmobatant and let them take it from there.
				BattleController.Instance.PrepareBehaviorEvaluation(source: this.combatant, targets: targets, behavior: behavior);

				/*// Decide on what the strongest intent for this enemy should be.
				IntentType strongestIntent = base.GetStrongestIntent();

				// Pick out a behavior that matches up with the intent and also has enough MP to use.
				List<BattleBehavior> potentialBehaviors =
					combatant.Behaviors[BehaviorType.Special]                           // Start with the special moves
					.Where(b => ((strongestIntent & b.IntentType) == strongestIntent))  // Find moves where the intents match.
					.ToList();

				BattleBehavior behavior;
				if (potentialBehaviors.Count > 0) {
					behavior = potentialBehaviors.First();
				} else {
					Debug.LogError("There was an issue with this enemy deciding on a move. Returning a random move from their moveset.");
					potentialBehaviors = combatant.Behaviors[BehaviorType.Special].Where(b => b.showInBattleMenu).ToList();
					behavior = potentialBehaviors[Random.Range(0, potentialBehaviors.Count)];
				}

				// Figure out what targets need to be used for this move.
				List<Combatant> targets = this.GetTargets(combatant: this.combatant, behavior: behavior);*/
			}
		}
		#endregion


	}


}