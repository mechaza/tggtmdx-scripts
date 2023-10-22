using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Grawly.UI.SubItem;

namespace Grawly.Toggles {

	/// <summary>
	/// This is where I can define common GameToggleDX's and their exact parameters.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Toggles Template")]
	public class GameTogglesTemplateDX : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// A list of GameToggles.
		/// </summary>
		[OdinSerialize, ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 40)]
		private List<GameToggleDX> gameToggles = new List<GameToggleDX>();
		#endregion

		#region OPERATIONS
		/// <summary>
		/// Clones the toggles in this template and makes a new list out of them.
		/// </summary>
		/// <returns>A new GameToggleSet formed from clones of the toggles inside this template.</returns>
		public GameToggleSetDX GenerateSet() {
			//	return new GameToggleSetDX(gameToggles: this.gameToggles.Select(gt => gt.Clone()).ToList());
			return new GameToggleSetDX(gameToggles: this.gameToggles.Select(gt => gt.GetDefaultConfiguration()).ToList());
		}
		#endregion
		
	}


}