using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.Battle.Modifiers;
using Grawly.Battle;
using Grawly.Chat;
using DG.Tweening;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// Makes enemies stronger.
	/// </summary>
	[InfoBox("Makes enemies stronger.")]
	[System.Serializable]
	public class StrongerEnemies : ShuffleCardBehavior, IOnBattleStart {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount to boost an enemy's attack by at the start of the battle.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private float attackMultiplier = 1f;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONBATTLESTART
		/// <summary>
		/// On battle start, give all the enemies a boost.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnBattleStart() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				// Go through each enemy and add a new modifier for a power boost.
				BattleController.Instance.Enemies.ForEach(e => e.AddModifier(
					// The modifier being added is persistent throughout the battle.
					modifier: new AttackAmplifier(
						multiplier: this.attackMultiplier, 
						elementTypes: System.Enum.GetValues(typeof(ElementType)),
						removeOnAttack: false
					)));
				// battleReactionSequence.ExecuteNextReaction();
				ChatControllerDX.GlobalOpen(
					scriptLine: "testing... the card works",
					chatClosedCallback: delegate {
						GameController.Instance.WaitThenRun(
							timeToWait: 0.5f,
							action: delegate {
								battleReactionSequence.ExecuteNextReaction();
							});
					});
			};
		}
		#endregion

	}


}