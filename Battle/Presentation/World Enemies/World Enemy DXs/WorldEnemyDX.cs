using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using Grawly.Toggles;
using UnityEngine.Serialization;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// The representation of enemies in the scene. By default, the arena will use the sprites (as defined by its setup behavior)
	/// but as long as something has a WorldEnemyDX component it can potentially be part of the battle.
	/// They are effectively animated by their WorldEnemyDXBehaviors. 
	/// </summary>
	public abstract class WorldEnemyDX : MonoBehaviour, ICombatantAnimator {
		
		#region FIELDS - STATE
		/// <summary>
		/// The enemy associated with this WorldEnemyDX.
		/// BattleController reads from this value directly to determine who is still in the battle.
		/// </summary>
		public Enemy Enemy { get; protected set; }
		#endregion

		#region ABSTRACT PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The GameObject that has all of the visuals for the world enemy.
		/// </summary>
		public abstract GameObject AllVisuals { get; }
		/// <summary>
		/// The GameObject containing the visuals for the enemy only.
		/// </summary>
		public abstract GameObject EnemyVisuals { get; }
		/// <summary>
		/// The camera that is used for the zoom in effect.
		/// </summary>
		public abstract CinemachineVirtualCamera ZoomInCamera { get; }
		/// <summary>
		/// The GameObject that Cinemachine should use when targeting this WorldEnemyDX.
		/// </summary>
		public abstract GameObject CinemachineTarget { get; }
		#endregion
		
		#region ABSTRACT PROPERTIES - POSITIONS
		/// <summary>
		/// Gets the position of where a EnemyCursorDX should be placed on any given frame.
		/// </summary>
		/// <returns></returns>
		public abstract Vector3 CursorPosition { get; }
		/// <summary>
		/// Gets the position of where particle effects should be spawned around this WorldEnemy (e.x., when getting attacked, might need some dust.)
		/// </summary>
		/// <returns></returns>
		public abstract Vector3 ParticleSpawnPosition { get; }
		#endregion
		
		#region ABSTRACT - PREPARATION
		/// <summary>
		/// Preps this WorldEnemyDX for use in battle.
		/// </summary>
		/// <param name="enemyTemplate">The enemy to assemble this template with.</param>
		public abstract void PrepareWorldEnemy(EnemyTemplate enemyTemplate);
		#endregion
		
		#region INTERFACE IMPLEMENTATION - EVENT ANIMATION
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public abstract void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public abstract void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public abstract void AnimateDownedStatus(Combatant combatant, bool isDown);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATION
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public abstract void AnimateHarmfulCalculation(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public abstract void AnimateRestorationCalculation(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public abstract void AnimateReflection(DamageCalculation damageCalculation);
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public abstract void AnimateStatusBoost(DamageCalculation damageCalculation);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : SIMPLE ANIMATING
		/// <summary>
		/// Animates damage when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of damage to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public abstract void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType);
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public abstract void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public abstract void AnimateFocusHighlight(Combatant combatant, float? time = null);
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public abstract void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false);
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : MISC ANIMATIONS
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// Very helpful when animating things like buffs/debuffs.
		/// </summary>
		/// <param name="combatant">The combatant who has this animator.</param>
		/// <param name="color">The color to pulse.</param>
		/// <param name="time">The amount of time to take when pulsing.</param>
		public abstract void AnimatePulseColor(Combatant combatant, Color color, float time);
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public abstract void AnimatePulseColor(DamageCalculation damageCalculation);
		#endregion

		#region INTERFACE IMPLEMENTATINO - ICOMBATANTANIMATOR : AFFLICTION ANIMATION
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		public abstract void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType);
		#endregion

	}


}