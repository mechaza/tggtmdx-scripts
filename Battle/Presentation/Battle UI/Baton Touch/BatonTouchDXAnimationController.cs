using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is the class that should manage how the baton touch is animated.
	/// </summary>
	public class BatonTouchDXAnimationController : MonoBehaviour {

		public static BatonTouchDXAnimationController instance;

		#region FIELDS - TOGGLES : SETTINGS
		/// <summary>
		/// The time it should take for the image masks to do a tween.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private float rectTransformTweenSpeed;
		/// <summary>
		/// The ease type to use on the mask images.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private Ease rectTransformTweenType;
		/// <summary>
		/// The time it should take for the bust ups to do a tween.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private float bustUpImageTweenSpeed;
		/// <summary>
		/// The ease type to use on the bust up images.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private Ease bustUpImageTweenType;
		/// <summary>
		/// The time it should take for the baton touch logo to appear.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private float batonTouchLogoTweenSpeed;
		/// <summary>
		/// The ease to use on the baton touch rect transform.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private Ease batonTouchLogoTweenType;
		/// <summary>
		/// The amount of time to wait until the animation should end.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Toggles")]
		private float fullAnimationTime;
		#endregion

		#region FIELDS - TOGGLES : INITIAL POSITIONS
		/// <summary>
		/// The initial position for the rect transform of all the visuals of the source player's cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 initialSourceCutInRectAnchorPos;
		/// <summary>
		/// The initial position for the source player's bust up image.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 initialSourceBustUpAnchorPos;
		/// <summary>
		/// The initial position for the rect transform of all the visuals of the target player's cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 initialTargetCutInRectAnchorPos;
		/// <summary>
		/// The initial position for the target player's bust up image.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 initialTargetBustUpAnchorPos;
		#endregion

		#region FIELDS - TOGGLES : FINAL POSITIONS
		/// <summary>
		/// The final position for the rect transform of all the visuals of the source player's cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 finalSourceCutInRectAnchorPos;
		/// <summary>
		/// The final position for the source player's bust up image.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 finalSourceBustUpAnchorPos;
		/// <summary>
		/// The final position for the rect transform of all the visuals of the target player's cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 finalTargetCutInRectAnchorPos;
		/// <summary>
		/// The final position for the source player's bust up image.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Positions")]
		private Vector2 finalTargetBustUpAnchorPos;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the baton touch's animation.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private GameObject allVisualsObject;
		/// <summary>
		/// The rect transform for all the visuals for the source player cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private RectTransform sourcePlayerCutInRectTransform;
		/// <summary>
		/// The rect transform that contains all the objects for the source player's bust up.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private RectTransform sourcePlayerBustUpRectTransform;
		/// <summary>
		/// The image showing the bust up of the player who called the baton touch.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private Image sourcePlayerBustUpImage;
		/// <summary>
		/// The image showing the dropshadow for the bust up of the player who called the baton touch.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private Image sourcePlayerBustUpDropshadowImage;
		/// <summary>
		/// The rect transform for all the visuals for the target player cut in.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private RectTransform targetPlayerCutInRectTransform;
		/// <summary>
		/// The rect transform that contains all the objects for the target player's bust up.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private RectTransform targetPlayerBustUpRectTransform;
		/// <summary>
		/// The image showing the bust up of the player who is receiving the baton.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private Image targetPlayerBustUpImage;
		/// <summary>
		/// The image showing the dropshadow for the bust up of the player who is receiving the baton.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private Image targetPlayerBustUpDropshadowImage;
		/// <summary>
		/// The rect transform that contains the visuals for the baton touch logo.
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Scene References")]
		private RectTransform batonTouchLogoRectTransform;
		#endregion

		#region FIELDS - DEBUG
		/// <summary>
		/// Is debug mode on or off?
		/// </summary>
		[SerializeField, TabGroup("Baton Touch", "Debug")]
		private bool debugMode = false;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Plays the baton touch animation.
		/// </summary>
		/// <param name="sourcePlayer">The player who called baton touch.</param>
		/// <param name="targetPlayer">The player who is receiving the baton.</param>
		public IEnumerator PlayBatonTouchAnimation(Player sourcePlayer, Player targetPlayer) {



			/*// Set the sprites on the bust up images.
			this.sourcePlayerBustUpImage.sprite = sourcePlayer.playerTemplate.bustUp;
			this.sourcePlayerBustUpDropshadowImage.sprite = sourcePlayer.playerTemplate.bustUp;
			this.targetPlayerBustUpImage.sprite = targetPlayer.playerTemplate.bustUp;
			this.targetPlayerBustUpDropshadowImage.sprite = targetPlayer.playerTemplate.bustUp;*/

			// Set the sprites on the bust up images.
			this.sourcePlayerBustUpImage.overrideSprite = sourcePlayer.playerTemplate.bustUp;
			this.sourcePlayerBustUpDropshadowImage.overrideSprite = sourcePlayer.playerTemplate.bustUp;
			this.targetPlayerBustUpImage.overrideSprite = targetPlayer.playerTemplate.bustUp;
			this.targetPlayerBustUpDropshadowImage.overrideSprite = targetPlayer.playerTemplate.bustUp;

			// Play the animation. I sorta just let this function handle everything.
			this.PlayBatonTouchAnimation();

			// Wait a hard amount of time before turning off the visuals and returning.
			yield return new WaitForSeconds(this.fullAnimationTime);
			this.allVisualsObject.SetActive(false);

		}
		#endregion

		#region DEBUG FUNCTIONS : POSITION SETTERS
		[TabGroup("Baton Touch", "Debug"), ShowInInspector, ShowIf("debugMode"), HideInPlayMode]
		private void ResetToInitialPositions() {

			// Reset the positions for the source graphics to the initial positions.
			this.sourcePlayerCutInRectTransform.anchoredPosition = this.initialSourceCutInRectAnchorPos;
			this.sourcePlayerBustUpRectTransform.anchoredPosition = this.initialSourceBustUpAnchorPos;

			// Reset the positions for the target graphics to the initial positions.
			this.targetPlayerCutInRectTransform.anchoredPosition = this.initialTargetCutInRectAnchorPos;
			this.targetPlayerBustUpRectTransform.anchoredPosition = this.initialTargetBustUpAnchorPos;

			// Reset the scale on the baton touch logo.
			this.batonTouchLogoRectTransform.localScale = Vector3.zero;

		}
		[TabGroup("Baton Touch", "Debug"), ShowInInspector, ShowIf("debugMode"), HideInPlayMode]
		private void ResetToFinalPositions() {

			// Reset the positions for the source graphics to the initial positions.
			this.sourcePlayerCutInRectTransform.anchoredPosition = this.finalSourceCutInRectAnchorPos;
			this.sourcePlayerBustUpRectTransform.anchoredPosition = this.finalSourceBustUpAnchorPos;

			// Reset the positions for the target graphics to the initial positions.
			this.targetPlayerCutInRectTransform.anchoredPosition = this.finalTargetCutInRectAnchorPos;
			this.targetPlayerBustUpRectTransform.anchoredPosition = this.finalTargetBustUpAnchorPos;

			// Reset the scale on the baton touch logo.
			this.batonTouchLogoRectTransform.localScale = Vector3.one;

		}
		/// <summary>
		/// Records the transform positions of the image masks.
		/// </summary>
		[TabGroup("Baton Touch", "Debug"), ShowInInspector, ShowIf("debugMode"), HideInPlayMode]
		private void SaveInitialPositions() {

			// Save the initial positions for the source player's graphics.
			this.initialSourceCutInRectAnchorPos = this.sourcePlayerCutInRectTransform.anchoredPosition;
			this.initialSourceBustUpAnchorPos = this.sourcePlayerBustUpRectTransform.anchoredPosition;

			// Save the initial positions for the target player's graphics.
			this.initialTargetCutInRectAnchorPos = this.targetPlayerCutInRectTransform.anchoredPosition;
			this.initialTargetBustUpAnchorPos = this.targetPlayerBustUpRectTransform.anchoredPosition;
		}
		[TabGroup("Baton Touch", "Debug"), ShowInInspector, ShowIf("debugMode"), HideInPlayMode]
		private void SaveFinalPositions() {

			// Save the final positions for the source player's graphics.
			this.finalSourceCutInRectAnchorPos = this.sourcePlayerCutInRectTransform.anchoredPosition;
			this.finalSourceBustUpAnchorPos = this.sourcePlayerBustUpRectTransform.anchoredPosition;

			// Save the final positions for the target player's graphics.
			this.finalTargetCutInRectAnchorPos = this.targetPlayerCutInRectTransform.anchoredPosition;
			this.finalTargetBustUpAnchorPos = this.targetPlayerBustUpRectTransform.anchoredPosition;

		}
		#endregion

		#region DEBUG FUNCTIONS : ANIMATION TESTS
		/// <summary>
		/// Just tests the animation to see if everything is working.
		/// </summary>
		[TabGroup("Baton Touch", "Debug"), ShowInInspector, ShowIf("debugMode"), HideInEditorMode]
		private void PlayBatonTouchAnimation() {

			// Turn the visuals on.
			this.allVisualsObject.SetActive(true);

			// Reset the transforms to their original positins.
			this.ResetToInitialPositions();


			// Tween the cut ins.
			this.sourcePlayerCutInRectTransform.DOAnchorPos(endValue: this.finalSourceCutInRectAnchorPos, duration: this.rectTransformTweenSpeed, snapping: true).SetEase(ease: this.rectTransformTweenType);
			this.targetPlayerCutInRectTransform.DOAnchorPos(endValue: this.finalTargetCutInRectAnchorPos, duration: this.rectTransformTweenSpeed, snapping: true).SetEase(ease: this.rectTransformTweenType);

			// Tween the bust ups.
			this.sourcePlayerBustUpRectTransform.DOAnchorPos(endValue: this.finalSourceBustUpAnchorPos, duration: this.bustUpImageTweenSpeed, snapping: true).SetEase(ease: this.bustUpImageTweenType);
			this.targetPlayerBustUpRectTransform.DOAnchorPos(endValue: this.finalTargetBustUpAnchorPos, duration: this.bustUpImageTweenSpeed, snapping: true).SetEase(ease: this.bustUpImageTweenType);

			// Tween the logo.
			this.batonTouchLogoRectTransform.DOScale(endValue: Vector3.one, duration: this.batonTouchLogoTweenSpeed).SetEase(ease: this.batonTouchLogoTweenType);

			/*// Wait a few moments, then turn the visuals off and send an event.
			GameController.Instance.WaitThenRun(timeToWait: this.fullAnimationTime, action: delegate {
				
			});*/
		}
		#endregion

	}


}