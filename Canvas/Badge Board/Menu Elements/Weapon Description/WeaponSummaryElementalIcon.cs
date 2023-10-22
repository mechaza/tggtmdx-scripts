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
	/// Encapsulates the graphics displaying the icon for whatever weapon is being edited.
	/// Probably overkill but I prefer cascading down my classes like this.
	/// </summary>
	public class WeaponSummaryElementalIcon : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that are used for this icon.
		/// </summary>
		[Title("Scene References"), SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image for the backing's front.
		/// </summary>
		[SerializeField]
		private Image backingImageFront;
		/// <summary>
		/// The image for the backing's dropshadow.
		/// </summary>
		[SerializeField]
		private Image backingImageDropshadow;
		/// <summary>
		/// The image for the elemental icon's front.
		/// </summary>
		[SerializeField]
		private Image elementalIconImageFront;
		/// <summary>
		/// The image for the elemental icon's dropshadow.
		/// </summary>
		[SerializeField]
		private Image elementalIconImageDropshadow;
		#endregion


	}
	
}