using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	[Title("Label")]
	public class LabelDirective : ChatDirective {

		/// <summary>
		/// The label. hte label.
		/// </summary>
		[GUIColor(0, 1, 0), LabelWidth(DEFAULT_LABEL_WIDTH)]
		public string label;

		#region CONSTRUCTORS
		public LabelDirective() {

		}
		public LabelDirective(ChatDirectiveParams directiveParams) {
			this.label = directiveParams.GetValue(key: "label");
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			// Nothing to do here.
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