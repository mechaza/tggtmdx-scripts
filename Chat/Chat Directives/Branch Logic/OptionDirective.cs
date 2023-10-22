using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;

namespace Grawly.Chat {
	[Title("Option")]
	public class OptionDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The parameters to use when assembling the option picker.
		/// </summary>
		[GUIColor(0.8f, 0.3f, 0.8f), SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		private ChatOptionPickerParams optionPickerParams = new ChatOptionPickerParams();
		#endregion

		#region CONSTRUCTORS
		public OptionDirective() {
			
		}
		/// <summary>
		/// Assembles an option directive with a string for Yes and a string for No
		/// </summary>
		public OptionDirective(string trueString, string falseString) {
			// isToggleOption = true;
			this.optionPickerParams = new ChatOptionPickerParams() {
				optionItemParams = new List<ChatOptionItemParams>() {
					new ChatOptionItemParams(){ optionLabelText = trueString, toggleToSet = true },
					new ChatOptionItemParams(){ optionLabelText = falseString, toggleToSet = false },
				}
			};
		}
		public OptionDirective(ChatDirectiveParams directiveParams) {

			this.optionPickerParams = new ChatOptionPickerParams() {

				// If I define the box to be positioned at the top right, do that. If not, just keep it at its normal spot.
				textBoxPosition = directiveParams.GetValue(key: "textBoxPos")?.ToLower() == "topright" ? ChatSpeakerPositionType.TopRight : ChatSpeakerPositionType.None,

				// Add the items.
				optionItemParams = new List<ChatOptionItemParams>() {
					new ChatOptionItemParams(){
						optionLabelText = directiveParams.GetValue(key: "option1"),
						// labelToJumpTo = directiveParams.GetValue(key: "option1"),
						toggleToSet = true
					},
					new ChatOptionItemParams(){
						optionLabelText = directiveParams.GetValue(key: "option2"),
						// labelToJumpTo = directiveParams.GetValue(key: "option2"),
						toggleToSet = false
					},
				}
			};

		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// Grab the standard picker from the chat controller for right now.
			ChatOptionPicker optionPicker = chatController.OptionPickers.First();

			optionPicker.PresentOptions(											// Tell the option picker to present the options...
				optionPickerParams: this.optionPickerParams.AssignOptionPicker(		// ...but also make sure to assign the params a reference to the picker itself.
					optionPicker: optionPicker));

		}
		#endregion


		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {
				return this.GetType().FullName;
			}
		}
		#endregion


	}

}