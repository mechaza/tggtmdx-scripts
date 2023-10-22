using System.Collections;
using System.Collections.Generic;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine;
using Sirenix.OdinInspector;
namespace Grawly.UI {

	/// <summary>
	/// The MenuList that manages the inventory. 
	/// </summary>
	public sealed class InventoryMenuList : MenuList {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// You may reference it.
		/// Once.
		/// </summary>
		public List<MenuItem> MenuListItems {
			get {
				return this.menuListItems;
			}
		}
		/// <summary>
		/// The GameObject that contains the info box.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private GameObject infoBoxGameObject;
		/// <summary>
		/// The STM that describes the currently selected item.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Menu List")]
		private SuperTextMesh infoBoxLabel;
		public SuperTextMesh InfoBoxLabel {
			get {
				return this.infoBoxLabel;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// this.initialPosition = this.GetComponent<RectTransform>().anchoredPosition;
		}
		#endregion

		#region PREPARING THE LIST
		/// <summary>
		/// Does an extra step where it adds the "no selections available!" thing if nothing is available.
		/// </summary>
		/// <param name="allMenuables">All of the menuables that should be part of this menu list.</param>
		/// <param name="startIndex">The index for which the menuables should be rebuilt.</param>
		public override void PrepareMenuList(List<IMenuable> allMenuables, int startIndex) {
			base.PrepareMenuList(allMenuables, startIndex);
		}
		public override void ClearMenuList() {
			base.ClearMenuList();
			this.infoBoxLabel.Text = "";
		}
		#endregion

	}


}