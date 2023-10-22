using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Grawly.Chat;

namespace Grawly.Menus.NameEntry {

	/// <summary>
	/// The advance button on the name entry screen.
	/// </summary>
	public class NameEntryAdvanceButton : ChatBoxAdvanceButton {

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// The amount of time to complete one loop of the advance button animation.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Advance", "Toggles")]
		private float advanceButtonAnimationTime = 0.4f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The ease to use when animating the advance button.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Advance", "Toggles")]
		private Ease advanceButtonAnimationEaseType = Ease.OutCirc;
		#endregion

		#region FIELDS - POSITIONING
		/// <summary>
		/// The position the child pivot on the advance button should be when starting the animation.
		/// </summary>
		[Title("Animation")]
		[SerializeField, TabGroup("Advance", "Position")]
		private Vector2 advanceButtonChildPivotInitialPosition;
		/// <summary>
		/// The position the child pivot on the advance button should be at the end of the animation.
		/// </summary>
		[SerializeField, TabGroup("Advance", "Position")]
		private Vector2 advanceButtonChildPivotFinalPosition;
		#endregion

		#region FIELDS - SCENE REFERENCES : SELECTABLES
		/// <summary>
		/// The Selectable that accepts events. Is part of its own object separate from the visuals so as to not confuse the two.
		/// </summary>
		[Title("Selectables")]
		[SerializeField, TabGroup("Advance", "Scene References")]
		private Selectable selectable;
		#endregion

		#region FIELDS - SCENE REFERENCES : RECT TRANSFORMS
		/// <summary>
		/// The main pivot for the advance button.
		/// </summary>
		[Title("RectTransforms")]
		[SerializeField, TabGroup("Advance", "Scene References")]
		private RectTransform advanceButtonMainPivotRectTransform;
		/// <summary>
		/// The main pivot for the advance button.
		/// </summary>
		[SerializeField, TabGroup("Advance", "Scene References")]
		private RectTransform advanceButtonChildPivotRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image that is a child to the child pivot.
		/// This is separate from the child pivot because that's using DOTween for a loop.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Advance", "Scene References")]
		private GameObject advanceButtonImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			// On start, set the animation on the advance button and have it loop. 
			this.advanceButtonChildPivotRectTransform.DOAnchorPos(
				endValue: this.advanceButtonChildPivotFinalPosition,
				duration: this.advanceButtonAnimationTime,
				snapping: true)
				.SetEase(ease: this.advanceButtonAnimationEaseType)
				.SetLoops(loops: -1)
				.OnComplete(delegate {
					// Upon completion of each loop, reset it back to its initial position.
					this.advanceButtonChildPivotRectTransform.anchoredPosition = this.advanceButtonChildPivotInitialPosition;
				});
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Sets whether or not the advance button can be selected.
		/// This is different from whether or not it is visible.
		/// </summary>
		/// <param name="status">Whether or not the button can be selected.</param>
		public override void SetSelectable(bool status) {

			// Just set the selectable's gameobject to be the same status.
			this.selectable.gameObject.SetActive(status);

			// May also take this opprotunity to set its selectability.

			if (status == true) {
				Debug.Log("Setting AdvanceButton's selectable to ACTIVE. Also telling the event system to select it.");
				EventSystem.current.SetSelectedGameObject(this.selectable.gameObject);
			} else {
				Debug.Log("Setting AdvanceButton's selectable to DISABLED. Also telling the event system to null out the current selection.");
				EventSystem.current.SetSelectedGameObject(null);
			}


		}
		/// <summary>
		/// Sets whether or not the advance button is visible.
		/// This is different from whether or not it is selectable.
		/// </summary>
		/// <param name="status">Whether or not the button is visible.</param>
		public override void SetVisible(bool status) {
			this.advanceButtonImage.gameObject.SetActive(status);
		}
		/// <summary>
		/// Sets the color of the button.
		/// </summary>
		/// <param name="color">The color to set the button to.</param>
		public override void SetColor(Color color) {
			// this.advanceButtonImage.GetComponent<Image>().DOKill(complete: true);
			// this.advanceButtonImage.GetComponent<Image>().DOColor(endValue: color, duration: 0.5f);
			this.advanceButtonImage.GetComponent<Image>().color = color;
		}
		#endregion

		#region MAIN CALLS - STATE
		/// <summary>
		/// Gets called when a Submit event is sent to the advance button.
		/// </summary>
		protected override void ButtonHit() {
			// Only move forward if the selectable is active.
			if (this.selectable.gameObject.activeInHierarchy == true) {
				// Send this event back up to the chatcontroller.
				ChatControllerDX.Instance.AdvanceButtonHit();
			}
		}
		#endregion

	}


}