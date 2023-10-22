using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace Grawly.Chat {
	
	/// <summary>
	/// A very basic chat script that lets me design a raw script it in the inspector.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Chat/Simple Chat Script")]
	public class SimpleChatScriptDX : SerializedScriptableObject, IChatScript {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the borders be shown when this chat is run?
		/// </summary>
		[SerializeField]
		[PropertyTooltip("Should the borders be shown by default when this chat is run?")]
		private bool useChatBorders = false;
		/// <summary>
		/// Is this script a placeholder? If so, a debug message will be shown instead of the raw text.
		/// </summary>
		[SerializeField]
		[PropertyTooltip("Is this script a placeholder? If so, a debug message will be shown instead of the raw text.")]
		private bool isPlaceholderScript = false;
		#endregion
		
		#region FIELDS - CHAT SCRIPT
		/// <summary>
		/// The raw text to create a chat script from.
		/// </summary>
		[SerializeField, TextArea(minLines: 5, maxLines: 50), HideIf("isPlaceholderScript")]
		private string rawChatText = "";
		#endregion
		
		#region PROPERTIES - CHAT SCRIPT
		/// <summary>
		/// The chat script, in a form of directives.
		/// </summary>
		private List<ChatDirective> ChatScript {
			get {
				
				// If this is a placeholder script, use the placeholder text to identify it.
				if (this.isPlaceholderScript == true) {
					string placeholderText = ": > PLACEHOLDER SCRIPT (" + this.name + "); checker: true";
					return  RawDirectiveParser.GetDirectives(text: placeholderText);
				} else {
					// If this isn't a placeholder, parse out the raw chat text.
					return  RawDirectiveParser.GetDirectives(text: this.rawChatText);
				}
				
			}
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
				return this.ChatScript
					.Append(new CloseDirective())
					.Prepend(new SlideChatBoxDirective() { show = this.useChatBorders })
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