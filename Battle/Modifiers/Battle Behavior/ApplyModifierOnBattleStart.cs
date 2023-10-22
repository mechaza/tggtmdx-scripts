using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Applies the specified modifier to the combatant at the beginning of the battle.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Applies the specified modifiers to the combatant at the beginning of the battle.")]
	public class ApplyModifierOnBattleStart : BattleBehaviorModifier, IOnBattleStart {

		#region FIELDS - MODIFIERS
		/// <summary>
		/// A list of modifiers to add at the beginning of the battle.
		/// </summary>
		[SerializeField]
		private List<CombatantModifier> modifiersToAdd = new List<CombatantModifier>();
		#endregion

		#region INTERFACE IMPLEMENTATION - IONBATTLESTART
		public BattleReaction OnBattleStart() {
			return delegate (BattleReactionSequence battleReactionSequence) {
				Debug.Log(this.combatant.metaData.name + " has a move that add 1 or more modifiers at the start of the battle. Adding them now.");
				this.modifiersToAdd.ForEach(m => this.combatant.AddModifier(modifier: m.Clone()));
				battleReactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Applies the specified modifiers to the combatant at the beginning of the battle.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion*/

	}


}