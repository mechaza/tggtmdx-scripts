using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// The foundation for building other ChatTextBoxes from.
	/// </summary>
	public abstract class ChatTextBox : SerializedMonoBehaviour {

		#region FIELDS - SCENE REFERENCES : ASSISTANT COMPONENTS
		/// <summary>
		/// The name tag that this text box is using.
		/// </summary>
		[Title("Assistant Components")]
		[SerializeField, TabGroup("Text Box", "Scene References"), ShowInInspector]
		public ChatBoxNameTag NameTag { get; private set; }
		/// <summary>
		/// The advance button that this text box is using.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References"), ShowInInspector]
		public ChatBoxAdvanceButton AdvanceButton { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : VISUALS
		/// <summary>
		/// The GameObject that contains all the visuals for the text box.
		/// </summary>
		[SerializeField, TabGroup("Text Box", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		#endregion

		#region FIELDS - COMPUTED PROPERTIES
		/// <summary>
		/// Is the text box still reading out text?
		/// </summary>
		public abstract bool IsStillReadingOut { get; }
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the chat box to its initial values.
		/// </summary>
		public abstract void ResetState();
		/// <summary>
		/// Presents the text box onto the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public abstract void PresentTextBox(ChatTextBoxParams textBoxParams);
		/// <summary>
		/// Dismisses the text box from the screen.
		/// </summary>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public abstract void DismissTextBox(ChatTextBoxParams textBoxParams);
		/// <summary>
		/// Transports the chat box to the desired location depending on who is speaking.
		/// </summary>
		/// <param name="speakerPositionType">The position of the current speaker.</param>
		public abstract void TransportToPosition(ChatSpeakerPositionType speakerPositionType);
		#endregion

		#region READING OUT
		/// <summary>
		/// Reads out text onto the screen box.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="textBoxParams">Any additional parameters that may help with presenting the text box.</param>
		public abstract void ReadText(string text, ChatTextBoxParams textBoxParams);
		/// <summary>
		/// Skips the text being read out to the very end, if it is reading out anything.
		/// </summary>
		public abstract void SkipToEnd();
		#endregion

	}


}