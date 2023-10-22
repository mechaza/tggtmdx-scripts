using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.Serialization;
using Cinemachine;

namespace Grawly.Chat {

	/// <summary>
	/// The bust up to use for speakers at the corners of the screen.
	/// </summary>
	public class StandardChatBustUp : ChatBustUp {

		#region FIELDS - STATE
		/// <summary>
		/// The standard bust up uses a passive speaker as well as the 'current' speaker, which is defined in the parent class.
		/// </summary>
		protected internal ChatSpeakerTemplate passiveChatSpeaker;
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to take when tweening the front bust up in by default.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float frontBustUpDefaultTweenInTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the front bust up out by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float frontBustUpDefaultTweenOutTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the rectangle in by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float propUpDecoractionRectangleTweenInTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the rectangle out by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float propUpDecoractionRectangleTweenOutTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the rectangle in by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float highlightSquareTweenInTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening the rectangle out by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float highlightSquareTweenOutTime = 0.3f;
		/// <summary>
		/// The amount of time to take when fading the sprites.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float spriteChangeFadeTime = 0.3f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease type to use when tweening the front bust up in by default.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease frontBustUpDefaultEaseInType = Ease.InQuint;
		/// <summary>
		/// The ease type to use when tweening the front bust up out by default.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease frontBustUpDefaultEaseOutType = Ease.OutQuint;
		/// <summary>
		/// The easing to use when tweening the prop up rectangle in.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease propUpDecoractionRectangleEaseInType = Ease.InQuint;
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease propUpDecoractionRectangleEaseOutType = Ease.OutQuint;
		/// <summary>
		/// The ease type to use when tweening the highlight square in.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease highlightSquareEaseInType = Ease.InQuint;
		/// <summary>
		/// The ease type to use when tweening the highlight square out.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private Ease highlightSquareEaseOutType = Ease.OutQuint;
		#endregion

		#region FIELDS - TOGGLES : ROTATIONS
		/// <summary>
		/// The direction the highlight square should rotate when this speaker is highlighted and the highlight square is showing.
		/// </summary>
		[Title("Rotations")]
		[SerializeField, TabGroup("Bust Up", "Toggles")]
		private float highlightSquareRotationDirection = 1f;
		#endregion

		#region FIELDS - POSITIONS
		/// <summary>
		/// The position the front speaker should be when active.
		/// </summary>
		[Title("Front Speaker")]
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 frontSpeakerActivePosition = new Vector2();
		/// <summary>
		/// The position the front speaker should be when listening.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 frontSpeakerListeningPosition = new Vector2();
		/// <summary>
		/// The position the front speaker should be when hiding.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 frontSpeakerHidingPosition = new Vector2();
		/// <summary>
		/// The position the passive speaker should be when listening.
		/// </summary>
		[Title("Passive Speaker")]
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 passiveSpeakerListeningPosition = new Vector2();
		/// <summary>
		/// The position the passive speaker should be when hiding.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 passiveSpeakerHidingPosition = new Vector2();
		/// <summary>
		/// The active position for the prop up rectangle.
		/// </summary>
		[Title("Prop Up Rectangle")]
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 propUpDecoractionRectangleActivePosition = new Vector2();
		/// <summary>
		/// The hiding position for the prop up rectangle.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 propUpDecoractionRectangleHidingPosition = new Vector2();
		/// <summary>
		/// The position for the highlight square when its active.
		/// This one is a bit tricky because other speakers will potentially want to reference it.
		/// </summary>
		[Title("Highlight Square")]
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 highlightSquareActivePosition = new Vector2();
		/// <summary>
		/// The position for the highlight square when its hiding.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Positions")]
		private Vector2 highlightSquareHidingPosition = new Vector2();
		#endregion

		#region FIELDS - SCENE REFERENCES : PIVOT RECT TRANSFORMS
		/// <summary>
		/// The RectTransform that acts as the main pivot for the front speaker.
		/// Handy for managing general positioning.
		/// </summary>
		[Title("Speaker Pivots")]
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform frontSpeakerMainPivotRectTransform;
		/// <summary>
		/// The RectTransform that acts as the child pivot for the front speaker.
		/// Handy for managing motion noise.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private ChatBustUpNoiseBehavior frontSpeakerChildPivotNoiseBehavior;
		/// <summary>
		/// The RectTransform that acts as the main pivot for the back speaker.
		/// Handy for managing general positioning.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform passiveSpeakerMainPivotRectTransform;
		/// <summary>
		/// The RectTransform that acts as the child pivot for the back speaker.
		/// Handy for managing motion noise.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private ChatBustUpNoiseBehavior passiveSpeakerChildPivotNoiseBehavior;
		#endregion

