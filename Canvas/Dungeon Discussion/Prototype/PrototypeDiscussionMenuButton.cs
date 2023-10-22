using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Grawly.Gauntlet;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using Grawly.UI.Legacy;
using Grawly.Chat;
using UnityEngine.Events;

namespace Grawly.UI  {
	
	/// <summary>
	/// A button to be used in the prototype screen for dungeon discussions.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
	public class PrototypeDiscussionMenuButton : MonoBehaviour, ISubmitHandler, IDeselectHandler, ISelectHandler, ICancelHandler {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The text that should be used on this button.
		/// </summary>
		[SerializeField, Title("Toggles")]
		private string buttonText = "";
		/// <summary>
		/// The button type associated with this... button.
		/// </summary>
		[SerializeField]
		private DungeonDiscussionButtonType discussionButtonType = DungeonDiscussionButtonType.None;
		/// <summary>
		/// The event that should be invoked upon submission.
		/// </summary>
		[SerializeField]
		private UnityEvent onButtonSubmit;
		#endregion

		#region PROPERTIES - TOGGLES
		/// <summary>
		/// The button type associated with this... button.
		/// </summary>
		public DungeonDiscussionButtonType DiscussionButtonType => this.discussionButtonType;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label that displays this button's text.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private SuperTextMesh buttonNameLabel;
		/// <summary>
		/// The image to use for the button's backing.
		/// </summary>
		[SerializeField]
		private Image buttonBackingFrontImage;
		#endregion

		#region UNITY CALLS
		private void Start() {
			this.Dehighlight();
		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Sets the graphics of this button to be highlighted.
		/// </summary>
		private void Highlight() {
			this.buttonBackingFrontImage.color = Color.white;
			this.buttonNameLabel.Text = "<c=black>" + this.buttonText;
		}
		/// <summary>
		/// Sets the graphics of this button to be dehighlighted.
		/// </summary>
		private void Dehighlight() {
			this.buttonBackingFrontImage.color = Color.black;
			this.buttonNameLabel.Text = "<c=white>" + this.buttonText;
		}
		#endregion
		
		#region EVENT SYSTEM
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			this.Dehighlight();
		}
		public void OnSubmit(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Select);
			// Null out the currently selected object (i.e., this button)
			EventSystem.current.SetSelectedGameObject(null);
			// Invoke the unity event.
			this.onButtonSubmit.Invoke();
		}
		public void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			// Null out the currently selected object (i.e., this button)
			EventSystem.current.SetSelectedGameObject(null);
			// Tell the controller the player wants to go back.
			PrototypeDiscussionController.Instance.BackToOverworld();
		}
		#endregion
		
	}
}