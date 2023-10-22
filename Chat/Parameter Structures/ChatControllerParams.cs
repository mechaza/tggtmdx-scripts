using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Chat {

	/// <summary>
	/// A way to encapsulate any parameters I may be sending to a ChatController when I open it.
	/// </summary>
	public class ChatControllerParams {


		#region FIELDS - MISC TOGGLES
		/// <summary>
		/// Should the chat box automatically be slid in by default before any directives are read out?
		/// </summary>
		public bool slideChatBoxOnOpen = true;
		#endregion

		#region FIELDS - CALLBACKS
		/// <summary>
		/// The callback to run at the very start of the script.
		/// </summary>
		public ChatOpenedCallback chatOpenedCallback;
		/// <summary>
		/// The callback to run when the chat gets closed.
		/// </summary>
		public ChatClosedCallback chatClosedCallback;
		#endregion

	}


}