using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Gauntlet.Nodes;

namespace Grawly.Gauntlet {

	#region CLASSES
	/// <summary>
	/// Contains commonly used variables that get passed into gauntlet events.
	/// </summary>
	public class GauntletNodeEventParams {

		#region FIELDS
		/// <summary>
		/// The GauntletNode the event was spawned on.
		/// </summary>
		public GauntletNode GauntletNode { get; private set; }
		/// <summary>
		/// The current gauntlet marker.
		/// </summary>
		public GauntletMarker GauntletMarker { get; private set; }
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Contains commonly used variables that get passed into gauntlet events.
		/// </summary>
		public GauntletNodeEventParams(GauntletNode gauntletNode = null, GauntletMarker gauntletMarker = null) {
			this.GauntletNode = gauntletNode;
			this.GauntletMarker = gauntletMarker;
		}
		#endregion

	}
	#endregion

	#region ENUMS
	/// <summary>
	/// A simple type to help me when creating inspectors.
	/// </summary>
	public enum GauntletNodeEventType {
		OnEnter = 0,
		OnExit = 1,
		OnSubmit = 2,
		OnComplete = 3,
	}
	#endregion

	#region GAMEPLAY EVENTS - GENERAL
	/// <summary>
	/// Anything that needs to know about when the gauntlet marker moves implements this.
	/// </summary>
	public interface IMarkerMoveReactor {

	}
	public interface IOnGauntletStart {
		void OnGauntletStart();
	}
	#endregion

	#region GAMEPLAY EVENTS - NODE SPECIFIC
	/// <summary>
	/// Gets called when a marker enters the node that owns whatever modifier implements this interface.
	/// </summary>
	public interface IOnEnterNode {
		/// <summary>
		/// Gets called when the player marker enters a node that implements this interface.
		/// </summary>
		/// <param name="gauntletNode">The gauntlet node that was just entered.</param>
		/// <param name="gauntletMarker">The marker that just entered the node.</param>
		/// <returns>The reaction that should be evaluated upon entering this node.</returns>
		GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams);
	}
	/// <summary>
	/// Gets called when a marker exits the node that owns whatever modifier implements this interface.
	/// </summary>
	public interface IOnExitNode {
		/// <summary>
		/// Gets called when the player marker exits a node that implements this interface.
		/// </summary>
		/// <param name="gauntletNode">The gauntlet node that was just exited.</param>
		/// <param name="gauntletMarker">The marker that just exited the node.</param>
		/// <returns>The reaction that should be evaluated upon exiting this node.</returns>
		GauntletReaction OnExitNode(GauntletNodeEventParams eventParams);
	}
	/// <summary>
	/// Gets called when the Submit event is sent from the Unity event system over to the selectable on the GauntletNode.
	/// </summary>
	public interface IOnSubmitNode {
		/// <summary>
		/// Gets called when the Submit event is sent from the Unity event system over to the selectable on the GauntletNode.
		/// </summary>
		/// <param name="gauntletNode">The GauntletNode that was submitted.</param>
		/// <returns>The reaction that should be evaluated upon submission of this node.</returns>
		GauntletReaction OnSubmitNode(GauntletNodeEventParams eventParams);
	}
	/// <summary>
	/// Some nodes have a state that marks them as complete. This should be run when that happens.
	/// </summary>
	public interface IOnCompleteNode {
		/// <summary>
		/// Gets called when a Complete event is sent over to this node.
		/// </summary>
		/// <param name="gauntletNode">The node that has whatever this behavior belongs to.</param>
		/// <returns></returns>
		GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams);
	}
	#endregion

	#region UI HANDLERS
	/// <summary>
	/// Any class that needs to use the GauntletNodeTitle should implement this so that it can design the node title any way it wants to.
	/// </summary>
	public interface IGauntletNodeTitleUser {
		/// <summary>
		/// Should the metadata visuals be visible on the node title?
		/// </summary>
		bool UseMetadataVisuals { get; }
		/// <summary>
		/// The status of this node. Mostly for the node title graphics.
		/// </summary>
		GauntletNodeStatusType NodeStatusType { get; }
		/// <summary>
		/// The string to use on the node title graphics.
		/// </summary>
		string NodeTitleString { get; }
		/// <summary>
		/// The string for the primary label on the metadata.
		/// </summary>
		string PrimaryMetadataString { get; }
		/// <summary>
		/// The string for the secondary label on the metadata.
		/// </summary>
		string SecondaryMetadataString { get; }
		/// <summary>
		/// The string on the uh. Completed thigny.
		/// </summary>
		string TertiaryMetadataString { get; }
		/// <summary>
		/// The sprite to use on the node title.
		/// </summary>
		Sprite NodeTitleSprite { get; }
	}
	#endregion

}