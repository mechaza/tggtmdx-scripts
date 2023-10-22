using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.UI;
using Grawly.UI.MenuLists;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Simply describes a specific detail of a badge's functionality in the info box.
	/// </summary>
	public class BadgeInfoDetail : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The label that describes one aspect of the badge.
		/// </summary>
		[SerializeField]
		private SuperTextMesh detailLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this object.
		/// </summary>
		public void ResetState() {
			this.detailLabel.Text = "";
			this.allObjects.SetActive(false);
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Present(BadgeBoardParams boardParams) {
			this.allObjects.SetActive(true);
		}
		/// <summary>
		/// Dismisses this element with the parameters provided.
		/// </summary>
		/// <param name="boardParams">The parameters driving this particular slot.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			this.allObjects.SetActive(false);
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds this info detail with the provided fact.
		/// </summary>
		/// <param name="badgeFact">The fact containing what to display.</param>
		public void BuildInfoDetail(BadgeFact badgeFact) {
			this.allObjects.SetActive(true);
			this.detailLabel.Text = badgeFact.FactText;
		}
		#endregion
		
	}
}