using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Cinemachine;
using Grawly.Battle;
using Grawly.Battle.BattleMenu;
using DG.Tweening;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// This is what should be used when making 3D enemies.
	/// </summary>
	public abstract class WorldEnemyDX3D : WorldEnemyDX {
		
		#region FIELDS - SCENE REFERENCES : VISUALS
		/// <summary>
		/// ALL of the visuals.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The visuals for the enemy, specifically.
		/// Does not contain other visuals or anchors.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject enemyVisuals;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : ANCHORS
		/// <summary>
		/// The anchor to use for the cursor on this enemy.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private Transform cursorAnchorTransform;
		/// <summary>
		/// The anchor to use when spawning particles.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private Transform particleSpawnAnchorTransform;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// The game object that shows whether this enemy is downed or not.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject downTagGameObject;
		/// <summary>
		/// The zoom in camera that should be used for this sprite.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private CinemachineVirtualCamera zoomInCamera;
		/// <summary>
		/// The target to focus on for cinemachine.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private GameObject cinemachineTarget;
		/// <summary>
		/// The animator to use when animating this world enemy.
		/// </summary>
		[SerializeField, TabGroup("World Enemy DX", "Scene References")]
		private Animator enemyAnimator;
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
		/// <summary>
		/// The animator to use when animating this world enemy.
		/// </summary>
		protected Animator EnemyAnimator {
			get {
				return this.enemyAnimator;
			}
		}
		#endregion
		
		#region PROPERTIES - POSITIONS
		/// <summary>
		/// The position the cursor should be.
		/// </summary>
		public override Vector3 CursorPosition {
			get {
				throw new NotImplementedException();
				/*// Return the calculation I was doing in the old WorldEnemySprite.
			return BattleCameraController.Instance.MainCamera.WorldToScreenPoint(
				position: worldEnemy.transform.position
				          + new Vector3(x: 0f, y: (worldEnemy.worldEnemySpriteRenderer.size.y / 2f) * worldEnemy.transform.lossyScale.y));*/
			}
		}
		/// <summary>
		/// The anchor for the particle spawn.
		/// </summary>
		public override Vector3 ParticleSpawnPosition {
			get {
				return this.particleSpawnAnchorTransform.position;
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
			
			// Reset the state appropriately. This probably is redundant but whatever.
			this.downTagGameObject.SetActive(false);
			this.AllVisuals.SetActive(true);
			
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			AudioController.instance?.PlaySFX(SFXType.PlayerAttack);
			this.EnemyAnimator.SetTrigger("Attack");
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public override void AnimateDownedStatus(Combatant combatant, bool isDown) {
			// Set the down tag active.
			this.downTagGameObject.SetActive(isDown);
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
	
			throw new NotImplementedException();
			

			/*// Kill any tweens running on the square's transform and its dropshadow.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOKill(complete: true);
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOKill(complete: true);

			// Change the color to blue. This is for enemies. Blue.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.DOColor(endValue: GrawlyColors.colorDict[GrawlyColorTypes.Blue], duration: 0f);

			// Have the highlight square tween in.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOScale(
				endValue: 2f, 
				duration: 0.5f);

			// Make it rotate indefinitely.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: 360f),
				duration: 3f, 
				mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: Ease.Linear)
				.SetLoops(loops: -1);

			// Also scale the dropshadow sprite in.
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOScaleX(
				endValue: 1f,
				duration: 0.5f);*/

			// If there is a time attached to this call, wait that amount and then dehighlight.
			if (time.HasValue == true) {
				GameController.Instance.WaitThenRun(timeToWait: time.Value, action: delegate {
					this.AnimateFocusDehighlight(combatant: combatant, instantaneous: false);
				});
			}
			
		}
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public override void AnimateFocusDehighlight(Combatant combatant, bool instantaneous) {

			
			throw new NotImplementedException();

			/*// Kill any tweens running on the highlight square/dropshadow transform.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOKill(complete: true);
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOKill(complete: true);

			// Have the highlight square tween out.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOScale(
				endValue: 0f,
				duration: 0.5f);

			// Also tween the dropshadow out.
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOScaleX(
				endValue: 0f,
				duration: 0.5f);*/

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATIONS
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {
	
			throw new NotImplementedException();

			// Complete any animations that were playing. This is important for multi hit animations that may shake rapidly.
			this.EnemyVisuals.transform.DOKill(complete: true);

			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.instance.BuildEnemyCursorFromDamageCalculation(worldEnemyDX: this, damageCalculation: damageCalculation);

			// Shake the sprite if the hit landed.
			if (damageCalculation.TargetTookHPDamage == true) {
				this.EnemyVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);
			} 

			// If the target was hit with an affliction, animate that too.
			if (damageCalculation.TargetWasAfflicted == true) {
				this.AnimateAfflictionSet(combatant: damageCalculation.FinalTarget, afflictionType: damageCalculation.afflictionType);
			}


			// If the target will die, play the death FX.
			if (damageCalculation.TargetWillDie == true) {
				this.PlayDeathBattleFX(worldEnemy: this, damageCalculation: damageCalculation);
			} else {
				this.PlayBattleFX(worldEnemy: this, damageCalculation: damageCalculation);
				// Since the target isn't going to die here either, set the downed status graphics.
				this.AnimateDownedStatus(combatant: damageCalculation.FinalTarget, isDown: damageCalculation.TargetWillBeDowned);
			}
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateRestorationCalculation(DamageCalculation damageCalculation) {
				
			throw new NotImplementedException();
			// Play the battle fx for now. i guess.
			this.PlayBattleFX(worldEnemy: this, damageCalculation: damageCalculation);

		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public override void AnimateReflection(DamageCalculation damageCalculation) {
			
				
			throw new NotImplementedException();
			AudioController.instance?.PlaySFX(SFXType.PlayerExploit);
		
			
			
			/*// Grab the shine effect from the sprite.
			_2dxFX_Shiny_Reflect shinyReflectEffect = worldEnemy.worldEnemyFlashSpriteRenderer.GetComponent<_2dxFX_Shiny_Reflect>();
			// Reset the values on the effect.
			shinyReflectEffect.UseShinyCurve = false;
			shinyReflectEffect.Light = -0.5f;
			// Tell DOTween to tween the shine.
			DOTween.To(
				getter: () => shinyReflectEffect.Light,
				setter: x => shinyReflectEffect.Light = x,
				endValue: 1.5f,
				duration: 1f);*/
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public override void AnimateStatusBoost(DamageCalculation damageCalculation) {
				
			throw new NotImplementedException();
			// Assert that the target was buffed/debuffed.
			Debug.Assert(damageCalculation.TargetWasBuffedOrDebuffed);
			
			BFXType bfxType = damageCalculation.TargetWasBuffed ? BFXType.Buff : BFXType.Debuff;
			GameObject obj = GameObject.Instantiate<GameObject>(DataController.Instance.GetBFX(bfxType));
			obj.transform.position = this.ParticleSpawnPosition;
			
			
			// Determine what kind of sfx type to use.
			SFXType sfxType = damageCalculation.TargetWasBuffed ? SFXType.AssistMove1 : SFXType.DebuffMove1;
			
			// Play that.
			AudioController.instance?.PlaySFX(sfxType);
			
			// Also animate the pulse.
			this.AnimatePulseColor(damageCalculation: damageCalculation);
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
	
			throw new NotImplementedException();
			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.instance.BuildEnemyCursorFromSimpleDamage(
				worldEnemyDX: this, 
				damageAmount: damageAmount,
				resourceType: resourceType);

			// Shake the enemy sprite.
			this.EnemyVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);

			Debug.LogWarning("I'M NOT COMFORTABLE WITH HOW THIS IS WRITTEN MAKE IT SO I DONT NEED TO CALL THESE FUNCTIONS THIS WAY");

			// If the combatant will die
			if (combatant.WillDieFromDamage(damageAmount: damageAmount) == true) {
				this.PlayDeathBattleFX(worldEnemy: this, damageCalculation: null);
			} else {
				this.PlayBattleFX(
					worldEnemy: this, 
					scriptableBFX: DataController.Instance.GetScriptableBFX(DataController.Instance.GetBehavior(commonBehaviorType: CommonBattleBehaviorType.Attack)));
			}

		}
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public override void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {
				
			throw new NotImplementedException();
			throw new System.NotImplementedException("Need to add this!");
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
	
			throw new NotImplementedException();
			/*// Complete any tweens currently working on the flash image.
			worldEnemy.worldEnemyPulseSpriteRenderer.DOKill(complete: true);
			// Immediately change the color.
			worldEnemy.worldEnemyPulseSpriteRenderer.color = color;
			// Tween the color back to clear.
			worldEnemy.worldEnemyPulseSpriteRenderer.DOColor(endValue: Color.clear, duration: time);*/
			
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public override void AnimatePulseColor(DamageCalculation damageCalculation) {

			// This may change later, but as of now I'm mostly using this as a means to animate power boosts.

			switch (damageCalculation.powerBoosts.First().Value) {
				case PowerBoostIntentionType.Buff:
					this.AnimatePulseColor(combatant: damageCalculation.FinalTarget, color: Color.white, time: 0.5f);
					break;
				case PowerBoostIntentionType.Debuff:
					this.AnimatePulseColor(combatant: damageCalculation.FinalTarget, color: Color.blue, time: 0.5f);
					break;
				default:
					throw new System.Exception("Couldn't figure out how to pulse the combatant!");
					break;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : AFFLICTION ANIMATION
		/// <summary>
		/// Animates an affliction being set on the combatant.
		/// </summary>
		/// <param name="combatant">The combatant who is having an affliction set on them.</param>
		/// <param name="afflictionType">The type of affliction on the combatant.</param>
		public override void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType) {
				
			throw new NotImplementedException();
			// If the affliction is literally any other kind than None, shake. Only horizontally though. Also don't do it if they already have the affliction.
			if (afflictionType != AfflictionType.None && afflictionType != combatant.Affliction.Type) {
				this.EnemyVisuals.transform.DOKill(complete: true);
				this.EnemyVisuals.transform.DOShakePosition(duration: 1f, strength: new Vector3(x: 0.5f, y: 0f, z: 0f), vibrato: 50);
			}

			// Set the material on the world sprite to the one assocaited with the given affliction.
		// 	worldEnemy.worldEnemySpriteRenderer.material = DataController.GetAfflictionMaterial(afflictionType: afflictionType);

		}
		#endregion

		#region GENERAL ANIMATIONS
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		protected virtual void PlayBattleFX(WorldEnemyDX3D worldEnemy, DamageCalculation damageCalculation) {
			
			Data.ScriptableBFX sbfx = DataController.Instance.GetScriptableBFX(battleBehavior: damageCalculation.behavior);
			GameObject obj = GameObject.Instantiate<GameObject>(sbfx.bfxPrefab);
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			AudioController.instance.PlaySFX(sbfx.bfxAudioClip);
			
		}
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		protected virtual void PlayBattleFX(WorldEnemyDX3D worldEnemy,  Data.ScriptableBFX scriptableBFX) {
			// Data.ScriptableBFX sbfx = DataController.Instance.GetScriptableBFX(battleBehavior: DataController.Instance.GetBehavior(afflictionType: AfflictionType.None));
			GameObject obj = GameObject.Instantiate<GameObject>(scriptableBFX.bfxPrefab);
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			AudioController.instance.PlaySFX(scriptableBFX.bfxAudioClip);
		}
		/// <summary>
		/// Spawns a Death prefab when its seen that this will kill a target.
		/// </summary>
		protected virtual void PlayDeathBattleFX(WorldEnemyDX3D worldEnemy, DamageCalculation damageCalculation) {
			GameObject obj = GameObject.Instantiate(DataController.Instance.GetBFX(BFXType.EnemyDeath));
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			worldEnemy.EnemyVisuals.SetActive(false);
			AudioController.instance.PlaySFX(SFXType.DefaultAttack);
		}
		#endregion
		
		
	}
	
}