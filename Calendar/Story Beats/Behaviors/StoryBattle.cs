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

namespace Grawly.Calendar.Behavior.General {
	
	[Title("Story Battle")]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
	public class StoryBattle : StoryBehavior {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The battle template to run when this behavior is run.
		/// </summary>
		[SerializeField]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private BattleTemplate battleTemplate;
		#endregion
		
		#region OVERRIDES
		/// <summary>
		/// Start up the battle upon loading this story behavior.
		/// </summary>
		/// <returns></returns>
		public override StoryBeatReaction OnStoryBeatLoad() {
			return delegate(StoryBeatReactionSequence sequence) {
				BattleController.Instance.StartBattle(
					battleTemplate: this.battleTemplate,
					freePlayerOnReturn: false,
					onBattleReturn: () => {
						// When returning from the results screen, the next reaction in the sequence should be executed.
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
				if (this.battleTemplate != null) {
					return "Run Story Battle (" + this.battleTemplate.BattleName + ")";
				} else {
					return "Run Story Battle (UNDEFINED)";
				}
				
			}
		}
		#endregion
		
	}
}