/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using System.Linq;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// The behavior that should be attached to enemies of the WorldEnemy3D variant.
	/// </summary>
	[System.Serializable]
	public class WorldEnemy3DBehavior : WorldEnemyDXBehavior {
		
		#region VECTOR3 POSITION GETTERS
		/// <summary>
		/// Gets the position of where a EnemyCursorDX should be placed on any given frame.
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX this behavior is attached to.</param>
		/// <returns></returns>
		public override Vector3 GetCursorPosition(WorldEnemyDX worldEnemyDX) {

			// Get the 3D version of the thing passed in.
			var worldEnemyDX3D = this.GetWorldEnemyDX3D(combatant: worldEnemyDX.Enemy);
			// Return the position of the anchor.
			return BattleCameraController.Instance.MainCamera.WorldToScreenPoint(position: worldEnemyDX3D.cursorAnchorTransform.position);
		
		}
		/// <summary>
		/// Gets the position of where particle effects should be spawned around this WorldEnemy (e.x., when getting attacked, might need some dust.)
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX this behavior is attached to.</param>
		/// <returns></returns>
		public override Vector3 GetDefaultParticleEffectSpawnPosition(WorldEnemyDX worldEnemyDX) {
			
			// Cast the world enemy.
			WorldEnemyDX3D worldEnemyDX3D = (worldEnemyDX as WorldEnemyDX3D);
			return worldEnemyDX3D.particleSpawnAnchorTransform.position;
			
		}
		#endregion

		#region CINEMACHINE VIRTUAL CAMERA GETTERS
		/// <summary>
		/// Returns the CinemachineVirtualCamera on this enemy used for zoom in effects.
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX that the camera is being gotten for.</param>
		/// <returns>The CinemachineVirtualCamera that acts as a zoom in.</returns>
		public override CinemachineVirtualCamera GetZoomInCamera(WorldEnemyDX worldEnemyDX) {
			// Return the zoom in camera.
			return worldEnemyDX.ZoomInCamera;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public override void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			
			AudioController.Instance?.PlaySFX(SFXType.PlayerAttack);
			
			// Grab the WorldEnemyDXSprite from the calculation set. It can be assumed that if this function is being called, this is the source.
			WorldEnemyDX3D worldEnemyDX3D = (WorldEnemyDX3D)((Enemy)damageCalculationSet.PrimarySource).WorldEnemyDX;
			Debug.LogError("Implement animation for 3D enemy behavior usage.");
			
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public override void AnimateDownedStatus(Combatant combatant, bool isDown) {
			
			// Grab the WorldEnemyDX sprite.
			WorldEnemyDX3D worldEnemyDX3D = ((combatant as Enemy).WorldEnemyDX as WorldEnemyDX3D);
			
			// Also set the down tag.
			worldEnemyDX3D.downTagGameObject.SetActive(isDown);
			Debug.LogError("Implement downed status animation.");
			
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public override void AnimateFocusHighlight(Combatant combatant, float? time = null) {


			// Grab the WorldEnemyDX from the combatant (which is, assumadly, an enemy.)
			WorldEnemyDX3D worldEnemyDX3D = this.GetWorldEnemyDX3D(combatant);
			Debug.LogError("Add the focus highlight animation.");
			

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
		public override void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false) {

			// Grab the WorldEnemyDX from the combatant (which is, assumadly, an enemy.)
			WorldEnemyDX3D worldEnemyDX3D = this.GetWorldEnemyDX3D(combatant);
			Debug.LogError("Add the focus dehighlight animation.");
			

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATION ANIMATIONS
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {
			
			// Grab the WorldEnemyDX from the enemy.
			WorldEnemyDX3D worldEnemyDX = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX as WorldEnemyDX3D;

			// Complete any animations that were playing. This is important for multi hit animations that may shake rapidly.
			worldEnemyDX.EnemyVisuals.transform.DOKill(complete: true);

			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.Instance.BuildEnemyCursorFromDamageCalculation(worldEnemyDX: worldEnemyDX, damageCalculation: damageCalculation);

			// Shake the sprite if the hit landed.
			if (damageCalculation.TargetTookHPDamage == true) {
				worldEnemyDX.EnemyVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);
			} 

			// If the target was hit with an affliction, animate that too.
			if (damageCalculation.TargetWasAfflicted == true) {
				this.AnimateAfflictionSet(combatant: damageCalculation.FinalTarget, afflictionType: damageCalculation.afflictionType);
			}


			// If the target will die, play the death FX.
			if (damageCalculation.TargetWillDie == true) {
				this.PlayDeathBattleFX(worldEnemyDX: (damageCalculation.FinalTarget as Enemy).WorldEnemyDX, damageCalculation: damageCalculation);
			} else {
				this.PlayBattleFX(worldEnemyDX: (damageCalculation.FinalTarget as Enemy).WorldEnemyDX, damageCalculation: damageCalculation);
				// Since the target isn't going to die here either, set the downed status graphics.
				this.AnimateDownedStatus(combatant: damageCalculation.FinalTarget, isDown: damageCalculation.TargetWillBeDowned);
			}
		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public override void AnimateRestorationCalculation(DamageCalculation damageCalculation) {
				
			// Grab the WorldEnemyDX from the enemy.
			WorldEnemyDX worldEnemyDX = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			// Play the battle fx for now. i guess.
			this.PlayBattleFX(worldEnemyDX: (damageCalculation.FinalTarget as Enemy).WorldEnemyDX, damageCalculation: damageCalculation);

		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public override void AnimateReflection(DamageCalculation damageCalculation) {
	
			
			AudioController.Instance?.PlaySFX(SFXType.PlayerExploit);
			// Grab the WorldEnemyDXSprite from the calculation set. It can be assumed that if this function is being called, this is the source.
			WorldEnemyDX3D worldEnemyDXSprite = (damageCalculation.OriginalTarget as Enemy).WorldEnemyDX as WorldEnemyDX3D;
			Debug.LogError("Add the reflection animation.");
			
			
		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public override void AnimateStatusBoost(DamageCalculation damageCalculation) {
			
			// throw new System.NotImplementedException();
			
			// Assert that the target was buffed/debuffed.
			Debug.Assert(damageCalculation.TargetWasBuffedOrDebuffed);
			
			// Grab the WorldEnemyDX from the enemy and instansiate the effect correctly.
			WorldEnemyDX worldEnemyDX = (damageCalculation.FinalTarget as Enemy).WorldEnemyDX;
			BFXType bfxType = damageCalculation.TargetWasBuffed ? BFXType.Buff : BFXType.Debuff;
			GameObject obj = GameObject.Instantiate<GameObject>(DataController.Instance.GetBFX(bfxType));
			obj.transform.position = worldEnemyDX.ParticleAnchor();
			
			
			// Determine what kind of sfx type to use.
			SFXType sfxType = damageCalculation.TargetWasBuffed ? SFXType.AssistMove1 : SFXType.DebuffMove1;
			
			// Play that.
			AudioController.Instance?.PlaySFX(sfxType);
			
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
			
			// Grab the WorldEnemyDX from the enemy.
			WorldEnemyDX3D worldEnemyDX3D = this.GetWorldEnemyDX3D(combatant: combatant);

			// Create a cursor from it. The cursor has its own animations to take care of; I don't have to worry about them in this class.
			EnemyCursorDXController.Instance.BuildEnemyCursorFromSimpleDamage(
				worldEnemyDX: worldEnemyDX3D, 
				damageAmount: damageAmount,
				resourceType: resourceType);

			// Shake the enemy sprite.
			worldEnemyDX3D.EnemyVisuals.transform.DOShakePosition(duration: 0.5f, vibrato: 50);

			Debug.LogWarning("I'M NOT COMFORTABLE WITH HOW THIS IS WRITTEN MAKE IT SO I DONT NEED TO CALL THESE FUNCTIONS THIS WAY");

			// If the combatant will die
			if (combatant.WillDieFromDamage(damageAmount: damageAmount) == true) {
				this.PlayDeathBattleFX(worldEnemyDX: worldEnemyDX3D, damageCalculation: null);
			} else {
				this.PlayBattleFX(
					worldEnemyDX: worldEnemyDX3D, 
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

			// Grab the WorldEnemyDXSprite from the calculation set. It can be assumed that if this function is being called, this is the source.
			WorldEnemyDX3D worldEnemyDX3D = (combatant as Enemy).WorldEnemyDX as WorldEnemyDX3D;
			Debug.LogError("Add the pulse color function.");
			
		
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
			
			WorldEnemyDX3D worldEnemyDX3D = this.GetWorldEnemyDX3D(combatant: combatant);
			Debug.LogError("Add the affliction set animation.");


		}
		#endregion

		#region GENERAL ANIMATIONS
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		private void PlayBattleFX(WorldEnemyDX worldEnemyDX, DamageCalculation damageCalculation) {
			
			Data.ScriptableBFX sbfx = DataController.Instance.GetScriptableBFX(battleBehavior: damageCalculation.behavior);
			GameObject obj = GameObject.Instantiate<GameObject>(sbfx.bfxPrefab);
			obj.transform.position = worldEnemyDX.ParticleAnchor();
			AudioController.Instance.PlaySFX(sbfx.bfxAudioClip);
			
		}
		/// <summary>
		/// Instansiates a BattleFX for an attack.
		/// </summary>
		private void PlayBattleFX(WorldEnemyDX worldEnemyDX, Data.ScriptableBFX scriptableBFX) {
			GameObject obj = GameObject.Instantiate<GameObject>(scriptableBFX.bfxPrefab);
			obj.transform.position = worldEnemyDX.ParticleAnchor();
			AudioController.Instance.PlaySFX(scriptableBFX.bfxAudioClip);
		}
		/// <summary>
		/// Spawns a Death prefab when its seen that this will kill a target.
		/// </summary>
		private void PlayDeathBattleFX(WorldEnemyDX worldEnemyDX, DamageCalculation damageCalculation) {
			GameObject obj = GameObject.Instantiate(DataController.Instance.GetBFX(BFXType.EnemyDeath));
			obj.transform.position = worldEnemyDX.ParticleAnchor();
			(worldEnemyDX as WorldEnemyDX3D).EnemyVisuals.SetActive(false);
			AudioController.Instance.PlaySFX(SFXType.DefaultAttack);
		}
		#endregion
		
		#region PRIVATE HELPER FUNCTIONS
		/// <summary>
		/// A private function that basically converts a passed in combatant to a world enemy dx. I do a lot of casting so i might as well have it happen here.
		/// This will obviously fail if the combatant is not an enemy but I don't think it's possible for this to be called in any other context in the first place so it's safe.
		/// </summary>
		/// <param name="combatant">The combatant who is an Enemy and I need their WorldEnemyDX3D.</param>
		/// <returns>The WorldEnemyDX3D on the enemy.</returns>
		private WorldEnemyDX3D GetWorldEnemyDX3D(Combatant combatant) {
			return (combatant as Enemy).WorldEnemyDX as WorldEnemyDX3D;
		}
		#endregion
		
		#region INSPECTOR STUFF
		private static string inspectorDescription = "The standard way enemies should be animated in 3D.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion
		
	}
	
}*/