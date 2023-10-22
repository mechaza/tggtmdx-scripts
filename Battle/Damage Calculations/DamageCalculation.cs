using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;
using System.Linq;

namespace Grawly.Battle {

	/// <summary>
	/// Stores the results and metadata for an attack/move between any source and target.
	/// </summary>
	public class DamageCalculation {

		#region FIELDS - IMPORTANT
		/// <summary>
		/// The behavior that was used in this calculation.
		/// </summary>
		public BattleBehavior behavior;
		/// <summary>
		/// The value that this calculation should deduct from whatever resource it is targeting.
		/// </summary>
		public int rawDamageAmount;
		/// <summary>
		/// Was this calculation made as a result of just wanting to impact the enemy quickly?
		/// </summary>
		private bool isSimpleCalculation = false;
		#endregion

		#region FIELDS - COMBATANTS
		/// <summary>
		/// The combatant who used the behavior associated with this calculation.
		/// </summary>
		public Combatant source;
		/// <summary>
		/// The combatant who is being targeted by this calculation.
		/// </summary>
		public Combatant target;
		/// <summary>
		/// The "Final Target" of this calculation.
		/// It can actually change depending on if this move was reflected or not.
		/// </summary>
		public Combatant FinalTarget {
			get {
				if (this.WasReflected == true) {
					// If this calculation was reflected, swap the target/source.
					return this.source;
				} else {
					// If this wasn't reflected, just return the target as normal.
					return this.target;
				}
			}
		}
		/// <summary>
		/// The original target, regardless of whether or not this calculation was reflected or not.
		/// </summary>
		public Combatant OriginalTarget {
			get {
				return this.target;
			}
		}
		#endregion

		#region FIELDS - ACCURACY/CRIT/RESISTANCE
		/// <summary>
		/// The resistance that the target has to the specified behavior.
		/// Note that resistance may be affected by modifiers so I don't think contacting the target directly is appropriate.
		/// </summary>
		public ResistanceType finalResistance;
		/// <summary>
		/// The "type" of accuracy. Is determined near the end of the standard calculation passes. 
		/// </summary>
		public AccuracyType accuracyType;
		/// <summary>
		/// The "raw" value of the accuracy, which is used for determining the type.
		/// </summary>
		public float rawAccuracy = 1f;
		/// <summary>
		/// The "raw" value of the critical rate. Gets set via the inital value in the BattleBehavior, but may be manipulated through interceptions and the like.
		/// </summary>
		public float rawCritRate = 0f;
		#endregion

		#region FIELDS - AFFLICTION
		/// <summary>
		/// A behavior could have multiple afflictions to choose from. The DamageCalculation stores the one that is ultimately picked.
		/// </summary>
		public AfflictionType afflictionType;
		/// <summary>
		/// Is this calculation going to set an affliction on the target when its done?
		/// </summary>
		public bool setAffliction;
		/// <summary>
		/// Is this calculation going to cure the target's affliction when its done?
		/// </summary>
		public bool cureAffliction;
		#endregion

		#region FIELDS - BUFFS/DEBUFFS
		/// <summary>
		/// A list of key/value pairs that say if a combatant should be buffed/debuffed for the given boost.
		/// </summary>
		public List<KeyValuePair<PowerBoostType, PowerBoostIntentionType>> powerBoosts = new List<KeyValuePair<PowerBoostType, PowerBoostIntentionType>>();
		#endregion

		#region FIELDS - STATE
		/// <summary>
		/// Was this calculation reflected?
		/// This affects how animations are handled.
		/// </summary>
		public bool WasReflected { get; private set; } = false;
		#endregion

		#region FIELDS - PRIVATE FLAGS
		/// <summary>
		/// If this is true, the target will not be downed even under circumstances which would ordinarily do so.
		/// </summary>
		private bool doNotDownTarget = false;
		#endregion

