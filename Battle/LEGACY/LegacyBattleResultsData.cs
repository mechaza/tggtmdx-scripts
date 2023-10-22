using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle {

	/// <summary>
	/// Holds data collected from the battle to be used for evaluation when the battle is complete. Holds EXP, Money, and Items.
	/// </summary>
	public class LegacyBattleResultsData {
		/// <summary>
		/// The amount of EXP obtained from this battle.
		/// </summary>
		public int exp;
		/// <summary>
		/// The amount of money obtained from this battle.
		/// </summary>
		public int money;
		/// <summary>
		/// The items obtained from this battle.
		/// </summary>
		public List<BattleBehavior> items = new List<BattleBehavior>();

		#region CONSTRUCTORS
		/// <summary>
		/// Creates an empty BattleResultsData.
		/// </summary>
		public LegacyBattleResultsData() {
		}
		/// <summary>
		/// Creates a battle results data from a list of templates.
		/// </summary>
		/// <param name="battleTemplates"></param>
		public LegacyBattleResultsData(BattleTemplate battleTemplate) {
			this.exp = battleTemplate.EnemyTemplates.Sum(et => et.drops.exp);
			this.items = battleTemplate.EnemyTemplates.SelectMany(et => et.drops.items).ToList();
			this.money = battleTemplate.EnemyTemplates.Sum(et => et.drops.money);

			this.exp =(int)((float)this.exp * GameController.Instance.DifficultyToggles.experienceMultiplier);
			this.money =(int)((float)this.money * GameController.Instance.DifficultyToggles.moneyMultiplier);

		}
		#endregion

	}

}