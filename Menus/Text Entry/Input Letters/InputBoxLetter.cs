using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Menus.Input {

	/// <summary>
	/// Defines a "letter" that can be used as part of an input box.
	/// </summary>
	public abstract class InputBoxLetter : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The letter that this input letter should send to the field.
		/// </summary>
		[SerializeField]
		private string letter = "";
		/// <summary>
		/// The letter that this input letter should send to the field.
		/// </summary>
		protected string Letter {
			get {
				return this.letter;
			}
			set {
				this.letter = value;
			}
		}
		#endregion

		#region SELECTABLES
		/// <summary>
		/// Allows this letter to be highlighted.
		/// Especially useful if I need to turn on a Selectable component, for example.
		/// </summary>
		public abstract void EnableLetter();
		/// <summary>
		/// Allows this letter to be highlighted.
		/// Especially useful if I need to turn OFF a Selectable component, for example.
		/// </summary>
		public abstract void DisableLetter();
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Shows this letter being highlighted.
		/// </summary>
		protected abstract void Highlight();
		/// <summary>
		/// Dehighlights this letter.
		/// </summary>
		protected abstract void Dehighlight();
		#endregion

	}


}