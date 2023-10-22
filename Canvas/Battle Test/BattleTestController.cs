using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using Grawly.UI;
using UnityEngine.EventSystems;
using Grawly.Battle;
using Grawly.UI.MenuLists;
using UnityEngine.UI;

namespace Grawly.Menus {
	
	/// <summary>
	/// A menu for presenting a list of GamePresets to load up.
	/// </summary>
	public class BattleTestController : MonoBehaviour {

		public static BattleTestController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The last battle template that was selected for a battle.
		/// This is needed so I can re-select it on returning from a battle.
		/// </summary>
		public GameObject LastSelectedBattleTemplateItem { get; set; }
		#endregion
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// A list of game preset templates to use.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private List<BattleTemplate> battleTemplates = new List<BattleTemplate>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu list showing the game presets available.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private BattleTestMenuList battleTestMenuList;
		/// <summary>
		/// The camera to use when inside the battle test menu.
		/// </summary>
		[SerializeField]
		private Camera menuCamera;
		/// <summary>
		/// The image that can be faded in/out as needed to fake a transition to battle.
		/// </summary>
		[SerializeField]
		private Image transitionFlasherImage;
		/// <summary>
		/// The camera to use when inside the battle test menu.
		/// </summary>
		public Camera MenuCamera => this.menuCamera;
		/// <summary>
		/// The image that can be faded in/out as needed to fake a transition to battle.
		/// </summary>
		public Image TransitionFlasherImage => this.transitionFlasherImage;
		/// <summary>
		/// Holds all of the game objects.
		/// </summary>
		[SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// Holds all of the game objects.
		/// </summary>
		public GameObject AllObjects {
			get {
				return this.allObjects;
			}
		}
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.battleTestMenuList.PrepareMenuList(allMenuables: this.battleTemplates.Cast<IMenuable>().ToList(), startIndex: 0);
			this.battleTestMenuList.SelectFirstMenuListItem();
		}
		#endregion

		
		
	}

	
}