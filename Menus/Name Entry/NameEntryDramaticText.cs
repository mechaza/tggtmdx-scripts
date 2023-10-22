using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.Menus.NameEntry {

	/// <summary>
	/// This is the text that says weird shit and prompts the player for junk.
	/// </summary>
	public class NameEntryDramaticText : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The color to use for the dramatic text.
		/// </summary>
		[TabGroup("Dramatic Text", "Toggles"), Title("Colors"), SerializeField]
		private Color textColor;
		/// <summary>
		/// The color to use for the back image.
		/// </summary>
		[TabGroup("Dramatic Text", "Toggles"), SerializeField]
		private Color imageColor;
		/// <summary>
		/// The amount of time to take when fading the text in/out.
		/// </summary>
		[TabGroup("Dramatic Text", "Toggles"), Title("Timing"), SerializeField]
		private float defaultTextFadeTime = 0.5f;
		/// <summary>
		/// The amouint of time to take when tweening the image that serves as background.
		/// </summary>
		[TabGroup("Dramatic Text", "Toggles"), SerializeField]
		private float defaultImageFadeTime = 0.5f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The Dramatic Text. Whoa.
		/// </summary>
		[TabGroup("Dramatic Text", "Scene References"), Title("Text"), SerializeField]
		private Text dramaticText;
		/// <summary>
		/// The image that serves as the backing for the text.
		/// </summary>
		[TabGroup("Dramatic Text", "Scene References"), Title("Images"), SerializeField]
		private Image backingImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.dramaticText.gameObject.SetActive(true);
			this.backingImage.gameObject.SetActive(true);
			this.dramaticText.text = "";
			this.backingImage.color = Color.clear;
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Fades the text into the screen.
		/// </summary>
		/// <param name="text">The text to show.</param>
		public void DisplayText(string text) {
			// Just call the default amount.
			this.DisplayText(text: text, fadeTime: this.defaultTextFadeTime);
		}
		/// <summary>
		/// Fades the text into the screen.
		/// </summary>
		/// <param name="text">The text to show.</param>
		/// <param name="fadeTime">The amount of time to take when fading the text.</param>
		public void DisplayText(string text, float fadeTime) {
			// Kill any tweens on the color.
			this.CompleteFading();
			// Set the text itself.
			this.dramaticText.text = text;
			// Set the color of the text to clear and tween in the correct color.
			this.dramaticText.color = Color.clear;
			// Tween the text in.
			this.dramaticText.DOColor(
				endValue: this.textColor,
				duration: fadeTime)
				.SetEase(ease: Ease.Linear);
			// Tween the backing image in.
			this.backingImage.DOColor(
				endValue: this.imageColor,
				duration: this.defaultImageFadeTime)
				.SetEase(ease: Ease.Linear);
		}
		/// <summary>
		/// Dismisses the dramatic text.
		/// </summary>
		public void DismissText() {
			// Just use the default amount.
			this.DismissText(fadeTime: this.defaultTextFadeTime);
		}
		/// <summary>
		/// Dismisses the dramatic text.
		/// </summary>
		/// <param name="fadeTime">The amount of time to take when fading the text out.</param>
		public void DismissText(float fadeTime) {
			// Kill any tweens on the color.
			this.CompleteFading();
			// Set the color of the text to clear and tween in the correct color.
			this.dramaticText.color = this.textColor;
			// Tween the text out.
			this.dramaticText.DOColor(
				endValue: Color.clear,
				duration: fadeTime)
				.SetEase(ease: Ease.Linear);
			// Tween the backing image out.
			this.backingImage.DOColor(
				endValue: Color.clear,
				duration: this.defaultImageFadeTime)
				.SetEase(ease: Ease.Linear);
		}
		/// <summary>
		/// Completes the fading animation on the text, if any is in progress.
		/// This is its own method because I may call it from Bolt.
		/// </summary>
		public void CompleteFading() {
			this.dramaticText.DOKill(complete: true);
			this.backingImage.DOKill(complete: true);
		}
		#endregion

	}


}