using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Grawly.Toggles;
using Grawly.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Grawly.Toggles.Display;
using UnityEngine.SceneManagement;
using System.Linq;
using Grawly.Console;
using Grawly.Dungeon;
using Grawly.DungeonCrawler;

namespace Grawly.Helpers {
	
	/// <summary>
	/// Used to mark something that needs to be done in the scene so I remember to do it.
	/// Ideally, should throw an error if it is not fufilled.
	/// </summary>
	public class ToDoNote : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// A description of the to-do.
		/// </summary>
		[SerializeField, TextArea(minLines: 5, maxLines:10)]
		private string noteDescription = "";
		/// <summary>
		/// Should an error be thrown on starting the scene?
		/// Helpful for making sure I catch things that need to be done.
		/// </summary>
		[SerializeField]
		private bool throwErrorOnStart = true;
		#endregion

		#region UNITY CALLS
		private void Start() {
			// Throw an error if marked to do so.
			if (this.throwErrorOnStart == true) {
				throw new NotImplementedException(this.noteDescription);
			}
		}
		#endregion
		
	}
}