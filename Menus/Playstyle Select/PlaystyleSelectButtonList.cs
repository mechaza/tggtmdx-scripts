using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Grawly.Menus.NameEntry;
using System.Linq;
using DG.Tweening;
using Grawly.Playstyle;

namespace Grawly.Menus {
	
	/// <summary>
	/// A basic class to help encapsulate the different buttons to be used in the Playstyle selection screen.
	/// Depending on how many I have to present, they will get turned on/off as needed.
	/// </summary>
	public class PlaystyleSelectButtonList : MonoBehaviour {

		#region PROPERTIES - STATE
		/// <summary>
		/// The number of buttons that are inside of this list.
		/// </summary>
		public int ButtonCount => this.PlaystyleButtons.Count;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The playstyle buttons that belong to this list specifically.
		/// </summary>
		[SerializeField]
		private List<PlaystyleSelectButton> playstyleButtons = new List<PlaystyleSelectButton>();
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The playstyle buttons that belong to this list specifically.
		/// </summary>
		public List<PlaystyleSelectButton> PlaystyleButtons => this.playstyleButtons;
		#endregion

	}
}