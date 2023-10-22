using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Cinemachine;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// The world enemy as represented by a sprite.
	/// </summary>
	public class WorldEnemyDXSprite : WorldEnemyDX {

		#region FIELDS - STATE
		/// <summary>
		/// The world behavior that should be used for this sprite.
		/// </summary>
		private WorldEnemyDXSpriteBehavior worldBehavior { get; set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES : ROOTS
		/// <summary>
		/// The GameObject containing ALL of the visuals for this enemy sprite.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References"), Title("Roots")]
		private GameObject allVisuals;
		/// <summary>
		/// The GameObject containing the visuals for only the enemy itself.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject enemyVisuals;
		#endregion

		#region FIELDS - SCENE REFERENCES - PROPS
		/// <summary>
		/// The zoom in camera that should be used for this sprite.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References"), Title("Props")]
		private CinemachineVirtualCamera zoomInCamera;
		/// <summary>
		/// The target to focus on for cinemachine.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject cinemachineTarget;
		/// <summary>
		/// The game object that shows whether this enemy is downed or not.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal GameObject downTagGameObject;
		/// <summary>
		/// The GameObject where affliction visuals should be set.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal GameObject afflictionVisualsGameObject;
		/// <summary>
		/// The component that contains the thought bubble for the enemy.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal WorldEnemySpriteThoughtBubble thoughtBubble;
		/// <summary>
		/// The component that contains the thought bubble for the enemy.
		/// </summary>
		public WorldEnemySpriteThoughtBubble ThoughtBubble {
			get {
				return this.thoughtBubble;
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES : SPRITE RENDERERS
		/// <summary>
		/// The sprite that shows the enemy.
		/// Is internal so I can access it from the behavior.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References"), Title("Sprite Renderers (Body)")]
		internal SpriteRenderer worldEnemySpriteRenderer;
		/// <summary>
		/// The sprite that is used for the flash effect. It is located directly in front of the worldEnemySprite.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal SpriteRenderer worldEnemyFlashSpriteRenderer;
		/// <summary>
		/// The sprite that is used for the pulse effect. It is located directly in front of the worldEnemySprite, but behind the flash.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal SpriteRenderer worldEnemyPulseSpriteRenderer;
		/// <summary>
		/// The sprite renderer for the enemy's highlight square sprite. 
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References"), Title("Sprite Renderers (Effect)")]
		internal SpriteRenderer worldEnemyHighlightSquareSpriteRenderer;
		/// <summary>
		/// The sprite that shows the enemy's dropshadow. This is especially helpful for the highlight effect.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		internal SpriteRenderer worldEnemyDropshadowSpriteRenderer;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The GameObject containing all visuals for this World Enemy.
		/// </summary>
		public override GameObject AllVisuals {
			get {
				return this.allVisuals;
			}
		}
		/// <summary>
		/// The GameObject containing all visuals for this World Enemy.
		/// </summary>
		public override GameObject EnemyVisuals {
			get {
				return this.enemyVisuals;
			}
		}
		/// <summary>
		/// The zoom in camera for this sprite.
		/// </summary>
		public override CinemachineVirtualCamera ZoomInCamera {
			get {
				return this.zoomInCamera;
			}
		}
		/// <summary>
		/// The target to focus on for cinemachine.
		/// </summary>
		public override GameObject CinemachineTarget {
			get {
				return this.cinemachineTarget;
			}
		}
		#endregion
		
		#region PROPERTIES - POSITIONS
		/// <summary>
		/// The position the cursor should be.
		/// </summary>
		public override Vector3 CursorPosition {
			get {
				return this.worldBehavior.GetCursorPosition(worldEnemy: this);
			}
		}
		/// <summary>
		/// The anchor for the particle spawn.
		/// </summary>
		public override Vector3 ParticleSpawnPosition {
			get {
				return this.worldBehavior.GetParticleAnchor(worldEnemy: this);
			}
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Prepares this WorldEnemyDXSprite for use in the battle.
		/// </summary>
		/// <param name="enemyTemplate">The template to use when preparing this WorldEnemyDXSprite.</param>
		public override void PrepareWorldEnemy(EnemyTemplate enemyTemplate) {
			// Set the state variables, which are absolutely super needed.
			this.Enemy = new Enemy(template: enemyTemplate, gameVariables: GameController.Instance.Variables, worldEnemyDX: this);
			
			// Create a new world behavior. This may be deprecated in the future? Idk.
			this.worldBehavior = new WorldEnemyDXSpriteBehavior();
			
			// Make sure to set the visuals objects active! They may have been turned off previously.
			this.AllVisuals.SetActive(true);
			this.EnemyVisuals.gameObject.SetActive(true);
			
			// Set the sprites to the ones stored in the template.
			this.ChangeEnemySprite(sprite: enemyTemplate.BodySprite);
			
			// Make sure the color on the sprite is white. If the last enemy was downed, it could be a problem.
			this.worldEnemySpriteRenderer.color = Color.white;
			// Also turn off the down tag. It shouldn't be on by default.
			this.downTagGameObject.SetActive(false);
			// Set the material on the sprite renderer to the default material. It may need resetting.
			this.worldEnemySpriteRenderer.material = DataController.GetAfflictionMaterial(AfflictionType.None);
			
		}
		#endregion

		#region SETTERS - ENEMY VISUALS
		/// <summary>
		/// Changes the sprite on this WorldEnemyDXSprite to the one passed in.
		/// </summary>
		/// <param name="sprite"></param>
		public void ChangeEnemySprite(Sprite sprite) {
			// Set the sprites to the ones stored in the template.
			this.worldEnemySpriteRenderer.sprite = sprite;
			this.worldEnemyFlashSpriteRenderer.sprite = sprite;
			this.worldEnemyPulseSpriteRenderer.sprite = sprite;
			this.worldEnemyDropshadowSpriteRenderer.sprite = sprite;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - EVENT ANIMATION
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			this.worldBehavior.AnimateBehaviorUse(damageCalculationSet);
		}
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet) {
			this.worldBehavior.AnimateBehaviorUseInterruption(damageCalculationSet);
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public override void AnimateDownedStatus(Combatant combatant, bool isDown) {
			this.worldBehavior.AnimateDownedStatus(combatant, isDown);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATION
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {
			this.worldBehavior.AnimateHarmfulCalculation(damageCalculation);
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateRestorationCalculation(DamageCalculation damageCalculation) {
			this.worldBehavior.AnimateRestorationCalculation(damageCalculation);
		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public override void AnimateReflection(DamageCalculation damageCalculation) {
			this.worldBehavior.AnimateReflection(damageCalculation);
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public override void AnimateStatusBoost(DamageCalculation damageCalculation) {
			this.worldBehavior.AnimateStatusBoost(damageCalculation);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : SIMPLE ANIMATING
		/// <summary>
		/// Animates damage when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of damage to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public override void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {
			this.worldBehavior.SimpleAnimateHarmfulCalculation(combatant, damageAmount, resourceType);
		}
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public override void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {
			this.worldBehavior.SimpleAnimateRestorationCalculation(combatant, damageAmount, resourceType);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public override void AnimateFocusHighlight(Combatant combatant, float? time = null) {
			this.worldBehavior.AnimateFocusHighlight(combatant, time);
		}
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public override void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false) {
			this.worldBehavior.AnimateFocusDehighlight(combatant, instantaneous);
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
			this.worldBehavior.AnimatePulseColor(combatant, color, time);
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public override void AnimatePulseColor(DamageCalculation damageCalculation) {
			this.worldBehavior.AnimatePulseColor(damageCalculation);
		}
		#endregion

		#region INTERFACE IMPLEMENTATINO - ICOMBATANTANIMATOR : AFFLICTION ANIMATION
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		public override void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType) {
			this.worldBehavior.AnimateAfflictionSet(combatant, afflictionType);
		}
		#endregion
		
	}

}