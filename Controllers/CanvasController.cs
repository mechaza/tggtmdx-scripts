using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Grawly.UI.Legacy;

namespace Grawly {
	/// <summary>
	/// Controls the different groups of UI elements that make use of the UI Canvas.
	/// </summary>
	public class CanvasController : MonoBehaviour {

		public static CanvasController instance;

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The canvas group displaying the battle HUD.
		/// </summary>
		[SerializeField]
		private GameObject battleCanvasGroup;
		/// <summary>
		/// The canvas group displaying the chat box.
		/// </summary>
		[SerializeField]
		private GameObject chatCanvasGroup;
		/// <summary>
		/// The canvas group displaying the battle's results.
		/// </summary>
		[SerializeField]
		private GameObject battleResultsCanvasGroup;
		/// <summary>
		/// The canvas group displaying the graphics for an all out attack.
		/// </summary>
		[SerializeField]
		private GameObject allOutAttackCanvasGroup;
		/// <summary>
		/// The canvas group displaying the persona selection/analysis screen.
		/// </summary>
		[SerializeField]
		private GameObject personaSelectionCanvasGroup;
		/// <summary>
		/// The canvas group displaying the pause menu.
		/// </summary>
		[SerializeField]
		private GameObject pauseScreenCanvasGroup;
		/// <summary>
		/// The canvas group displaying different parts of the dungeon's hud.
		/// </summary>
		[SerializeField]
		private GameObject dungeonHUDCanvasGroup;
		/// <summary>
		/// The canvas group displaying the player's statuses at the lower right.
		/// </summary>
		[SerializeField]
		private GameObject playerStatusCanvasGroup;
		#endregion


		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}

		#region FLASH AND FADE
		/// <summary>
		/// Flashes the screen instantly for a nice effect.
		/// </summary>
		public void Flash() {
			Debug.LogWarning("CanvasController called for Flash. Please remember to fix this.");
			Flasher.instance.Flash();
		}
		/// <summary>
		/// Fades the screen for a nice effect.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="fadeOut"></param>
		/// <param name="fadeIn"></param>
		/// <param name="interlude"></param>
		public void Fade(Color color, float fadeOut = .5f, float fadeIn = .5f, float interlude = 1f) {
			Debug.Log("NOTE: CanvasController called for Fade. Please remember to fix this.");
			Flasher.instance.Fade(color, fadeOut, fadeIn, interlude);
		}
		public void FadeOut(Color color, float fadeTime = .5f) {
			Debug.LogWarning("CanvasController called for FadeOut. Please remember to fix this.");
			Flasher.instance.FadeOut(color, fadeTime);
		}
		public void FadeIn(float fadeTime = .5f) {
			Debug.LogWarning("CanvasController called for FadeIn. Please remember to fix this.");
			Flasher.instance.FadeIn(fadeTime);
		}
		#endregion


		
		public Transform GetBattleCanvas() {
			return battleCanvasGroup.transform;
		}

		public void SetChatCanvas(bool status) {
			chatCanvasGroup.SetActive(status);
		}

		public void SetAllOutAttackCanvas(bool status) {
			allOutAttackCanvasGroup.SetActive(status);
		}
		public void SetPersonaSelectionCanvas(bool status) {
			personaSelectionCanvasGroup.SetActive(status);
		}

	}

}