		#region PROPERTIES : STATE
		/// <summary>
		/// Was the target hit?
		/// </summary>
		public bool TargetWasHit {
			get {
				// Just return true if it's anything other than a miss.
				return this.accuracyType != AccuracyType.Miss;
			}
		}
		/// <summary>
		/// Was the target's HP healed?
		/// </summary>
		public bool TargetHPWasHealed {
			get {
				return this.TargetWasHit && this.HPDamage < 0;
			}
		}
		/// <summary>
		/// Was the target's MP healed?
		/// </summary>
		public bool TargetMPWasHealed {
			get {
				return this.TargetWasHit && this.MPDamage < 0;
			}
		}
		/// <summary>
		/// Is the target combatant intentionally being harmed?
		/// </summary>
		public bool TargetTookHPDamage {
			get {
				// If damage was dealt and it was intentionally targeting a resource, return true.
				return (this.HPDamage > 0 && behavior.targetResource != BehaviorCostType.None);
			}
		}
		/// <summary>
		/// Will the target die if this calculation is evaluated?
		/// </summary>
		public bool TargetWillDie {
			get {
				return ((this.FinalTarget.HP - this.HPDamage) <= 0);
			}
		}
		/// <summary>
		/// Will the target of this behavior be downed once it hits them?
		/// </summary>
		public bool TargetWillBeDowned {
			get {

				// First and foremost, if the 'do not down' flag is true, return false. This happens when defending.
				if (this.doNotDownTarget == true || this.TargetWasHit == false) { return false; }

				// Right now I'm just checking for whether or not the move was Weak or Critical. Also check if the target is already down to begin with.
				return (this.finalResistance == ResistanceType.Wk || this.accuracyType == AccuracyType.Critical || this.FinalTarget.IsDown == true);
			}
		}
		#endregion

		#region PROPERTIES : RESOURCE DAMAGE
		/// <summary>
		/// The absolute value of the damage calculation.
		/// Takes into account the fact that restoration is technically negative damage.
		/// </summary>
		public int DamageMagnitude {
			get {
				return Mathf.Abs(this.rawDamageAmount);
			}
		}
		/// <summary>
		/// The amount of damage that is going to be deducted from the health of the target.
		/// </summary>
		public int HPDamage {
			get {
				return this.behavior.targetResource == BehaviorCostType.HP ? this.rawDamageAmount : 0;
			}
		}
		/// <summary>
		/// The amount of damage that is going to be deducted from the health of the target.
		/// </summary>
		public int MPDamage {
			get {
				return this.behavior.targetResource == BehaviorCostType.MP ? this.rawDamageAmount : 0;
			}
		}
		#endregion

		#region PROPERTIES : HP VALUES
		/// <summary>
		/// The HP of the target before it is attacked.
		/// </summary>
		public int OldFinalTargetHP {
			get {
				return this.FinalTarget.HP;
			}
		}
		/// <summary>
		/// The target's HP once this calculation is finished being evaluated.
		/// </summary>
		public int NewFinalTargetHP {
			get {
				return Mathf.Clamp(value: this.FinalTarget.HP - this.HPDamage, min: 0, max: this.FinalTarget.MaxHP);
			}
		}
		/// <summary>
		/// The ratio that the final target's HP will be once the calculation is evaluated.
		/// </summary>
		public float NewFinalTargetHPRatio {
			get {
				return (float)this.NewFinalTargetHP / (float)this.FinalTarget.MaxHP;
			}
		}
		#endregion

		#region PROPERTIES : MP VALUES
		/// <summary>
		/// The target's MP once this calculation is finished being evaluated.
		/// </summary>
		public int NewFinalTargetMP {
			get {
				return Mathf.Clamp(value: this.FinalTarget.MP - this.MPDamage, min: 0, max: this.FinalTarget.MaxMP);
			}
		}
		/// <summary>
		/// The ratio that the final target's MP will be once the calculation is evaluated.
		/// </summary>
		public float NewFinalTargetMPRatio {
			get {
				return (float)this.NewFinalTargetMP / (float)this.FinalTarget.MaxMP;
			}
		}
		#endregion

		#region PROPERTIES : AFFLICTION
		/// <summary>
		/// Was the target affected by an affliction?
		/// </summary>
		public bool TargetWasAfflicted {
			get {
				// In regards to animation, both set and cure technically will want to animate since cure just sets the None affliction.
				// ONLY return true if the base accuracy was measured as a hit.
				return (this.setAffliction || this.cureAffliction) && this.TargetWasHit == true;
			}
		}
		#endregion

		#region PROPERTIES : BOOSTS
		/// <summary>
		/// Was this target given a power boost of some kind? Buff or debuff.
		/// </summary>
		public bool TargetWasBuffedOrDebuffed {
			get {
				return this.TargetWasBuffed || this.TargetWasDebuffed;
			}
		}
		/// <summary>
		/// Was the target buffed?
		/// </summary>
		public bool TargetWasBuffed {
			get {
				return this.powerBoosts.Count(kvp => kvp.Value == PowerBoostIntentionType.Buff) > 0;
			}
		}
		/// <summary>
		/// Was the target debuffed?
		/// </summary>
		public bool TargetWasDebuffed {
			get {
				return this.powerBoosts.Count(kvp => kvp.Value == PowerBoostIntentionType.Debuff) > 0;
			}
		}
		#endregion

