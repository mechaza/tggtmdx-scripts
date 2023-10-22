using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// The standard set of calculations that should be performed for most moves.
	/// </summary>
	[System.Serializable]
	public class StandardBattleBehaviorFunction : BattleBehaviorFunction {

		#region FUNCTION
		/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			BattleController.Instance.StartCoroutine(ExecuteAsCoroutine(source, targets, battleBehavior));
		}
		/// <summary>
		/// Executes the function as a coroutine.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		/// <returns></returns>
		protected IEnumerator ExecuteAsCoroutine(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			yield return new WaitForEndOfFrame();
			
			// If the behavior was an item, remove it from the inventory. If not, deduct the cost.
			if (battleBehavior.behaviorType == BehaviorType.Item) {
				GameController.Instance.Variables.RemoveItemFromInventory(battleBehavior);
			} else {
				// If this is a tactics move, find the participants and deduct the cost from them.
				if (battleBehavior.behaviorType == BehaviorType.Tactics) {
					foreach (Combatant combatant in GetParticipantsFromTacticsBehavior(self: battleBehavior)) {
						combatant.DeductBehaviorCost(battleBehavior);
					}
				} else {
					// If this isn't a tactics move, just deduct the cost from the source.
					source.DeductBehaviorCost(battleBehavior);
				}

			}

			// Calculate the damage calculation set.
			DamageCalculationSet damageCalculationSet = this.GenerateDamageCalculationSet(source, targets, battleBehavior);

			// If the target was an enemy, mark them as having their resistance known.
			targets
				.Where(c => c is Enemy)
				.Cast<Enemy>()
				.ToList()
				.ForEach(e => GameController.Instance.Variables.SetKnownEnemyResistance(enemy: e, elementType: battleBehavior.elementType, resistanceKnown: true));

			// Play the animation that is associated with this move.
			yield return battleBehavior.BehaviorAnimation.ExecuteAnimation(damageCalculationSet: damageCalculationSet);

			// Go through each damage calculation and evaluate the calculation on the target.
			damageCalculationSet
				.damageCalculations
				.ForEach(dc => dc.FinalTarget.EvaluateDamageCalculation(dc));

			
			// Create a new reaction sequence for handing OnAttacked reactions.
			BattleReactionSequence attackedReactionSequence = new BattleReactionSequence();
			
			// The finish callback should check for one mores and go back to the battle controller.
			attackedReactionSequence.Prepare(defaultFinishCallback: () => {
				damageCalculationSet
					.damageCalculations
					.ForEach(dc => BattleBehaviorFunction.CheckForOneMore(dc, battleBehavior));
				BattleBehaviorFunction.BackToBattleController();
			});

			// Generate OnAttacked reactions.
			var attackedReactions = damageCalculationSet.Attacked
				.SelectMany(c => c.GetModifiers<IOnAttacked>())
				.Where(m => m.ReadyToTrigger == true)
				.Select(m => m.OnAttacked(damageCalculationSet: damageCalculationSet))
				.ToList();
			
			// Add them to the sequence.
			attackedReactionSequence.AddToSequence(attackedReactions);
			
			// Run it.
			attackedReactionSequence.ExecuteNextReaction();
			
			/*// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].Prepare(defaultFinishCallback: delegate { this.FSM.SendEvent("Get Next Combatant"); });

			// Grab the reactions from the battle modifiers as well as all the alive combatants.
			List<BattleReaction> battleModifierReactions = this.battleModifiers.Where(bm => bm is IOnPreTurn).Cast<IOnPreTurn>().Select(i => i.OnPreTurn()).ToList();
			List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnPreTurn>()).Select(i => i.OnPreTurn()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());


			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].ExecuteNextReaction();*/
			
			/*// Go through each attacked combatant and tell them that they were just attacked.
			damageCalculationSet
				.Attacked
				.SelectMany(c => c.GetModifiers<IOnAttacked>())
				.Where(m => m.ReadyToTrigger == true)
				.ToList()
				.ForEach(m => m.OnAttacked(damageCalculationSet: damageCalculationSet));*/

		/*	// Go through each of the calculations and see if One More was reached.
			damageCalculationSet
				.damageCalculations
				.ForEach(dc => BattleBehaviorFunction.CheckForOneMore(dc, battleBehavior));

			// Signal back to the BattleController to resume gameplay.
			BattleBehaviorFunction.BackToBattleController();*/
		}
		#endregion

		#region CALCULATION DELEGATES
		/// <summary>
		/// Generate the damage calculations. This is a requisite of the function.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		/// <returns></returns>
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {

			// Create a new damage calculation set, throw it through some modifications, and return finalized calculations.
			return new DamageCalculationSet(source: source, targets: targets, battleBehavior: self) // Create the new DamageCalculationSet.
				.CalculatePass(this.StandardCalculationPass)    // Standard calculations.
				.CalculatePass(this.PowerBoostPass)				// Calculate regular boosts.
				.CalculatePass(this.TargetInterceptionPass)		// Check for combatants that intercept calculations.
				.CalculatePass(this.ReflectionPass)				// Reflect any moves that need to be reflected.
				.CalculatePass(this.CollapseCalculationPass)	// Finally determine accuracy and final targets.
				.CalculatePass(this.LateInterceptionPass);      // One more interception for combatants who have modifiers that intercept after the collapse.

		}
		/// <summary>
		/// The "standard" way in which calculations can be performed.
		/// </summary>
		/// <param name="damageCalculationSet">The set of damage calculations.</param>
		/// <returns>The set of damage calculations in a state where basic numbers have been calculated.</returns>
		protected virtual DamageCalculationSet StandardCalculationPass(DamageCalculationSet damageCalculationSet) {

			List<Combatant> sources = damageCalculationSet.Sources;
			List<Combatant> targets = damageCalculationSet.Targets;
			BattleBehavior battleBehavior = damageCalculationSet.BattleBehavior;
			Combatant source = sources[0];

			foreach (Combatant target in targets) {

				for (int i = 0; i < battleBehavior.HitFrequency; i++) {
					DamageCalculation damageCalculation = new DamageCalculation();

					damageCalculation.behavior = battleBehavior;
					damageCalculation.source = source;
					damageCalculation.target = target;

					// Amount
					switch (battleBehavior.calculationType) {
						case BattleCalculationType.Dynamic:
							damageCalculation.rawDamageAmount = DynamicCalculation(source, target, battleBehavior);
							break;
						case BattleCalculationType.Static:
							damageCalculation.rawDamageAmount = StaticCalculation(source, target, battleBehavior);
							break;
						case BattleCalculationType.Percentage:
							damageCalculation.rawDamageAmount = PercentageCalculation(source, target, battleBehavior);
							break;
						case BattleCalculationType.None:
							damageCalculation.rawDamageAmount = 0;
							break;
						default:
							Debug.LogError("Couldn't determine calculation type!");
							damageCalculation.rawDamageAmount = 0;
							break;
					}

					// Acuracy is calculated as the base * the power boost ratios.
					damageCalculation.rawAccuracy = CalculateRawAccuracy(source, target, battleBehavior);
					// Crit rate just returns the behavior's crit rate as is.
					damageCalculation.rawCritRate = CalculateRawCritRate(source, target, battleBehavior);
					// Resistance probes the target directly.
					damageCalculation.finalResistance = target.CheckResistance(battleBehavior);
					// Affliction just picks one at random, but if none exist, returns AfflictionType.None.
					damageCalculation.afflictionType = PickAfflictionType(source, target, battleBehavior);
					// Picks a random value and checks if its below the behavior's affliction accuracy rate. (if it has afflictions)
					damageCalculation.setAffliction = CalculateSetAffliction(source, target, battleBehavior);
					// If the target is currently afflicted by the type this behavior should cure, return true.
					damageCalculation.cureAffliction = CalculateCureAffliction(source, target, battleBehavior);

					// Add the damage calculation.
					damageCalculationSet.damageCalculations.Add(damageCalculation);
				}
			}
			return damageCalculationSet;
		}
		/// <summary>
		/// Modifies the DamageCalculations to take into account if the move specified has buffs/debuffs.
		/// </summary>
		/// <param name="dcs">The DamageCalculationSet to be modified.</param>
		/// <returns></returns>
		protected virtual DamageCalculationSet PowerBoostPass(DamageCalculationSet dcs) {

			// Go through each damage calculation where the boosts are higher than zero.
			foreach (DamageCalculation dc in dcs.damageCalculations.Where(dc => dc.behavior.boostTypes.Count > 0)) {

				List<KeyValuePair<PowerBoostType, PowerBoostIntentionType>> powerBoostsToAdd = dc
					.behavior
					.boostTypes
					.Select(bt => new KeyValuePair<PowerBoostType, PowerBoostIntentionType>(
						key: bt, 
						value: Combatant.CombatantTypeMatch(dc.source, dc.target) ? PowerBoostIntentionType.Buff : PowerBoostIntentionType.Debuff))
					.ToList();

				dc.powerBoosts.AddRange(powerBoostsToAdd);
			}

			return dcs;
		}
		/// <summary>
		/// Modifies the DamageCalculationSet if any of the behaviors on the targets in the set need to do that.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet to be modified.</param>
		/// <returns></returns>
		protected virtual DamageCalculationSet TargetInterceptionPass(DamageCalculationSet dcs) {
		
			foreach (Combatant self in dcs.Targets) {
				self.GetModifiers<IInterceptIncomingDCS>()
				.ForEach(m => dcs = m.InterceptIncomingDCS(dcs, self));              // Go through each of those interfaces, and directly modify the dcs.
			}
			foreach (Combatant self in dcs.Sources) {
				self.GetModifiers<IInterceptIncomingDCS>()
				.ForEach(m => dcs = m.InterceptIncomingDCS(dcs, self));              // Go through each of those interfaces, and directly modify the dcs.
			}

			return dcs;
		}
		/// <summary>
		/// This should be run after interception and is used to "collapse" the amount that will be used in the final calculation.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		protected virtual DamageCalculationSet CollapseCalculationPass(DamageCalculationSet dcs) {

			// The idea here is that some interceptions may be able to change the "accuracy" or final amount of the calculation.
			// When this pass is reached, a final calculation will be performed.

			dcs.damageCalculations.ForEach(dc => {

				// FINALLY calculate the accuracy type.
				dc.accuracyType = CalculateAccuracyType(damageCalculation: dc);

				// Confirm whether or not this calculation is static. If it is, certain steps need to be taken.
				if (dc.behavior.calculationType == BattleCalculationType.Static) {

					Debug.Log("Behavior has a static calculation. Ignoring Critical/Weak multipliers.");
					if (dc.accuracyType == AccuracyType.Miss) {
						dc.rawDamageAmount = 0;
					} else if (dc.finalResistance == ResistanceType.Nul) {
						dc.rawDamageAmount = 0;
					}

				} else {
					// If it's not static, feel free to multiply it if it's critical.

					// Use this to tweak the damage as needed.
					if (dc.accuracyType == AccuracyType.Miss) {
						dc.rawDamageAmount = 0;
					} else if (dc.finalResistance == ResistanceType.Nul) {
						dc.rawDamageAmount = 0;
					} else if (dc.accuracyType == AccuracyType.Critical) {
						dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * 2.25f);
					} else if (dc.finalResistance == ResistanceType.Wk) {
						dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * 2.25f);
					} else if (dc.finalResistance == ResistanceType.Str) {
						dc.rawDamageAmount = (int)((float)dc.rawDamageAmount * 0.5f);
					} else if (dc.finalResistance == ResistanceType.Abs) {
						dc.rawDamageAmount = -dc.rawDamageAmount;
					} 

				}

			});

			return dcs;
		}
		/// <summary>
		/// Checks if any of the calculations are to be reflected and adjusts the calculations accordingly.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		protected virtual DamageCalculationSet ReflectionPass(DamageCalculationSet dcs) {
			// Find each type that was reflected and call the Reflection method.
			dcs.damageCalculations
				.Where(dc => dc.finalResistance == ResistanceType.Ref)
				.ToList()
				.ForEach(dc => dc.Reflect());
			return dcs;
		}
		/// <summary>
		/// A final pass which is very similar to IInterceptIncomingDCS, but is performed after all other operations have been taken into account.
		/// This is helpful for modifiers like Endure, which depend on that final result.
		/// </summary>
		/// <param name="dcs"></param>
		/// <returns></returns>
		protected virtual DamageCalculationSet LateInterceptionPass(DamageCalculationSet dcs) {
			foreach (Combatant self in dcs.Targets) {
				self.GetModifiers<IInterceptIncomingDCSLate>()
				.ForEach(m => dcs = m.InterceptIncomingDCSLate(dcs, self));
			}
			foreach (Combatant self in dcs.Sources) {
				self.GetModifiers<IInterceptIncomingDCSLate>()
				.ForEach(m => dcs = m.InterceptIncomingDCSLate(dcs, self));
			}
			return dcs;
		}
		#endregion

		#region GENERAL CALCULATIONS
		protected static int DynamicCalculation(Combatant source, Combatant target, BattleBehavior self) {

			float attackStat = (self.costType == BehaviorCostType.MP) ? source.DynamicMA : source.DynamicST;
			if (source is Player) {
				attackStat *= GameController.Instance.DebugPlayerAttackMultiplier;
			}
			
			// If the move ignores EN, the defense factor is 1. Otherwise, its the target's EN.
			float defenseFactor = self.ignoreEN ? 1f : target.DynamicEN;

			Debug.Log("BATTLE: SOURCE ATTACK: " + attackStat + ", TARGET DEFENSE: " + defenseFactor);
			
			// Multiply the base power by the debug value stored in the game controller.
			float adjustedBasePower = self.basePower * GameController.Instance.DebugBasePowerMultiplier * GameController.Instance.DifficultyToggles.BattleBehaviorPowerMultiplier;
			
			// Five times square root of source attack/target defense ratio. The Math.Pow multiplier is just for debugging.
			int dmg = (int)((
				5f 
				* Mathf.Sqrt((attackStat / defenseFactor)
				* (int)(Mathf.Pow(
					f: adjustedBasePower,
					p: GameController.Instance.DifficultyToggles.BattleBehaviorPowerExponent)))) 
				* Random.Range(0.95f, 1.05f));
			

			// If this was a healing move, negate the amount.
			if (self.elementType == ElementType.Healing) {
				dmg = -dmg;
			} else {
				// Make sure to grab the boost multipliers.
				dmg = (int)((float)dmg * source.GetPowerBoost(PowerBoostType.Attack) / target.GetPowerBoost(PowerBoostType.Defense));
			}

			// Multiply by the Final Multiplier. As a debug thing.
			dmg = (int)(dmg * GameController.Instance.DebugFinalAmountMultiplier * GameController.Instance.DifficultyToggles.FinalValueMultiplier);

			Debug.Log("RAW CALCULATED DAMAGE: " + dmg);

			return dmg;
		}
		protected static int DynamicCalculation(List<Combatant> sources, Combatant target, BattleBehavior self) {
			int totalST = 0;
			foreach (Combatant source in sources) { // Get the total of all the sources' levels/STs.
				totalST += source.DynamicST;
			}
			totalST /= 2;

			// If the move ignores EN, the defense factor is 1. Otherwise, its the target's EN.
			float defenseFactor = self.ignoreEN ? 1f : target.DynamicEN;

			int dmg = (int)((
				5f 
				* Mathf.Sqrt(((float)totalST / defenseFactor)
				* (int)(Mathf.Pow(
					f: self.basePower * GameController.Instance.DifficultyToggles.BattleBehaviorPowerMultiplier,
					p: GameController.Instance.DifficultyToggles.BattleBehaviorPowerExponent)))) 
				* Random.Range(0.95f, 1.05f));

			// Multiply by the Final Multiplier. As a debug thing.
			dmg = (int)(dmg * GameController.Instance.DifficultyToggles.FinalValueMultiplier);

			return dmg;
		}
		protected static int StaticCalculation(Combatant source, Combatant target, BattleBehavior self) {
			
			// Multiply the base power by the debug value stored in the game controller.
			float adjustedBasePower = self.basePower * GameController.Instance.DebugBasePowerMultiplier * GameController.Instance.DifficultyToggles.BattleBehaviorPowerMultiplier;
			
			if (self.elementType == ElementType.Healing) { return -(int)adjustedBasePower; }
			return (int) adjustedBasePower;
			
			/*if (self.elementType == ElementType.Healing) { return -self.basePower; }
			return self.basePower;*/
		}
		protected static int PercentageCalculation(Combatant source, Combatant target, BattleBehavior self) {
			float healMult = 1f;
			if (self.elementType == ElementType.Healing) { healMult *= -1f; }
			switch (self.targetResource) {
				case BehaviorCostType.HP:
					return (int)((float)target.MaxHP * ((float)self.basePower / 100f) * healMult);
				case BehaviorCostType.MP:
					return (int)((float)target.MaxMP * ((float)self.basePower / 100f) * healMult);
				default:
					Debug.LogError("Couldn't determine target resource pool!");
					return 0;
			}
		}
		#endregion

		#region TACTICS CALCULATIONS
		/// <summary>
		/// Find the participants involved in a tactics behavior.
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static List<Combatant> GetParticipantsFromTacticsBehavior(BattleBehavior self) {
			List<Combatant> participants = new List<Combatant>();
			foreach (TacticsParticipantType participantType in self.participants) {
				participants.Add(BattleController.Instance.Players.Find(j => j.participantType == participantType));
			}
			return participants;
		}
		#endregion

		#region OTHER CALCULATIONS
		/// <summary>
		/// Calculates the accuracy before any interceptions have been made.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		protected static float CalculateRawAccuracy(Combatant source, Combatant target, BattleBehavior self) {
			// The accuracy should NOT be affected if the battle behavior is an item.
			if (self.behaviorType == BehaviorType.Item) {
				Debug.Log("Behavior is an item. Will override first pass accuracy.");
				return self.baseAccuracy;
			} else {
				// If it's not an item, take the accuracies of the source/target into account.
				return self.baseAccuracy * source.GetPowerBoost(PowerBoostType.Accuracy) / target.GetPowerBoost(PowerBoostType.Accuracy);
			}
			
		}
		/// <summary>
		/// Just calculates the base critical hit rate for the damage calculation.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		public static float CalculateRawCritRate(Combatant source, Combatant target, BattleBehavior self) {
			return self.baseCritRate;
		}
		/// <summary>
		/// A somewhat updated version of calculating accuracy.
		/// </summary>
		/// <param name="damageCalculation"></param>
		/// <returns></returns>
		protected static AccuracyType CalculateAccuracyType(DamageCalculation damageCalculation) {

			// If the calculation type is None, just back return Normal.
			if (damageCalculation.behavior.calculationType == BattleCalculationType.None) {
				Debug.Log("BATTLE: Just to make sure, " + damageCalculation.behavior.behaviorName + " was used," +
					"has a calculation of None, but resulted in rawAccuracy of " + damageCalculation.rawAccuracy + ". Returning Normal accuracy.");
				return AccuracyType.Normal;
			}

			Combatant source = damageCalculation.source;
			Combatant target = damageCalculation.target;
			BattleBehavior self = damageCalculation.behavior;

			// Generate two floats between 0 and 1
			float value1 = Random.value;
			float value2 = Random.value;
			float accuracy = damageCalculation.rawAccuracy;

			// If the first value is less than the base accuraccy, the move hits.
			Debug.Log("Base Accuracy: " + self.baseAccuracy + " -- Modified Accuracy: " + accuracy + " (Higher means more chance to hit)");
			if (value1 < accuracy) {
				// Check for critical next.
				if (value2 < self.baseCritRate) {
					return AccuracyType.Critical;
				} else {
					return AccuracyType.Normal;
				}
			} else {
				return AccuracyType.Miss;
			}
		}
		/// <summary>
		/// A battle behavior could have multiple afflictions to pick from. Choose one here.
		/// </summary>
		protected static AfflictionType PickAfflictionType(Combatant source, Combatant target, BattleBehavior self) {
			if (self.afflictionTypes.Count > 0) {
				return self.afflictionTypes[Random.Range(minInclusive: 0, maxExclusive: self.afflictionTypes.Count - 1)];
			} else {
				return AfflictionType.None;
			}
		}
		/// <summary>
		/// Similar to getting the normal accuracy, but only takes the affliction accuracy into account.
		/// </summary>
		protected static bool CalculateSetAffliction(Combatant source, Combatant target, BattleBehavior self) {

			// If the combatant is already affected by an affliction, don't set a new one.
			if (target.Affliction.Type != AfflictionType.None) {
				return false;
			}

			float value1 = Random.value;
			if (value1 < self.afflictionAccuracy && self.afflictionTypes.Count > 0) {
				// Actually make sure there are afflictions to set.
				return true;
			} else {
				return false;
			}
		}
		/// <summary>
		/// Determines whether this move will cure the target of an affliction.
		/// </summary>
		protected static bool CalculateCureAffliction(Combatant source, Combatant target, BattleBehavior self) {
			if (self.afflictionTypeCures.Contains(target.Affliction.Type)) {
				return true;
			} else {
				return false;
			}
		}
		#endregion


		#region INSPECTOR JUNK
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "The standard way in which BattleBehaviors should be calculated. You'll be using this 95% of the time.";
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