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
using Grawly.Battle;
using Grawly.UI.MenuLists;

namespace Grawly.Shop {
	
	/// <summary>
	/// A representation of a player in the shop.
	/// This is effectively a container class because I hate myself and I'm drunk.
	/// </summary>
	public class ShopPlayer : IMenuable {

		#region FIELDS - STATE
		/// <summary>
		/// The player currently assigned to this ShopPlayer.
		/// Hell is real!
		/// </summary>
		public Player Player { get; set; }
		#endregion
		
		#region INTERFACE IMPLEMENTATION : IMENUABLE
		public string PrimaryString => this.Player.metaData.name;
		public string QuantityString => "";
		public string DescriptionString => "";
		public Sprite Icon => null;
		#endregion

	}
	
}