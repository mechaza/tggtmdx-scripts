using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using Grawly.Toggles;
using Grawly.Toggles.Proto;

namespace Grawly.Chat {

	/// <summary>
	/// Writes out dialogue to the chat text.
	/// </summary>
	[Title("Dialogue")]
	[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
	public class DialogueDirective : ChatDirective {

		#region CONSTANTS
		/// <summary>
		/// The maximum length of characters that can be displayed in the foldout group.
		/// </summary>
		private const int NOTES_CUTOFF_POINT = 50;
		#endregion
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The name of the speaker.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public string speakerShorthand = "";
		/// <summary>
		/// The bust up type to use for this directive.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public ChatBustUpType bustUpType = ChatBustUpType.None;
		/// <summary>
		/// The dialogue to be written out.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), MultiLineProperty(3), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public string dialogue;
		#endregion

		#region FIELDS - TOGGLES : ADVANCED
		/// <summary>
		/// Can this text be skipped?
		/// </summary>
		// [FoldoutGroup("Advanced")]
		[FoldoutGroup("$FoldoutGroupTitle" + "/Advanced")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public bool unskippable = false;
		/// <summary>
		/// Should this text be advancable via the Advance button? 
		/// This is different from unskippable; if autoAdvance is off,
		/// the dialogue will still wait for player input.
		/// </summary>
		// [FoldoutGroup("Advanced")]
		[FoldoutGroup("$FoldoutGroupTitle"+ "/Advanced")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public bool autoAdvance = false;
		/// <summary>
		/// The amount of time to wait after reading out the text if autoAdvance is on.
		/// </summary>
		// [FoldoutGroup("Advanced")]
		[FoldoutGroup("$FoldoutGroupTitle"+ "/Advanced")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), ShowIf("autoAdvance"), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public float timeToWaitAfterReadingOut = 0.05f;
		/// <summary>
		/// The theme type to use for the box. Usually will be solid.
		/// </summary>
		// [FoldoutGroup("Advanced")]
		[FoldoutGroup("$FoldoutGroupTitle"+ "/Advanced")]
		[GUIColor(0.3f, 0.8f, 0.8f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public ChatBoxThemeType boxThemeType = ChatBoxThemeType.SolidColor;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Assembles a dialogue directive from a MatchCollection created from a RawDirective.
		/// </summary>
		/// <param name="directiveParams"></param>
		public DialogueDirective(ChatDirectiveParams directiveParams) {
			this.speakerShorthand = directiveParams.FirstLabel;
			this.dialogue = directiveParams.GetValue(key: directiveParams.FirstLabel);
			this.unskippable = directiveParams.GetBool(key: "unskippable");
			this.autoAdvance = directiveParams.GetBool(key: "autoAdvance");
			this.bustUpType = directiveParams.GetBustUpType(key: "e");

			// Get the toggle from the settings just in case I want to force solid color chat windows.
			bool forceSolidBoxTheme = ToggleController.GetToggle<DisableSpecialChatBackgrounds>().GetToggleBool();
			// I am allowed to use the checker theme if I'm not forcing a solid window and if the checker keyword was used.
			bool useCheckerTheme = (forceSolidBoxTheme == false) && (directiveParams.GetBool(key: "checker") == true);
			this.boxThemeType = (useCheckerTheme == true) ? ChatBoxThemeType.CheckerPan : ChatBoxThemeType.SolidColor;
			
			// this.boxThemeType = directiveParams.GetBool(key: "checker") ? ChatBoxThemeType.CheckerPan : ChatBoxThemeType.SolidColor;
			
		}
		/// <summary>
		/// Basic constructor.
		/// </summary>
		public DialogueDirective() {
			// Debug.Log("Assembling blank directive.");
		}
		public DialogueDirective(string speakerShorthand, string dialogue) {
			this.speakerShorthand = speakerShorthand;
			this.dialogue = dialogue;
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Prep the name tag. This actually also sets the advance button's color.
			this.ProcessNameTag(speakerShorthand: this.speakerShorthand, chatController: chatController);

			// Pass the text to read out over to the text box.
			chatController.TextBox.ReadText(
				// text: this.dialogue,
				text: this.ProcessNicknamesInDialogue(text: this.dialogue),
				textBoxParams: new ChatTextBoxParams() {
					
					
					// Set the speaker voice to be the voice of the current speaker. If it doesn't work, just assign it a blank string.
					speakerVoice = chatController.GetRuntimeChatSpeaker(speakerShorthand: this.speakerShorthand)?.DefaultVoice ?? "",
					
					// Pass it the position of this speaker in the scene so the arrows can be adjusted accordingly.
					speakerPositionType = chatController.GetSpeakerPosition(speakerShorthand: this.speakerShorthand),
					// Set the color of the box.
					boxRectangleColor = chatController.GetRuntimeChatSpeaker(speakerShorthand: this.speakerShorthand)?.ChatSpeaker.ChatBoxBackingColor ?? Color.white,
					// Set the theme of the box. Most of the time this will be SolidColor, but certain things can override it.
					themeType = this.boxThemeType,
					// Also set the color of the chat text. This will override to white if using the checker theme.
					textColor = (this.boxThemeType == ChatBoxThemeType.CheckerPan) 
						? Color.white
						: chatController.GetRuntimeChatSpeaker(speakerShorthand: this.speakerShorthand)?.ChatSpeaker.ChatBoxTextColor ?? Color.black,
					// Set whether or not the chat box should auto advance once it is read out.
					autoAdvance = this.autoAdvance,
					// Also pass in the amount of time to wait before advancing. This is only relevant if autoAdvance is on.
					timeToWaitBeforeAdvance = this.timeToWaitAfterReadingOut
				});

			// Also transport the text box to the appropriate position.
			chatController.TextBox.TransportToPosition(speakerPositionType: chatController.GetSpeakerPosition(speakerShorthand: this.speakerShorthand));

			// Set the advance button to be hidden, but selectable. 
			chatController.TextBox.AdvanceButton.SetVisible(false);


			// If the dialogue is unskippable, make sure the advance button cannot be selected.
			// REMINDER: ChatTextBox will re-enable the advance button once it has been read out.
			// If unskippable is on AND autoAdvance is on, ChatTextBox will call EvaluateNextDirective.
			if (this.unskippable == true) {
				chatController.TextBox.AdvanceButton.SetSelectable(false);
			} else {
				chatController.TextBox.AdvanceButton.SetSelectable(true);
			}

			// Tell the controller to focus on this speaker as well.
			chatController.FocusOnSpeaker(speakerShorthand: this.speakerShorthand, bustUpType: this.bustUpType);

		}
		/// <summary>
		/// The event for when the advance button is hit.
		/// </summary>
		public override void AdvanceButtonHit(ChatControllerDX chatController) {

			// If the text is still reading out, skip to the end.
			if (chatController.TextBox.IsStillReadingOut == true) {
				chatController.TextBox.SkipToEnd();
			} else {
				// Make sure the selectable gets turned back off.
				chatController.TextBox.AdvanceButton.SetSelectable(false);
				// If it's not reading out, evaluate the next directive.
				chatController.EvaluateNextDirective();
			}
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Helps set up how the name tag is supposed to look.
		/// Moving this from the main EvaluateDirective function for the sake of organization. 
		/// </summary>
		/// <param name="speakerShorthand">The shorthand referencing a speaker.</param>
		/// <param name="chatController">The uhm. Chat controller.</param>
		private void ProcessNameTag(string speakerShorthand, ChatControllerDX chatController) {
			
			// If the speaker shorthand is NOT empty, present the tag with the name associated with it.
			// If there IS one. 
			if (speakerShorthand != "") {
				// Display the tag and, when doing so, also give the color to the parameters.
				chatController.TextBox.NameTag.DisplayTag(
					text: chatController.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand).ChatSpeaker.SpeakerName,
					nameTagParams: new ChatNameTagParams() {
						nameTagLabelColorType = chatController.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand).ChatSpeaker.nameTagLabelColorType,
						nameTagBackingColorType = chatController.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand).ChatSpeaker.nameTagBackingColorType,
					});

				// If the speaker shorthand exists, also set the color for the advance button.
				chatController.TextBox.AdvanceButton.SetColor(
					color: GrawlyColors.colorDict[chatController.GetRuntimeChatSpeaker(speakerShorthand: speakerShorthand).ChatSpeaker.nameTagBackingColorType]
				);

			} else {
				// If the shorthand IS empty, assume I don't need a name and dismiss the tag.
				chatController.TextBox.NameTag.DismissTag(nameTagParams: new ChatNameTagParams() { });
			}
		}
		/// <summary>
		/// Processes the dialogue text so that nicknames are slipped inside.
		/// </summary>
		/// <param name="text">The raw dialogue text. May contain special characters to replace.</param>
		/// <returns></returns>
		private string ProcessNicknamesInDialogue(string text) {
			// Just call it as it is inside the CharacterIDMap class.
			return GameController.Instance.Variables.CharacterIDMap.ProcessDialogue(text: text);
		}
		#endregion
		
		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {

				string shorthandStr = this.speakerShorthand;
				string bustupStr = (this.bustUpType == ChatBustUpType.None) ? "" : this.bustUpType.ToString();
				string dialogueStr = this.dialogue;
				if (dialogueStr.Length > NOTES_CUTOFF_POINT) {
					dialogueStr = dialogueStr.Substring(startIndex: 0, length: NOTES_CUTOFF_POINT - 3) + "...";
				}
				
				string foldoutStr = "";
				foldoutStr += "" + shorthandStr + " ";
				if (this.bustUpType != ChatBustUpType.None) { foldoutStr += "[" + bustupStr + "] "; }
				foldoutStr += "" + dialogueStr + "";
				return foldoutStr;
				
				// return this.GetType().FullName;
			}
		}
		#endregion

	}

}