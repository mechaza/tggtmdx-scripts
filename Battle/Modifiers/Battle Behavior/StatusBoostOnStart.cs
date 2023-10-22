using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {


	/// <summary>
	/// Applies the specified status boosts on startup.
	/// </summary>
	[System.Serializable]
	[TypeInfoBox("Applies the specified status boosts on startup.")]
	public class StatusBoostOnStart : BattleBehaviorModifier, IOnBattleStart {

		#region FIELDS
		[SerializeField]
		private List<PowerBoostType> powerBoostTypes = new List<PowerBoostType>();
		#endregion


		#region INTERFACE IMPLEMENTATION - IONBATTLESTART
		public BattleReaction OnBattleStart() {
			Debug.Log("TESTING: HERE IS BATTLE START BEING CALLED");
			return delegate (BattleReactionSequence battleReactionSequence) {
				Debug.Log("TESTING: HERE ITS BEING RUN!!!");
				this.combatant.statusModifiers.SetPowerBoost(boostTypes: this.powerBoostTypes, debuff: false);
				battleReactionSequence.ExecuteNextReaction();
			};
			
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Applies the specified status boosts on startup.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion*/

	}


}