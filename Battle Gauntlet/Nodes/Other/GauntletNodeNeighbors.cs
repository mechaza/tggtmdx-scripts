using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// The pathways that the marker can go from this node.
	/// </summary>
	[System.Serializable]
	public class GauntletNodeNeighbors {

		#region FIELDS - NEIGHBORS
		[SerializeField, BoxGroup("Up")]
		private GauntletNode upNeighborNode;
		[SerializeField, BoxGroup("Up")]
		private GauntletNodeNeighborType upNeighborType = GauntletNodeNeighborType.Alpha;

		[SerializeField, BoxGroup("Down")]
		private GauntletNode downNeighborNode;
		[SerializeField, BoxGroup("Down")]
		private GauntletNodeNeighborType downNeighborType = GauntletNodeNeighborType.Alpha;

		[SerializeField, BoxGroup("Left")]
		private GauntletNode leftNeighborNode;
		[SerializeField, BoxGroup("Left")]
		private GauntletNodeNeighborType leftNeighborType = GauntletNodeNeighborType.Alpha;

		[SerializeField,  BoxGroup("Right")]
		private GauntletNode rightNeighborNode;
		[SerializeField, BoxGroup("Right")]
		private GauntletNodeNeighborType rightNeighborType = GauntletNodeNeighborType.Alpha;
		#endregion

		#region SELECTABLE HELPERS
		/// <summary>
		/// Gets the navigation that is computed as a result of the connections between the neighbors.
		/// </summary>
		/// <param name="gauntletNode">The gauntlet node that owns this neighbors.</param>
		/// <returns></returns>
		public Navigation GetNavigation(GauntletNode gauntletNode) {
			// Create a new navigation.
			Navigation navigation = new Navigation();

			// Set its mode to explicit.
			navigation.mode = Navigation.Mode.Explicit;

			// Check the directions for if their pathways are open by probing the variables. If they are, assign the associated node.
			navigation.selectOnUp = (gauntletNode.Variables.CheckForOpenPath(neighborType: this.upNeighborType) && this.upNeighborNode != null) ? this.upNeighborNode.GetComponent<Selectable>() : null;
			navigation.selectOnDown = (gauntletNode.Variables.CheckForOpenPath(neighborType: this.downNeighborType) && this.downNeighborNode != null) ? this.downNeighborNode.GetComponent<Selectable>() : null;
			navigation.selectOnLeft = (gauntletNode.Variables.CheckForOpenPath(neighborType: this.leftNeighborType) && this.leftNeighborNode != null) ? this.leftNeighborNode.GetComponent<Selectable>() : null;
			navigation.selectOnRight = (gauntletNode.Variables.CheckForOpenPath(neighborType: this.rightNeighborType) && this.rightNeighborNode != null) ? this.rightNeighborNode.GetComponent<Selectable>() : null;

			// Return the navigation.
			return navigation;

		}
		#endregion

		#region EDITOR TOOLS
		/// <summary>
		/// Resets the neighbors.
		/// </summary>
		[ShowInInspector]
		private void ResetNeighbors() {
			this.upNeighborNode = null;
			this.rightNeighborNode = null;
			this.downNeighborNode = null;
			this.leftNeighborNode = null;
		}
		#endregion

	}

	/// <summary>
	/// Basically an easier way for me to mark the directions that the pathways can take.
	/// All neighbors may share the same enum, all neighbors may have differences.
	/// </summary>
	public enum GauntletNodeNeighborType {
		Alpha = 0,
		Beta = 1,
		Delta = 2,
		Omega = 3,
	}

}