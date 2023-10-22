using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.Menus.Input {

	/// <summary>
	/// Contains a basic set of letters that can be sent over to an InputEntryField.
	/// </summary>
	public class StandardInputBox : InputBox {

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time it takes for the front to tween in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), Title("Timing"), SerializeField]
		private float frontTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time it takes for the backing to tween in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"),  SerializeField]
		private float backingTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time it takes for the backing to tween in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private float frontTweenOutTime = 0.5f;
		/// <summary>
		/// The amount of time it takes for the backing to tween in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private float backingTweenOutTime = 0.5f;
		#endregion

		#region FIELDS - TOGGLES : ROTATION
		/// <summary>
		/// The rotation the front rectangle should be in before it gets tweened in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), Title("Rotation"), SerializeField]
		private float frontInitialRotation = 0f;
		/// <summary>
		/// The rotation the back rectangle should be in before it gets tweened in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"),SerializeField]
		private float backingInitialRotation = 0f;
		/// <summary>
		/// The rotation the front rectangle should be in before it gets tweened in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"),SerializeField]
		private float frontFinalRotation = 0f;
		/// <summary>
		/// The rotation the back rectangle should be in before it gets tweened in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private float backingFinalRotation = 0f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease to use when tweening the front in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), Title("Easing"), SerializeField]
		private Ease frontEaseInType = Ease.InOutQuint;
		/// <summary>
		/// The ease to use when tweening the backing in.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private Ease backingEaseInType = Ease.InOutQuint;
		/// <summary>
		/// The ease to use when tweening the backing out.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private Ease frontEaseOutType = Ease.InOutQuint;
		/// <summary>
		/// The ease to use when tweening the front out.
		/// </summary>
		[TabGroup("Input Box", "Toggles"), SerializeField]
		private Ease backingEaseOutType = Ease.InOutQuint;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The front rectangle's rect transform.
		/// </summary>
		[TabGroup("Input Box", "Scene References"), Title("Rect Transforms"), SerializeField]
		private RectTransform frontRectTransform;
		/// <summary>
		/// The Backing's rect transform.
		/// </summary>
		[TabGroup("Input Box", "Scene References"), SerializeField]
		private RectTransform backingRectTransform;
		/// <summary>
		/// The rect transform containing all the input letters.
		/// </summary>
		[TabGroup("Input Box", "Scene References"), SerializeField]
		private RectTransform inputLettersGroupRectTransform;
		/// <summary>
		/// The "fake" image I use for the front to give the illusion of the text fading in.
		/// </summary>
		[TabGroup("Input Box", "Scene References"), Title("Images"), SerializeField]
		private Image fakeFrontImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// I have the game object inactive in the editor by default for the sake of making things look clear in design time.
			this.fakeFrontImage.gameObject.SetActive(true);
		}
		private void Start() {
			// Call reset to clear the visuals.
			this.ResetInputBox();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Requests input from this input box.
		/// This basically means turning everything on.
		/// </summary>
		public override void RequestInput() {
			// Enable all the letters.
			this.EnableAllLetters();
			// Set the current selected game object as the first input letter.
			EventSystem.current.SetSelectedGameObject(this.InputLetters.First().gameObject);
		}
		/// <summary>
		/// Tells this input box that it's no longer needed and it should skidaddle.
		/// </summary>
		public override void FinishInput() {
			// Note that when I disable letters, each letter checks if it's the currently selected game object.
			// It will tell the event system to knock if off if it is.
			this.DisableAllLetters();
		}
		/// <summary>
		/// When the length limit is reached, snap to the OK button.
		/// </summary>
		public override void LengthLimitReached() {
			EventSystem.current.SetSelectedGameObject(this.InputLetters.First(l => l is ConfirmInputBoxLetter).gameObject);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Totally resets the visuals of the input box.
		/// </summary>
		public override void ResetInputBox() {
			// Set the initial rotations/scales.
			this.frontRectTransform.transform.localEulerAngles = new Vector3(0f, 0f, this.frontInitialRotation);
			this.fakeFrontImage.transform.localEulerAngles = new Vector3(0f, 0f, this.frontInitialRotation);
			this.backingRectTransform.transform.localEulerAngles = new Vector3(0f, 0f, this.backingInitialRotation);
			this.inputLettersGroupRectTransform.localEulerAngles = new Vector3(0f, 0f, this.frontInitialRotation);
			this.frontRectTransform.transform.localScale = Vector3.zero;
			this.fakeFrontImage.transform.localScale = Vector3.zero;
			this.backingRectTransform.transform.localScale = Vector3.zero;
			this.inputLettersGroupRectTransform.transform.localScale = Vector3.zero;
			// Also set the fake front image to black.
			this.fakeFrontImage.color = Color.black;
		}
		/// <summary>
		/// Plays the animation that shows the input box.
		/// </summary>
		[Button]
		public override void PresentInputBox() {
			// Kill any tweens currently running on the elements.
			this.frontRectTransform.DOKill(complete: true);
			this.backingRectTransform.DOKill(complete: true);
			this.fakeFrontImage.GetComponent<RectTransform>().DOKill(complete: true);
			this.fakeFrontImage.DOKill(complete: true);
			this.inputLettersGroupRectTransform.DOKill(complete: true);

			// Reset.
			this.ResetInputBox();

			// Tween the scale.
			this.frontRectTransform.DOScale(
				endValue: 1f, 
				duration: this.frontTweenInTime)
				.SetEase(ease: this.frontEaseInType);

			this.backingRectTransform.DOScale(
				endValue: 1f,
				duration: this.backingTweenInTime)
				.SetEase(ease: this.backingEaseInType);

			this.inputLettersGroupRectTransform.DOScale(
				endValue: 1f,
				duration: this.frontTweenInTime)
				.SetEase(ease: this.frontEaseInType);

			// Tween the rotation.
			this.frontRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontFinalRotation), 
				duration: this.frontTweenInTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseInType);

			this.backingRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.backingFinalRotation),
				duration: this.backingTweenInTime,
				mode: RotateMode.FastBeyond360)
				.SetEase(ease: this.backingEaseInType);

			this.inputLettersGroupRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontFinalRotation),
				duration: this.frontTweenInTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseInType);

			// Make sure to tween the rotation/scale on the fake rectangle as well.
			this.fakeFrontImage.GetComponent<RectTransform>().DOScale(
				endValue: 1f,
				duration: this.frontTweenInTime)
				.SetEase(ease: this.frontEaseInType);

			this.fakeFrontImage.GetComponent<RectTransform>().DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontFinalRotation),
				duration: this.frontTweenInTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseInType);

			// Finally, the color. I am so fucking tired.
			this.fakeFrontImage.DOColor(
				endValue: Color.clear,
				duration: this.frontTweenInTime)
				.SetEase(ease: Ease.Linear);
		}
		/// <summary>
		/// Plays the animation that dismisses the input box.
		/// </summary>
		[Button]
		public override void DismissInputBox() {
			this.frontRectTransform.DOKill(complete: true);
			this.backingRectTransform.DOKill(complete: true);
			this.fakeFrontImage.GetComponent<RectTransform>().DOKill(complete: true);
			this.fakeFrontImage.DOKill(complete: true);
			this.inputLettersGroupRectTransform.DOKill(complete: true);

			// Tween the scale.
			this.frontRectTransform.DOScale(
				endValue: 0f,
				duration: this.frontTweenOutTime)
				.SetEase(ease: this.frontEaseOutType);

			this.backingRectTransform.DOScale(
				endValue: 0f,
				duration: this.backingTweenOutTime)
				.SetEase(ease: this.backingEaseOutType);

			this.inputLettersGroupRectTransform.DOScale(
				endValue: 0f,
				duration: this.frontTweenOutTime)
				.SetEase(ease: this.frontEaseOutType);

			// Tween the rotation.
			this.frontRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontInitialRotation),
				duration: this.frontTweenOutTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseOutType);

			this.backingRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.backingInitialRotation),
				duration: this.backingTweenOutTime,
				mode: RotateMode.FastBeyond360)
				.SetEase(ease: this.backingEaseOutType);

			this.inputLettersGroupRectTransform.DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontInitialRotation),
				duration: this.frontTweenOutTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseOutType);

			// Make sure to tween the rotation/scale on the fake rectangle as well.
			this.fakeFrontImage.GetComponent<RectTransform>().DOScale(
				endValue: 0f,
				duration: this.frontTweenOutTime)
				.SetEase(ease: this.frontEaseOutType);

			this.fakeFrontImage.GetComponent<RectTransform>().DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: this.frontInitialRotation),
				duration: this.frontTweenOutTime,
				mode: RotateMode.Fast)
				.SetEase(ease: this.frontEaseOutType);

		}
		#endregion

		#region CASING
		/// <summary>
		/// Toggles the case of all the standard box letters.
		/// </summary>
		public void ToggleCase() {
			Debug.Log("TOGGLING CASE. CONSIDER MAKING A CACHE PLEASE FOR THE LOVE OF GOD.");
			this.InputLetters
				.Where(l => l is StandardInputBoxLetter)
				.Cast<StandardInputBoxLetter>()
				.ToList()
				.ForEach(l => l.ToggleCase());
		}
		#endregion

	}

}