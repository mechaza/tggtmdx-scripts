using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
namespace Grawly.Battle.TurnBehaviors {

	/// <summary>
	/// The way in which a combatant should pick and chose their moves. Doesn't get used for Players because those get entered in by the actual irl player.
	/// </summary>
	public abstract class CombatantTurnBehavior {
		
		#region FIELDS - COMBATANT
		/// <summary>
		/// The combatant that is using this TurnBehavior.
		/// </summary>
		protected Combatant combatant;
		#endregion
		
		#region TURN EXECUTION
		/// <summary>
		/// Just pick out a move as you normally would.
		/// </summary>
		public abstract void ExecuteTurn();
		#endregion

		#region GETTERS - STATE
		/// <summary>
		/// Does this combatant have more actions on this turn?
		/// </summary>
		/// <param name="turnActionCount">The number of actions the combatant has taken so far.</param>
		/// <returns>Whether or not this combatant has more actions they can take this turn.</returns>
		public abstract bool HasMoreTurnActions(int turnActionCount);
		#endregion
		
		#region HELPERS
		/// <summary>
		/// Gets the strongest intent that will help this combatant pick a move.
		/// </summary>
		/// <returns></returns>
		protected virtual IntentType GetStrongestIntent() {
			// If this combatant REALLY needs to heal themselves, please do that.
			if (this.combatant.HPRatio < 0.25f) {
				return IntentType.SelfPreservation;
			}

			// If one or more of this combatant's allies needs to be healed, do that.
			if (this.combatant.Allies.Exists(c => c.HPRatio < 0.5f) == true) {
				return IntentType.Assistive;
			}

			// By default, just return Malicious.
			return IntentType.Malicious;

		}
		/// <summary>
		/// Gets the target combatants assocaited with a particular battle behavior.
		/// </summary>
		/// <param name="combatant"></param>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public List<Combatant> GetTargets(Combatant combatant, BattleBehavior behavior) {
			// CONSIDER LIKE.. ADDING THIS TO THE BATTLE BEHAVIOR AHH..
			List<Combatant> targets = new List<Combatant>();
			switch (behavior.targetType) {
				case TargetType.AllAliveAllies:
					targets = combatant.Allies.Where(c => c.IsDead == false).ToList();
					break;
				case TargetType.AllAliveEnemies:
					targets = combatant.Opponents.Where(c => c.IsDead == false).ToList();
					break;
				case TargetType.AllAliveCombatants:
					targets = combatant.Opponents.Where(c => c.IsDead == false).ToList().Concat(combatant.Allies.Where(c => c.IsDead == false).ToList()).ToList();
					break;
				case TargetType.AllDeadAllies:
					targets = combatant.Allies.Where(c => c.IsDead == true).ToList();
					break;
				case TargetType.OneDeadAlly:
					targets = combatant.Allies.Where(c => c.IsDead == true).ToList();
					targets.Shuffle();
					if (targets.Count > 0) {
						targets = targets.GetRange(0, 1);
					}
					break;
				case TargetType.OneAliveAlly:
					targets = combatant.Allies.Where(c => c.IsDead == false).ToList();
					targets.Shuffle();
					if (targets.Count > 0) {
						targets = targets.GetRange(0, 1);
					}
					break;
				case TargetType.OneAliveEnemy:
					targets = combatant.Opponents.Where(c => c.IsDead == false).ToList();
					targets.Shuffle();
					targets = targets.GetRange(0, 1);
					break;
				case TargetType.Self:
					targets.Add(combatant);
					break;
			}
			return targets;
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Clones the specified Turn Behavior. Usually relevant when it gets set to a combatant.
		/// </summary>
		/// <param name="turnBehavior">The Turn behavior to clone.</param>
		/// <param name="combatant">The combatant that will be using this turn behavior.</param>
		/// <returns></returns>
		public static CombatantTurnBehavior Clone(CombatantTurnBehavior turnBehavior, Combatant combatant) {
			// Clone the turn behavior.
			CombatantTurnBehavior clonedTurnBehavior = (CombatantTurnBehavior)turnBehavior.MemberwiseClone();
			// Give it a reference to the combatant that's going to be using it.
			clonedTurnBehavior.combatant = combatant;
			// Return it.
			return clonedTurnBehavior;
		}
		#endregion

	}

}