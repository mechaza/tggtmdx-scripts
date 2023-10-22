using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Chat {

	/// <summary>
	/// Represents a chat speaker engaging in a chat.
	/// Can be derived to create standard chat bust ups that appear at the lower corners of the screen,
	/// ones that appear at the top corners, etc.
	/// </summary>
	public abstract class ChatBustUp : SerializedMonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The current ChatSpeaker assigned to this bust up.
		/// </summary>
		protected internal ChatSpeakerTemplate currentChatSpeaker;
		/// <summary>
		/// The BustUpParams that were passed in when this speaker was presented.
		/// </summary>
		protected ChatBustUpParams bustUpParams;
		/// <summary>
		/// Is the bust up active in the current chat?
		/// This is needed for when I need to adjust the focus and need to know who to set Listening and who to not call period if they're, for Instance, hiding.
		/// </summary>
		protected internal bool ActiveInChat { get; protected set; } = false;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The position that this speaker lays on the screen in the abstract sense.
		/// Had to separate this because of issues with Odin's serialization.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Toggles"), Title("General")]
		private ChatSpeakerPositionType speakerPositionType = ChatSpeakerPositionType.Alpha;
		/// <summary>
		/// The position that this speaker lays on the screen in the abstract sense.
		/// </summary>
		[TabGroup("Bust Up", "Toggles"), Title("General")]
		protected internal ChatSpeakerPositionType SpeakerPositionType {
			get {

				if (this.speakerPositionType == ChatSpeakerPositionType.Alpha) {
					Debug.LogWarning("CHAT: Position was set to alpha. Attempting to infer the correct position from GameObject name.");
					if (this.gameObject.name.Contains("Top Right")) {
						this.speakerPositionType = ChatSpeakerPositionType.TopRight;
					} else if (this.gameObject.name.Contains("Left")) {
						this.speakerPositionType = ChatSpeakerPositionType.Left;
					} else if (this.gameObject.name.Contains("Right")) {
						this.speakerPositionType = ChatSpeakerPositionType.Right;
					}
				}

				return this.speakerPositionType;
			}
			protected set {
				this.speakerPositionType = value;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the bust up.
		/// </summary>
		[SerializeField, TabGroup("Bust Up", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		#endregion


		#region MAIN CALLS
		/// <summary>
		/// Presents a ChatSpeaker onto the screen.
		/// </summary>
		/// <param name="chatSpeaker">The speaker to present.</param>
		public abstract void PresentSpeaker(ChatSpeakerTemplate chatSpeaker, ChatBustUpParams bustUpParams);
		/// <summary>
		/// Dismisses the bust up totally.
		/// </summary>
		public void DismissSpeaker() {
			// Call the version of this function that accepts the bust up params.
			this.DismissSpeaker(bustUpParams: this.bustUpParams);
		}
		/// <summary>
		/// Dismisses the bust up totally. 
		/// </summary>
		/// <param name="bustUpParams">The bust up params that were passed in when this was made.</param>
		protected abstract void DismissSpeaker(ChatBustUpParams bustUpParams);
		#endregion

		#region COLORS
		/// <summary>
		/// Sets the colors of the decorations around the speaker.
		/// </summary>
		/// <param name="chatSpeaker">The template of the current speaker to present.</param>
		/// <param name="bustUpParams">Additional bust up params to use with the speaker.</param>
		protected abstract void SetColors(ChatSpeakerTemplate chatSpeaker, ChatBustUpParams bustUpParams);
		#endregion

		#region SPECIFIC ANIMATIONS
		/// <summary>
		/// Reverts the ChatBustUp to the state where it should be before it gets popped up.
		/// </summary>
		public abstract void ResetState();
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is speaking.
		/// </summary>
		/// <param name="bustUpParams">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal abstract void SetSpeaking(ChatBustUpParams bustUpParams);
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is listening.
		/// </summary>
		/// <param name="bustUpParams">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal abstract void SetListening(ChatBustUpParams bustUpParams);
		/// <summary>
		/// Sets the graphics on the bust up so the graphics look like the bust up is hiding.
		/// </summary>
		/// <param name="bustUpParams">A struct that may contain overrides. Null fields mean they should use defaults.</param>
		protected internal abstract void SetHiding(ChatBustUpParams bustUpParams);
		#endregion

		#region SPECIAL EFFECTS
		/// <summary>
		/// Shakes the bust up vigorously.
		/// </summary>
		/// <param name="time">The amount of time to spend shaking it like you mean it.</param>
		/// <param name="magnitude">How powerful is your groove?</param>
		public abstract void Shake(float time = 0.2f, float magnitude = 10f);
		#endregion

	}


}