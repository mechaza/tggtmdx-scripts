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

	/// <summary>
	/// Toggles a GameObject on or off. Good for things like virtual cameras.
	/// </summary>
	[Title("Toggle GameObject")]
	public class ToggleGameObjectDirective : ChatDirective {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Should the target game object be turned on or off?
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private bool active = true;
		/// <summary>
		/// The GameObject that should be activated/deactivated.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private GameObject targetGameObject;
		#endregion

		#region CONSTRUCTORS
		public ToggleGameObjectDirective() {

		}
		public ToggleGameObjectDirective(ChatDirectiveParams directiveParams) {

		}
		#endregion

		#region CHAT DIRECTIVE IMPLEMENTATION
		/// <summary>
		/// Turn the target gameobject on or off.
		/// </summary>
		/// <param name="chatController"></param>
		public override void EvaluateDirective(ChatControllerDX chatController) {
			// Turn the target gameobject on or off.
			targetGameObject.SetActive(value: this.active);
			// Evaluate the next directive.
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