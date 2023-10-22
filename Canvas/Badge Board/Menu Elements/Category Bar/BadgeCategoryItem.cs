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
	/// An item that represents a category of badges the player can select from.
	/// Placed on a bar above the menu list of badges.
	/// </summary>
	public class BadgeCategoryItem : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The type of category that this item is representing.
		/// </summary>
		[SerializeField]
		private BadgeCategoryBarItemType categoryItemType = BadgeCategoryBarItemType.SingleElement;
		/// <summary>
		/// The elemental type associated with this category.
		/// </summary>
		[SerializeField, ShowIf("IsSingleElement")]
		private ElementType elementType = ElementType.None;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The elemental type associated with this category.
		/// </summary>
		public ElementType ElementType {
			get {
				if (this.categoryItemType == BadgeCategoryBarItemType.All) {
					throw new System.Exception("This should not be called on the All category!");
				} else {
					return this.elementType;
				}
			}
		}
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that are children of this category item.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image that represents the back of this item.
		/// </summary>
		[SerializeField]
		private Image categoryBackingImage;
		/// <summary>
		/// The image that represents the elemental affinity of this category.
		/// </summary>
		[SerializeField]
		private Image categoryIconImage;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of this item.
		/// </summary>
		public void ResetState() {
			// I guess I can just dehighlight this shit.
			// this.Dehighlight();
			this.allObjects.SetActive(false);
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the element using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this element should be.</param>
		public void Present(BadgeBoardParams boardParams) {
			Debug.LogWarning("MAKE THIS NICER");
			this.allObjects.SetActive(true);
			this.Dehighlight();
		}
		/// <summary>
		/// Dismisses this element from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		public void Dismiss(BadgeBoardParams boardParams) {
			Debug.LogWarning("MAKE THIS NICER");
			this.allObjects.SetActive(false);
		}
		#endregion
		
		#region HIGHLIGHTING
		/// <summary>
		/// Highlights this category item when the associated elemental badges are being displayed.
		/// </summary>
		public void Highlight() {
			this.categoryIconImage.overrideSprite = DataController.GetDefaultElementalIcon(elementType: this.elementType);
		}
		/// <summary>
		/// Dehighlights this category item.
		/// </summary>
		public void Dehighlight() {
			this.categoryIconImage.overrideSprite = DataController.GetDefaultElementalIcon(elementType: this.elementType);
		}
		#endregion
		
		#region ODIN HELPERS
		private bool IsSingleElement() {
			return this.categoryItemType == BadgeCategoryBarItemType.SingleElement;
		}
		#endregion
		
	}
}