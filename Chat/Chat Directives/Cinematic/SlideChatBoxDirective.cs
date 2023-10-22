using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	[Title("Slide")]
	public class SlideChatBoxDirective : ChatDirective {

		[SerializeField]
		public bool show = false;

		#region CONSTRUCTORS
		public SlideChatBoxDirective() {

		}
		public SlideChatBoxDirective(ChatDirectiveParams directiveParams) {
			this.show = directiveParams.GetValue(key: "slide") == "true" ? true : false;
		}
		#endregion

		/// <summary>
		/// Slides the chat box depending on what the "show" was set to.
		/// </summary>
		/// <param name="chatController"></param>
		public override void EvaluateDirective(ChatControllerDX chatController) {
			// Debug.Log("SHOW IS EQUAL TO " + this.show.ToString());
			if (this.show == true) {
				chatController.TextBox.PresentTextBox(new ChatTextBoxParams() { });
				chatController.EvaluateNextDirective();
			} else {
				chatController.TextBox.DismissTextBox(new ChatTextBoxParams() { });
				chatController.EvaluateNextDirective();
			}
		}
		
		
		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {
				return this.GetType().FullName;
			}
		}
		#endregion

		
	}


}