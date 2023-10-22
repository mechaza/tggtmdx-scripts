using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Grawly.Menus {
	
	/// <summary>
	/// Represents a game preset in the scene.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class GamePresetThumbnail : MonoBehaviour, ISubmitHandler, ICancelHandler, ISelectHandler, IDeselectHandler {


		
		#region FIELDS - STATE
		/// <summary>
		/// The preset template that was used to prepare this thumbnail.
		/// </summary>
		private GamePresetTemplate presetTemplate;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image that should be manipulated when this thumbnail is highlighted or not.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private Image backingImage;
		/// <summary>
		/// The image that shows the preview screenshot.
		/// </summary>
		[SerializeField]
		private Image screenshotImage;
		/// <summary>
		/// The STM for the preset name label.
		/// </summary>
		[SerializeField]
		private SuperTextMesh presetNameLabel;
		/// <summary>
		/// The selectable attached to this thumbnail.
		/// </summary>
		private Selectable selectable;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Grab the selectable.
			this.selectable = this.GetComponent<Selectable>();
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Resets the state of the thumbnail.
		/// </summary>
		public void ResetState() {
			this.selectable.interactable = false;
			this.backingImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red] * Color.gray;
			this.gameObject.SetActive(false);
		}
		/// <summary>
		/// Preps this thumbnail for use with the given preset template.
		/// </summary>
		/// <param name="presetTemplate">The preset template to use when creating this thumbnail.</param>
		public void Prepare(GamePresetTemplate presetTemplate) {
			
			// Save the template currently in use.
			this.presetTemplate = presetTemplate;
			
			// Turn on this game object and set it to be interactable.
			this.gameObject.SetActive(true);
			this.selectable.interactable = true;
			
			// Set the screenshot.
			this.screenshotImage.overrideSprite = presetTemplate.PreviewScreenshot;
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Sets the graphics so it looks like this thumbnail is highlighted.
		/// </summary>
		/// <param name="presetTemplate"></param>
		private void Highlight(GamePresetTemplate presetTemplate) {
			
			// Set the text on the label.
			this.presetNameLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.presetNameLabel.Text = "<c=white>" + presetTemplate.PresetName;
			
			// Set the color on the backing and screenshot.
			this.backingImage.color = presetTemplate.PrimaryColor;
			this.screenshotImage.color = Color.white;
		}
		/// <summary>
		/// Sets the graphics so it looks like this thumbnail is dehighlighted.
		/// </summary>
		/// <param name="presetTemplate"></param>
		private void Dehighlight(GamePresetTemplate presetTemplate) {
			
			// Set the text on the label.
			this.presetNameLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");
			this.presetNameLabel.Text = "<c=gray>" + presetTemplate.PresetName;

			// Set the color on the backing and screenshot.
			this.backingImage.color = presetTemplate.PrimaryColor * Color.gray;
			this.screenshotImage.color = Color.gray;
			
		}
		#endregion

		#region EVENT HANDLERS
		public void OnSubmit(BaseEventData eventData) {

			GameObject selectedObject = eventData.selectedObject;
			
			OptionPicker.instance.Display(
				prompt: "Open this preset?", 
				option1: "Yes", 
				option2: "No", 
				callback1: delegate {
					GameController.Instance.OpenPreset(presetTemplate: this.presetTemplate, loadFirstScene: true);
					// GameController.instance.OpenPreset(presetTemplate: this.presetTemplate, withScreenFade: true);
				},
				callback2: delegate {
					EventSystem.current.SetSelectedGameObject(selectedObject);
				}, 
				reselectOnDone: false);
		}

		public void OnCancel(BaseEventData eventData) {
			GameObject selectedObject = eventData.selectedObject;
			
			OptionPicker.instance.Display(
				prompt: "Return to title?", 
				option1: "Yes", 
				option2: "No", 
				callback1: delegate {
					SceneController.instance.BasicLoadSceneWithFade(sceneIndex: 2);
				},
				callback2: delegate {
					EventSystem.current.SetSelectedGameObject(selectedObject);
				}, 
				reselectOnDone: false);
		}

		public void OnSelect(BaseEventData eventData) {
			this.Highlight(this.presetTemplate);
		}

		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance.PlaySFX(SFXType.Hover);
			this.Dehighlight(this.presetTemplate);
		}
		#endregion

		
	}

	
}