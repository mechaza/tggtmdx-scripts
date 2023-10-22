using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Grawly.Chat {
	public class PlainChatScript : IChatScript {

		#region FIELDS
		/// <summary>
		/// The dialogue for this plain script.
		/// </summary>
		[SerializeField]
		private List<ChatDirective> chatScript = new List<ChatDirective>();
		#endregion

		#region CONSTRUCTORS
		public PlainChatScript(string rawText) {
			this.chatScript = RawDirectiveParser.GetDirectives(text: rawText);
		}
		public PlainChatScript(List<string> rawTextList) {
			rawTextList.ForEach(s => this.chatScript.AddRange(RawDirectiveParser.GetDirectives(text: s)));
		}
		public PlainChatScript(List<ChatDirective> chatScript) {
			this.chatScript = chatScript;
		}
		#endregion

		#region CHAT SCRIPT IMPLEMENTATION
		/// <summary>
		/// Returns all of the directives for this script.
		/// </summary>
		public List<ChatDirective> Directives {
			get {
				// Return this script, but also make sure to append a close directive.
				// If it already has one, it should be fine and this upcoming one will never be reached,
				// but you never know.
				return this.chatScript
					.Append(new CloseDirective())
					.Prepend(new SlideChatBoxDirective() {show = true })
					.ToList();
			}
		}
		public List<RuntimeChatSpeaker> RuntimeChatSpeakers {
			get {

				// Return a list with just the blank speaker.
				return this.Directives
					.OfType<DefineDirective>()
					.Select(dd => new RuntimeChatSpeaker(
						chatSpeaker: DataController.GetChatSpeaker(speakerName: dd.speakerName), 
						speakerShorthand: dd.speakerShorthand))
					.Append(new RuntimeChatSpeaker(
						speakerShorthand: "",
						chatSpeaker: DataController.GetChatSpeaker("")))  
					.ToList();

	
			}
		}
		#endregion

	}

}