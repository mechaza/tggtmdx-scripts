using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Toggles.Proto {
	
	/// <summary>
	/// Disables any backgrounds for the chat window that isn't a solid color.
	/// This is being implemented because I'm having issues on macOS with the checker background.
	/// </summary>
	[System.Serializable, Title("Force Solid ChatBox")]
	public class DisableSpecialChatBackgrounds : StandardBoolToggle {
		
		#region FIELDS - METADATA
		public override string ToggleName => "Force Solid ChatBox";
		public override string ToggleDescription => "Forces the chat box to be a solid color and not use any special effects..";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		#endregion
		
	}
}