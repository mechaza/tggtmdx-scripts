using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
// using Parabox.STL;
using UnityEngine.Serialization;

namespace Grawly.Chat {
	
	/// <summary>
	/// Gets attached to a ChatControllerDX to enable debugging.
	/// I am probably only going to use this in one place but whatever.
	/// </summary>
	//[RequireComponent(typeof(ChatControllerDX))]
	public class ChatDebugControllerDX : MonoBehaviour {

		public static ChatDebugControllerDX instance;

		#region FIELDS - STATE
		/// <summary>
		/// Is a text asset currently the one currently being previewed?
		/// </summary>
		private bool isTextAsset = true;
		/// <summary>
		/// The serialized script that is currently being considered.
		/// </summary>
		private SerializedChatScriptDX currentlySelectedSerializedChatScript;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// Whether or not debug mode should be active.
		/// </summary>
		[SerializeField]
		private bool debugMode = false;
		/// <summary>
		/// The text chat scripts to use if in debug mode.
		/// </summary>
		[SerializeField]
		private List<TextAsset> debugTextChatScripts = new List<TextAsset>();
		/// <summary>
		/// Serialized chat script assets to use.
		/// </summary>
		[SerializeField]
		private List<SerializedChatScriptDX> debugSerializedChatScripts = new List<SerializedChatScriptDX>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A reference to all the visuals that are part of this script.
		/// </summary>
		[FormerlySerializedAs("allVisuals")]
		[SerializeField, Title("Scene References")]
		private GameObject allConsoleVisuals;
		/// <summary>
		/// Contains all the visuals for the documentation page.
		/// </summary>
		[SerializeField]
		private GameObject allDocumentationVisuals;
		/// <summary>
		/// The text field where a player can input their own scripts.
		/// </summary>
		[SerializeField]
		private InputField scriptInputField;
		/// <summary>
		/// The transform of the content panel.
		/// </summary>
		[SerializeField]
		private Transform contentPanelTransform;
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// The prefab to use when creating the buttons inside the content panel.
		/// </summary>
		[SerializeField, Title("Resources")]
		private GameObject exampleButtonPrefab;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		private void Start() {
			
			// Testing this out.
			GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: () => {
				this.SetMouseInput(true);
			});
			
