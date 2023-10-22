using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Menus.Input {

	/// <summary>
	/// A method for inputting data to an InputEntryField.
	/// </summary>
	public abstract class InputBox : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains the input letters that this box is in charge of managing.
		/// </summary>
		[TabGroup("Input Box", "Scene References"), Title("Base Class")]
		[SerializeField]
		private List<InputBoxLetter> inputLetters = new List<InputBoxLetter>();
		/// <summary>
		/// Contains the input letters that this box is in charge of managing.
		/// </summary>
		protected List<InputBoxLetter> InputLetters {
			get {
				return this.inputLetters;
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Requests input from this input box.
		/// This basically means turning everything on.
		/// </summary>
		public abstract void RequestInput();
		/// <summary>
		/// Tells this input box that it's no longer needed and it should skidaddle.
		/// </summary>
		public abstract void FinishInput();
		/// <summary>
		/// This is what gets called when the length limit is reached.
		/// </summary>
		public abstract void LengthLimitReached();
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Totally resets the visuals of the input box.
		/// </summary>
		public abstract void ResetInputBox();
		/// <summary>
		/// Plays the animation that shows the input box.
		/// </summary>
		public abstract void PresentInputBox();
		/// <summary>
		/// Plays the animation that dismisses the input box.
		/// </summary>
		public abstract void DismissInputBox();
		#endregion

		#region SELECTABLES
		/// <summary>
		/// Turns on ALL of the available letters.
		/// </summary>
		protected void EnableAllLetters() {
			this.InputLetters.ForEach(i => i.EnableLetter());
		}
		/// <summary>
		/// Turns off ALL the available letters.
		/// </summary>
		protected void DisableAllLetters() {
			this.InputLetters.ForEach(i => i.DisableLetter());
		}
		#endregion

	}

}