using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grawly.Menus {
	
	/// <summary>
	/// A very basic script that automatically highlights/dehighlights super text meshes based on toggles.
	/// </summary>
	[RequireComponent(typeof(SuperTextMesh))]
	[RequireComponent(typeof(Selectable))]
	public class BasicButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The interactivity state. Saved because I might do shit.
		/// </summary>
		private bool savedInteractivity = false;
		/// <summary>
		/// The text on this button. Used so I can remember it.
		/// </summary>
		private string rawText = "";
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The text mesh attached to this button.
		/// </summary>
		private SuperTextMesh textLabel;
		/// <summary>
		/// The selectable attached to this button.
		/// </summary>
		private Selectable selectable;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			
			// Get the super text mesh and the selectable.
			this.textLabel = this.GetComponent<SuperTextMesh>();
			this.selectable = this.GetComponent<Selectable>();
			
			// Grab the text from the stm.
			this.rawText = this.textLabel.text;
			
			// Save the interactivity.
			this.savedInteractivity = this.selectable.IsInteractable();
		}
		private void OnEnable() {
			
			// Restore the interactivity.
			this.selectable.interactable = this.savedInteractivity;
			
		}
		private void OnDisable() {
			
			// Save the interactivity.
			this.savedInteractivity = this.selectable.IsInteractable();
			
			// Make it not interactive.
			this.selectable.interactable = false;
			
			// Reset the text.
			this.textLabel.Text = this.rawText;
			
		}
		#endregion
		
		#region MAIN CALLS
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			this.textLabel.Text = this.rawText;
		}
		public void OnSelect(BaseEventData eventData) {
			this.textLabel.Text = "<c=black>" + this.rawText;
		}
		public void OnSubmit(BaseEventData eventData) {
			/*// If disabling, do that.
			if (this.disableInteractionOnSubmit == true) {
				this.GetComponent<Selectable>().interactable = false;
			}*/
			AudioController.instance?.PlaySFX(type: SFXType.Select);
			this.textLabel.Text = this.rawText;
		}
		#endregion
		
	}

	
}