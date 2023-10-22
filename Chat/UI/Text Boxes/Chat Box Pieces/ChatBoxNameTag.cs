using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// The name tag that appears on a chat box to represent who is talking.
	/// </summary>
	public abstract class ChatBoxNameTag : SerializedMonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the name tag.
		/// </summary>
		[SerializeField, TabGroup("Name Tag", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Resets the state of the name tag.
		/// </summary>
		public abstract void ResetState();
		/// <summary>
		/// Displays the name tag.
		/// </summary>
		/// <param name="text">The text to show on the name tag.</param>
		/// <param name="nameTagParams">Any additional parameters to show on the tag.</param>
		public abstract void DisplayTag(string text, ChatNameTagParams nameTagParams);
		/// <summary>
		/// Dismisses the name tag.
		/// </summary>
		/// <param name="nameTagParams">Any additional parameters for the tag.</param>
		public abstract void DismissTag(ChatNameTagParams nameTagParams);
		#endregion

	}


}