using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// A way to encapsulate parameters when making calls to the chat box.
	/// </summary>
	public class ChatTextBoxParams {

		#region FIELDS - MAIN
		/// <summary>
		/// The position of the person using the text box.
		/// Good for if I need to move the arrows around.
		/// </summary>
		public ChatSpeakerPositionType speakerPositionType = ChatSpeakerPositionType.None;
		/// <summary>
		/// The voice this speaker should prepend to their dialogue.
		/// Might be blank in some cases.
		/// </summary>
		public string speakerVoice = "";
		#endregion

		#region FIELDS - TOGGLES : THEME
		/// <summary>
		/// The kind of "theme" to use.
		/// </summary>
		public ChatBoxThemeType themeType = ChatBoxThemeType.SolidColor;
		/// <summary>
		/// The color that the text box should be.
		/// Usually white, but may change.
		/// </summary>
		public Color boxRectangleColor = Color.white;
		/// <summary>
		/// The color that the text should be.
		/// Usually black, but may change.
		/// </summary>
		public Color textColor = Color.black;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The color that the text should be.
		/// Gets converted to hex value from the textColor field.
		/// </summary>
		public string TextColorTag {
			get {
				return "<c=" + ColorUtility.ToHtmlStringRGB(color: this.textColor) + ">";
			}
		}
		#endregion

		#region FIELDS - TOGGLES : TIMING
		/// <summary>
		/// Should the dialogue advance automatically?
		/// This is handy when used in conjunction with Unskippable.
		/// </summary>
		public bool autoAdvance = false;
		/// <summary>
		/// If autoAdvance is true, this is the amount of time that will
		/// pass before automatically evaluating the next directive.
		/// </summary>
		public float timeToWaitBeforeAdvance = 0.05f;
		#endregion

	}


}