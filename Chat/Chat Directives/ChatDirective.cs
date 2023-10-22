using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {


	/// <summary>
	/// Gets used as part of a chat script to determine what to do.
	/// </summary>
	[HideReferenceObjectPicker, InlineProperty]
	public abstract class ChatDirective {

		#region CONSTANTS
		/// <summary>
		/// The default width a label should be on a chat directive when editing in the inspector.
		/// </summary>
		protected const float DEFAULT_LABEL_WIDTH = 150f;
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Parameterless constructor. Good for. Lots of. Stuff.
		/// </summary>
		public ChatDirective() { }
		/// <summary>
		/// Constructor that takes a set of parameters specific to creating a directive. Usually used as a result of a parser needing to convey this information.
		/// </summary>
		/// <param name="directiveParams">The parameters to be used in constructing this directive.</param>
		public ChatDirective(ChatDirectiveParams directiveParams) { }
		#endregion

		#region RUNTIME STUFF
		/// <summary>
		/// Gets called when figuring out what to do.
		/// </summary>
		/// <param name="chatController">The ChatController evaluating this directive.</param>
		public abstract void EvaluateDirective(ChatControllerDX chatController);
		/// <summary>
		/// The event for when the advance button is hit. May be blank.
		/// </summary>
		public virtual void AdvanceButtonHit(ChatControllerDX chatController) {

		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// The string to use in the inspector for the group title of this directive.
		/// </summary>
		protected abstract string FoldoutGroupTitle { get; }
		#endregion

	}


}