using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// The representation of a ShuffleCard in the Shuffle Time scene.
	/// </summary>
	public class ShuffleCardSprite : MonoBehaviour {

		#region FIELDS - STATE : LOGIC
		/// <summary>
		/// The shuffle card that is attached to this card sprite.
		/// </summary>
		public ShuffleCard ShuffleCard { get; private set; }
		/// <summary>
		/// The table this card sprite currently belongs to.
		/// I need this so I can send it events and junk.
		/// </summary>
		private ShuffleTimeTable Table { get; set; }
		/// <summary>
		/// Is this card showing its face?
		/// </summary>
		public bool IsShowing { get; private set; } = false;
		#endregion

		#region FIELDS - STATE : ANIMATION
		/// <summary>
		/// Is this card playing the "flow" animation?
		/// </summary>
		[SerializeField]
		private bool isFlowing = false;
		/// <summary>
		/// The sequence for this card when its being highlighted.
		/// </summary>
		private Sequence highlightSequence;
		#endregion

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The noise to apply to the shake pivot at each frame.
		/// </summary>
		[Title("General")]
		[TabGroup("Sprite","Toggles"), SerializeField]
		private NoiseSettings noiseSettings;
		/// <summary>
		/// The amount to "offset" the noise's GetValue() function.
		/// Good for multiple cards.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private Vector3 noiseOffset = new Vector3();
		#endregion

		#region FIELDS - TOGGLES : COLORS
		/// <summary>
		/// The color to use for the highlight sprite renderer.
		/// </summary>
		[Title("Color")]
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private Color highlightColor = new Color(1f, 1f, 1f, 0.5f);
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to take when flipping the card.
		/// </summary>
		[Title("Timing")]
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private float flipTime = 0.2f;
		/// <summary>
		/// The amount of time to take when highlighting the card on.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private float highlightInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when highlighting the card off.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private float highlightOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take between highlights.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private float highlightPauseTime = 0.5f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease to use when flipping in.
		/// </summary>
		[Title("Easing")]
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private Ease cardFlipEase = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when highlighting a card on.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private Ease highlightInEase = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when highlighting a card out.
		/// </summary>
		[TabGroup("Sprite", "Toggles"), SerializeField]
		private Ease highlightOutEase = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES : PIVOTS
		/// <summary>
		/// All of the objects for this sprite.
		/// </summary>
		[Title("Anchors")]
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// Contains all of the visuals for this shuffle card sprite.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private GameObject allVisuals;
		/// <summary>
		/// The transform for the outer pivot.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private Transform mainPivot1;
		/// <summary>
		/// The transform for the inner pivot.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private Transform mainPivot2;
		/// <summary>
		/// The transform for the shake pivot.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private Transform shakePivot1;
		/// <summary>
		/// The transform for the flip pivot.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private Transform flipPivot1;
		#endregion

		#region FIELDS - SCENE REFERENCES : OTHER
		/// <summary>
		/// The selectable for this card sprite.
		/// </summary>
		[Title("Other")]
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private Selectable selectable;
		/// <summary>
		/// The selectable for this card sprite.
		/// </summary>
		public Selectable Selectable {
			get {
				return this.selectable;
			}
		}
		/// <summary>
		/// The sprite renderer that shows this card's graphic.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private SpriteRenderer frontSpriteRenderer;
		/// <summary>
		/// The sprite renderer for the back of the card.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private SpriteRenderer backSpriteRenderer;
		/// <summary>
		/// A nasty trick I'm using where I just tween a fake sprite renderer to simulate highlighting.
		/// Fuck it.
		/// </summary>
		[TabGroup("Sprite", "Scene References"), SerializeField]
		private SpriteRenderer backSpriteHighlightRenderer;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The sprite renderer of the face that is currently aimed at the camera.
		/// </summary>
		private SpriteRenderer CurrentFaceSpriteRenderer {
			get {
				return (this.IsShowing == true) ? this.frontSpriteRenderer : this.backSpriteRenderer;
			}
		}
		/// <summary>
		/// The sprite renderer of the face that is currently aimed away from camera.
		/// </summary>
		private SpriteRenderer CurrentHiddenSpriteRenderer {
			get {
				return (this.IsShowing == false) ? this.frontSpriteRenderer : this.backSpriteRenderer;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Update() {
			// If flowing, move the pivot around.
			if (this.isFlowing == true) {
				Vector3 positionOffset = this.noiseSettings.PositionNoise[0].GetValueAt(time: Time.time, timeOffsets: this.noiseOffset);
				Vector3 rotationOffset = this.noiseSettings.OrientationNoise[0].GetValueAt(time: Time.time, timeOffsets: this.noiseOffset);
				this.shakePivot1.localPosition = positionOffset;
				this.shakePivot1.localEulerAngles = rotationOffset;
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Resets the state of the card to its initial settings.
		/// </summary>
		public void ResetState() {

			this.ShuffleCard = null;
			this.Table = null;
			this.Dehighlight();
		
			this.shakePivot1.localPosition = Vector3.zero;
			this.shakePivot1.localEulerAngles = Vector3.zero;

			// Flip Back also resets Is Showing to false.
			this.FlipBack(instant: true);

			// Turn the selectable off. For redundancy.
			this.ActivateSpriteSelectable(false);

			// Turn off all the rest of the objects.
			this.allObjects.SetActive(false);
		}
		/// <summary>
		/// Preps this card sprite to be used with the specified shuffle card.
		/// </summary>
		/// <param name="shuffleCard">The shuffle card to use for this sprite.</param>
		/// <param name="table">The table that is preparing this card.</param>
		public ShuffleCardSprite Prepare(ShuffleCard shuffleCard, ShuffleTimeTable table) {

			// Turn the visuals on.
			this.allObjects.SetActive(true);

			// Remember the card.
			this.ShuffleCard = shuffleCard;
			// Also remember the table.
			this.Table = table;
			// Set the sprite.
			this.frontSpriteRenderer.sprite = shuffleCard.Template.CardSprite;

			// Set the "is showing" variable to whatever the table has in store for this card.
			bool isShowing = table.GetDefaultCardFace(shuffleCard: shuffleCard);
			this.Flip(show: isShowing, instant: true);
			
			// Return this sprite, because I use it sometimes.
			return this;
			
		}
		#endregion

		#region SELECTABLES
		/// <summary>
		/// Sets whether this card is selectable by Unity's event system.
		/// </summary>
		/// <param name="status">Whether this card should be selectable or not.</param>
		public void ActivateSpriteSelectable(bool status) {
			this.selectable.interactable = status;
			this.selectable.gameObject.SetActive(status);
		}
		#endregion

		#region ANIMATIONS - ATTENTION
		/// <summary>
		/// Highlights the card sprite to show its being focused.
		/// </summary>
		[Button, HideInEditorMode]
		public void Highlight() {

			// I might want to call Dehighlight just so I can confirm the state was reset.
			this.Dehighlight();

			// Snap the highlighter on because that gives immediate feedback.
			this.backSpriteHighlightRenderer.color = this.highlightColor;

			// Anyway, make the sequence.
			this.highlightSequence = DOTween.Sequence();
			this.highlightSequence.AppendInterval(interval: 0.02f);
			this.highlightSequence.Append(this.backSpriteHighlightRenderer.DOColor(endValue: Color.clear, duration: this.highlightOutTime).SetEase(ease: this.highlightOutEase));
			this.highlightSequence.Append(this.backSpriteHighlightRenderer.DOColor(endValue: this.highlightColor, duration: this.highlightInTime).SetEase(ease: this.highlightInEase));
			this.highlightSequence.AppendInterval(this.highlightPauseTime);
			this.highlightSequence.SetLoops(-1);
			this.highlightSequence.Play();
			
		}
		/// <summary>
		/// Dehighlights the card sprite to dismiss focus.
		/// </summary>
		[Button, HideInEditorMode]
		public void Dehighlight() {
			this.backSpriteHighlightRenderer.DOKill(complete: true);
			this.highlightSequence?.Kill(complete: true);
			this.backSpriteHighlightRenderer.color = Color.clear;
		}
		#endregion

		#region ANIMATIONS - ATTENTION
		/// <summary>
		/// Fades the card out.
		/// I call it pseudo fade becasue I kinda hack it and assume one face needs to be facing away.
		/// </summary>
		public void PseudoFade() {
			// turn off the renderer not shown to the player
			// fade out the one that is.
			throw new System.NotImplementedException("");
		}
		#endregion

		#region ANIMATIONS - MOTION
		/// <summary>
		/// Flips the card to be shown or not.
		/// </summary>
		/// <param name="instant">Should this flip be animated?</param>
		[Button, HideInEditorMode]
		public void Flip(bool instant = false) {
			this.Flip(
				show: !this.IsShowing, 
				instant: instant);
		}
		/// <summary>
		/// Flips the card to be displaying to the specified setting.
		/// </summary>
		/// <param name="show">Whether the card should be showing or not.</param>
		/// <param name="instant">Whether the flip is instantaneous or not.</param>
		public void Flip(bool show, bool instant = false) {
			this.Flip(show: show, time: instant ? 0f : this.flipTime);		
		}
		/// <summary>
		/// Flips the card to be displaying its front.
		/// Does nothing if currently showing the front.
		/// </summary>
		/// <param name="instant">Should this flip be animated?</param>
		public void FlipFront(bool instant = false) {
			if (this.IsShowing == false) {
				this.Flip(instant: instant);
			}
		}
		/// <summary>
		/// Flips the card to be displaying its back.
		/// Does nothing if currently showing the back.
		/// </summary>
		/// <param name="instant">Should this flip be animated?</param>
		public void FlipBack(bool instant = false) {
			if (this.IsShowing == true) {
				this.Flip(instant: instant);
			}
		}
		/// <summary>
		/// Flips the card to be shown or not.
		/// Also sets the IsShowing variable.
		/// </summary>
		/// <param name="show">The face of the card to show.</param>
		/// <param name="time">The amount of time to take to flip it.</param>
		private void Flip(bool show, float time) {
			// Only flip if the option specified is not the current one.
			if (this.IsShowing != show) {
				Debug.Log("FLIPPING " + this.gameObject.name + " TO " + show, context: this.gameObject);
				this.IsShowing = show;
				this.PlayFlipAnimation(show: show, time: time);
			}
		}
		/// <summary>
		/// Plays the animation that
		/// </summary>
		/// <param name="show"></param>
		/// <param name="time"></param>
		private void PlayFlipAnimation(bool show, float time) {
			Vector3 targetRotation = show == true ? Vector3.zero : new Vector3(0f, 180f);
			if (time != 0f) {
				this.flipPivot1.DOLocalRotate(
					endValue: targetRotation,
					duration: time, mode: RotateMode.Fast)
					.SetEase(ease: this.cardFlipEase);
			} else {
				this.flipPivot1.localEulerAngles = targetRotation;
			}
		}
		#endregion

		#region EVENT INTERFACE IMPLEMENTATION
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnCancel(BaseEventData eventData) {
			this.Table.CancelRequested(sprite: this);
		}
		public void OnSubmit(BaseEventData eventData) {
			Debug.Log("Submit hit on " + this.gameObject.name, this.gameObject);
			this.Table.CardPicked(sprite: this);
		}
		public void OnDeselect(BaseEventData eventData) {
			this.Dehighlight();
		}
		public void OnMove(BaseEventData eventData) {
			AxisEventData axisData = eventData as AxisEventData;
			Debug.Log("Move hit on " + this.gameObject.name, this.gameObject);
		}
		#endregion

	}


}