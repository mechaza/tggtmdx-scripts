using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// The new representation of a PlayerStatus in scene.
	/// </summary>
	public class PlayerStatusDX : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler, IDeselectHandler, ICombatantAnimator {

		#region FIELDS - STATE : PLAYER
		/// <summary>
		/// The player who has been assigned to this PlayerStatusDX.
		/// </summary>
		private Player player;
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time it should take for the player icon to tween.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Toggles")]
		private float iconTweenTime = 0.2f;
		/// <summary>
		/// The ease type to use for tweening the icon.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Toggles")]
		private Ease iconTweenEaseType;
		#endregion

		#region FIELDS - SCENE REFERENCES : VISUALS
		/// <summary>
		/// The selectable that is attached to this PlayerStatusDX.
		/// Need a reference so I can tweak it on/off.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Selectable selectable;
		/// <summary>
		/// The GameObject that has the player icon. Mostly so I can just tween it.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private GameObject playerIconGameObject;
		/// <summary>
		/// The image used for the player icon.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image playerIconImageFront;
		/// <summary>
		/// The image that should be used when doing shine effects on the icon.
		/// Good for buffs/debuffs. 
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image playerIconFlashImage;
		/// <summary>
		/// The dropshadow used for the player icon.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image playerIconImageDropshadow;
		/// <summary>
		/// The square image for when the player is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image playerHighlightSquareImage;
		/// <summary>
		/// The GameObject that has the value bars. Mostly for tweening.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private GameObject valueBarsGameObject;
		/// <summary>
		/// The image used for the health bar.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image healthBarImage;
		/// <summary>
		/// The image used for the magic bar.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private Image magicBarImage;
		/// <summary>
		/// The GameObject that contains the value labels for health/magic. Tweening.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private GameObject valueLabelsGameObject;
		/// <summary>
		/// The text mesh used for the health value.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private SuperTextMesh healthValueLabel;
		/// <summary>
		/// The text mesh used for the magic value.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private SuperTextMesh magicValueLabel;
		/// <summary>
		/// The label for showing how much damage this player has taken.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private SuperTextMesh damageValueLabel;
		/// <summary>
		/// The GameObject for the backing visual. Again just for. Tweening.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Misc References")]
		private GameObject backingGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : RESISTANCE TAGS
		/// <summary>
		/// The image for showing the Weak resistance type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image weakTagImage;
		/// <summary>
		/// The image for showing the Block resistance type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image blockTagImage;
		/// <summary>
		/// The image for showing the Critical accuracy type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image critTagImage;
		/// <summary>
		/// The image for showing the Miss accuracy type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image missTagImage;
		/// <summary>
		/// The image for showing the Reflect resistance type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image reflectTagImage;
		/// <summary>
		/// The image for showing the Resist resistance type.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private Image resistTagImage;
		/// <summary>
		/// The game object that shows whether this player is downed or not.
		/// </summary>
		[SerializeField, TabGroup("Player Status DX", "Tag References")]
		private GameObject downTagGameObject;
		/// <summary>
		/// A list of the tags listed above. Mostly so I can quickly turn them off when needed.
		/// </summary>
		private List<Image> resistanceTagImages;
		#endregion


		#region FIELDS - SCENE REFERENCES : BOOST TAGS
		/// <summary>
		/// Contains all the boost gameobjects.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject allBoostsGameObject;
		/// <summary>
		/// The GameObject that has all the visuals for the accuracy boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject accuracyBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the accuracy boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image accuracyBoostArrowImage;
		/// <summary>
		/// The GameObject that has all the visuals for the defense boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject defenseBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the defeense boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image defenseBoostArrowImage;
		/// <summary>
		/// The GameObject that has all the visuals for the attack boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject attackBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the attack boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image attackBoostArrowImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.resistanceTagImages = new List<Image>() { this.weakTagImage, this.blockTagImage, this.critTagImage, this.missTagImage, this.reflectTagImage, this.resistTagImage };
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this PlayerStatusDX for use by the specified player.
		/// </summary>
		/// <param name="player">The player who should be associated with this status.</param>
		public void AssignPlayer(Player player) {
			this.player = player;
			// Also tell the player to assign its status to this too.
			this.player.AssignPlayerStatus(playerStatusDX: this);
			// Rebuild the status quickly.
			this.QuickRebuild();
		}
		/// <summary>
		/// Sets whether the active visuals on this combatant are a thing.
		/// </summary>
		/// <param name="active"></param>
		public void SetActiveVisuals(bool active) {

			// Kill any tweens on the square image.
			this.playerHighlightSquareImage.DOKill(complete: true);
			this.playerHighlightSquareImage.GetComponent<RectTransform>().DOKill(complete: true);
			
			this.playerHighlightSquareImage.transform.DOScale(
				endValue: (active) ? 1f : 0f, 
				duration: 0.2f)
				.SetEase(Ease.InOutBounce);
			
			this.playerHighlightSquareImage.transform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: 360f), 
				duration: 10f, mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: Ease.Linear)
				.SetLoops(loops: -1);
			
			// Do a quick rebuild.
			this.QuickRebuild();
			
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Rebuilds the status with changes shown immediately.
		/// </summary>
		public void QuickRebuild() {
			
			// Set the sprite on the icon.
			this.playerIconImageFront.overrideSprite = this.player.playerTemplate.playerStatusIcon;
			this.playerIconFlashImage.overrideSprite = this.player.playerTemplate.playerStatusIcon;
			this.playerIconImageDropshadow.overrideSprite = this.player.playerTemplate.playerStatusIcon;
			
			// Set the fill amounts on the health/magic bar images.
			this.healthBarImage.fillAmount = this.player.HPRatio;
			this.magicBarImage.fillAmount = this.player.MPRatio;
			
			// Also set the labels.
			this.healthValueLabel.Text = this.player.HP.ToString();
			this.magicValueLabel.Text = this.player.MP.ToString();
			
			// Go inside the status boosts and tweak em.
			this.EnableBoostIcons(player: this.player);

			// Also crossfade the color based on whether the combatant is downed/dead or not.
			this.CrossFadeIconColor(
				color: (this.player.IsDown || this.player.IsDead) ? Color.gray : Color.white,
				duration: 0f);
		}
		/// <summary>
		/// Rebuilds the status with changes shown immediately, but also attempts to predict the final values from the calculations since they arent added to the combatant yet.
		/// </summary>
		/// <param name="damageCalculation"></param>
		private void QuickRebuild(DamageCalculation damageCalculation) {
			// Set the fill amounts on the health/magic bar images.
			this.healthBarImage.fillAmount = damageCalculation.NewFinalTargetHPRatio;
			this.magicBarImage.fillAmount = damageCalculation.NewFinalTargetMPRatio;
			// Also update the text meshes showing the HP/MP.
			this.healthValueLabel.Text = damageCalculation.NewFinalTargetHP.ToString();
			this.magicValueLabel.Text = damageCalculation.NewFinalTargetMP.ToString();
			// If this player is going to be downed, set the downed graphics.
			this.AnimateDownedStatus(combatant: this.player, isDown: damageCalculation.TargetWillBeDowned);
			// Go inside the status boosts and tweak em.
			this.EnableBoostIcons(player: this.player, damageCalculation: damageCalculation);

			// Also crossfade the color based on whether the combatant is downed/dead or not.
			this.CrossFadeIconColor(
				color: (this.player.IsDown || this.player.IsDead) ? Color.gray : Color.white,
				duration: 0f);
		}
		/// <summary>
		/// Rebuilds the status with a simple amount of damage to add/subtract.
		/// Helpful for things like burns and whatnot.
		/// </summary>
		/// <param name="damageTaken">The amount of damage to take away from the resource.</param>
		/// <param name="resourceType">The resource to subtract from (HP? MP?)</param>
		private void QuickRebuild(Player player, int damageTaken, BehaviorCostType resourceType) {
			switch (resourceType) {
				case BehaviorCostType.HP:
					int newHP = Mathf.Clamp(value: (player.HP - damageTaken), min: 0, max: player.MaxHP);
					this.healthBarImage.fillAmount =  (float)newHP / (float)player.MaxHP;
					this.healthValueLabel.Text = newHP.ToString();

					// If this is going to kill the player, fade their icon.
					if (player.WillDieFromDamage(damageAmount: damageTaken) == true) {
						this.CrossFadeIconColor(color: Color.gray, duration: 0f);
					}

					break;
				case BehaviorCostType.MP:
					int newMP = Mathf.Clamp(value: (player.MP - damageTaken), min: 0, max: player.MaxMP);
					this.magicBarImage.fillAmount = (float)newMP / (float)player.MaxMP;
					this.magicValueLabel.Text = newMP.ToString();
					break;
				default:
					Debug.LogError("Couldn't do rebuild on player status!");
					break;
			}
		}
		/// <summary>
		/// Sets whether the selectable on this PlayerStatus is enabled or not.
		/// </summary>
		/// <param name="status">Whether this status is selectable or not.</param>
		public void SetSelectable(bool status) {
			this.selectable.enabled = status;
		}
		#endregion

		#region HIGHLIGHTS
		
		/// <summary>
		/// Shows this PlayerStatusDX to be highlighted. Helpful when picking a move.
		/// </summary>
		public void Highlight() {
			/*this.playerIconImageFront.CrossFadeColor(
				targetColor: Color.red, duration: 0f, 
				ignoreTimeScale: true,
				useAlpha: true);*/
			
			this.CrossFadeIconColor(
				color: Color.red,
				duration: 0f);
		}
		/// <summary>
		/// Shows this PlayerStatusDX to be highlighted. Helpful when picking a move.
		/// </summary>
		public void Dehighlight() {
			// this.playerIconImageFront.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.CrossFadeIconColor(
				color: (this.player.IsDown || this.player.IsDead) ? Color.gray : Color.white,
				duration: 0f);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tweens the PlayerStatusDX to be big.
		/// </summary>
		internal void TweenBigAnimation() {

			// ICON:
			// Position: x: 0f, y: -.5f
			// Rotation: z: -10f
			// Scale: x: 0.68f, y: 0.68f
			this.playerIconGameObject.GetComponent<RectTransform>().DOAnchorPosY(endValue: -0.5f, duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.playerIconGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.68f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);


			// BACKING:
			// Position: x: -12f, y: -71f
			// Rotation: z: 0f
			// Scale: x: 0.48f, y: 0.48f
			this.backingGameObject.GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -12f, y: -71f), duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.backingGameObject.GetComponent<RectTransform>().DOLocalRotate(endValue: new Vector3(x: 0f, y: 0f, z: 0f), duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);
			this.backingGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.48f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);

			// VALUE BARS:
			// Position: x: -26f, y: -62f
			// Rotation: x: 0f
			// Scale: x: 0.52f, y: 0.52f
			this.valueBarsGameObject.GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -26f, y: -62f), duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.valueBarsGameObject.GetComponent<RectTransform>().DOLocalRotate(endValue: new Vector3(x: 0f, y: 0f, z: 0f), duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);
			this.valueBarsGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.52f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);

			this.valueLabelsGameObject.SetActive(true);
			this.allBoostsGameObject.SetActive(true);
			this.QuickRebuild();
		}
		/// <summary>
		/// Tweens the PlayerStatusDX to be big.
		/// This is what I used before I had the battle navigator.
		/// </summary>
		internal void OldTweenBigAnimation() {

			// ICON:
			// Position: x: 0f, y: 12f
			// Rotation: z: -10f
			// Scale: x: 0.85f, y: 0.85f
			this.playerIconGameObject.GetComponent<RectTransform>().DOAnchorPosY(endValue: 12f, duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.playerIconGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.85f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);


			// BACKING:
			// Position: x: -12f, y: -71f
			// Rotation: z: 0f
			// Scale: x: 0.6f, y: 0.6f
			this.backingGameObject.GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -12f, y: -71f), duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.backingGameObject.GetComponent<RectTransform>().DOLocalRotate(endValue: new Vector3(x: 0f, y: 0f, z: 0f), duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);
			this.backingGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.6f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);

			// VALUE BARS:
			// Position: x: -36f, y: -62f
			// Rotation: x: 0f
			// Scale: x: 0.65f, y: 0.65f
			this.valueBarsGameObject.GetComponent<RectTransform>().DOAnchorPos(endValue: new Vector2(x: -36f, y: -62f), duration: this.iconTweenTime, snapping: true).SetEase(ease: this.iconTweenEaseType);
			this.valueBarsGameObject.GetComponent<RectTransform>().DOLocalRotate(endValue: new Vector3(x: 0f, y: 0f, z: 0f), duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);
			this.valueBarsGameObject.GetComponent<RectTransform>().DOScale(endValue: 0.65f, duration: this.iconTweenTime).SetEase(ease: this.iconTweenEaseType);

			this.valueLabelsGameObject.SetActive(true);
			this.allBoostsGameObject.SetActive(true);
			this.QuickRebuild();
		}
		/// <summary>
		/// Tweens the PlayerStatusDX to be big.
		/// </summary>
		internal void TweenSmallAnimation() {
			// ICON:
			// Position: x: 0f, y: -17f
			// Rotation: z: -10f
			// Scale: x: 0.5f, y: 0.5f
			this.playerIconGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0f, y: -17f);
			this.playerIconGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);

			// BACKING:
			// Position: x: -12f, y: -71f
			// Rotation: z: 17f
			// Scale: x: 0.45f, y: 0.45f
			this.backingGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: -12f, y: -71f);
			this.backingGameObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 15f);
			this.backingGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.45f, y: 0.45f, z: 0.45f);

			// VALUE BARS:
			// Position: x: -13f, y: -62f
			// Rotation: x: 6f
			// Scale: x: 0.4f, y: 0.4f
			this.valueBarsGameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: -13f, y: -62f);
			this.valueBarsGameObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, 6f);
			this.valueBarsGameObject.GetComponent<RectTransform>().localScale = new Vector3(0.4f, y: 0.4f, z: 0.4f);

			this.valueLabelsGameObject.SetActive(false);
			this.allBoostsGameObject.SetActive(false);
			this.QuickRebuild();
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : EVENTS
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculation that should be animated.</param>
		public void AnimateBehaviorUse(DamageCalculationSet damageCalculationSet) {
			AudioController.instance?.PlaySFX(SFXType.PlayerAttack);
			// This is sort of cheating but tell the menu to shine the bust up.
			BattleMenuControllerDX.instance.PlayerBustUpShineAnimation();
			// Also call the quick rebuild that predicts what the player will have as their final stats.
			this.QuickRebuild();
		}
		/// <summary>
		/// Animates a damage calculation of a behavior about to be used with this combatant as the source,
		/// but shows it also being interrupted in the process.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet that was generated prior to animating.</param>
		public void AnimateBehaviorUseInterruption(DamageCalculationSet damageCalculationSet) {
			AudioController.instance?.PlaySFX(SFXType.PAttackInterrupted);
			// This is sort of cheating but tell the menu to shine the bust up.
			BattleMenuControllerDX.instance.PlayerBustUpShineAnimation(endLightValue: 0.7f, shineTime: 0.6f);
			// Also call the quick rebuild that predicts what the player will have as their final stats.
			this.QuickRebuild();
		}
		/// <summary>
		/// Animates the combatant to respond to being downed or not.
		/// </summary>
		/// <param name="combatant">The combatant that owns this animator.</param>
		/// <param name="isDown">Is the combatant "down"?</param>
		public void AnimateDownedStatus(Combatant combatant, bool isDown) {
			// Crossfade the icon with Gray or White depending if the combatant is downed.
			// this.playerIconImageFront.CrossFadeColor(targetColor: (isDown == true ? Color.gray : Color.white), duration: 0f, ignoreTimeScale: true, useAlpha: false);
			this.CrossFadeIconColor(color: (isDown == true ? Color.gray : Color.white), duration: 0f);
			// Turn the down tag on/off depending on that status as well.
			this.downTagGameObject.SetActive(value: isDown);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : CALCULATIONS
		/// <summary>
		/// Animates a damage calculation that is intended to harm this combatant as the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public void AnimateHarmfulCalculation(DamageCalculation damageCalculation) {

			// Do a quick rebuild but also try to predict what the final values will be based on the damage calculation passed in.
			this.QuickRebuild(damageCalculation: damageCalculation);

			// Shake the status. Complete animations before doing so.
			this.GetComponent<RectTransform>().DOKill(complete: true);

			if (damageCalculation.TargetWasHit == true) {
				this.GetComponent<RectTransform>().DOShakeAnchorPos(duration: 0.5f, strength: 50, vibrato: 50);
			}
			
			// Play an SFX.
			AudioController.instance?.PlaySFX(SFXType.DefaultAttack);

			// Call the method that enables a resistance tag if any of them should be enabled.
			this.EnableResistanceTag(damageCalculation: damageCalculation);

			// Set the text on the damage label.
			if (damageCalculation.TargetShouldShowDamageAmount == true) {
				this.damageValueLabel.Text = (damageCalculation.TargetTookHPDamage ? "<j=sample><c=red>" : "<c=greeny>") + damageCalculation.DamageMagnitude.ToString();
			}

			// If the target was hit with an affliction, animate that too.
			if (damageCalculation.TargetWasAfflicted == true) {
				this.AnimateAfflictionSet(combatant: damageCalculation.FinalTarget, afflictionType: damageCalculation.afflictionType);
			}

			// Turn the combatant's color red and then tween it back to white.
			this.playerIconImageFront.color = Color.red;
			this.playerIconImageFront.DOColor(endValue: Color.white, duration: 0.5f);

			// Wait a second and then turn off the resistance tag images and the text.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				this.resistanceTagImages.ForEach(i => i.gameObject.SetActive(false));
				this.damageValueLabel.Text = "";
			});

		}
		/// <summary>
		/// Animates a damage calculation that is intended to heal this combatant the target.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated.</param>
		public void AnimateRestorationCalculation(DamageCalculation damageCalculation) {

			// Do a quick rebuild but also try to predict what the final values will be based on the damage calculation passed in.
			this.QuickRebuild(damageCalculation: damageCalculation);

			// Play an SFX.
			AudioController.instance?.PlaySFX(SFXType.HealingMove1);

			// Turn the combatant's color red green then tween it back to white.
			this.AnimatePulseColor(combatant: damageCalculation.FinalTarget, color: Color.green, time: 0.5f);

			// Show the amount of restoration.
			this.damageValueLabel.Text = "<c=greeny>" + damageCalculation.DamageMagnitude.ToString();

			// Wait a second and then turn off the text.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				this.damageValueLabel.Text = "";
			});

		}
		/// <summary>
		/// Animates a buff or debuff being applied to the given combatant.
		/// </summary>
		/// <param name="damageCalculation">The calculation that contains the required information.</param>
		public void AnimateStatusBoost(DamageCalculation damageCalculation) {
			
			// Assert that the target was buffed/debuffed.
			Debug.Assert(damageCalculation.TargetWasBuffedOrDebuffed);
			
			// Determine what kind of sfx type to use.
			SFXType sfxType = damageCalculation.TargetWasBuffed ? SFXType.AssistMove1 : SFXType.DebuffMove1;
			
			// Play that.
			AudioController.instance?.PlaySFX(sfxType);
			
			// Also animate the pulse.
			this.AnimatePulseColor(damageCalculation: damageCalculation);
		}
		/// <summary>
		/// Animates the combatant reflecting an incoming attack.
		/// </summary>
		/// <param name="damageCalculation">The DamageCalculation that should be animated</param>
		public void AnimateReflection(DamageCalculation damageCalculation) {
			throw new System.NotImplementedException("Implement the ability to animate reflections.");
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
		public void SimpleAnimateHarmfulCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {

			// Do a quick rebuild on the player's status to update the values.
			this.QuickRebuild(player: combatant as Player, damageTaken: damageAmount, resourceType: resourceType);

			// Turn the combatant's color red and then tween it back to white.
			this.playerIconImageFront.color = Color.red;
			this.playerIconImageFront.DOColor(endValue: Color.white, duration: 0.5f);

			// Wait a second and then turn off the resistance tag images and the text.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				this.damageValueLabel.Text = "";
			});
		}
		/// <summary>
		/// Animates restoration when only being passed in a number.
		/// Good for things like afflictions that I don't have behaviors attached to.
		/// </summary>
		/// <param name="combatant">The combatant who is being animated.</param>
		/// <param name="damageAmount">The amount of restoration to give to the combatant.</param>
		/// <param name="resourceType">The resource that this animation is targeting.</param>
		public void SimpleAnimateRestorationCalculation(Combatant combatant, int damageAmount, BehaviorCostType resourceType) {

			// Do a quick rebuild on the player's status to update the values.
			this.QuickRebuild(player: combatant as Player, damageTaken: damageAmount, resourceType: resourceType);

			// Turn the combatant's color red and then tween it back to white.
			this.playerIconImageFront.color = Color.green;
			this.playerIconImageFront.DOColor(endValue: Color.white, duration: 0.5f);

			// Wait a second and then turn off the resistance tag images and the text.
			GameController.Instance.WaitThenRun(timeToWait: 1.5f, action: delegate {
				this.damageValueLabel.Text = "";
			});

		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ICOMBATANTANIMATOR : HIGHLIGHTING
		/// <summary>
		/// Animates this combatant to be noticable.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="time">The amount of time to spend highlighting this combatant. If null, it should be indefinite.</param>
		public void AnimateFocusHighlight(Combatant combatant, float? time = null) {
			Debug.LogError("This still needs to be done!");
			// throw new System.NotImplementedException("Still need to get this baby working.");
		}
		/// <summary>
		/// Animates this combatant to put away their highlight graphics.
		/// </summary>
		/// <param name="combatant">The combatant who owns this animator.</param>
		/// <param name="instantaneous">Should this tween take a normal amount of time, or should it be instantaneous?</param>
		public void AnimateFocusDehighlight(Combatant combatant, bool instantaneous = false) {
			// throw new System.NotImplementedException("Need to add this");
			Debug.LogError("This still needs to be done!");
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
		public void AnimatePulseColor(Combatant combatant, Color color, float time) {
			// Complete any tweens currently working on the flash image.
			this.playerIconFlashImage.DOKill(complete: true);
			// Immediately change the color.
			this.playerIconFlashImage.color = color;
			// Tween the color back to clear.
			this.playerIconFlashImage.DOColor(endValue: Color.clear, duration: time);
		}
		/// <summary>
		/// Animates the combatant to pulse a certain color.
		/// This may or may not get used depending on the results of the calculation are.
		/// </summary>
		/// <param name="damageCalculation">A damage calculation that needs to be animated in response to the pulse.</param>
		public void AnimatePulseColor(DamageCalculation damageCalculation) {

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
		public void AnimateAfflictionSet(Combatant combatant, AfflictionType afflictionType) {

			// If the affliction is literally any other kind than None, shake. Only horizontally though.
			if (afflictionType != AfflictionType.None) {

				// Only shake the status if the affliction is different from what the player has.
				if (combatant.Affliction.Type != afflictionType) {
					// Complete any animations before shaking.
					this.GetComponent<RectTransform>().DOKill(complete: true);
					this.GetComponent<RectTransform>().DOShakeAnchorPos(duration: 0.5f, strength: new Vector2(x: 25f, y: 0f), vibrato: 50, snapping: true);
				}

				// Regardless of type, set the material to the material associated with the specified affliction.
				this.playerIconImageFront.material = DataController.GetAfflictionMaterial(afflictionType: afflictionType);

			} else {
				// If the None affliction is being used, set the material to null. This, for some reason, works because null materials just. Use the default shader I guess.
				this.playerIconImageFront.material = null;

			}

			// This is a bit... shoehorned, but... it's all I can think of rn. Also animate the bust up on the menu if you can.
			if (BattleMenuControllerDX.instance.PlayerBustUpGameObject.activeInHierarchy && BattleController.Instance.CurrentCombatant == this.player) {
				BattleMenuControllerDX.instance.SetPlayerBustUpSprite(player: this.player);
			}

		}
		#endregion

		#region HELPERS - TAG ANIMATION
		/// <summary>
		/// A helper function for enabling a resistance tag as a result of a damage calculation.
		/// </summary>
		/// <param name="damageCalculation">The calculation that should be read from.</param>
		private void EnableResistanceTag(DamageCalculation damageCalculation) {
			// If the damage calculation had a specific accuracy/resistance it needs to show, turn on the appropriate object. Accuracy takes priority over resistance.
			if (damageCalculation.accuracyType == AccuracyType.Critical || damageCalculation.accuracyType == AccuracyType.Miss) {
				switch (damageCalculation.accuracyType) {
					case AccuracyType.Critical:
						this.critTagImage.gameObject.SetActive(true);
						break;
					case AccuracyType.Miss:
						this.missTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			} else if (damageCalculation.finalResistance != ResistanceType.Nm) {
				switch (damageCalculation.finalResistance) {
					case ResistanceType.Wk:
						this.weakTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Str:
						this.resistTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Ref:
						this.reflectTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Abs:
						throw new System.NotImplementedException("I never actually made an absorb tag?");
						break;
					case ResistanceType.Nul:
						this.blockTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			}
		}
		/// <summary>
		/// Just a helper routine for setting the boost icons up.
		/// </summary>
		/// <param name="worldEnemyDX"></param>
		private void EnableBoostIcons(Player player) {

			if (player.GetPowerBoost(PowerBoostType.Accuracy) != 1f) {
				this.accuracyBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Accuracy) > 1f) {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.accuracyBoostGameObject.SetActive(false);
			}

			if (player.GetPowerBoost(PowerBoostType.Defense) != 1f) {
				this.defenseBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Defense) > 1f) {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.defenseBoostGameObject.SetActive(false);
			}

			if (player.GetPowerBoost(PowerBoostType.Attack) != 1f) {
				this.attackBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Attack) > 1f) {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.attackBoostGameObject.SetActive(false);
			}

		}
		/// <summary>
		/// Enables the boost icon graphics based on the player's current status and what can be expected from the incoming damage calculation.
		/// </summary>
		/// <param name="player">The player who owns this status.</param>
		/// <param name="damageCalculation">The damage calculation being used to predict what will happen.</param>
		private void EnableBoostIcons(Player player, DamageCalculation damageCalculation) {
			if (player.GetPowerBoost(PowerBoostType.Accuracy) != 1f) {
				this.accuracyBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Accuracy) > 1f) {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.accuracyBoostGameObject.SetActive(false);
			}

			if (player.GetPowerBoost(PowerBoostType.Defense) != 1f) {
				this.defenseBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Defense) > 1f) {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.defenseBoostGameObject.SetActive(false);
			}

			if (player.GetPowerBoost(PowerBoostType.Attack) != 1f) {
				this.attackBoostGameObject.SetActive(true);
				if (player.GetPowerBoost(PowerBoostType.Attack) > 1f) {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.attackBoostGameObject.SetActive(false);
			}
		}
		#endregion

		#region HELPERS - COLOR
		/// <summary>
		/// A central location where I can crossfade the color of the iimage front.
		/// </summary>
		/// <param name="color">The color to set the icon image front to.</param>
		/// <param name="duration">The amount of time to spend tweening the color.</param>
		private void CrossFadeIconColor(Color color, float duration) {
			this.playerIconImageFront.CrossFadeColor(targetColor: color, duration: 0f, ignoreTimeScale: true, useAlpha: false);
		}
		#endregion

		#region EVENT SYSTEMS INTERFACE IMPLEMENTATIONS
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnCancel(BaseEventData eventData) {
			BattleMenuControllerDX.instance.CancelCombatantSelection();
			AudioController.instance?.PlaySFX(SFXType.Close);
		}
		public void OnSubmit(BaseEventData eventData) {
			// Disable the selectables on all the player statuses (including this one, obviously.)
			PlayerStatusDXController.instance.SetPlayerStatusSelectables(status: false);
			// Null out the currently selected game object. Even when I set the selectable to not be turned on, sometimes it still. does accept events.
			EventSystem.current.SetSelectedGameObject(null);
			// Upon submitting, tell the battle menu controller that this was the enemy destined to be picked.
			BattleMenuControllerDX.instance.SetCurrentTargetCombatants(combatants: new List<Combatant>() { this.player });
			
		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.Dehighlight();
		}
		#endregion

	}


}