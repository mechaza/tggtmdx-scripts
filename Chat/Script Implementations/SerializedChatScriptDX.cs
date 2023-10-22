using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grawly.Chat {

	/// <summary>
	/// Gonna try re-adding this... with a twist!
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Chat/Serialized Chat Script")]
	public class SerializedChatScriptDX : SerializedScriptableObject, IChatScript {

		#region FIELDS - METADATA
		/// <summary>
		/// The name for this script.
		/// Mostly for organization.
		/// </summary>
		[HorizontalGroup("Split", width: 0.3f)]
		[BoxGroup("Split/Left"), LabelWidth(100f)]
		public string chatScriptName = "";
		#endregion

		#region FIELDS - SPEAKERS
		/// <summary>
		/// The chat speakers to be used at runtime, with their shorthand already defined.
		/// </summary>
		[SerializeField, BoxGroup("Split/Left"), ListDrawerSettings(DraggableItems = false), HideLabel]
		private List<RuntimeChatSpeaker> runtimeSpeakers = new List<RuntimeChatSpeaker>();
		#endregion

		#region ODIN BUTTONS
		/// <summary>
		/// Quickly adds a raw directive to the end of the list.
		/// </summary>
		// [BoxGroup("Quick Actions"), ButtonGroup("Quick Actions/Quick Add"), LabelText("Raw")]
		[VerticalGroup("Split/Left/Quick Add")]
		// [BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add"), LabelText("Raw")]
		[BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add/1"), LabelText("Raw")]
		private void AddRawDirective() {
			this.chatDirectives.Add(new RawDirective());
		}
		/// <summary>
		/// Quickly adds a dialogue directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		// [BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add"), LabelText("Dialogue")]
		[BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add/2"), LabelText("Dialogue")]
		private void AddDialogueDirective() {
			this.chatDirectives.Add(new DialogueDirective());
		}
		/// <summary>
		/// Quickly adds a show directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		// [BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add"), LabelText("Show")]
		[BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add/2"), LabelText("Show")]
		private void AddShowDirective() {
			this.chatDirectives.Add(new ShowDirective());
		}
		/// <summary>
		/// Quickly adds a label directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add/2"), LabelText("Label")]
		private void AddLabelDirective() {
			this.chatDirectives.Add(new LabelDirective());
		}
		/// <summary>
		/// Quickly adds a jump directive to the end of the list.
		/// Button gets accessed in the inspector.
		/// </summary>
		[BoxGroup("Split/Left"), ButtonGroup("Split/Left/Quick Add/2"), LabelText("Jump")]
		private void AddJumpDirective() {
			this.chatDirectives.Add(new JumpDirective());
		}
		#endregion

		#region FIELDS - DIRECTIVES
		/// <summary>
		/// The ChatDirectives to use for this script.
		/// </summary>
		[SerializeField, BoxGroup("Split/Right"), HideLabel, ListDrawerSettings(Expanded = true)]
		private List<ChatDirective> chatDirectives = new List<ChatDirective>();
		#endregion

		#region INTERFACE IMPLEMENTATION - ICHATSCRIPT
		/// <summary>
		/// The speakers being used for this script.
		/// </summary>
		public List<RuntimeChatSpeaker> RuntimeChatSpeakers {
			get {
				return this.runtimeSpeakers;
			}
		}
		/// <summary>
		/// The ChatDirectives for this script.
		/// </summary>
		public List<ChatDirective> Directives {
			get {
				return this.chatDirectives
					.SelectMany(c => {
						if (c is RawDirective) {
							// If there is a raw directive, transform it into its own list.
							return RawDirectiveParser.GetDirectives(text: (c as RawDirective).rawText);
						} else {
							// If this directive is not raw, return a new list with just the single element.
							return new List<ChatDirective>() { c };
						}
					})
					.ToList();
			}
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Renames the asset name for this script file based on the title for this script.
		/// </summary>
		[HorizontalGroup("Split", width: 0.3f)]
		[BoxGroup("Split/Left"), LabelWidth(100f)]
		[Button("Rename Asset")]
		private void RenameScriptFileName() {
#if UNITY_EDITOR
			if (this.chatScriptName.IsNullOrWhitespace() == true) {
				Debug.LogError("Script Name is null or whitespace! Add text before renaming this file.");
				return;
			}

			string scriptAssetPath = AssetDatabase.GetAssetPath(this);
			AssetDatabase.RenameAsset(
				pathName: scriptAssetPath, 
				newName: this.chatScriptName);
			
#endif
		}
		#endregion
		
	}


}