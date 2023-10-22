using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
	
namespace Grawly.Chat {
	
	/// <summary>
	/// Gets attached to a button as part of the content pane in the chat debugger.
	/// </summary>
	public class ExampleChatScriptButton : MonoBehaviour, ISubmitHandler, IPointerClickHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The name that should be given to this script on the button.
		/// </summary>
		private string scriptName = "";
		/// <summary>
		/// The script that is associated with this button.
		/// </summary>
		private TextAsset textAsset;
		/// <summary>
		/// Another possible script associated with this button.
		/// </summary>
		private SerializedChatScriptDX serializedChatScript;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The text label that will appear on the button.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private Text textLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this button with the info it needs to get displayed.
		/// </summary>
		/// <param name="textAsset">The script to associate this button with.</param>
		/// <param name="scriptName">The name to but on the button's label.</param>
		public void Prepare(TextAsset textAsset, string scriptName) {
			// Save the information.
			this.textAsset = textAsset;
			this.scriptName = scriptName;
			// Make the serialized script null. Just for the sake of it.
			this.serializedChatScript = null;
			// Change the label on the button.
			this.textLabel.text = scriptName;
		}
		/// <summary>
		/// Preps this button with the info it needs to get displayed.
		/// </summary>
		/// <param name="serializedChatScript">The SerializedChatScript to associate this button with.</param>
		/// <param name="scriptName">The name of the script.</param>
		public void Prepare(SerializedChatScriptDX serializedChatScript, string scriptName) {
			this.textAsset = null;
			this.serializedChatScript = serializedChatScript;
			this.scriptName = scriptName;
			this.textLabel.text = this.scriptName;
		}
		#endregion


		#region EVENT SYSTEM
		/// <summary>
		/// When this button gets clicked, it should tell the debug controller as much.
		/// </summary>
		/// <param name="eventData"></param>
		/// <exception cref="NotImplementedException"></exception>
		public void OnSubmit(BaseEventData eventData) {
			// ChatDebugControllerDX.Instance.ScriptExampleButtonSubmitted(textAsset: this.textAsset);
			// ChatDebugControllerDX.Instance.TextInputSubmitted();
			// throw new System.NotImplementedException();
		}
		public void OnPointerClick(PointerEventData eventData) {

			// I'm nto feeling good.

			// If the text asset isn't null, use that.
			if (this.textAsset != null) {
				ChatDebugControllerDX.instance.ScriptExampleButtonSubmitted(textAsset: this.textAsset);
			} else {
				ChatDebugControllerDX.instance.ScriptExampleButtonSubmitted(this.serializedChatScript);
			}
			
		}
		#endregion


		
	}

}