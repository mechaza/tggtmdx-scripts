using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif


namespace Grawly.Battle.BehaviorAnimation {

	/// <summary>
	/// The animation that gets played when this behavior is used.
	/// Effectively takes controls of most of the visuals.
	/// </summary>
	public abstract class BattleBehaviorAnimation {

		#region ROUTINES
		/// <summary>
		/// Runs the animation as normal. Remember that this takes place before I have actually evaluated the damage calculations themselves (e.x., enemies being hit still have their original HP etc)
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		public abstract IEnumerator ExecuteAnimation(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Gets run if the source of the move was a player.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		protected abstract IEnumerator PlayerSourceAnimationRoutine(DamageCalculationSet damageCalculationSet);
		/// <summary>
		/// Gets run if the source of the move was an enemy.
		/// </summary>
		/// <param name="damageCalculationSet">The DamageCalculationSet associated with the behavior being evaluated right now.</param>
		/// <returns></returns>
		protected abstract IEnumerator EnemySourceAnimationRoutine(DamageCalculationSet damageCalculationSet);
		#endregion

		#region INSPECTOR BULLSHIT
#if UNITY_EDITOR
		/// <summary>
		/// This is what I need to use for making sure info boxes appear in the inspector without actually having to assign a field to accompany it.
		/// </summary>
		[PropertyOrder(int.MinValue), OnInspectorGUI]
		private void DrawIntroInfoBox() {
			SirenixEditorGUI.InfoMessageBox(this.InspectorDescription);
		}
#endif
		/// <summary>
		/// The string that gets used in the info box that describes this BattleBehaviorFunction.
		/// </summary>
		protected abstract string InspectorDescription { get; }
		#endregion

	}


}