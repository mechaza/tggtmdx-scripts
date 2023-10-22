using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// A very basic directive that just waits and then runs.
	/// </summary>
	[Title("Wait")]
	public class WaitDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The amount of time to wait.
		/// </summary>
		[SerializeField, LabelWidth(DEFAULT_LABEL_WIDTH)]
		private float timeToWait = 1f;
		#endregion

		#region CONSTRUCTORS
		public WaitDirective() {

		}
		public WaitDirective(ChatDirectiveParams directiveParams) {
			this.timeToWait = (float)directiveParams.GetInt(key: "wait");
		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			// Just wait a moment or two and then evaluate the next directive.
			GameController.Instance.WaitThenRun(timeToWait: this.timeToWait, action: delegate {
				chatController.EvaluateNextDirective();
			});
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