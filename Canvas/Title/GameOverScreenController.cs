using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Grawly {

	/// <summary>
	/// Controls the stuff that goes on in the game over screen.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class GameOverScreenController : MonoBehaviour, ISubmitHandler, ICancelHandler {

		public static GameOverScreenController instance;

		#region FIELDS - STATE
		/// <summary>
		/// Has the transition to the title started?
		/// </summary>
		private bool transitionToTitleStarted = false;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The start position of the text.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private Vector2 textStartPos = new Vector2();
		/// <summary>
		/// The ending position of the text.
		/// </summary>
		[SerializeField]
		private Vector2 textEndPos = new Vector2();
		/// <summary>
		/// The amount of time it should take for the text to tween.
		/// </summary>
		[SerializeField]
		private float textTweenTime = 40f;
		/// <summary>
		/// The amount of time to take when fading out the audio before destroying the GameController.
		/// </summary>
		[SerializeField]
		private float audioFadeOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take when fading out.
		/// </summary>
		[SerializeField]
		private float imageFadeTime = 5f;
		/// <summary>
		/// The amount of time to wait after the fader image has faded completely.
		/// </summary>
		[SerializeField]
		private float imageFadeOvertime = 2f;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The rect transform of the text that scrolls across the screen.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private RectTransform textRectTransform;
		/// <summary>
		/// A fader image to use because I'm going to destroy the GameObject lol.
		/// </summary>
		[SerializeField]
		private Image faderImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
			// Killing all tweens is needed because otherwise it would throw errors for nulled tweens.
			// DOTween.KillAll(complete: false);
			
		}
		private IEnumerator Start() {

			Grawly.UI.Legacy.Flasher.instance?.FadeIn(0.5f);

			// Tell the event system to select this GameObject.
			EventSystem.current.SetSelectedGameObject(this.gameObject);

			// Snap the text to its start position.
			this.textRectTransform.anchoredPosition = this.textStartPos;
			// Tween the text.
			this.textRectTransform.DOAnchorPos(
				endValue: this.textEndPos,
				duration: this.textTweenTime,
				snapping: true)
				.SetEase(Ease.Linear);

			// Wait the specified amount of time.
			yield return new WaitForSeconds(this.textTweenTime);

			// Call exit to title, if no input has been received.
			this.ExitToTitle();


		}
		#endregion

		#region MAIN METHODS
		/// <summary>
		/// The method to call when transitioning to the title.
		/// Has a state variable to prevent repeated calls.
		/// </summary>
		private void ExitToTitle() {
			if (this.transitionToTitleStarted == false) {
				// Set the transition variable to true.
				this.transitionToTitleStarted = true;
				StartCoroutine(this.ExitToTitleRoutine());
			}
		}
		private IEnumerator ExitToTitleRoutine() {
			// Fade the audio out.
			AudioController.instance?.StopMusic(track: 0, fade: this.audioFadeOutTime);
			// Fade the fader image out.
			this.faderImage.DOColor(endValue: Color.black, duration: this.imageFadeTime).SetEase(Ease.Linear);
			// Wait for the audio to fade out completely.
			yield return new WaitForSeconds(this.audioFadeOutTime + 0.1f);

			// Destroy the GameController.
			if (GameController.Instance != null) {
				Destroy(GameController.Instance.gameObject);
			}


			// Wait a bit.
			yield return new WaitForSeconds(this.imageFadeTime + this.imageFadeOvertime);
			// Load the title screen.
			SceneManager.LoadScene("Initialization");
		}
		#endregion

		#region UI EVENTS
		public void OnSubmit(BaseEventData eventData) {
			// If input is captured, just call ExitToTitle().
			this.ExitToTitle();
		}

		public void OnCancel(BaseEventData eventData) {
			// If input is captured, just call ExitToTitle().
			this.ExitToTitle();
		}
		#endregion

	}


}