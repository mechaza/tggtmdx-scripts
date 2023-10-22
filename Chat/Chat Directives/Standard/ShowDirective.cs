using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;
using System.Linq;

namespace Grawly.Chat {

	[Title("Show")]
	[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
	public class ShowDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The name for this speaker.
		/// </summary>
		[GUIColor(0.8f, 0.8f, 0.3f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public string speakerShorthand = "";
		/// <summary>
		/// The bust up type to use for this directive.
		/// </summary>
		[GUIColor(0.8f, 0.8f, 0.3f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public ChatBustUpType bustUpType = ChatBustUpType.None;
		/// <summary>
		/// The position for this speaker to show up.
		/// </summary>
		[GUIColor(0.8f, 0.8f, 0.3f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public ChatSpeakerPositionType position = ChatSpeakerPositionType.None;
		#endregion

		#region CONSTRUCTORS
		public ShowDirective() {

		}
		public ShowDirective(ChatDirectiveParams directiveParams) {

			// Assign the speaker name from the directive params given.
			this.speakerShorthand = directiveParams.GetValue(key: "show") ?? "";
			this.bustUpType = directiveParams.GetBustUpType(key: "e");
			// I can't have a type of None for presenting. If it is none, override it.
			if (this.bustUpType == ChatBustUpType.None) {
				this.bustUpType = ChatBustUpType.Neutral;
			}
			
			// Go through each chat position and find the one contained as a string.
			foreach (ChatSpeakerPositionType position in System.Enum.GetValues(typeof(ChatSpeakerPositionType))) {
				if (position.ToString().ToLower() == directiveParams.GetValue(key: "on").ToLower()) {
					this.position = position;
					break;
				}
			}
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		/// <summary>
		/// Grabs the speaker that's needed for the chat and props them up on the appropriate side.
		/// </summary>
		/// <param name="chatController">The ChatController being used for this operation.</param>
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Debug.LogError("SHOWWW");

			// Grab the ChatSpeakerTemplate associated with the specified shorthand from the ChatController.
			ChatSpeakerTemplate chatSpeaker = chatController.GetRuntimeChatSpeaker(speakerShorthand: this.speakerShorthand).ChatSpeaker;

			if (chatController.GetBustUp(positionType: this.position) == null) {
				Debug.LogError("AHHH");
			}

			// Find the bust up that matches the desired position and tell that bust up to present the speaker.
			chatController.GetBustUp(positionType: this.position).PresentSpeaker(chatSpeaker: chatSpeaker, bustUpParams: new ChatBustUpParams() {
				BustUpType = this.bustUpType
			});

			// Evaluate the next directive.
			chatController.EvaluateNextDirective();

		}
		#endregion

		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {

				string foldoutStr = "Show - ";
				foldoutStr += "[" + this.speakerShorthand + "] ";
				if (this.bustUpType != ChatBustUpType.None) { foldoutStr += "[" + this.bustUpType.ToString() + "] "; }
				if (this.position != ChatSpeakerPositionType.None) { foldoutStr += "[" + this.position.ToString() + "] "; }
				return foldoutStr;
				
				// return this.GetType().FullName;
			}
		}
		#endregion

	}
}