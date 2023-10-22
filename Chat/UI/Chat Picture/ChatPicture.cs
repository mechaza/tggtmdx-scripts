using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Sirenix.Serialization;


namespace Grawly.Chat {

	/// <summary>
	/// The component of the chat controller that manages showing pictures on screen.
	/// </summary>
	public abstract class ChatPicture : SerializedMonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The params that were used to create the chat picture currently on display.
		/// </summary>
		protected ChatPictureParams chatPictureParams;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents a picture onto the chat.
		/// </summary>
		/// <param name="chatPictureParams">The parameters containing the picture as well as any other data needed.</param>
		public abstract void PresentPicture(ChatPictureParams chatPictureParams);
		/// <summary>
		/// Dismisses the picture totally.
		/// </summary>
		public void DismissPicture() {
			// Just calls the version of this function that actually takes parameters.
			this.DismissPicture(chatPictureParams: this.chatPictureParams);
		}
		/// <summary>
		/// Dismisses the chat picture.
		/// </summary>
		/// <param name="chatPictureParams">The parameters that were used in constructing this chat picture.</param>
		protected abstract void DismissPicture(ChatPictureParams chatPictureParams);
		#endregion

		#region SPECIFIC ANIMATIONS
		/// <summary>
		/// Reverts the ChatPicture to the state where it should be before it gets tweened in.
		/// </summary>
		public abstract void ResetState();
		#endregion

		#region SPECIAL EFFECTS
		/// <summary>
		/// Shakes the picture vigorously.
		/// </summary>
		/// <param name="time">The amount of time to spend shaking it like you mean it.</param>
		/// <param name="magnitude">How powerful is your groove?</param>
		public abstract void Shake(float time = 0.2f, float magnitude = 10f);
		#endregion

	}


}