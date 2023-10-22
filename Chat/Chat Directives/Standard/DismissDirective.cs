using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// Just dismisses the speaker on the assigned part of the screen.
	/// </summary>
	[Title("Dismiss")]
	public class DismissDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The position of the speaker to dismiss.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private ChatSpeakerPositionType positionType = ChatSpeakerPositionType.None;
		/// <summary>
		/// This is sort of weird. When serializing this class, use the position type.
		/// If using directive params (e.x., this will be null), grab the speaker and hide them.
		/// </summary>
		private string speakerShorthand;
		#endregion

		#region CONSTRUCTORS
		public DismissDirective() {

		}
		public DismissDirective(ChatDirectiveParams chatDirectiveParams) {
			this.speakerShorthand = chatDirectiveParams.GetValue(key: "dismiss");
		}
		#endregion

		#region IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {

			// If the speaker shorthand is not null (i.e., read this from text) dissmiss the speaker that way.
			if (this.speakerShorthand != null) {
				chatController.GetBustUp(speakerShorthand: speakerShorthand).DismissSpeaker();
			} else {
				// Dismiss the speaker on the specified position if it was not defined.
				chatController.GetBustUp(positionType: this.positionType).DismissSpeaker();
			}

			// Execute the next directive.
			chatController.EvaluateNextDirective();
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