using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;

namespace Grawly.Chat {
	
	/// <summary>
	/// Sets the value stored in a global flag.
	/// </summary>
	[Title("Global Flag")]
	public class GlobalFlagDirective : ChatDirective {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of flag to set.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public GlobalFlagType flagType = GlobalFlagType.None;
		/// <summary>
		/// The value to assign the specified flag.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public bool value = false;
		#endregion

		#region CONSTRUCTORS
		public GlobalFlagDirective() {
			
		}
		public GlobalFlagDirective(ChatDirectiveParams directiveParams) {
			
			
			this.flagType = directiveParams.GetEnum<GlobalFlagType>(key: "setflag");
			this.value = directiveParams.GetBool(key: "value");
		}
		#endregion
		
		#region DIRECTIVE IMPLEMENTATION
		public override void EvaluateDirective(ChatControllerDX chatController) {
			GlobalFlagController.Instance.SetFlag(flagType: this.flagType, value: this.value);
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