		#region FIELDS - SCENE REFERENCES : BUST UP IMAGES
		/// <summary>
		/// The main image for the front speaker.
		/// </summary>
		[Title("Speaker Images")]
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image frontSpeakerMainImage;
		/// <summary>
		/// The image for when I need to fake a sprite change.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image frontSpeakerFaderImage;
		/// <summary>
		/// The dropshadow image for the front speaker.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image frontSpeakerDropshadowImage;
		/// <summary>
		/// The main image for the back speaker.
		/// The back speaker does not have a dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image passiveSpeakerMainImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : DECORATIVE RECTANGLES
		/// <summary>
		/// The main pivot for the prup up decoration rectangle.
		/// </summary>
		[Title("Decorative Rectangles")]
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform propUpDecoractionRectangleMainPivotRectTransform;
		/// <summary>
		/// The main pivot for the highlight square.
		/// Good for positioning and shit.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform highlightSquareMainPivotRectTransform;
		/// <summary>
		/// The child pivot for the highlight square.
		/// Good for noise motion.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private RectTransform highlightSquareChildPivotRectTransform;
		/// <summary>
		/// The image that actually shows the highlight square.
		/// Can change color and junk.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image highlightSquareImage;
		/// <summary>
		/// The image for the prop up decoration rectangle.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References")]
		private Image propUpDecorationRectangleImage;
		#endregion

		#region FIELDS - RESOURCES 
		/// <summary>
		/// The noise settings to use for active speakers.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Resources")]
		private NoiseSettings activeSpeakerNoiseSettings;
		/// <summary>
		/// The noise settings to use for listening speakers.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Resources")]
		private NoiseSettings listeningSpeakerNoiseSettings;
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Totally reset the state on start.
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents a ChatSpeaker onto the screen.
		/// </summary>
		/// <param name="chatSpeaker">The speaker to present.</param>
		public override void PresentSpeaker(ChatSpeakerTemplate chatSpeaker, ChatBustUpParams bustUpParams) {

			Debug.Log("Presenting bust up for " + chatSpeaker.rawSpeakerName);

			// Set this bust up as active.
			this.ActiveInChat = true;

			// Save the speaker, as it will be needed for things like GetSpeakerPosition in ChatControllerDX.
			this.currentChatSpeaker = chatSpeaker;

			// Set the colors on the highlight square/prop up rectangles.
			this.SetColors(chatSpeaker: chatSpeaker, bustUpParams: bustUpParams);

			// Play the animation for the rotating highlight square.
			this.PlayHighlightSquareRotationAnimation();
			
			// Change the sprite on the active bust up.
			// this.ChangeActiveBustUpSprite(chatSpeaker: chatSpeaker, bustUpType: bustUpParams.BustUpType);
			
			// Set the speaker to be... speaking. This is also what changes the sprite now.
			this.SetSpeaking(bustUpParams: bustUpParams);
		}
		/// <summary>
		/// Dismisses the bust up totally. 
		/// </summary>
		/// <param name="bustUpParams">The bust up params that were passed in when this was made.</param>
		protected override void DismissSpeaker(ChatBustUpParams bustUpParams) {
			// For right now I guess just call SetHiding.
			this.SetHiding(bustUpParams: bustUpParams);
		}
		#endregion

		#region COLORS
		/// <summary>
		/// Sets the colors of the decorations around the speaker.
		/// </summary>
		/// <param name="chatSpeaker">The template of the current speaker to present.</param>
		/// <param name="bustUpParams">Additional bust up params to use with the speaker.</param>
		protected override void SetColors(ChatSpeakerTemplate chatSpeaker, ChatBustUpParams bustUpParams) {
			Debug.LogWarning("TODO: Refactor this so that it grabs the colors in a better way.");
			if (this.SpeakerPositionType == ChatSpeakerPositionType.TopRight) {
				this.propUpDecorationRectangleImage.color = GrawlyColors.colorDict[chatSpeaker.nameTagBackingColorType];
				this.highlightSquareImage.color = Color.white;
			} else {
				this.propUpDecorationRectangleImage.color = Color.white;
				this.highlightSquareImage.color = GrawlyColors.colorDict[chatSpeaker.nameTagBackingColorType];
			}
		}
		#endregion


		#region MAIN ANIMATIONS
		/// <summary>
		/// Reverts the ChatBustUp to the state where it should be before it gets popped up.
		/// </summary>
		public override void ResetState() {

			// Make sure this speaker is marked as inactive in chat.
			this.ActiveInChat = false;

			// Move the front speaker's main pivot to the hiding position.
			this.frontSpeakerMainPivotRectTransform.anchoredPosition = this.frontSpeakerHidingPosition;

			// Move the prop up rectangle out of the way.
			this.propUpDecoractionRectangleMainPivotRectTransform.anchoredPosition = this.propUpDecoractionRectangleHidingPosition;

			// Also move the highlight square.
			this.highlightSquareMainPivotRectTransform.anchoredPosition = this.highlightSquareHidingPosition;

			// Also reset the color.
			this.frontSpeakerMainImage.color = Color.clear;

		}
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is speaking.
		/// </summary>
		/// <param name="tweenToggles">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal override void SetSpeaking(ChatBustUpParams bustUpParams) {

