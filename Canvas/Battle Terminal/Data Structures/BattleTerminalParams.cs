using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Menus.BattleTerminal {
	/// <summary>
	/// A set of data that can be used to build a battle terminal.
	/// </summary>
	public class BattleTerminalParams {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The BattleTemplates that should be used for these parameters in the battle terminal.
		/// </summary>
		public List<BattleTemplate> BattleTemplates { get; private set; } = new List<BattleTemplate>();
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Preps this set of parameters with the list of battle templates passed in.
		/// </summary>
		/// <param name="battleTemplates"></param>
		public BattleTerminalParams(List<BattleTemplate> battleTemplates) {
			this.BattleTemplates = battleTemplates;
		}
		#endregion
		
	}
}