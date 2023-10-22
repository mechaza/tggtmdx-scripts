using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
 
namespace Grawly.Battle.Functions {

	/// <summary>
	/// This is what gets run when the player picks the Escape option.
	/// </summary>
	[System.Serializable]
	public class Escape : BattleBehaviorFunction {

		#region FUNCTION
		/// <summary>
		/// Execute the function and perform the necessary calculations.
		/// </summary>
		/// <param name="source">Who the move is originating from.</param>
		/// <param name="targets">A list of targets that are being affected by the move.</param>
		/// <param name="battleBehavior">The BattleBehavior this function is attached to.</param>
		public override void Execute(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			BattleController.Instance.StartCoroutine(this.ExecuteAsCoroutine(source: source, targets: targets, battleBehavior: battleBehavior));
		}
		private IEnumerator ExecuteAsCoroutine(Combatant source, List<Combatant> targets, BattleBehavior battleBehavior) {
			Debug.LogWarning("ENDING THE BATTLE RIGHT NOW.");
			yield return new WaitForSeconds(1.5f);
			BattleController.Instance.FSM.SendEvent("Battle Complete");
		}
		#endregion

		#region CALCULATION DELEGATES 
		protected override DamageCalculationSet GenerateDamageCalculationSet(Combatant source, List<Combatant> targets, BattleBehavior self) {
			throw new System.NotImplementedException();
		}
		#endregion

		#region INSPECTOR BULLSHIT
		/// <summary>
		/// A description of the BattleBehaviorFunction. Keeping it as a ref so a string doesn't need to be generated every OnInspectorGUI call. Wastes memory. Etc.
		/// </summary>
		private static string descriptionText = "The function to run when the player picks the escape option from the top level buttons.";
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