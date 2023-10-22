using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// Branches based on what the current toggle in the runtime chat is set to.
	/// </summary>
	[Title("Branch")]
	public class BranchDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The label to jump to if the runtime chat toggle is true.
		/// </summary>
		private string trueLabel;
		/// <summary>
		/// The label to jump to if the runtime chat toggle is false.
		/// </summary>
		private string falseLabel;
		#endregion

		#region CONSTRUCTOR
		public BranchDirective(ChatDirectiveParams directiveParams) {
			this.trueLabel = directiveParams.GetValue(key: "branch");
			this.falseLabel = directiveParams.GetValue(key: "else");
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			if (chatController.GetToggle() == true) {
				chatController.JumpToLabel(label: this.trueLabel);
			} else {
				chatController.JumpToLabel(label: this.falseLabel);
			}
			/*if (chatController.runtimeScript.toggle == true) {
				chatController.runtimeScript.JumpToLabel(label: this.trueLabel);
			} else {
				chatController.runtimeScript.JumpToLabel(label: this.falseLabel);
			}
			chatController.EvaluateNextDirective();*/
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