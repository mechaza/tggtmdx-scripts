using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Grawly.Dungeon;
using Grawly.Battle;

namespace Grawly.PlayMakerActions {

	[ActionCategory("Grawly - Battle"), Tooltip("Opens up a battle with the specified battle template.")]
	public class PrepareBattle : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The BattleTemplate to open up this battle with.
		/// </summary>
		[ObjectType(typeof(Battle.BattleTemplate))]
		public FsmObject battleTemplate;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			// Tell the dungeon controller to prep the battle with the given template.
			// DungeonController.Instance.PrepareBattle(battleTemplate: (BattleTemplate)this.battleTemplate.Value);
			throw new System.NotImplementedException("i removed the dungeon controller");
			base.Finish();
		}
		#endregion

	}


}