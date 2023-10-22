using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// Basically a way for me to load up beforehand who is giong to be in the script so I am prepared when the script needs to load.
	/// </summary>
	[Title("Dismiss")]
	public class DefineDirective : ChatDirective {

		#region FIELDS
		/// <summary>
		/// The speaker that will be present in the script. Need this to actually load up the speaker.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public string speakerName { get; private set; } = "";
		/// <summary>
		/// The shorthand to use when referring to this speaker in the script.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public string speakerShorthand { get; private set; }
		#endregion

		#region CONSTRUCTORS
		public DefineDirective(ChatDirectiveParams directiveParams) {
			this.speakerName = directiveParams.GetValue("define");
			// If the shorthand is not available, just set it to the speaker name.
			this.speakerShorthand = directiveParams.GetValue("as") ?? this.speakerName;
		}
		#endregion

		public override void EvaluateDirective(ChatControllerDX chatController) {
			// Nothing to do here!
			chatController.EvaluateNextDirective();
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