			// Call DOTween to tween the main pivot.
			this.frontSpeakerMainPivotRectTransform.DOAnchorPos(
				endValue: this.frontSpeakerActivePosition,
				duration: this.frontBustUpDefaultTweenInTime,
				snapping: true)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);

			// Tell the noise behavior to use the speaking noise settings.
			this.frontSpeakerChildPivotNoiseBehavior
				.SetNoise(noiseSettings: this.activeSpeakerNoiseSettings);

			// If the bust up type is something other than none, change the active sprite.
			if (bustUpParams.BustUpType != ChatBustUpType.None) {
				// Try starting the fader at whatever the current bust up sprite is this immediate moment.
				this.ChangeActiveBustUpSprite(
					chatSpeaker: this.currentChatSpeaker,
					startColor: this.frontSpeakerMainImage.color,
					bustUpType: bustUpParams.BustUpType);
			}
			
			// Also tween the color.
			this.frontSpeakerMainImage.DOColor(
				endValue: Color.white,
				duration: this.frontBustUpDefaultTweenInTime)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);

			// Also tween the prop up rectangle/highlight square.
			this.propUpDecoractionRectangleMainPivotRectTransform.DOAnchorPos(
				endValue: this.propUpDecoractionRectangleActivePosition,
				duration: this.propUpDecoractionRectangleTweenInTime,
				snapping: true)
				.SetEase(ease: this.propUpDecoractionRectangleEaseInType);

			// Tween the highlight square in.
			this.highlightSquareMainPivotRectTransform.DOAnchorPos(
				endValue: this.highlightSquareActivePosition,
				duration: this.highlightSquareTweenInTime,
				snapping: true)
				.SetEase(ease: this.highlightSquareEaseInType);

			

		}
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is listening.
		/// </summary>
		/// <param name="tweenToggles">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal override void SetListening(ChatBustUpParams bustUpParams) {

			// Complete the tweens, if there are any.
			// this.KillTweensOnMainRectTransforms();

			// Call DOTween to tween the main pivot.
			this.frontSpeakerMainPivotRectTransform.DOAnchorPos(
				endValue: this.frontSpeakerListeningPosition,
				duration: this.frontBustUpDefaultTweenInTime,
				snapping: true)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);

			// If the bust up type is something other than none, change the active sprite.
			if (bustUpParams.BustUpType != ChatBustUpType.None) {
				// Try starting the fader at whatever the current bust up sprite is this immediate moment.
				this.ChangeActiveBustUpSprite(
					chatSpeaker: this.currentChatSpeaker,
					startColor: this.frontSpeakerMainImage.color,
					bustUpType: bustUpParams.BustUpType);
			}
			
			// Also tween the color.
			this.frontSpeakerMainImage.DOColor(
				endValue: Color.gray,
				duration: this.frontBustUpDefaultTweenInTime)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);

			// Also tween the prop up rectangle/highlight square.
			this.propUpDecoractionRectangleMainPivotRectTransform.DOAnchorPos(
				endValue: this.propUpDecoractionRectangleActivePosition,
				duration: this.propUpDecoractionRectangleTweenInTime,
				snapping: true)
				.SetEase(ease: this.propUpDecoractionRectangleEaseInType);

			// Tween the highlight square out.
			this.highlightSquareMainPivotRectTransform.DOAnchorPos(
				endValue: this.highlightSquareHidingPosition,
				duration: this.highlightSquareTweenOutTime,
				snapping: true)
				.SetEase(ease: this.highlightSquareEaseOutType);

			// Tell the noise behavior to use the listening noise settings.
			this.frontSpeakerChildPivotNoiseBehavior.SetNoise(noiseSettings: this.listeningSpeakerNoiseSettings);

		}
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is hiding.
		/// </summary>
		/// <param name="tweenToggles">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal override void SetHiding(ChatBustUpParams bustUpParams) {

			// If I'm setting the speaker to hiding, it probably means they're not active anymore.
			this.ActiveInChat = false;

			// Complete the tweens, if there are any.
			// this.KillTweensOnMainRectTransforms();

			// Call DOTween to tween the main pivot.
			this.frontSpeakerMainPivotRectTransform.DOAnchorPos(
				endValue: this.frontSpeakerHidingPosition,
				duration: this.frontBustUpDefaultTweenInTime,
				snapping: true)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);
			
			// Also tween the color.
			this.frontSpeakerMainImage.DOColor(
				endValue: Color.clear,
				duration: this.frontBustUpDefaultTweenInTime)
				.SetEase(ease: this.frontBustUpDefaultEaseInType);

			// Hide the prop up.
			this.propUpDecoractionRectangleMainPivotRectTransform.DOAnchorPos(
				endValue: this.propUpDecoractionRectangleHidingPosition,
				duration: this.propUpDecoractionRectangleTweenOutTime,
				snapping: true)
				.SetEase(ease: this.propUpDecoractionRectangleEaseOutType);

			// Tween the highlight square out.
			this.highlightSquareMainPivotRectTransform.DOAnchorPos(
				endValue: this.highlightSquareHidingPosition,
				duration: this.highlightSquareTweenOutTime,
				snapping: true)
				.SetEase(ease: this.highlightSquareEaseOutType);

		}
		/// <summary>
		/// Kills any tweens on the main rect transforms. I need to do this when setting speaking/listening/hiding.
		/// </summary>
		private void KillTweensOnMainRectTransforms() {
			this.frontSpeakerMainPivotRectTransform.DOKill(complete: true);
			this.frontSpeakerMainImage.DOKill(complete: true);
			this.frontSpeakerFaderImage.DOKill(complete: true);
			this.propUpDecoractionRectangleMainPivotRectTransform.DOKill(complete: true);
			this.highlightSquareMainPivotRectTransform.DOKill(complete: true);
		}
		#endregion

		#region OTHER CHANGES
		/// <summary>
		/// Changes the sprite on the bust up to match that of the speaker being passed in.
		/// </summary>
		/// <param name="chatSpeaker">The chat speaker who's bust up sprite should be displayed.</param>
		/// <param name="startColor">The color the fader image should be at at the start.</param>
		/// <param name="bustUpType">The type of sprite to use for the speaker.</param>
		private void ChangeActiveBustUpSprite(ChatSpeakerTemplate chatSpeaker, Color startColor, ChatBustUpType bustUpType) {
			
			Debug.Assert(bustUpType != ChatBustUpType.None);
			
			// Kill any tweens on the fader and snap its color to white.
			this.frontSpeakerFaderImage.DOKill(complete: true);
			this.frontSpeakerFaderImage.color = startColor;
			
			// Pass the override sprite from the main image to the fader.
			this.frontSpeakerFaderImage.overrideSprite = this.frontSpeakerMainImage.overrideSprite;
			
			// Change the override sprites on the image.
			this.frontSpeakerMainImage.overrideSprite = chatSpeaker.bustUpSpriteDict[bustUpType];
			this.frontSpeakerDropshadowImage.overrideSprite = chatSpeaker.bustUpSpriteDict[bustUpType];
			
			// Fade the fader image. This makes the transition look less jarring.
			this.frontSpeakerFaderImage.DOFade(endValue: 0f, duration: this.spriteChangeFadeTime).SetEase(Ease.Linear);
			
		}
		/// <summary>
		/// Changes the sprite on the bust up in the back to be the one specified.
		/// </summary>
		/// <param name="chatSpeaker">The chat speaker who's bust up sprite should be displayed.</param>
		/// <param name="bustUpType">The type of sprite to use for the speaker.</param>
		private void ChangePassiveBustUpSprite(ChatSpeakerTemplate chatSpeaker, ChatBustUpType bustUpType = ChatBustUpType.None) {
			// this.passiveSpeakerMainImage.sprite = chatSpeaker.bustUpSpriteDict[bustUpType];
			this.passiveSpeakerMainImage.overrideSprite = chatSpeaker.bustUpSpriteDict[bustUpType];
		}
		#endregion

		#region SPECIAL EFFECTS
		/// <summary>
		/// A method to quickly play the animation that makes the highlight square rotate.
		/// </summary>
		private void PlayHighlightSquareRotationAnimation() {
			// Kill any animations on the square just in case.
			this.highlightSquareChildPivotRectTransform.DOKill();
			// The direction the highlight square rotates is dependent on the multiplier.
			this.highlightSquareChildPivotRectTransform.transform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: 360f * this.highlightSquareRotationDirection),
				duration: 10f,
				mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: Ease.Linear)
				.SetLoops(loops: -1);
		}
		/// <summary>
		/// Shakes the bust up vigorously.
		/// </summary>
		/// <param name="time">The amount of time to spend shaking it like you mean it.</param>
		/// <param name="magnitude">How powerful is your groove?</param>
		public override void Shake(float time = 0.2F, float magnitude = 10) {
			throw new System.NotImplementedException();
		}
		#endregion






	}


}