using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// The standard way in which WorldEnemyDXSprite's should be animated. It is assumed this behavior will be attached to a WorldEnemyDXSprite; things will break otherwise.
	/// </summary>
	[System.Serializable]
	public class WorldEnemyDXSpriteBehavior : WorldEnemyDXBehavior<WorldEnemyDXSprite> {

		#region GETTERS
		/// <summary>
		/// Gets the position of where a EnemyCursorDX should be placed on any given frame.
		/// </summary>
		/// <param name="worldEnemy">The WorldEnemyDX this behavior is attached to.</param>
		/// <returns></returns>
		public Vector3 GetCursorPosition(WorldEnemyDXSprite worldEnemy) {
			
			// Return the calculation I was doing in the old WorldEnemySprite.
			return BattleCameraController.Instance.MainCamera.WorldToScreenPoint(
				position: worldEnemy.transform.position
				          + new Vector3(x: 0f, y: (worldEnemy.worldEnemySpriteRenderer.size.y / 2f) * worldEnemy.transform.lossyScale.y));
		}
		/// <summary>
		/// Gets the position of where particle effects should be spawned around this WorldEnemy (e.x., when getting attacked, might need some dust.)
		/// </summary>
		/// <param name="worldEnemy">The WorldEnemyDX this behavior is attached to.</param>
		/// <returns></returns>
		public Vector3 GetParticleAnchor(WorldEnemyDXSprite worldEnemy) {
			
			return worldEnemy.AllVisuals.transform.position
			       + new Vector3(x: 0f, y: +(worldEnemy.worldEnemySpriteRenderer.size.y / 1.5f), z: 0f);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet, WorldEnemyDXSprite worldEnemy) {
			AudioController.instance?.PlaySFX(SFXType.PlayerAttack);
		
			_2dxFX_Shiny_Reflect shinyReflectEffect = worldEnemy.worldEnemyFlashSpriteRenderer.GetComponent<_2dxFX_Shiny_Reflect>();
			// Reset the values on the effect.
			shinyReflectEffect.UseShinyCurve = false;
			shinyReflectEffect.Light = -0.5f;
			// Tell DOTween to tween the shine.
			DOTween.To(
				getter: () => shinyReflectEffect.Light,
				setter: x => shinyReflectEffect.Light = x,
				endValue: 1.5f,
				duration: 1f);
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateDownedStatus(Combatant combatant, bool isDown, WorldEnemyDXSprite worldEnemy) {
		
			// Set the color on the sprite renderer.
			worldEnemy.worldEnemySpriteRenderer.color = (isDown == true) ? Color.gray : Color.white;
			// Also set the down tag.
			worldEnemy.downTagGameObject.SetActive(isDown);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateFocusHighlight(Combatant combatant, float? time, WorldEnemyDXSprite worldEnemy) {

			

			// Kill any tweens running on the square's transform and its dropshadow.
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
				duration: 0.5f);

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
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateFocusDehighlight(Combatant combatant, bool instantaneous, WorldEnemyDXSprite worldEnemy) {

		

			// Kill any tweens running on the highlight square/dropshadow transform.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOKill(complete: true);
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOKill(complete: true);

			// Have the highlight square tween out.
			worldEnemy.worldEnemyHighlightSquareSpriteRenderer.transform.DOScale(
				endValue: 0f,
				duration: 0.5f);

			// Also tween the dropshadow out.
			worldEnemy.worldEnemyDropshadowSpriteRenderer.transform.DOScaleX(
				endValue: 0f,
				duration: 0.5f);

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATIONS
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateHarmfulCalculation(DamageCalculation damageCalculation, WorldEnemyDXSprite worldEnemy) {


			// Complete any animations that were playing. This is important for multi hit animations that may shake rapidly.
			worldEnemy.AllVisuals.transform.DOKill(complete: true);
			worldEnemy.EnemyVisuals.transform.DOKill(complete: true);

			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.instance.BuildEnemyCursorFromDamageCalculation(worldEnemyDX: worldEnemy, damageCalculation: damageCalculation);

			// Shake the sprite if the hit landed.
			if (damageCalculation.TargetTookHPDamage == true) {
				// worldEnemy.AllVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);
				worldEnemy.EnemyVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);
			} 

			// If the target was hit with an affliction, animate that too.
			if (damageCalculation.TargetWasAfflicted == true) {
				this.AnimateAfflictionSet(combatant: damageCalculation.FinalTarget, afflictionType: damageCalculation.afflictionType);
			}


			// If the target will die, play the death FX.
			if (damageCalculation.TargetWillDie == true) {
				this.PlayDeathBattleFX(worldEnemy: worldEnemy, damageCalculation: damageCalculation);
			} else {
				this.PlayBattleFX(worldEnemy: worldEnemy, damageCalculation: damageCalculation);
				// Since the target isn't going to die here either, set the downed status graphics.
				this.AnimateDownedStatus(combatant: damageCalculation.FinalTarget, isDown: damageCalculation.TargetWillBeDowned);
			}
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateRestorationCalculation(DamageCalculation damageCalculation, WorldEnemyDXSprite worldEnemy) {
			
			
			// Play the battle fx for now. i guess.
			this.PlayBattleFX(worldEnemy: worldEnemy, damageCalculation: damageCalculation);

		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateReflection(DamageCalculation damageCalculation, WorldEnemyDXSprite worldEnemy) {
			AudioController.instance?.PlaySFX(SFXType.PlayerExploit);
		
			
			
			// Grab the shine effect from the sprite.
			_2dxFX_Shiny_Reflect shinyReflectEffect = worldEnemy.worldEnemyFlashSpriteRenderer.GetComponent<_2dxFX_Shiny_Reflect>();
			// Reset the values on the effect.
			shinyReflectEffect.UseShinyCurve = false;
			shinyReflectEffect.Light = -0.5f;
			// Tell DOTween to tween the shine.
			DOTween.To(
				getter: () => shinyReflectEffect.Light,
				setter: x => shinyReflectEffect.Light = x,
				endValue: 1.5f,
				duration: 1f);
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimateStatusBoost(DamageCalculation damageCalculation, WorldEnemyDXSprite worldEnemy) {
			
			// Assert that the target was buffed/debuffed.
			Debug.Assert(damageCalculation.TargetWasBuffedOrDebuffed);
			
			BFXType bfxType = damageCalculation.TargetWasBuffed ? BFXType.Buff : BFXType.Debuff;
			GameObject obj = GameObject.Instantiate<GameObject>(DataController.Instance.GetBFX(bfxType));
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			
			
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
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType, WorldEnemyDXSprite worldEnemy) {

			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.instance.BuildEnemyCursorFromSimpleDamage(
				worldEnemyDX: worldEnemy, 
				damageAmount: damageAmount,
				resourceType: resourceType);

			// Shake the enemy sprite.
			worldEnemy.AllVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);

			Debug.LogWarning("I'M NOT COMFORTABLE WITH HOW THIS IS WRITTEN MAKE IT SO I DONT NEED TO CALL THESE FUNCTIONS THIS WAY");

			// If the combatant will die
			if (combatant.WillDieFromDamage(damageAmount: damageAmount) == true) {
				this.PlayDeathBattleFX(worldEnemy: worldEnemy, damageCalculation: null);
			} else {
				this.PlayBattleFX(
					worldEnemy: worldEnemy, 
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
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType, WorldEnemyDXSprite worldEnemy) {
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
		protected override void AnimatePulseColor(Combatant combatant, Color color, float time, WorldEnemyDXSprite worldEnemy) {

			// Complete any tweens currently working on the flash image.
			worldEnemy.worldEnemyPulseSpriteRenderer.DOKill(complete: true);
			// Immediately change the color.
			worldEnemy.worldEnemyPulseSpriteRenderer.color = color;
			// Tween the color back to clear.
			worldEnemy.worldEnemyPulseSpriteRenderer.DOColor(endValue: Color.clear, duration: time);
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		/// <param name="worldEnemy">The world enemy attached to this behavior.</param>
		protected override void AnimatePulseColor(DamageCalculation damageCalculation, WorldEnemyDXSprite worldEnemy) {

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
		protected override void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType, WorldEnemyDXSprite worldEnemy) {
			
			// If the affliction is literally any other kind than None, shake. Only horizontally though. Also don't do it if they already have the affliction.
			if (afflictionType != AfflictionType.None && afflictionType != combatant.Affliction.Type) {
				worldEnemy.AllVisuals.transform.DOKill(complete: true);
				worldEnemy.AllVisuals.transform.DOShakePosition(duration: 1f, strength: new Vector3(x: 0.5f, y: 0f, z: 0f), vibrato: 50);
			}

			// Set the material on the world sprite to the one assocaited with the given affliction.
			worldEnemy.worldEnemySpriteRenderer.material = DataController.GetAfflictionMaterial(afflictionType: afflictionType);

		}
		#endregion

		#region GENERAL ANIMATIONS
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		private void PlayBattleFX(WorldEnemyDXSprite worldEnemy, DamageCalculation damageCalculation) {
			
			Data.ScriptableBFX sbfx = DataController.Instance.GetScriptableBFX(battleBehavior: damageCalculation.behavior);
			GameObject obj = GameObject.Instantiate<GameObject>(sbfx.bfxPrefab);
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			AudioController.instance.PlaySFX(sbfx.bfxAudioClip);
			
		}
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		private void PlayBattleFX(WorldEnemyDXSprite worldEnemy,  Data.ScriptableBFX scriptableBFX) {
			// Data.ScriptableBFX sbfx = DataController.Instance.GetScriptableBFX(battleBehavior: DataController.Instance.GetBehavior(afflictionType: AfflictionType.None));
			GameObject obj = GameObject.Instantiate<GameObject>(scriptableBFX.bfxPrefab);
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			AudioController.instance.PlaySFX(scriptableBFX.bfxAudioClip);
		}
		/// <summary>
		/// Spawns a Death prefab when its seen that this will kill a target.
		/// </summary>
		private void PlayDeathBattleFX(WorldEnemyDXSprite worldEnemy, DamageCalculation damageCalculation) {
			GameObject obj = GameObject.Instantiate(DataController.Instance.GetBFX(BFXType.EnemyDeath));
			obj.transform.position = worldEnemy.ParticleSpawnPosition;
			worldEnemy.EnemyVisuals.SetActive(false);
			AudioController.instance.PlaySFX(SFXType.DefaultAttack);
		}
		#endregion

	}


}