using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.Chat {

	/// <summary>
	/// The "standard" way I want to build a chat option item.
	/// Actually this is the radial version so it's not exactly standard.
	/// </summary>
	public class StandardChatOptionItem : ChatOptionItem {

		#region FIELDS - TOGGLES : TIME
		/// <summary>
		/// The amount of time to take when tweening in the main pivot.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Option Item", "Toggles")]
		private float mainPivotTweenInTime = 0.2f;
		/// <summary>
		/// The amount of time to take when tweening out the main pivot.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Toggles")]
		private float mainPivotTweenOutTime = 0.2f;
		#endregion

		#region FIELDS - TOGGLES : EASING
		/// <summary>
		/// The amount of time to take when tweening in the main pivot.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Option Item", "Toggles")]
		private Ease mainPivotTweenInEaseType = Ease.OutBack;
		/// <summary>
		/// The amount of time to take when tweening out the main pivot.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Toggles")]
		private Ease mainPivotTweenOutEaseType = Ease.OutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES : SELECTABLES
		/// <summary>
		/// The Selectable for this item. I need this exposed so I can. Well, select it.
		/// </summary>
		[Title("Selectables")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		public Selectable Selectable { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : RECTTRANSFORMS
		/// <summary>
		/// The RectTransform for the main pivot.
		/// </summary>
		[Title("RectTransforms")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The RectTransform for the child pivot.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private RectTransform childPivotRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image for the front of the option.
		/// </summary>
		[Title("Images")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private Image optionBackingFrontImage;
		/// <summary>
		/// The image for the dropshadow of the option.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private Image optionBackingDropshadowImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TEXT
		/// <summary>
		/// The SuperTextMesh that shows this option.
		/// </summary>
		[Title("Text")]
		[SerializeField, TabGroup("Option Item", "Scene References")]
		private SuperTextMesh optionTextLabel;
		#endregion

		#region FIELDS - POSITIONS
		/// <summary>
		/// The position of the main anchor when the item is active.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Option Item", "Positions")]
		private Vector2 mainAnchorActivePosition = new Vector2();
		/// <summary>
		/// The position of the main anchor when the item is being hidden.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Positions")]
		private Vector2 mainAnchorHidingPosition = new Vector2();
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the state of the option item.
		/// </summary>
		protected override void ResetState() {
			// Totally reset the scale.
			this.mainPivotRectTransform.localScale = Vector3.zero;
			// Move the pivot back to the hiding position.
			this.mainPivotRectTransform.anchoredPosition = this.mainAnchorHidingPosition;
			// Set the option label text back to the default.
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			// Also reset the text itself.
			this.optionTextLabel.Text = "";
		}
		/// <summary>
		/// Builds the option item with the parameters passed in.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		public override void Build(ChatOptionItemParams optionItemParams) {

			// Save a reference to the item parameters.
			this.optionItemParams = optionItemParams;

			// Set the graphics to be the dehighlighted version.
			this.Dehighlight(optionItemParams: optionItemParams);

			// Tween in the scale.
			this.mainPivotRectTransform.DOScale(
				endValue: 1f, 
				duration: this.mainPivotTweenInTime)
				.SetEase(ease: this.mainPivotTweenInEaseType);

			// Also tween in the position.
			this.mainPivotRectTransform.DOAnchorPos(
				endValue: this.mainAnchorActivePosition,
				duration: this.mainPivotTweenInTime,
				snapping: true)
				.SetEase(ease: this.mainPivotTweenInEaseType);

		}
		/// <summary>
		/// Dismisses this specific option.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Dismiss(ChatOptionItemParams optionItemParams) {

			// Tween out the scale.
			this.mainPivotRectTransform.DOScale(
				endValue: 0f,
				duration: this.mainPivotTweenOutTime)
				.SetEase(ease: this.mainPivotTweenOutEaseType);

			// Also tween out the position.
			this.mainPivotRectTransform.DOAnchorPos(
				endValue: this.mainAnchorHidingPosition,
				duration: this.mainPivotTweenOutTime,
				snapping: true)
				.SetEase(ease: this.mainPivotTweenOutEaseType);

		}
		/// <summary>
		/// Gets called when this is the option that was picked.
		/// </summary>
		/// <param name="optionItemParams">The parameters that were used when constructing this option item.</param>
		protected override void PickedOption(ChatOptionItemParams optionItemParams) {
			// Make sure the event system knows to null out this option.
			EventSystem.current.SetSelectedGameObject(null);
			// Upon picking the item, just tell the picker to do something about it.
			optionItemParams.optionPicker.PickedOption(
				optionItemParams: optionItemParams,
				optionItem: this);
		}
		#endregion

		#region GRAPHICS
		/// <summary>
		/// Sets the graphics on the item to look like it has been highlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Highlight(ChatOptionItemParams optionItemParams) {
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.optionTextLabel.Text = "<c=white>" + optionItemParams.optionLabelText;
			this.optionBackingFrontImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];
			this.optionBackingDropshadowImage.color = Color.white;
			this.mainPivotRectTransform.DOScale(endValue: 1.2f, duration: 0f);

			// Also play a sound effect.
			AudioController.instance?.PlaySFX(SFXType.Hover, scale: 1f);

		}
		/// <summary>
		/// Sets the graphics on the item to look like it has been dehighlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected override void Dehighlight(ChatOptionItemParams optionItemParams) {
			this.optionTextLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			this.optionTextLabel.Text = optionItemParams.optionLabelText;
			this.optionBackingFrontImage.color = Color.white;
			this.optionBackingDropshadowImage.color = Color.black;
			this.mainPivotRectTransform.DOScale(endValue: 1f, duration: 0f);
		}
		#endregion

	}


}