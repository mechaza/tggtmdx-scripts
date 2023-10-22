using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.UI {

	/// <summary>
	/// This is what gets used to highlight and build the settings on the right of the screen.
	/// I think.
	/// </summary>
	public class SettingsMenuDXTopLevelButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler, ISubmitHandler, IMoveHandler {

		#region FIELDS - TOGGLES : METADATA
		/// <summary>
		/// The category type for this top level button.
		/// Helps decide things like that color it should be.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private GameToggleCategoryType categoryType = GameToggleCategoryType.None;
		/// <summary>
		/// The category type for this top level button.
		/// Helps decide things like that color it should be.
		/// </summary>
		public GameToggleCategoryType CategoryType {
			get {
				return this.categoryType;
			}
		}
		/// <summary>
		/// The string that should be printed on the button.
		/// </summary>
		[SerializeField]
		private string labelName = "";
		#endregion

		#region FIELDS - TOGGLES : TWEENING
		/// <summary>
		/// The position this button should be in when it's actively highlighted.
		/// </summary>
		[SerializeField, Title("Tweening")]
		private Vector2Int activeHighlightPosition = new Vector2Int();
		/// <summary>
		/// The position this button should be in when it's passively highlighted.
		/// </summary>
		[SerializeField]
		private Vector2Int passiveHighlightPosition = new Vector2Int();
		/// <summary>
		/// The position the button should be in when just being standard.
		/// </summary>
		[SerializeField]
		private Vector2Int standardPosition = new Vector2Int();
		/// <summary>
		/// The position this button should be in when its in hiding.
		/// Like. Not on the screen at all.
		/// </summary>
		[SerializeField]
		private Vector2Int hidingPosition = new Vector2Int();
		/// <summary>
		/// The amount of time to take when tweening to passive from hiding.
		/// </summary>
		[SerializeField]
		private float tweenInFromHidingTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening to passive to hiding.
		/// </summary>
		[SerializeField]
		private float tweenOutToHidingTime = 0.3f;
		/// <summary>
		/// The amount of time to take when tweening to active from passive.
		/// </summary>
		[SerializeField]
		private float tweenToActiveTime = 0.1f;
		/// <summary>
		/// The amount of time to take when tweening to passive from active.
		/// </summary>
		[SerializeField]
		private float tweenToPassiveTime = 0.1f;
		/// <summary>
		/// The amount of time to take when tweening from active to passive.
		/// </summary>
		[SerializeField]
		private float tweenBackTime = 0.1f;
		/// <summary>
		/// The easing to use when tweening in. From wherever.
		/// </summary>
		[SerializeField]
		private Ease tweenInEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening out. From wherever.
		/// </summary>
		[SerializeField]
		private Ease tweenOutEaseType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The recttransform for all of the visuals.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private RectTransform allVisuals;
		/// <summary>
		/// The label displaying what setting this is.
		/// </summary>
		[SerializeField]
		private SuperTextMesh categoryLabel;
		/// <summary>
		/// The image that can change color if its highlighted or not.
		/// </summary>
		[SerializeField]
		private Image backingImage;
		/// <summary>
		/// The selectable attached to this button.
		/// I mostly need it so I can check if there is anything around it when OnMove is called.
		/// </summary>
		private Selectable selectable;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.selectable = this.GetComponent<Selectable>();
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Totally resests the state of this button.
		/// </summary>
		public void ResetState() {
			// Snap the button to its hiding position.
			this.allVisuals.anchoredPosition = this.hidingPosition;
		}
		/// <summary>
		/// Presents this button onto the screen from its hiding position.
		/// </summary>
		public void Present() {
			// Kill any tweens currently on this button.
			this.allVisuals.DOKill(complete: true);
			// Tween to the passive position.
			this.allVisuals.DOAnchorPos(this.standardPosition, this.tweenInFromHidingTime, true).SetEase(this.tweenInEaseType);
		}
		/// <summary>
		/// Dismisses this button from the screen to its hiding position.
		/// </summary>
		public void Dismiss() {
			// whatever
			this.allVisuals.DOKill(complete: true);
			this.allVisuals.DOAnchorPos(this.hidingPosition, duration: this.tweenOutToHidingTime, true).SetEase(this.tweenOutEaseType);

			// Also make sure I turn the colors back.
			this.backingImage.color = Color.white;
			this.categoryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.categoryLabel.Text = "<c=black>" + this.labelName;

		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Changes the color so that it's obvious this is the button currently being highlighted.
		/// </summary>
		/// <param name="categoryType"></param>
		private void ActiveHighlight(GameToggleCategoryType categoryType) {

			// Kill any tweens currently on this button.
			this.allVisuals.DOKill(complete: true);

			// Tween it into the active position.
			this.allVisuals.DOAnchorPos(this.activeHighlightPosition, this.tweenToActiveTime, true).SetEase(this.tweenInEaseType);

			// Change the colors on this button.
			this.backingImage.color = GrawlyColors.GetColorFromToggleCategory(categoryType: this.CategoryType);
			this.categoryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.categoryLabel.Text = "<c=white>" + this.labelName;
		}
		/// <summary>
		/// Changes the color so that the control looks like it was transfered to the menu list,
		/// but this is the category being represented.
		/// </summary>
		/// <param name="categoryType"></param>
		private void PassiveHighlight(GameToggleCategoryType categoryType) {

			// Kill any tweens currently on this button.
			this.allVisuals.DOKill(complete: true);

			// Tween it to the passive highlight position.
			this.allVisuals.DOAnchorPos(this.passiveHighlightPosition, this.tweenToPassiveTime, true).SetEase(this.tweenInEaseType);

			// The color should be multiplied by gray to make it seem a bit dimmed.
			this.backingImage.color = GrawlyColors.GetColorFromToggleCategory(categoryType: this.CategoryType) * Color.gray;
			this.categoryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.categoryLabel.Text = "<c=white>" + this.labelName;
		}
		/// <summary>
		/// Just resets this button to its default state.
		/// </summary>
		private void Dehighlight() {

			// Kill any tweens currently on this button.
			this.allVisuals.DOKill(complete: true);

			// Tween it to its standard position.
			this.allVisuals.DOAnchorPos(this.standardPosition, this.tweenBackTime, true).SetEase(this.tweenOutEaseType);

			// Do the standard dehighlighting.
			this.backingImage.color = Color.white;
			this.categoryLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.categoryLabel.Text = "<c=black>" + this.labelName;
		}
		
		#endregion

		#region EVENT SYSTEM IMPLEMENTATIONS
		public void OnCancel(BaseEventData eventData) {
			SettingsMenuControllerDX.instance.TopLevelCancelled();
		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
		}
		public void OnSelect(BaseEventData eventData) {
			// Highlight this button. Actively.
			this.ActiveHighlight(categoryType: this.categoryType);
			// Tell the settings menu controller to preview the toggles from this category.
			SettingsMenuControllerDX.instance.PreviewToggles(categoryType: this.categoryType);
		}
		public void OnSubmit(BaseEventData eventData) {
			
			if (this.categoryType == GameToggleCategoryType.None) {
				Debug.Log("Toggle Category Of None Hit");
				// If this is a None type I'm probably just gonna do an event trigger.
				return;
			}

			// If the settings menu controller has no toggles for this category, just back out.
			if (SettingsMenuControllerDX.instance.GetToggleCount(this.CategoryType) == 0) {
				Debug.Log("No toggles!");
				AudioController.instance?.PlaySFX(type: SFXType.Invalid);
				return;
			}

			// Passive highlight this category type.
			this.PassiveHighlight(categoryType: this.categoryType);

			// Tell the settings menu controller that this was picked.
			SettingsMenuControllerDX.instance.SubmittedToggleCategoryButton(categoryType: this.CategoryType);

		}
		public void OnMove(AxisEventData eventData) {

			// Alright this is kinda fucked up because I can't do Deselect with the wierd logic involved.
			// I may need to Dehighlight or PassiveHighlight based on where I'm going.
			if (this.selectable.FindSelectableOnUp() != null && eventData.moveDir == MoveDirection.Up) {
				this.Dehighlight();
			} else if (this.selectable.FindSelectableOnDown() != null && eventData.moveDir == MoveDirection.Down) {
				this.Dehighlight();
			}

		}
		#endregion

	}


}