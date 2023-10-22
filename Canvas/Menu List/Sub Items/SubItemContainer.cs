using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.UI.SubItem {

	/// <summary>
	/// Basically a way for me to easily make a prefab that contains other subitems as children.
	/// A normal menulistitem can interface with this scriptand request a certain kind of sub item to be displayed.
	/// </summary>
	public class SubItemContainer : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the sub items I may want.
		/// </summary>
		[SerializeField]
		private List<SubItem> allSubItems = new List<SubItem>();
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Turn off all the sub items, if they aren't already disabled.
			this.DisableAllSubItems();
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Does what it says. Turns off all the gameobjects for each sub item.
		/// </summary>
		private void DisableAllSubItems() {
			this.allSubItems.ForEach(s => s.gameObject.SetActive(false));
		}
		/// <summary>
		/// Enables the sub item associated with the specified type.
		/// </summary>
		/// <param name="subItemType">The type of sub item to use.</param>
		private void EnableSubItems(SubItemType subItemType) {
			this.allSubItems
				.Where(s => s.SubItemType == subItemType)
				.Select(s => s.gameObject)
				.ToList()
				.ForEach(go => go.SetActive(true));
		}
		#endregion

		#region VISUALS
		/// <summary>
		/// Builds the sub item for the first time.
		/// </summary>
		/// <param name="subItemParams"></param>
		protected internal void Build(SubItemParams subItemParams) {

			// Disable all the sub items.
			this.DisableAllSubItems();
			// Enable sub items associated with the specified type.
			this.EnableSubItems(subItemType: subItemParams.SubItemType);

			// Build the ones that are enabled.
			this.allSubItems
				.Where(s => s.gameObject.activeInHierarchy == true)
				.ToList()
				.ForEach(s => s.Build(subItemParams: subItemParams));

		}
		/// <summary>
		/// Puts this sub item in the highlighted state, when the regular item is highlighted.
		/// </summary>
		protected internal void Highlight(SubItemParams subItemParams) {
			this.allSubItems
				.Where(s => s.gameObject.activeInHierarchy == true)
				.ToList()
				.ForEach(s => s.Highlight(subItemParams: subItemParams));
		}
		/// <summary>
		/// Puts this sub item in the dehighlighted state, when the regular item is dehighlighted.
		/// </summary>
		protected internal void Dehighlight(SubItemParams subItemParams) {
			this.allSubItems
				.Where(s => s.gameObject.activeInHierarchy == true)
				.ToList()
				.ForEach(s => s.Dehighlight(subItemParams: subItemParams));
		}
		#endregion

	}
}
