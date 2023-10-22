using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Grawly.UI;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.Functions {

	/// <summary>
	/// A function that can be used in the pause menu.
	/// </summary>
	public abstract class PauseBehaviorFunction {

		#region FIELDS - EXECUTION CONTEXT
		/// <summary>
		/// Is this function asynchonous? I.e., healing probably isn't, but the menu to bring up learning a skill card is.
		/// </summary>
		public abstract bool IsAsynchronous { get; }
		#endregion

		#region EXECUTION
		/// <summary>
		/// The function to run inside the pause menu.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="targets"></param>
		/// <param name="self"></param>
		public abstract void Execute(Combatant source, List<Combatant> targets, BattleBehavior self);
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