using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Chat {
	
	/// <summary>
	/// Defines a picture that will be used during a chat dialogue.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Chat/Chat Picture Template")]
	public class ChatPictureTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The ID associated with this chat picture.
		/// </summary>
		[SerializeField]
		private string chatPictureID = "";
		/// <summary>
		/// The sprite that should be associated with the ID above.
		/// </summary>
		[SerializeField]
		private Sprite chatPictureSprite;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The ID associated with this chat picture.
		/// </summary>
		public string ID => this.chatPictureID;
		/// <summary>
		/// The sprite associated with this template.
		/// </summary>
		public Sprite Sprite => this.Sprite;
		#endregion

	}
}