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
	/// Sets a given story flag to a specified value.
	/// </summary>
	[Title("Set Story Flag")]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class SetStoryFlag : StoryBehavior {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The flag to update when this behavior is run.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private StoryFlagType storyFlagType = StoryFlagType.None;
		/// <summary>
		/// The status to assign to the flag type.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private bool flagStatus = true;
		#endregion
		
		#region OVERRIDES
		public override StoryBeatReaction OnStoryBeatLoad() {
			return delegate(StoryBeatReactionSequence sequence) {
				// Set the flag in the variables.
				GameController.Instance.Variables.StoryFlags.SetFlag(flagType: this.storyFlagType, value: this.flagStatus);
				// Execute the next reaction.
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
				return "Set Story FLag (" + this.storyFlagType.ToString() + " -- " + this.flagStatus.ToString() + ")";
			}
		}
		#endregion
		
	}
}