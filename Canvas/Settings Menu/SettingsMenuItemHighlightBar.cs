using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Grawly.UI {

	/// <summary>
	/// A quick script to be used in the settings menu.
	/// Gets placed behind a menu item to make it look like its being highlighted.
	/// </summary>
	public class SettingsMenuItemHighlightBar : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The image that should be used in the highlighting process.
		/// </summary>
		[SerializeField]
		private Image highlightImage;
		#endregion

		#region PRESENTATION
		public void Highlight() {

		}
		#endregion

	}


}