			// On start, prep the buttons.
			this.PrepareExampleScripts(chatScripts: this.debugTextChatScripts);

#if UNITY_EDITOR
			this.PrepareExampleScripts(chatScripts: this.debugSerializedChatScripts, initialPos: this.debugTextChatScripts.Count);
#endif
			
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Creates buttons from the specified text assets and adds them to the content panel.
		/// </summary>
		/// <param name="chatScripts">The chat scripts to use in prepping the buttons</param>
		private void PrepareExampleScripts(List<TextAsset> chatScripts, int initialPos = 0) {

			int i = initialPos;
			
			foreach (TextAsset chatScript in chatScripts) {
				GameObject button = (GameObject) Instantiate(this.exampleButtonPrefab);
				button.GetComponent<ExampleChatScriptButton>().Prepare(textAsset: chatScript, scriptName: chatScript.name);
				button.transform.SetParent(this.contentPanelTransform);
				button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0f, y: i * -70f);
				i += 1;
			}
		}
		/// <summary>
		/// Creates buttons from the specified serialized chat scripts and adds them to the content panel.
		/// </summary>
		/// <param name="chatScripts"></param>
		/// <param name="initialPos"></param>
		private void PrepareExampleScripts(List<SerializedChatScriptDX> chatScripts, int initialPos = 0) {
			int i = initialPos;
			foreach (SerializedChatScriptDX chatScript in chatScripts) {
				GameObject button = (GameObject)Instantiate(this.exampleButtonPrefab);
				button.GetComponent<ExampleChatScriptButton>().Prepare(serializedChatScript: chatScript, scriptName: chatScript.name);
				button.transform.SetParent(this.contentPanelTransform);
				button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0f, y: i * -70f);
				i += 1;
			}
		}
		#endregion

		#region EVENT HANDLING
		/// <summary>
		/// Gets called when the help button is clicked.
		/// </summary>
		public void ShowHelpButtonClicked() {
			this.allDocumentationVisuals.SetActive(true);
			this.allConsoleVisuals.SetActive(false);
		}
		/// <summary>
		/// Gets called when the dismiss help button is clicked.
		/// </summary>
		public void DismissHelpButtonClicked() {
			this.allDocumentationVisuals.SetActive(false);
			this.allConsoleVisuals.SetActive(true);
		}
		/// <summary>
		/// Gets called when an example button is hit.
		/// </summary>
		/// <param name="textAsset">The text asset associated with the example button.</param>
		public void ScriptExampleButtonSubmitted(TextAsset textAsset) {
			this.scriptInputField.text = textAsset.text;
			this.isTextAsset = true;
			this.currentlySelectedSerializedChatScript = null;
		}
		/// <summary>
		/// Gets called when an example button is hit.
		/// </summary>
		/// <param name="serializedChatScript">The script that was sent over.</param>
		public void ScriptExampleButtonSubmitted(SerializedChatScriptDX serializedChatScript) {
			this.scriptInputField.text = "This is a serialized script. No preview is available.";
			this.isTextAsset = false;
			this.currentlySelectedSerializedChatScript = serializedChatScript;
		}
		/// <summary>
		/// Gets called when the submit button under the input field is pressed.
		/// </summary>
		public void TextInputSubmitted() {
			// i hate THIS
			Debug.Log("SUBMITTING TEXT INPUT");
			if (this.isTextAsset == true) {
				this.ExecuteTextAssetScript(text: this.scriptInputField.text);
			} else {
				this.ExecuteSerializedChatScript(serializedChatScript: this.currentlySelectedSerializedChatScript);
			}
			
			
		}
		/// <summary>
		/// Executes the chat script via text.
		/// </summary>
		/// <param name="text"></param>
		private void ExecuteTextAssetScript(string text) {

			// Enable the script that allows reloading of the scene.
			this.GetComponent<ReloadSceneOnKeystroke>().enabled = true;

			// Disable mouse input.
			this.SetMouseInput(status: false);

			// Get the text.
			string inputText = text;

			// Create a script from it.
			PlainChatScript plainChatScript = new PlainChatScript(rawText: inputText);

			// Turn the input field off.
			this.allConsoleVisuals.SetActive(false);
			// Also tell the event system to deselect the input text and disallow mouse input.
			EventSystem.current.SetSelectedGameObject(null);
			this.SetMouseInput(false);

			// Send it to the chat controller with the callback of re-enabling the input field.
			ChatControllerDX.GlobalOpen(chatScript: plainChatScript, chatClosedCallback: ((str, num, toggle) => {
				Debug.Log("CHAT FINISHED WITH VARIABLES:\nstr: " + str + "\nnum: " + num + "\ntoggle:" + toggle);
				this.SetMouseInput(true);
				this.allConsoleVisuals.SetActive(true);
				this.GetComponent<ReloadSceneOnKeystroke>().enabled = false;
			}));
		}
		/// <summary>
		/// Executes the chat script via an asset.
		/// </summary>
		/// <param name="serializedChatScript"></param>
		private void ExecuteSerializedChatScript(SerializedChatScriptDX serializedChatScript) {
			// Enable the script that allows reloading of the scene.
			this.GetComponent<ReloadSceneOnKeystroke>().enabled = true;

			// Disable mouse input.
			this.SetMouseInput(status: false);

			// Turn the input field off.
			this.allConsoleVisuals.SetActive(false);
			// Also tell the event system to deselect the input text and disallow mouse input.
			EventSystem.current.SetSelectedGameObject(null);
			this.SetMouseInput(false);

			// Send it to the chat controller with the callback of re-enabling the input field.
			ChatControllerDX.GlobalOpen(chatScript: serializedChatScript, chatClosedCallback: ((str, num, toggle) => {
				Debug.Log("CHAT FINISHED WITH VARIABLES:\nstr: " + str + "\nnum: " + num + "\ntoggle:" + toggle);
				this.SetMouseInput(true);
				this.allConsoleVisuals.SetActive(true);
				this.GetComponent<ReloadSceneOnKeystroke>().enabled = false;
			}));
		}
		/// <summary>
		/// Sets whether or not mouse input should be allowed.
		/// Relevant for when I'm going back and forth between the advance button and the input field.
		/// </summary>
		/// <param name="status">Whether or not mouse input should be allowed.</param>
		private void SetMouseInput(bool status) {
			Debug.Log("SETTING MOUSE INPUT STATUS TO " + status);
			// FindObjectOfType<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = status;
			// InputController.Instance.GetComponent<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = status;
			InputController.Instance.GetComponent<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = status;
		}
		#endregion
		
	}

}