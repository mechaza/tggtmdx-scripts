using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Nodes have their own variables which keep track of state I may need to save or otherwise remember long term.
	/// </summary>
	[System.Serializable]
	public class GauntletNodeVariables {

		#region FIELDS - METADATA
		/// <summary>
		/// The ID for this node. Important for saving. 
		/// </summary>
		[SerializeField, TabGroup("Node", "General")]
		public int nodeId = -1;
		/// <summary>
		/// The title for this node. Gets used in the GauntletNodeTitle.
		/// </summary>
		[SerializeField, TabGroup("Node", "General")]
		public string nodeTitle = "";
		#endregion

		#region FIELDS - STATE : GENERAL
		/// <summary>
		/// The number of times the marker has been to this node.
		/// </summary>
		[SerializeField, TabGroup("Node", "General")]
		public int enterCount = 0;
		/// <summary>
		/// Has the marker been to this node?
		/// </summary>
		public bool Visited {
			get {
				// If the count is higher than zero, it means the marker has been here.
				return this.enterCount > 0;
			}
		}
		/// <summary>
		/// Has the main task of this node been accomplished?
		/// </summary>
		[SerializeField, TabGroup("Node", "General")]
		public bool completed = false;
		#endregion

		#region FIELDS - STATE : PATH
		/// <summary>
		/// A list of pathway types that are open.
		/// GauntletNodeNeighbors makes use of this.
		/// </summary>
		[SerializeField, TabGroup("Node", "Paths")]
		private List<GauntletNodeNeighborType> openPathTypes = new List<GauntletNodeNeighborType>();
		#endregion

		#region STATE
		/// <summary>
		/// Adds the specified neighbor type to the list of open pathway types.
		/// </summary>
		/// <param name="neighborType">The type of neighbor path to open</param>
		public void OpenNeighborPathway(GauntletNodeNeighborType neighborType) {
			// If the open paths already contains this type, break out.
			if (this.openPathTypes.Contains(neighborType)) {
				return;
			}
			// Add the pathway type.
			this.openPathTypes.Add(neighborType);
		}
		/// <summary>
		/// Closes the specified neighbor type.
		/// </summary>
		/// <param name="neighborType">The neighbor type to close.</param>
		public void CloseNeighborPathway(GauntletNodeNeighborType neighborType) {
			this.openPathTypes.Remove(neighborType);
		}
		/// <summary>
		/// Checks if the pathway of the specified neighbor type is open.
		/// </summary>
		/// <param name="neighborType">The neighbor type being checked.</param>
		/// <returns>Whether or not the given path is open.</returns>
		public bool CheckForOpenPath(GauntletNodeNeighborType neighborType) {
			return this.openPathTypes.Contains(neighborType);
		}
		#endregion

	}


}