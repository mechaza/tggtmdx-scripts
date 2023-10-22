using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle {

	/// <summary>
	/// This is what I use inside battlebehavioranimations as a general way to tell a combatant to animate.
	/// This is becasue players and enemies have grossly different animation routines but I need a way to abstract that out and be able to call their behaviors.
	/// </summary>
	public interface ICombatantAnimator {

		#region EVENT ANIMATING
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		void AnimateDownedStatus(Combatant combatant, bool isDown);
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		void AnimateFocusHighlight(Combatant combatant, float? time = null);
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false);
		#endregion

		#region CALCULATION ANIMATING
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		void AnimateHarmfulCalculation(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		void AnimateRestorationCalculation(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		void AnimateStatusBoost(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		void AnimateReflection(DamageCalculation damageCalculation);
		#endregion

		#region SIMPLE ANIMATING
		/// <summary>
		/// Animates damage when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of damage to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType);
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType);
		#endregion

		#region MISC EFFECTS
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// Very helpful when animating things like buffs/debuffs.
		/// </summary>
		/// <param name="combatant">The combatant who has this animator.</param>
		/// <param name="color">The color to pulse.</param>
		/// <param name="time">The amount of time to take when pulsing.</param>
		void AnimatePulseColor(Combatant combatant, Color color, float time);
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		void AnimatePulseColor(DamageCalculation damageCalculation);
		#endregion

		#region AFFLICTION ANIMATING
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType);
		#endregion

	}


}