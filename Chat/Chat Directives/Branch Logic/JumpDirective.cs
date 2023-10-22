using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	[Title("Jump")]
	public class JumpDirective : ChatDirective {

		/// <summary>
		/// The label to jump to.
		/// </summary>
		[GUIColor(1, 0.6f, 0.4f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public string label;

		public JumpDirective() {

		}
		public JumpDirective(ChatDirectiveParams directiveParams) {
			this.label = directiveParams.GetValue(key: "jump");
		}

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			chatController.JumpToLabel(label: this.label);
			/*chatController.runtimeScript.JumpToLabel(label: this.label);
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