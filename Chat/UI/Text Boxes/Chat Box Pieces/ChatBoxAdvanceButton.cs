using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Chat {

	/// <summary>
	/// The advance button for a chat box. The little arrow thing.
	/// </summary>
	public abstract class ChatBoxAdvanceButton : SerializedMonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the advance button.
		/// </summary>
		[SerializeField, TabGroup("Advance", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		#endregion

		#region MAIN CALLS - SETTERS
		/// <summary>
		/// Sets whether or not the advance button can be selected.
		/// This is different from whether or not it is visible.
		/// </summary>
		/// <param name="status">Whether or not the button can be selected.</param>
		public abstract void SetSelectable(bool status);
		/// <summary>
		/// Sets whether or not the advance button is visible.
		/// This is different from whether or not it is selectable.
		/// </summary>
		/// <param name="status">Whether or not the button is visible.</param>
		public abstract void SetVisible(bool status);
		/// <summary>
		/// Sets the color of the button.
		/// </summary>
		/// <param name="color">The color to set the button to.</param>
		public abstract void SetColor(Color color);
		#endregion

		#region MAIN CALLS - STATE
		/// <summary>
		/// Gets called when a Submit event is sent to the advance button.
		/// </summary>
		protected abstract void ButtonHit();
		#endregion

	}


}