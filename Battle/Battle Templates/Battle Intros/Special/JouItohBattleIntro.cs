using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Dungeon;
using Grawly.Calendar;
using Grawly.UI;
using Grawly.Battle.BattleArena;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;
using Grawly.Dungeon.UI;
using Grawly.DungeonCrawler;
using Sirenix.Serialization;


namespace Grawly.Battle.Intros.Special {
	
	/// <summary>
	/// The intro to use when battling Jou Itoh.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("The intro to use when battling Jou Itoh.")]
	public class JouItohBattleIntro : EasyBattleIntro {

		#region BATTLE PREPARATION
		public override void PlayIntro(BattleTemplate template, BattleParams battleParams) {
			Debug.Log("JOU ITOH BATTLE NOW");
			base.PlayIntro(template, battleParams);
		}
		#endregion
		
	}
}