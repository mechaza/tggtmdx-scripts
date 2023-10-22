using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Grawly.Chat;
using UnityEngine;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Opens up the chat controller with the given script.
	/// </summary>
	[ActionCategory("Grawly - Chat"), HutongGames.PlayMaker.Tooltip("Opens up a chat with the specified script.")]
	public class OpenChatScript : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The chat script that is used when opening the chat controller.
		/// </summary>
		[ObjectType(typeof(TextAsset))]
		public FsmObject chatScript;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			ChatControllerDX.GlobalOpen(
				textAsset: (TextAsset)this.chatScript.Value, 
				chatClosedCallback: delegate { base.Finish(); } );
		}
		#endregion

	}


}