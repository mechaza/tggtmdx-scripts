using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using HutongGames.PlayMaker;
using Grawly.Calendar;
using UnityEngine.SceneManagement;
using System;

namespace Grawly.UI {

	/// <summary>
	/// The assets that get used in showing a tutorial to the player. Nothing fancy.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Other/Tutorial Template")]
	public class TutorialTemplate : SerializedScriptableObject {

		#region FIELDS
		/// <summary>
		/// The name that should be on the top of the tutorial.
		/// </summary>
		[SerializeField]
		private string tutorialName = "";
		/// <summary>
		/// The name that should be on the top of the tutorial.
		/// </summary>
		public string TutorialName {
			get {
				return this.tutorialName;
			}
		}
		/// <summary>
		/// Should the background be darkened when this tutorial is in use?
		/// </summary>
		[SerializeField]
		private bool darkenBackground = false;
		/// <summary>
		/// Should the background be darkened when this tutorial is in use?
		/// </summary>
		public bool DarkenBackground => this.darkenBackground;
		/// <summary>
		/// The sprite and text to show on any given screen of the tutorial screen.
		/// The sprite is the thumbnail, and the string is the info text.
		/// </summary>
		[SerializeField]
		private List<TutorialScreen> tutorialExplainationScreens = new List<TutorialScreen>();
		/// <summary>
		/// The sprite and text to show on any given screen of the tutorial screen.
		/// The sprite is the thumbnail, and the string is the info text.
		/// </summary>
		public IList<TutorialScreen> TutorialExplainationScreens {
			get {
				// When being accessed publically, it should only be returned as a read only list.
				// I don't want to change the contents.
				return this.tutorialExplainationScreens.AsReadOnly();
			}
		}
		#endregion

	}

	/// <summary>
	/// I was getting tired of using tuples and bullshit like that.
	/// </summary>
	public class TutorialScreen {
		public Sprite sprite;
		[MultiLineProperty(lines: 3)]
		public string paragraphText = "";
	}


}