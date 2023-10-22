using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// The class that defines how GauntletNodes should respond to certain events handed their way.
	/// </summary>
	public abstract class GauntletNodeBehavior {

		#region FIELDS - PARENT NODE
		/// <summary>
		/// The GauntletNode that owns this behavior.
		/// </summary>
		protected GauntletNode gauntletNodeOwner;
		#endregion

		#region INITIALIZATION
		/// <summary>
		/// Assigns a node to this behavior. 
		/// Is important for remembering shit.
		/// </summary>
		/// <param name="gauntletNode">The gauntlet node to assign to this behavior.</param>
		public void AssignGauntletNode(GauntletNode gauntletNode) {
			this.gauntletNodeOwner = gauntletNode;
		}
		#endregion

		/*#region INTERFACE IMPLEMENTATION - IGAUNTLETNODETITLEUSER
		/// <summary>
		/// Should the metadata visuals be visible on the node title?
		/// </summary>
		public abstract bool UseMetadataVisuals { get; }
		/// <summary>
		/// The status of this node. Mostly for the node title graphics.
		/// </summary>
		public abstract GauntletNodeStatusType NodeStatusType { get; }
		/// <summary>
		/// The string to use on the node title graphics.
		/// </summary>
		public abstract string NodeTitleString { get; }
		/// <summary>
		/// The string for the primary label on the metadata.
		/// </summary>
		public abstract string PrimaryMetadataString { get; }
		/// <summary>
		/// The string for the secondary label on the metadata.
		/// </summary>
		public abstract string SecondaryMetadataString { get; }
		/// <summary>
		/// The string that gets displayed on the Completed thingy.
		/// </summary>
		public abstract string TertiaryMetadataString { get; }
		/// <summary>
		/// The sprite to use on the node title.
		/// </summary>
		public abstract Sprite NodeTitleSprite { get; }
		#endregion*/



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