using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// A way for me to encapsulate the parameters I may need to send to a ChatOptionPicker when I build it.
	/// </summary>
	public class ChatOptionPickerParams {

		#region FIELDS - REFERENCES
		/// <summary>
		/// The current chat controller being used.
		/// </summary>
		[SerializeField]
		private ChatControllerDX chatController;
		/// <summary>
		/// The current chat controller being used.
		/// Will return the Instance if the one in here is null.
		/// </summary>
		public ChatControllerDX ChatController {
			get {
				return this.chatController ?? ChatControllerDX.Instance;
			} set {
				this.chatController = value;
			}
		}
		#endregion

		#region FIELDS - OPTION PARAMS
		/// <summary>
		/// The parameters to use when building the actual option items.
		/// </summary>
		public List<ChatOptionItemParams> optionItemParams = new List<ChatOptionItemParams>();
		#endregion

		#region FIELDS - TEXT BOX POSITION
		/// <summary>
		/// The position the text box should be in. If it's null, will default to whatever I send in at the time.
		/// </summary>
		public ChatSpeakerPositionType? textBoxPosition;
		#endregion

		#region HELPERS
		/// <summary>
		/// Assigns the item params a parent to refer back to.
		/// </summary>
		/// <param name="optionPicker">The option picker to assign to all the little baby children parameters.</param>
		/// <returns>The updated parameters. The same thing.</returns>
		public ChatOptionPickerParams AssignOptionPicker(ChatOptionPicker optionPicker) {
			// Just go through each one and transform it with the picker passed in.
			this.optionItemParams = this.optionItemParams
				.Select(p => p.SetOptionPicker(optionPicker: optionPicker))
				.ToList();
			return this;
		}
		#endregion

	}


}