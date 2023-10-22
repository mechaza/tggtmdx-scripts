using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.Menus;
using Grawly.UI.MenuLists;

namespace Grawly.Shop.UI {
	
	/// <summary>
	/// Displays a brief statistic about an item in the shop.
	/// (e.x., POW, ACC, POS)
	/// </summary>
	public class ShopItemStat : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// man whatever
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The image for the circle backing of this stat.
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private Image statCircleBackingImage;
		/// <summary>
		/// The image to show the up arrow, for stats that demonstrate an increase.
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private Image upArrowImage;
		/// <summary>
		/// The image to show the down arrow, for stats that demonstrate a decrease.
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private Image downArrowImage;
		/// <summary>
		/// The label showing the title for this stat.
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private SuperTextMesh statTitleLabel;
		/// <summary>
		/// The label showing the value for this stat.
		/// </summary>
		[SerializeField, TabGroup("Stat", "Scene References")]
		private SuperTextMesh statValueLabel;
		#endregion

	}
}