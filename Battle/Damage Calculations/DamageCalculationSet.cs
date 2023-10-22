using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle {

	/// <summary>
	/// Stores the multiple DamageCalculations that get used in the battle in an effort to take into account passive behaviors that rely on knowledge of the rest of the set.
	/// </summary>
	public class DamageCalculationSet {

		#region FIELDS - CONTEXT SENSITIVE
		/// <summary>
		/// The combatants originating this calculation set.
		/// </summary>
		public List<Combatant> Sources { get; private set; }
		/// <summary>
		/// The combatants that are the targets of this calculation set.
		/// </summary>
		public List<Combatant> Targets { get; private set; }
		/// <summary>
		/// The battle behavior that spawned this calculation set.
		/// </summary>
		public BattleBehavior BattleBehavior { get; private set; }
		#endregion

		#region FIELDS - SHIT TO HELP ME BECAUSE IM SICK OF DOING THIS MANUALLY
		/// <summary>
		/// The "primary" source of this damage calculation. Usually, the one who selected it to begin with.
		/// </summary>
		public Combatant PrimarySource {
			get {
				return this.Sources[0];
			}
		}
		#endregion

		#region FIELDS - CALCULATIONS
		/// <summary>
		/// A list of calculations that are being worked with.
		/// For reference, I used to just be using a list without containing it inside another object.
		/// </summary>
		public List<DamageCalculation> damageCalculations = new List<DamageCalculation>();
		#endregion

		#region FIELDS - HELPERS
		/// <summary>
		/// A list of combatants who were attacked with malicious intent in this calculation.
		/// </summary>
		public List<Combatant> Attacked {
			get {
				return this.damageCalculations
					.Where(dc => dc.rawDamageAmount > 0)			// Go through the damage calculations where the amount dealt is higher than zero.
					.Select(dc => dc.target)			// Select the targets of those damage calculations.
					.Distinct()							// Depending on the move, some calculations might have multiples of the same target. This list should only have one of each.
					.ToList();
			}
		}
		#endregion

		#region FIELDS - COMPUTED
		/// <summary>
		/// Do any of the targets intercept the attack animation?
		/// </summary>
		public bool TargetsInterceptAttackAnimation {
			get {
				// If any of the targets have a modifier that intercepts the attack animation, return true.
				return this.Targets.SelectMany(c => c.GetModifiers<IInterceptOpponentAttackAnimation>()).ToList().Count > 0;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public DamageCalculationSet() {
			Debug.LogWarning("DamageCalculationSet made with no parameters.");
		}
		public DamageCalculationSet(List<Combatant> sources, List<Combatant> targets, BattleBehavior battleBehavior) {
			this.Prepare(sources, targets, battleBehavior);
		}
		public DamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			Debug.LogWarning("DamageCalculationSet made with single source. Consider refactoring.");
			this.Prepare(sources: new List<Combatant> { source }, targets: targets, battleBehavior: battleBehavior);
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this calculation set for use for the different functions that will pass through it.
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="targets"></param>
		/// <param name="battleBehavior"></param>
		private void Prepare(List<Combatant> sources, List<Combatant> targets, BattleBehavior battleBehavior) {
			this.Sources = sources;
			this.Targets = targets;
			this.BattleBehavior = battleBehavior;
		}
		#endregion

		#region GENERAL CALCULATIONS
		/// <summary>
		/// Does some grunt work on the damage calculation set. Can be chained together with multiple passes.
		/// </summary>
		/// <param name="calculationDelegate">The delegate being used to perform this calculation.</param>
		/// <returns></returns>
		public DamageCalculationSet CalculatePass(DamageCalculationPass calculationDelegate) {
			return calculationDelegate(damageCalculationSet: this);
		}
		#endregion

	}


}