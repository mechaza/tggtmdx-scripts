using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
namespace Grawly.Chat {

	/*/// <summary>
	/// A collection of data that makes it easier to store definitions for speakers or the data required to prepare them in one centralized location.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Chat/Data/Chat Asset Data")]
	public class ChatAssetData : SerializedScriptableObject {

		#region FIELDS - DATA
		/// <summary>
		/// A set of Chat Speakers to be used across many different chat scripts.
		/// </summary>
		[SerializeField]
		private List<ChatSpeaker> chatSpeakers = new List<ChatSpeaker>();
		#endregion

		#region GETTERS
		/// <summary>
		/// Get the Chat Speaker who's shorthand label matches up with the label passed in.
		/// </summary>
		/// <param name="speakerShorthand">The shorthand label for the requested speaker.</param>
		/// <returns></returns>
		public ChatSpeaker GetSpeaker(string speakerShorthand) {
			try {
				return this.chatSpeakers.Find(s => s.speakerShorthandDefault == speakerShorthand);
			} catch (System.Exception e) {
				Debug.LogError("Couldn't find speaker! Returning debug speaker.");
				// TODO: Add a definition for a "Blank Speaker" that can be returned in situations like... this.
				return new ChatSpeaker();
			}
		}
		#endregion


	}*/


}