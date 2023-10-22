using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {
	[CreateAssetMenu(menuName = "Grawly/Chat/Chat Speaker Template")]
	public class ChatSpeakerTemplate : SerializedScriptableObject {

		#region FIELDS
		/// <summary>
		/// Should this speaker give their name via a reference to their ID?
		/// If this is true, it will use characterIDType to ask the GameVariables what the name of this speaker is.
		/// </summary>
		[Title("Profile")]
		[SerializeField]
		private bool useIDForName = false;
		/// <summary>
		/// The CharacterIDType to use for this template. 
		/// </summary>
		[SerializeField, ShowIf("useIDForName")]
		private CharacterIDType characterIDType = CharacterIDType.Other;
		/// <summary>
		/// The name of the speaker as it should appear on the name label.
		/// This is how it exists within the template.
		/// </summary>
		[InfoBox("I might still need this on even if I'm using an ID. I think I use it for finding the correct speaker template.", VisibleIf = "useIDForName")]
		public string rawSpeakerName = "";
		/// <summary>
		/// The name of the speaker as it should appear on the name label.
		/// This is how it exists within the template.
		/// </summary>
		public string SpeakerName {
			get {
				// If using an ID for this speaker, get their nickname from the GameVariables.
				if (this.useIDForName == true) {
					return GameController.Instance.Variables.CharacterIDMap.GetName(characterIDType: this.characterIDType);
				} else {
					// If not using a nickname, just return the raw speaker name.
					return this.rawSpeakerName;
				}
			}
		}
		/// <summary>
		/// Should this speaker use a custom voice?
		/// </summary>
		[SerializeField]
		private bool useCustomVoice = false;
		/// <summary>
		/// Should this speaker use a custom voice?
		/// </summary>
		public bool UseCustomVoice {
			get {
				return this.useCustomVoice;
			}
		}
		/// <summary>
		/// The custmo voice, if that is being used.
		/// </summary>
		[SerializeField, ShowIf("useCustomVoice")]
		private string customVoice = "";
		/// <summary>
		/// The prefix voice for this speaker. Only relevant if it's uh. Set to true. Idk.
		/// </summary>
		public string TemplateVoice {
			get {
				if (this.useCustomVoice == true) {
					return this.customVoice;
				} else {
					return "";
				}
			}
		}
		[Title("Theming")]
		/// <summary>
		/// The color to use for the name tag's text label.
		/// </summary>
		public GrawlyColorTypes nameTagLabelColorType = GrawlyColorTypes.White;
		/// <summary>
		/// The color to use for the name tag's backing color.
		/// </summary>
		public GrawlyColorTypes nameTagBackingColorType = GrawlyColorTypes.Purple;
		/// <summary>
		/// The color to use for the text inside the chat box.
		/// </summary>
		public GrawlyColorTypes chatBoxTextColorType = GrawlyColorTypes.Black;
		/// <summary>
		/// The color to use for the backing inside the chat text box.
		/// </summary>
		public GrawlyColorTypes chatBoxBackingColorType = GrawlyColorTypes.White;
		[Title("Sprites")]
		/// <summary>
		/// The bust ups of the speaker.
		/// </summary>
		public Dictionary<ChatBustUpType, Sprite> bustUpSpriteDict = new Dictionary<ChatBustUpType, Sprite>();
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The color to use for the text in the chat box.
		/// </summary>
		public Color ChatBoxTextColor {
			get {
				return GrawlyColors.colorDict[this.chatBoxTextColorType];
			}
		}
		/// <summary>
		/// The color to use for the backing in the chat box.
		/// </summary>
		public Color ChatBoxBackingColor {
			get {
				return GrawlyColors.colorDict[this.chatBoxBackingColorType];
			}
		}
		#endregion

	}
}