using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle.Functions {

	/// <summary>
	/// Just a simple way to test out the pause menu. I might actually use this though.
	/// </summary>
	[System.Serializable]
	public class LevelUp : PauseBehaviorFunction {

		#region FIELDS - INHERITED
		/// <summary>
		/// Is this function asynchonous? I.e., healing probably isn't, but the menu to bring up learning a skill card is.
		/// </summary>
		public override bool IsAsynchronous {
			get {
				return false;
			}
		}
		#endregion

		#region FUNCTION
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior self) {
			// Go through each of the targets and add the exp required fro them to level up.
			targets.ForEach(c => c.TotalEXP += c.ExpForNextLevel());
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "Just makes them level up.";
		/// <summary>
		/// A string that details how this function works.
		/// </summary>
		protected override string InspectorDescription {
			get {
				return descriptionText;
			}
		}
		#endregion

	}


}