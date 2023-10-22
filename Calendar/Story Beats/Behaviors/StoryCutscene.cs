using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Chat;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Grawly.Battle;
using Grawly.Cutscenes;

namespace Grawly.Calendar.Behavior.General {
	
	/// <summary>
	/// Plays a cutscene. Note that there should only be one cutscene per scene as I've set the game up for right now.
	/// </summary>
	[Title("Story Cutscene")]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class StoryCutscene : StoryBehavior {
		
		#region OVERRIDES
		/// <summary>
		/// Tell the cutscene controller to begin, and that when its done it should execute the next reaction.
		/// </summary>
		/// <returns></returns>
		public override StoryBeatReaction OnStoryBeatLoad() {
			return delegate(StoryBeatReactionSequence sequence) {
				CutsceneController.Instance.PlayCutscene(onCutsceneComplete: () => {
					sequence.ExecuteNextReaction();
				});
			};
		}
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// The string to use for the foldout groups in the inspector.
		/// </summary>
		protected override string FoldoutGroupTitle {
			get {
				return "Story Cutscene";

			}
		}
		#endregion
		
	}
}