		#region PROPERTIES : ANIMATION RELATED
		/// <summary>
		/// Should the target show damage in the UI?
		/// Some resistances/accuracies shouldn't because it would just be zero or something.
		/// </summary>
		public bool TargetShouldShowDamageAmount {
			get {

				// Override this property if the battle behavior specifically says to do so.
				if (this.behavior.showDamageAmountToggle != BattleBehaviorAnimationToggleType.Dynamic) {
					return this.behavior.showDamageAmountToggle == BattleBehaviorAnimationToggleType.Always ? true : false;
				}

				// If the calculation missed, or the combatant blocked/reflected the move, damage should not be shown.
				// Damage should not be shown for behaviors where the cost type is None, either.

				if (this.accuracyType == AccuracyType.Miss || this.finalResistance == ResistanceType.Nul || this.finalResistance == ResistanceType.Ref || this.behavior.targetResource == BehaviorCostType.None) {
					return false;
				}
				return true;

			}
		}
		/// <summary>
		/// Should the target show the tag that shows their resistance?
		/// </summary>
		public bool TargetShouldShowResistanceTag {
			get {

				// Override this property if the battle behavior specifically says to do so.
				if (this.behavior.showResistanceTagToggle != BattleBehaviorAnimationToggleType.Dynamic) {
					return this.behavior.showResistanceTagToggle == BattleBehaviorAnimationToggleType.Always ? true : false;
				}

				if (this.behavior.targetResource != BehaviorCostType.None) {
					return true;
				} else {
					return false;
				}
			}
		}
		#endregion

		#region CALCULATION MODIFIERS
		/// <summary>
		/// Tweaks the damage calculation in such a way that handles situations when a move gets reflected.
		/// </summary>
		/// <returns></returns>
		public DamageCalculation Reflect() {
			// Check the resistance the source has to the behavior.
			this.finalResistance = this.source.CheckResistance(behavior: this.behavior);
			// If the source ALSO reflects it, just make it null.
			if (this.finalResistance == ResistanceType.Ref) {
				this.finalResistance = ResistanceType.Nul;
			}
			// Set the flag to say this calculation was reflected.
			this.WasReflected = true;
			return this;
		}
		/// <summary>
		/// Nullifies the damage calculation.
		/// </summary>
		public void Nullify() {
			Debug.Log("NULLIFYING");
			this.finalResistance = ResistanceType.Nul;
			this.rawDamageAmount = 0;
		}
		/// <summary>
		/// Nullifies the affliction.
		/// </summary>
		public void NullifyAffliction() {
			Debug.LogWarning("Moves that have multiple affliction types may be completely nullified if just ONE of them is an affliction that a combatant is able to nullify. See if you can change this.");
			this.setAffliction = false;
		}
		/// <summary>
		/// Changes the resistance type to Absorb.
		/// </summary>
		public void Absorb() {
			Debug.Log("Changing final resistance type to Absorb");
			this.finalResistance = ResistanceType.Abs;
		}
		/// <summary>
		/// Overrides the downed status of the target so that it will Not be downed.
		/// </summary>
		/// <param name="downTarget"></param>
		public void DoNotAllowTargetToBeDowned() {
			Debug.Log("Setting 'Do Not Down' flag on " + this.target.metaData.name);
			this.doNotDownTarget = true;
		}
		#endregion

		#region BUILDERS
		/// <summary>
		/// Builds up a basic TargetDamageTuple with placeholder information. Not all may be needed.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static DamageCalculation BuildSimpleCalculation(Combatant target, int amt) {
			DamageCalculation dc = new DamageCalculation();
			dc.source = target;
			dc.target = target;
			// damageTuple.behavior = BattleController.Instance.allOutAttack;
			dc.behavior = DataController.Instance.GetBehavior(behaviorName: "All Out Attack");
			dc.finalResistance = ResistanceType.Nm;
			dc.accuracyType = AccuracyType.Normal;
			dc.afflictionType = AfflictionType.None;
			dc.setAffliction = false;
			dc.cureAffliction = false;
			dc.rawDamageAmount = amt;
			dc.isSimpleCalculation = true;
			return dc;
		}
		#endregion

	}

}