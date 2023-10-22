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
using Sirenix.Serialization;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Encapsulates the data that should be used to prepare the badge board controller.
	/// </summary>
	public class BadgeBoardParams {

		#region FIELDS - STATE : CONFIGURATION
		/// <summary>
		/// The callback to run when the board params have been closed out,
		/// if one is defined.
		/// </summary>
		public System.Action OnCloseCallback { get; set; } = null;
		/// <summary>
		/// The variables that contain references to the inventory and whatnot.
		/// </summary>
		public GameVariables CurrentVariables { get; set; }
		/// <summary>
		/// The type of entry that was used for this badge board.
		/// Helpful in branching at different spots in the FSM.
		/// </summary>
		public BadgeBoardEntryType CurrentEntryType { get; set; } = BadgeBoardEntryType.None;
		#endregion
		
		#region FIELDS - STATE : MENU DRIVEN
		/// <summary>
		/// The player currently having their board edited.
		/// </summary>
		public Player CurrentPlayer { get; set; }
		/// <summary>
		/// The weapon whose badge grid is currently being edited in the grid.
		/// </summary>
		public Weapon CurrentWeapon { get; set; }
		/// <summary>
		/// The weapon action type that was picked last.
		/// </summary>
		public WeaponActionItemType CurrentWeaponActionType { get; set; } = WeaponActionItemType.None;
		/// <summary>
		/// The badge that is currently being manipulated on the crane after being selected from the badge list.
		/// </summary>
		public Badge CurrentSelectedBadge { get; set; }
		#endregion
		
	}
}