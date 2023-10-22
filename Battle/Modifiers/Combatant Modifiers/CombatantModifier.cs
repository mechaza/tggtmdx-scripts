using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Functions;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.Modifiers.Afflictions;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif


namespace Grawly.Battle.Modifiers {


	/// <summary>
	/// A class that can be used to give a combatant certain modifications that respond to stimuli.
	/// </summary>
	public abstract class CombatantModifier {

		#region FIELDS - STATE
		/// <summary>
		/// The combatant who owns this modifier.
		/// </summary>
		protected Combatant combatantOwner;
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// A class that can be used to give a combatant certain modifications that respond to stimuli.
		/// </summary>
		public CombatantModifier() {
			// The blank constructor is more or less for situations where the modifier is inside of a template or something.
		}
		/// <summary>
		/// A class that can be used to give a combatant certain modifications that respond to stimuli.
		/// </summary>
		/// <param name="self"></param>
		public CombatantModifier(Combatant self) {
			// Make sure this modifier is aware of who its attached to.
			this.combatantOwner = self;
		}
		/// <summary>
		/// Assigns a combatant to be used for this modifier and then returns this modifier.
		/// Helpful for LINQ shit.
		/// </summary>
		/// <param name="combatant">The combatant to assign to this modifier.</param>
		/// <returns>This modifier.</returns>
		public CombatantModifier AssignCombatant(Combatant combatant) {
			this.combatantOwner = combatant;
			return this;
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Creates a combatant affliction from a specific type.
		/// </summary>
		public static ICombatantAffliction CombatantAfflictionFromType(AfflictionType type, Combatant combatant) {
			switch (type) {
				case AfflictionType.None:
					return new NullAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Burn:
					return new BurnAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Freeze:
					return new FreezeAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Poison:
					return new PoisonAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Paralyze:
					return new ParalyzeAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Dizzy:
					return new DizzyAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Silence:
					return new SilenceAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Sleep:
					return new SleepAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Confuse:
					return new ConfuseAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Fear:
					return new FearAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Despair:
					return new DespairAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Rage:
					return new RageAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Brainwash:
					return new BrainwashAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Hunger:
					return new HungerAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Forget:
					return new ForgetAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				case AfflictionType.Horny:
					return new HornyAffliction().AssignCombatant(combatant) as ICombatantAffliction;
				default:
					Debug.LogError("Type not found: " + type.ToString() + ". Using null affliction.");
					return new NullAffliction().AssignCombatant(combatant) as ICombatantAffliction;
			}
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Clones the combatant modifier so it can be used outside of the BattleBehavior, or any other place where it's sourced.
		/// </summary>
		/// <param name="combatant">The combatant to hand over to the modifier, because it's going to need that actually.</param>
		/// <returns></returns>
		public CombatantModifier Clone() {
			// Clone this modifier for use.
			CombatantModifier modifierClone = (CombatantModifier)this.MemberwiseClone();
			/*// Pass it a reference to the combatant, because it will need that.
			modifierClone.self = combatant;
			// All set! Return it.*/
			return modifierClone;
		}
		#endregion

		#region INSPECTOR BULLSHIT
#if UNITY_EDITOR
		/// <summary>
		/// This is what I need to use for making sure info boxes appear in the inspector without actually having to assign a field to accompany it.
		/// </summary>
		[PropertyOrder(int.MinValue), OnInspectorGUI]
		private void DrawIntroInfoBox() {
			SirenixEditorGUI.InfoMessageBox(this.InspectorDescription);
		}
#endif
		/// <summary>
		/// The string that gets used in the info box that describes this BattleBehaviorFunction.
		/// </summary>
		protected abstract string InspectorDescription { get; }
		#endregion

	}


}