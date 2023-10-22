using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI.Legacy;

namespace Grawly.Overworld {
	public class OverworldController : MonoBehaviour {

		public static OverworldController instance;

		/// <summary>
		/// The current overworld screen being displayed.
		/// </summary>
		[SerializeField]
		private GameObject currentScreen;

		private void Awake() {
			instance = this;
		}

		#region TRANSITIONS
		/// <summary>
		/// Hides the current screen in the overworld scene and shows the new one.
		/// </summary>
		/// <param name="overworldScreen"></param>
		public void MoveToScreen(OverworldScreen screen) {
			StartCoroutine(MoveToScreenRoutine(screen));
		}
		/// <summary>
		/// The routine for handling MoveToScreen.
		/// </summary>
		/// <param name="screen"></param>
		/// <returns></returns>
		private IEnumerator MoveToScreenRoutine(OverworldScreen screen) {
			// Fade out.
			Flasher.instance.Fade(Color.black);
			// Wait one second.
			yield return new WaitForSeconds(1f);
			// Disable the old screen and set up the new one.
			currentScreen.SetActive(false);
			currentScreen = screen.gameObject;
			// Change the boundaries.
			throw new System.Exception("Do I even use this anymore?");
			/*ProCamera2DNumericBoundaries b = GameObject.FindObjectOfType<ProCamera2DNumericBoundaries>();
			b.TopBoundary = screen.bounds.x;
			b.RightBoundary = screen.bounds.y;
			b.BottomBoundary = screen.bounds.z;
			b.LeftBoundary = screen.bounds.w;
			currentScreen.SetActive(true);*/
		}
		#endregion


	}
}
