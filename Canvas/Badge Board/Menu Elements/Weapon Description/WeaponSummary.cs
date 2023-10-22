using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// The box that details the weapon whos grid is currently being edited.
	/// </summary>
	public class WeaponSummary : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that make up this summary.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The icon representing the elemental affinity of the current weapon being edited.
		/// </summary>
		[SerializeField]
		private WeaponSummaryElementalIcon weaponElementalIcon;
		/// <summary>
		/// The label used for describing the name of the current weapon being edited.
		/// </summary>
		[SerializeField]
		private SuperTextMesh weaponNameLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			this.allObjects.SetActive(false);
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the board controller using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this board should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			// this.allObjects.SetActive(true);
			// Debug.LogError("This needs to be added appropriately!");
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			this.allObjects.SetActive(false);
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the visuals of the summary to reflect the weapon that was just sent in.
		/// </summary>
		/// <param name="weapon"></param>
		public void BuildWeaponSummary(Weapon weapon) {
			// Overwrite the name.
			this.weaponNameLabel.Text = weapon.PrimaryString;
			Debug.LogError("This needs to be added appropriately!");
		}
		#endregion
		
	}
	
}