using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


namespace Grawly.Chat {

	/// <summary>
	/// Surrounds the chat for when dialogue is a bit more cinematic.
	/// </summary>
	public abstract class ChatBorders : SerializedMonoBehaviour {

		#region MAIN CALLS
		/// <summary>
		/// Totally resets the chat borders to its initial values.
		/// </summary>
		public abstract void ResetState();
		/// <summary>
		/// Presents the borders onto the screen with default values.
		/// </summary>
		public void PresentBorders() {
			this.PresentBorders(borderParams: new ChatBorderParams() {
				// Apparently the border params have nothing to offer.
			});
		}
		/// <summary>
		/// Dismisses the borders from screen with default values.
		/// </summary>
		public void DismissBorders() {
			this.DismissBorders(borderParams: new ChatBorderParams() {
				// Apparently the border params have nothing to offer.
			});
		}
		/// <summary>
		/// Presents the borders onto the screen.
		/// </summary>
		/// <param name="borderParams">The parameters used in presenting the borders onto the screen.</param>
		public abstract void PresentBorders(ChatBorderParams borderParams);
		/// <summary>
		/// Dismisses the borders off the screen.
		/// </summary>
		/// <param name="borderParams">The parameters used in dismissing the borders off the screen.</param>
		public abstract void DismissBorders(ChatBorderParams borderParams);
		#endregion

	}


}