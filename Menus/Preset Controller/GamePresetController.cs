using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.UI;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Menus {
	
	/// <summary>
	/// A menu for presenting a list of GamePresets to load up.
	/// </summary>
	public class GamePresetController : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// A list of game preset templates to use.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private List<GamePresetTemplate> presetTemplates = new List<GamePresetTemplate>();
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The menu list showing the game presets available.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private GamePresetMenuList presetList;
		#endregion

		#region UNITY CALLS
		private void Start() {
			
			// this.presetList.PrepareMenuList(allMenuables: this.presetTemplates.Cast<IMenuable>().ToList(), startIndex: 0);
			List<GamePresetTemplate> gamePresetTemplates = DataController.GetGamePresetTemplateReference().ValidPresetTemplates;
			this.presetList.PrepareMenuList(allMenuables: gamePresetTemplates.Cast<IMenuable>().ToList(), startIndex: 0);
			this.presetList.SelectFirstMenuListItem();
			
			
		}
		#endregion

	
		
	}

	
}