using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle.WorldEnemies {

	
	/// <summary>
	/// The way in which the purple cube should be animated.
	/// </summary>
	public class PurpleCubeWorldEnemy : WorldEnemyDX3D {

		#region PREPARATION
		/// <summary>
		/// Prepares this WorldEnemyDXSprite for use in the battle.
		/// </summary>
		/// <param name="enemyTemplate">The template to use when preparing this WorldEnemyDXSprite.</param>
		public override void PrepareWorldEnemy(EnemyTemplate enemyTemplate) {
			base.PrepareWorldEnemy(enemyTemplate);
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			base.AnimateBehaviorUse(damageCalculationSet);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public override void AnimateDownedStatus(Combatant combatant, bool isDown) {
			base.AnimateDownedStatus(combatant, isDown);
			throw new NotImplementedException();
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public override void AnimateFocusHighlight(Combatant combatant, float? time) {
			base.AnimateFocusHighlight(combatant, time);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public override void AnimateFocusDehighlight(Combatant combatant, bool instantaneous) {
			base.AnimateFocusDehighlight(combatant, instantaneous);
			throw new NotImplementedException();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATIONS
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {
			base.AnimateHarmfulCalculation(damageCalculation);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateRestorationCalculation(DamageCalculation damageCalculation) {
			base.AnimateRestorationCalculation(damageCalculation);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public override void AnimateReflection(DamageCalculation damageCalculation) {
			base.AnimateReflection(damageCalculation);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public override void AnimateStatusBoost(DamageCalculation damageCalculation) {
			base.AnimateStatusBoost(damageCalculation);
			throw new NotImplementedException();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : MISC ANIMATIONS
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// Very helpful when animating things like buffs/debuffs.
		/// </summary>
		/// <param name="combatant">The combatant who has this animator.</param>
		/// <param name="color">The color to pulse.</param>
		/// <param name="time">The amount of time to take when pulsing.</param>
		public override void AnimatePulseColor(Combatant combatant, Color color, float time) {
			base.AnimatePulseColor(combatant, color, time);
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public override void AnimatePulseColor(DamageCalculation damageCalculation) {
			base.AnimatePulseColor(damageCalculation);
			throw new NotImplementedException();
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : AFFLICTION ANIMATION
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		public override void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType) {
			base.AnimateAfflictionSet(combatant, afflictionType);
			throw new NotImplementedException();
		}
		#endregion
		
	}
	
}