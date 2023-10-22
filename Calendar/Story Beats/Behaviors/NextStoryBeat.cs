using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace Grawly.Calendar.Behavior.General {
	
	/// <summary>
	/// Transitions to the next story beat. Note that this MUST be the last behavior in a story beat if in use!
	/// </summary>
	// [InfoBox("Transitions to the next story beat. Note that this MUST be the last behavior in a story beat if in use!")]
	[Title("Next Story Beat")]
	[System.Serializable]
	[FoldoutGroup("$FoldoutGroupTitle")]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class NextStoryBeat : StoryBehavior {

		#region OVERRIDES
		public override StoryBeatReaction OnStoryBeatLoad() {
			return delegate(StoryBeatReactionSequence sequence) {
				CalendarController.Instance.GoToNextStoryBeat();
				sequence.ExecuteNextReaction();
			};
		}
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		protected override string FoldoutGroupTitle {
			get {
				return "Next Story Beat";
			}
		}
		#endregion
		
	}
}