using System.Collections;
using System.Collections.Generic;
using Grawly.Battle.BattleMenu;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Only allows for the specified top level selections to be used in battle.
	/// </summary>
	[System.Serializable]
	public class UnescapableBattle : BattleModifier, ITopLevelSelectionRestrictor {

		#region FIELDS - TOGGLES
		[SerializeField, TabGroup("Modifier", "Toggles")]
		private List<BattleMenuDXTopLevelSelectionType> usableTopLevelSelections = new List<BattleMenuDXTopLevelSelectionType>() {
			BattleMenuDXTopLevelSelectionType.Analysis,
			BattleMenuDXTopLevelSelectionType.Attack,
			BattleMenuDXTopLevelSelectionType.BatonTouch,
			BattleMenuDXTopLevelSelectionType.Escape,
			BattleMenuDXTopLevelSelectionType.Guard,
			BattleMenuDXTopLevelSelectionType.Item,
			BattleMenuDXTopLevelSelectionType.Masque,
			BattleMenuDXTopLevelSelectionType.Skill,
		};
		#endregion

		/// <summary>
		/// Returns the list of top level selections that are allowed.
		/// </summary>
		/// <returns></returns>
		public List<BattleMenuDXTopLevelSelectionType> RestrictTopLevelSelections() {
			return this.usableTopLevelSelections;
		}
	}


}