using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using Sirenix.Serialization;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// This is the function that gets used primarily for how to animate a WorldEnemyDX under different circumstances.
	/// Is stored in the WorldEnemyDX and makes use of its fields to manipulate it. Think of it as the 'brains.'
	/// </summary>
	/// <remarks>
	/// If tighter control is needed (e.x., references need to be made to things in scene,) this class should not be used.
	/// Create a custom WorldEnemyDX instead.
	/// </remarks>
	/// <typeparam name="T">The type of WorldEnemy that this behavior should be controlling.</typeparam>
	public abstract class WorldEnemyDXBehavior<T> : ICombatantAnimator where T : WorldEnemyDX {

		#region INTERFACE IMPLEMENTATION - EVENT ANIMATION
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculationSet.PrimarySource as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateBehaviorUse(damageCalculationSet: damageCalculationSet, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet, T worldEnemy);
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculationSet.PrimarySource as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateBehaviorUseInterruption(damageCalculationSet: damageCalculationSet, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet, T worldEnemy) {
			throw new System.NotImplementedException("I have not actually implemented this for enemies.");
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public void AnimateDownedStatus(Combatant combatant, bool isDown) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateDownedStatus(combatant: combatant, isDown: isDown, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateDownedStatus(Combatant combatant, bool isDown, T worldEnemy);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATION
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			// Call the version of this function that actually takes it.
			this.AnimateHarmfulCalculation(damageCalculation, worldEnemy as T);
		}
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateHarmfulCalculation(DamageCalculation damageCalculation, T worldEnemy);
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public void AnimateRestorationCalculation(DamageCalculation damageCalculation) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			// Call the version of this function that actually takes it.
			this.AnimateRestorationCalculation(damageCalculation, worldEnemy as T);
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateRestorationCalculation(DamageCalculation damageCalculation, T worldEnemy);
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public void AnimateReflection(DamageCalculation damageCalculation){
			// Grab the world enemy.
			// WorldEnemyDX worldEnemy = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			WorldEnemyDX worldEnemy = (damageCalculation.OriginalTarget as Enemy).WorldEnemyDX;
			// Call the version of this function that actually takes it.
			this.AnimateReflection(damageCalculation, worldEnemy as T);
		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateReflection(DamageCalculation damageCalculation, T worldEnemy);
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public  void AnimateStatusBoost(DamageCalculation damageCalculation){
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			// Call the version of this function that actually takes it.
			this.AnimateStatusBoost(damageCalculation, worldEnemy as T);
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateStatusBoost(DamageCalculation damageCalculation, T worldEnemy);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : SIMPLE ANIMATING
		/// <summary>
		/// Animates damage when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of damage to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes this information.
			this.SimpleAnimateHarmfulCalculation(
				combatant: combatant, 
				damageAmount: damageAmount, 
				resourceType: resourceType, 
				worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates damage when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of damage to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType, T worldEnemy);
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes this information.
			this.SimpleAnimateRestorationCalculation(
				combatant: combatant, 
				damageAmount: damageAmount, 
				resourceType: resourceType, 
				worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType, T worldEnemy);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public void AnimateFocusHighlight(Combatant combatant, float? time = null) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateFocusHighlight(combatant: combatant, time: time, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateFocusHighlight(Combatant combatant, float? time, T worldEnemy);
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateFocusDehighlight(combatant: combatant, instantaneous: instantaneous, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateFocusDehighlight(Combatant combatant, bool instantaneous, T worldEnemy);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : MISC ANIMATIONS
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// Very helpful when animating things like buffs/debuffs.
		/// </summary>
		/// <param name="combatant">The combatant who has this animator.</param>
		/// <param name="color">The color to pulse.</param>
		/// <param name="time">The amount of time to take when pulsing.</param>
		public void AnimatePulseColor(Combatant combatant, Color color, float time) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimatePulseColor(combatant: combatant, color: color, time: time, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// Very helpful when animating things like buffs/debuffs.
		/// </summary>
		/// <param name="combatant">The combatant who has this animator.</param>
		/// <param name="color">The color to pulse.</param>
		/// <param name="time">The amount of time to take when pulsing.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimatePulseColor(Combatant combatant, Color color, float time, T worldEnemy);
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public void AnimatePulseColor(DamageCalculation damageCalculation) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			// Call the version of this function that actually takes it.
			this.AnimatePulseColor(damageCalculation: damageCalculation, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimatePulseColor(DamageCalculation damageCalculation, T worldEnemy);
		#endregion
		#region INTERFACE IMPLEMENTATINO - ICOMBATANTANIMATOR : AFFLICTION ANIMATION

		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		public void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType) {
			// Grab the world enemy.
			WorldEnemyDX worldEnemy = (combatant as Enemy).WorldEnemyDX;
			// Call the version of this function that takes it.
			this.AnimateAfflictionSet(combatant: combatant, afflictionType: afflictionType, worldEnemy: worldEnemy as T);
		}
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected abstract void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType, T worldEnemy);
		#endregion
		
		#region CLONING
		/// <summary>
		/// Clones the behavior so that it can retain state outside of the template it came from.
		/// </summary>
		/// <returns></returns>
		public WorldEnemyDXBehavior<T> Clone() {
			WorldEnemyDXBehavior<T> clonedBehavior =  this.MemberwiseClone() as WorldEnemyDXBehavior<T>;
			return clonedBehavior;
		}
		#endregion	

	}


}