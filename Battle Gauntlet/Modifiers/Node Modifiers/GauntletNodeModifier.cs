using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet.Nodes;
using Grawly.Battle;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Gauntlet.Modifiers {

	/// <summary>
	/// A gauntlet node may have modifiers attached to them. 
	/// </summary>
	public abstract class GauntletNodeModifier {

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
		/// The string that gets used in the info box that describes this GauntletNodeModifier.
		/// </summary>
		protected abstract string InspectorDescription { get; }
		#endregion

	}


}