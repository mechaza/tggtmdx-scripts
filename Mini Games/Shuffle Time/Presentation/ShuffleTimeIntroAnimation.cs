using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// Controls the animation for the shuffle time intro. And possibly outro.
	/// </summary>
	public class ShuffleTimeIntroAnimation : MonoBehaviour {

		public static ShuffleTimeIntroAnimation Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The sequence as it is currently running.
		/// May be needed so I can cancel it later.
		/// </summary>
		private Sequence currentSequence;
		#endregion

		#region FIELDS - TOGGLES : DATA
		/// <summary>
		/// The text to show on the first text label.
		/// </summary>
		[Title("Config")]
		[TabGroup("Animation", "Toggles"), SerializeField]
		private string textLabel1String = "";
		/// <summary>
		/// The text to show on the second text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private string textLabel2String = "";
		#endregion

		#region FIELDS - TOGGLES : POSITIONING
		/// <summary>
		/// The starting position for the first text label.
		/// </summary>
		[Title("Positions")]
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel1StartPos = new Vector2();
		/// <summary>
		/// The starting position for the second text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel2StartPos = new Vector2();
		/// <summary>
		/// The ending position for the first text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel1MidPos = new Vector2();
		/// <summary>
		/// The ending position for the second text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel2MidPos = new Vector2();
		/// <summary>
		/// The ending position for the first text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel1EndPos = new Vector2();
		/// <summary>
		/// The ending position for the second text label.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Vector2 textLabel2EndPos = new Vector2();
		#endregion

		#region FIELDS - TOGGLES : EASING AND TIMING
		/// <summary>
		/// The amount of time to wait before tweening the text in.
		/// </summary>
		[Title("Timing")]
		[TabGroup("Animation", "Toggles"), SerializeField]
		private float textTweenDelayTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the text in.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private float textTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take to show the text on screen.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private float textHaltTime = 0.5f;
		/// <summary>
		/// The amount of time for the text to fade in.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private float textFadeInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening the back square in.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private float backSquareScaleInTime = 0.2f;
		/// <summary>
		/// The easing to use when tweening the text in.
		/// </summary>
		[Title("Easing")]
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Ease textEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the back square in.
		/// </summary>
		[TabGroup("Animation", "Toggles"), SerializeField]
		private Ease backSquareEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The color for the back square.
		/// </summary>
		private Color backSquareColor;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All the objects.
		/// </summary>
		[Title("All Objects")]
		[TabGroup("Animation", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// All of the objects specific to the intro animation.
		/// </summary>
		[TabGroup("Animation", "Scene References"), SerializeField]
		private GameObject allAnimationObjects;
		/// <summary>
		/// All of the
		/// </summary>
		[TabGroup("Animation", "Scene References"), SerializeField]
		private GameObject allDebugObjects;
		/// <summary>
		/// The rect transform for the first text label.
		/// </summary>
		[Title("Rect Transforms")]
		[TabGroup("Animation", "Scene References"), SerializeField]
		private RectTransform textLabel1RectTransform;
		/// <summary>
		/// The rect transform for the second text label.
		/// </summary>
		[TabGroup("Animation", "Scene References"), SerializeField]
		private RectTransform textLabel2RectTransform;
		/// <summary>
		/// The rect transform for the first square behind the text.
		/// </summary>
		[TabGroup("Animation", "Scene References"), SerializeField]
		private RectTransform backSquareRectTransform;
		/// <summary>
		/// The rect transform for the second square behind the text.
		/// </summary>
		[TabGroup("Animation", "Scene References"), SerializeField]
		private RectTransform frontSquareRectTransform;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The first text label.
		/// Mostly using this as a property because itll be easier.
		/// </summary>
		private SuperTextMesh TextLabel1 {
			get {
				return this.textLabel1RectTransform.GetComponent<SuperTextMesh>();
			}
		}
		/// <summary>
		/// The second text label.
		/// Mostly using this as a property because itll be easier.
		/// </summary>
		private SuperTextMesh TextLabel2 {
			get {
				return this.textLabel2RectTransform.GetComponent<SuperTextMesh>();
			}
		}
		/// <summary>
		/// The image for the back square.
		/// </summary>
		private Image BackSquareImage {
			get {
				return this.backSquareRectTransform.GetComponent<Image>();
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			// Remember the color of the back square.
			this.backSquareColor = this.BackSquareImage.color;
		}
		private void Start() {
			this.ResetState();
			// this.textDecorationSquare1RectTransform.GetComponent<SuperTextMesh>().Text = this.textLabel1String;
			// this.textDecorationSquare2RectTransform.GetComponent<SuperTextMesh>().Text = this.textLabel2String;
			// this.allObjects.SetActive(false);
			// 
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Resets the state of the animation controller completely..
		/// </summary>
		[Button, HideInEditorMode]
		private void ResetState() {

			this.allObjects.SetActive(false);
			this.currentSequence?.Kill(complete: true);

			this.textLabel1RectTransform.DOKill(complete: true);
			this.textLabel2RectTransform.DOKill(complete: true);
			this.textLabel1RectTransform.anchoredPosition = this.textLabel1StartPos;
			this.textLabel2RectTransform.anchoredPosition = this.textLabel2StartPos;

			this.TextLabel1.DOKill(complete: true);
			this.TextLabel2.DOKill(complete: true);
			this.TextLabel1.Text = this.textLabel1String;
			this.TextLabel2.Text = this.textLabel2String;
			this.TextLabel1.color = Color.clear;
			this.TextLabel2.color = Color.clear;

			this.backSquareRectTransform.DOKill(complete: true);
			this.frontSquareRectTransform.DOKill(complete: true);
			this.backSquareRectTransform.transform.DOKill(complete: true);				// I kill on both the rect transforms and the normal transforms
			this.frontSquareRectTransform.transform.DOKill(complete: true);				// because the normal transform is the one that technically rotates.
			this.backSquareRectTransform.localScale = Vector3.zero;
			this.frontSquareRectTransform.localScale = Vector3.zero;

			this.BackSquareImage.DOKill(complete: true);
			this.BackSquareImage.color = this.backSquareColor;

		}
		/*/// <summary>
		/// Presents the different elements of the animation.
		/// </summary>
		/// <returns>The shuffle time sequence.</returns>
		private Sequence CreatePresentationSequence() {
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(delegate {
				this.textDecorationSquare1RectTransform.transform.DOScale(endValue: 1f, duration: 0.5f).SetEase(ease: Ease.InOutCirc);
			});
			return seq;

		}*/
		/// <summary>
		/// Plays the complete shuffle time intro animation.
		/// </summary>
		[Button, HideInEditorMode]
		public void PlayAnimation() {

			/*// Make it rotate indefinitely.
			worldEnemyDXSprite.worldEnemyHighlightSquareSpriteRenderer.transform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: 360f),
				duration: 3f, 
				mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: Ease.Linear)
				.SetLoops(loops: -1);*/

			// Reset the state but turn the objects back on.
			this.ResetState();
			this.allObjects.SetActive(true);

			// Play the sound effect.
			AudioController.instance?.PlaySFX(type: SFXType.ShuffleTime);

			// Make a new sequence. If something was already in motion, it would have been killed in ResetState().
			this.currentSequence = DOTween.Sequence();

			// Fix up the square...
			this.currentSequence.AppendCallback(delegate {


				// ...by scaling it in...
				this.backSquareRectTransform.DOScale(
					endValue: 1f, 
					duration: this.backSquareScaleInTime)
				.SetEase(ease: this.backSquareEaseInType);

				// ...and having it loop.
				this.backSquareRectTransform.transform.DOLocalRotate(
					endValue: new Vector3(x: 0f, y: 0f, z: 360f),
					duration: 3f,
					mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: Ease.Linear)
				.SetLoops(loops: -1);

			});

			// Delay a bit.
			this.currentSequence.AppendInterval(this.textTweenDelayTime);

			// Fix up the text...
			this.currentSequence.AppendCallback(delegate {

				// ...by tweening them in.
				this.textLabel1RectTransform.DOAnchorPos(
					endValue: this.textLabel1MidPos,
					duration: this.textTweenInTime)
				.SetEase(ease: this.textEaseInType);
				this.textLabel2RectTransform.DOAnchorPos(
					endValue: this.textLabel2MidPos,
					duration: this.textTweenInTime)
				.SetEase(ease: this.textEaseInType);


				// Also make sure to tween the color.
				DOTween.To(
					getter: () => this.TextLabel1.color,
					setter: x => {
						this.TextLabel1.color = x;
						this.TextLabel1.Rebuild();
					},
					endValue: Color.black,
					duration: this.textFadeInTime)
				.SetEase(ease: Ease.Linear);
				DOTween.To(
					getter: () => this.TextLabel2.color,
					setter: x => {
						this.TextLabel2.color = x;
						this.TextLabel2.Rebuild();
					},
					endValue: Color.black,
					duration: this.textFadeInTime)
				.SetEase(ease: Ease.Linear);
			});

			// Wait a few seconds.
			this.currentSequence.AppendInterval(3f);




			// Fade everything out.
			this.currentSequence.AppendCallback(delegate {

				// TWEENING THE TEXT OUT
				this.textLabel1RectTransform.DOAnchorPos(
					endValue: this.textLabel1EndPos,
					duration: this.textTweenInTime)
				.SetEase(ease: this.textEaseInType);
				this.textLabel2RectTransform.DOAnchorPos(
					endValue: this.textLabel2EndPos,
					duration: this.textTweenInTime)
				.SetEase(ease: this.textEaseInType);


				// Also make sure to tween the color.
				DOTween.To(
					getter: () => this.TextLabel1.color,
					setter: x => {
						this.TextLabel1.color = x;
						this.TextLabel1.Rebuild();
					},
					endValue: Color.clear,
					duration: this.textFadeInTime)
				.SetEase(ease: Ease.Linear);
				DOTween.To(
					getter: () => this.TextLabel2.color,
					setter: x => {
						this.TextLabel2.color = x;
						this.TextLabel2.Rebuild();
					},
					endValue: Color.clear,
					duration: this.textFadeInTime)
				.SetEase(ease: Ease.Linear);

				this.BackSquareImage.DOColor(endValue: Color.clear, duration: this.textFadeInTime).SetEase(ease: Ease.Linear);

			});



			this.currentSequence.Play();

			// To be safe, reset the state when everything is done.
			GameController.Instance?.WaitThenRun(timeToWait: 10f, action: delegate {
				this.ResetState();
				// Note that maybe i could do this in an OnComplete if it becomes an issue?
			});

		}
		#endregion

	}


}