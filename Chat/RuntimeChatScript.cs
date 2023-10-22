using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// The version of a chat script that gets used at runtime.
	/// Important because I need some amount of mutable state.
	/// </summary>
	public class RuntimeChatScript {

		#region FIELDS - CONTROL STATE
		/// <summary>
		/// The current line being read out from the script.
		/// </summary>
		private int currentIndex = -1;
		#endregion

		#region FIELDS - SCRIPT
		/// <summary>
		/// The directives that get read from at runtime. Should not be manipulated.
		/// </summary>
		private readonly IList<ChatDirective> chatDirectives;
		/// <summary>
		/// A cache for labels in the script and the indicies where they can be found.
		/// </summary>
		private readonly IDictionary<string, int> labelDict;
		/// <summary>
		/// The speakers that should be used for this script.
		/// </summary>
		private List<RuntimeChatSpeaker> runtimeChatSpeakers;
		#endregion

		#region FIELDS - COMPUTED STUFF
		/// <summary>
		/// Returns the ChatDirective that is currently being evaluated.
		/// </summary>
		public ChatDirective CurrentDirective {
			get {
				return this.chatDirectives[this.currentIndex];
			}
		}
		#endregion

		#region FIELDS - RUNTIME VARIABLES
		/// <summary>
		/// A basic variable to be manipulated via misc means during a script and passed back to a ChatClosedCallback.
		/// </summary>
		public string str { get; private set; } = "";
		/// <summary>
		/// A basic variable to be manipulated via misc means during a script and passed back to a ChatClosedCallback.
		/// </summary>
		public int num { get; private set; } = 0;
		/// <summary>
		/// A basic variable to be manipulated via misc means during a script and passed back to a ChatClosedCallback.
		/// </summary>
		public bool toggle { get; private set; } = false;
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Processes a chat script template to be used at runtime.
		/// </summary>
		/// <param name="chatScript">The chat script that is going to be used.</param>
		/// <param name="chatClosedCallback">A callback that should be run when the chat is closed.</param>
		public RuntimeChatScript(IChatScript chatScript) {

			// Grab the chat speakers from the script.
			this.runtimeChatSpeakers = chatScript.RuntimeChatSpeakers;

			// Initialize the dictionary of active speakers.
			// this.activeSpeakerDict = RuntimeChatScript.GetDefaultActiveSpeakerDict();

			// Get the directives. Note that in this form, they cannot be manipulated.
			this.chatDirectives = chatScript.Directives.AsReadOnly();

			// Also get the labels.
			this.labelDict = RuntimeChatScript.GetLabels(chatDirectives: this.chatDirectives.ToList());

			// Set the closed callback.
			// this.chatClosedCallback = chatClosedCallback;
		}
		#endregion

		#region CONTROL
		/// <summary>
		/// Gets the next directive to be evaluated.
		/// </summary>
		/// <returns>The directive that should be evaluated.</returns>
		public ChatDirective GetNextDirective() {
			// Increment the current index.
			this.currentIndex += 1;
			// Return the directive that should be evaluated next.
			try {
				return this.chatDirectives[this.currentIndex];
			} catch (System.Exception e) {
				Debug.Log("No more directives available. Returning close directive.");
				return new CloseDirective();
			}
		}
		/// <summary>
		/// Tells the runtime script to jump to the index of the specified label.
		/// </summary>
		/// <param name="label">The label to jump to.</param>
		public void JumpToLabel(string label) {
			Debug.Log("Jumping to label: " + label);
			// Override the current index to the one stored in the dictionary.
			this.currentIndex = this.labelDict[label];
		}
		#endregion

		#region SCRIPT GETTERS
		/// <summary>
		/// Returns the speaker associated with the given name.
		/// </summary>
		/// <param name="speakerShorthand">The shorthand name of the speaker.</param>
		/// <returns>The asset that contains all the other data for the speaker.</returns>
		public RuntimeChatSpeaker GetRuntimeChatSpeaker(string speakerShorthand) {
			// Go through the runtime chat speakers and find the first one whos shorthand matches the parameter
			try {
				return this.runtimeChatSpeakers
				.First(rcs => rcs.SpeakerShorthand.ToLower() == speakerShorthand.ToLower());
			} catch (System.Exception e) {
				Debug.LogWarning("Could not find speaker for shorthand: " + speakerShorthand + ". Returning null.");
				return null;
			}
		}
		#endregion

		#region RUNTIME VARIABLES
		/// <summary>
		/// Sets the integer variable. Used for misc kinds of logic.
		/// </summary>
		/// <param name="num"></param>
		public void SetIntVariable(int num) {
			Debug.Log("CHAT: Setting the int variable to " + num);
			this.num = num;
		}
		/// <summary>
		/// Sets the bool variable. Used for misc kinds of logic.
		/// </summary>
		/// <param name="toggle"></param>
		public void SetBoolVariable(bool toggle) {
			Debug.Log("CHAT: Setting the bool variable to " + toggle);
			this.toggle = toggle;
		}
		/// <summary>
		/// Sets the string variable. Used for misc kinds of logic.
		/// </summary>
		/// <param name="str"></param>
		public void SetStringVariable(string str) {
			Debug.Log("CHAT: Setting the string variable to " + str);
			this.str = str;
		}
		#endregion

		#region PURE FUNCTIONS
		/// <summary>
		/// Returns a dictionary that contains string/int pairs to help go back and forth between labels in the script.
		/// </summary>
		/// <param name="chatDirectives">The directives to be parsed.</param>
		/// <returns>A dictionary with labels as keys and indicies as values.</returns>
		private static Dictionary<string, int> GetLabels(List<ChatDirective> chatDirectives) {

			// Prepare a new label dict.
			Dictionary<string, int> labelDict = new Dictionary<string, int>();
			// Create a number to keep track of the index.
			int index = 0;

			// Go through each directive and check if its a label.
			chatDirectives.ForEach(c => {
				if (c is LabelDirective) {
					// If it is, grab the label and add it to the dict.
					string label = ((LabelDirective)c).label;
					labelDict.Add(key: label, value: index);
				}
				// In any case, increment the index.
				index += 1;
			});

			// When all is said and done, return the label dict.
			return labelDict;

		}
		#endregion


	}
}