using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grawly.UI.Legacy {

	public class Flasher : MonoBehaviour {

		public static Flasher instance;

		[SerializeField]
		private Image flasher;
		[SerializeField]
		private SuperTextMesh loadingText;

		private void Awake() {
			if (instance == null) {
				instance = this;
				DontDestroyOnLoad(this.gameObject);
				// Have it so the loading text is turned off from the start.
				SetLoadingTextActive(false);
			} else {
				Destroy(this.gameObject);
			}
		}

		#region FLASHING/FADING
		public void Flash() {
			flasher.gameObject.SetActive(true);
			flasher.color = Color.white;
			StartCoroutine(FlashRoutine());
		}
		private IEnumerator FlashRoutine() {
			flasher.CrossFadeAlpha(0f, .1f, true);
			yield return new WaitForSeconds(.1f);
			flasher.gameObject.SetActive(false);
			flasher.color = Color.white;
		}
		/// <summary>
		/// Fade the flasher pic so that you can make a good transition between scenes.
		/// </summary>
		public void Fade(Color color, float fadeOut = .5f, float fadeIn = .5f, float interlude = 1f, bool showLoadingText = false) {
			flasher.gameObject.SetActive(true);
			flasher.color = Color.clear;
			StartCoroutine(FadeRoutine(color, fadeOut, fadeIn, interlude, showLoadingText));
		}
		private IEnumerator FadeRoutine(Color color, float fadeOut, float fadeIn, float interlude, bool showLoadingText) {
			// Set the time to zero
			float time = 0;

			// Fade to the new color
			while (time < fadeOut) {
				flasher.color = Color.Lerp(Color.clear, color, time / fadeOut);
				time += Time.deltaTime;
				yield return null;
			}
			flasher.color = color;

			// Show the loading text (if that's what was requested)
			loadingText.gameObject.SetActive(showLoadingText);

			// Wait a few moments
			yield return new WaitForSeconds(interlude);

			// Turn off the loading text (if it was turned on)
			loadingText.gameObject.SetActive(false);

			time = 0f;
			// Fade to clear
			while (time < fadeIn) {
				flasher.color = Color.Lerp(color, Color.clear, time / fadeIn);
				time += Time.deltaTime;
				yield return null;
			}
			flasher.gameObject.SetActive(false);
		}

		/// <summary>
		/// Fades out with basic parameters.
		/// </summary>
		public static void FadeOut() {
			instance.FadeOut(color: Color.black);
		}
		public static void FadeOut(float fadeTime) {
			instance.FadeOut(color: Color.black, fadeTime: fadeTime);
		}
		public void FadeOut(Color color, float fadeTime = .5f) {
			flasher.gameObject.SetActive(true);
			flasher.color = Color.clear;
			StartCoroutine(FadeOneWayRoutine(flasher.color, Color.black, fadeTime));
		}

		public static void FadeIn() {
			instance.FadeIn();
		}
		public void FadeIn(float fadeTime = .5f) {
			StartCoroutine(FadeInRoutine(fadeTime));
		}
		private IEnumerator FadeInRoutine(float fadeTime) {
			yield return FadeOneWayRoutine(flasher.color, Color.clear, fadeTime);
			// flasher.gameObject.SetActive(false);
		}
		private IEnumerator FadeOneWayRoutine(Color startColor, Color finishColor, float fadeTime) {
			float time = 0;
			while (time < fadeTime) {
				flasher.color = Color.Lerp(startColor, finishColor, time / fadeTime);
				time += Time.deltaTime;
				yield return null;
			}
			flasher.color = finishColor;
		}
		#endregion
		#region LOADING TEXT
		/// <summary>
		/// Set whether or not to show the "loading" text.
		/// </summary>
		/// <param name="status"></param>
		public void SetLoadingTextActive(bool status) {
			loadingText.gameObject.SetActive(status);
		}
		#endregion


	}


}