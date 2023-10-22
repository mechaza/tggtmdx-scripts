using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Chat {

	/// <summary>
	/// A way to contain both a chat speaker and the shorthand being used for them at runtime.
	/// </summary>
	[HideReferenceObjectPicker]
	public class RuntimeChatSpeaker {

		#region FIELDS
		/// <summary>
		/// The shorthand being used currently for this speaker.
		/// </summary>
		[OdinSerialize, Title("Shorthand")]
		public string SpeakerShorthand { get; private set; }
		/// <summary>
		/// The voice to use by default for this speaker.
		/// </summary>
		[OdinSerialize, Title("Default Voice")]
		public string DefaultVoice { get; private set; }
		/// <summary>
		/// The chat speaker associated with the given shorthand.
		/// </summary>
		[OdinSerialize, InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Foldout), LabelWidth(120f)]
		public ChatSpeakerTemplate ChatSpeaker { get; private set; }
		#endregion

		/// <summary>
		/// Default constructor for Odin.
		/// </summary>
		public RuntimeChatSpeaker() {

		}
		/// <summary>
		/// Assembles a runtimechatspeaker with the given speakershorthand and chatspeaker to associate it with.
		/// </summary>
		/// <param name="speakerShorthand"></param>
		/// <param name="chatSpeaker"></param>
		public RuntimeChatSpeaker(string speakerShorthand, ChatSpeakerTemplate chatSpeaker) {
			this.SpeakerShorthand = speakerShorthand;
			this.ChatSpeaker = chatSpeaker;
			// There's no reason it's like this. I'm just very tired.
			this.DefaultVoice = this.DefaultVoice + chatSpeaker.TemplateVoice;
		}

	}


}