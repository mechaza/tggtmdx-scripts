using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Menus.Input {

	/// <summary>
	/// The standard way in which letters should be tracked.
	/// </summary>
	public class StandardEntryFieldLetter : EntryFieldLetter {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label that shows the recorded letter.
		/// </summary>
		[SerializeField]
		private SuperTextMesh letterLabel;
		/// <summary>
		/// The GameObject that contains the visuals for the highlight.
		/// </summary>
		[SerializeField]
		private GameObject highlightVisualsGameObject;
		#endregion

		#region UNITY CALLS
		/// <summary>
		/// On start, I might need to clear out the letter.
		/// </summary>
		private void Start() {
			this.ClearLetter();
			this.Dehighlight();
		}
		#endregion

		#region VISUALS
		/// <summary>
		/// Highlights the entry letter to show the player this is what is currently being used.
		/// </summary>
		public override void Highlight() {
			this.highlightVisualsGameObject.SetActive(true);
			this.letterLabel.Text = "<c=purpley>" + this.CurrentLetter;
		}
		/// <summary>
		/// Dehighlights the entry letter to show this is not the letter currently accepting input.
		/// </summary>
		public override void Dehighlight() {
			this.highlightVisualsGameObject.SetActive(false);
			this.letterLabel.Text = this.CurrentLetter;
		}
		#endregion

	}


}