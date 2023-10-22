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
using Grawly.Chat;
using Grawly.Playstyle;
using Grawly.UI;
using UnityEngine.EventSystems;

namespace Grawly.Menus {
	
	/// <summary>
	/// This is what should be interacted with when picking a mode in the mode select.
	/// </summary>
	public class PlaystyleSelectButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The PlaystyleTemplate currently assigned to this button.
		/// </summary>
		public PlaystyleTemplate CurrentPlaystyleTemplate { get; private set; }
		#endregion
		
		#region FIELDS - TWEENING : POSITIONS
		/// <summary>
		/// The position the button should be in when starting up.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Vector2 buttonStartPos = new Vector2();
		/// <summary>
		/// The position the button should be in when displayed on screen.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Vector2 buttonDisplayPos = new Vector2();
		/// <summary>
		/// The position the button should be in when confirming its usage.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Vector2 buttonConfirmationPos = new Vector2();
		/// <summary>
		/// The position the button should be in when dismissed from screen.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Vector2 buttonDismissPos = new Vector2();
		#endregion

		#region FIELDS - TWEENING : EASING
		/// <summary>
		/// The easing to use when tweening the button in.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Ease buttonEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the button out.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Ease buttonEaseOutType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the button into its confirmation position.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Ease buttonConfirmEaseInType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the button out of its confirmation position.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private Ease buttonConfirmEaseOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time to take when tweening the button in.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private float buttonTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the button out.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private float buttonTweenOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the button to its confirmation position.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private float buttonConfirmTweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the button out of its confirmation position.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Tweening")]
		private float buttonConfirmTweenOutTime = 0.5f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects inside of this mode button
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The RectTransform to manipulate when tweening.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private RectTransform mainPivotRectTransform;
		/// <summary>
		/// The selectable for this button.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private Selectable buttonSelectable;
		/// <summary>
		/// The label describing the name of the mode itself.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private SuperTextMesh modeNameLabel;
		/// <summary>
		/// The label outlining the description of the mode.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private SuperTextMesh modeDescriptionLabel;
		/// <summary>
		/// The image that serves as the backing for the button.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private Image buttonBackingFrontImage;
		/// <summary>
		/// The image representing the icon for the mode.
		/// </summary>
		[SerializeField, TabGroup("Selection", "Scene References")]
		private Image modeIconImage;
		#endregion

		#region PROPERTIES - SCENE REFERENCES
		/// <summary>
		/// The selectable for this button.
		/// </summary>
		public Selectable ButtonSelectable => this.buttonSelectable;
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the button.
		/// </summary>
		public void ResetState() {
			
			// Null out the playstyle template.
			this.CurrentPlaystyleTemplate = null;
			
			// Kill all tweens on the pivot's rect transform and also complete them.
			this.mainPivotRectTransform.DOKill(complete: true);
			
			// Snap the anchored position to its start position.
			this.mainPivotRectTransform.anchoredPosition = this.buttonStartPos;
			
			// Turn the objects off completely.
			this.allObjects.SetActive(false);

		}
		/// <summary>
		/// Preps this button for use with the specified playstyle template.
		/// </summary>
		/// <param name="playstyleTemplate">The playstyle template containing the information about the playstyle.</param>
		public void Prepare(PlaystyleTemplate playstyleTemplate) {
			
			// Turn the object back on.
			this.allObjects.SetActive(true);
			
			// Prep the button with the information it needs.
			this.CurrentPlaystyleTemplate = playstyleTemplate;
			this.modeNameLabel.Text = playstyleTemplate.PlaystyleName;
			this.modeDescriptionLabel.Text = playstyleTemplate.PlaystyleDescription;
			this.modeIconImage.overrideSprite = playstyleTemplate.PlaystyleIcon;
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents the selection button to the player.
		/// </summary>
		public void Present() {
			// Just cascade down to the tween function.
			this.TweenToPosition(stateType: PlaystyleSelectButtonStateType.Displayed);
		}
		/// <summary>
		/// Tweens the button to the specified state type.
		/// </summary>
		/// <param name="stateType">The state type to tween to.</param>
		/// <param name="snapToPosition">Should the position be snapped to without tweening?</param>
		private void TweenToPosition(PlaystyleSelectButtonStateType stateType, bool snapToPosition = false) {
			
			// Kill any tweens currently operating on the rect transform.
			this.mainPivotRectTransform.DOKill(complete: true);
			
			// Determine the position/timing/easing to use.
			float targetTime = 0f;
			Ease targetEase = Ease.Linear;
			Vector2 targetPosition = new Vector2();
			
			// Assign the appropriate variables.
			switch (stateType) {
				case PlaystyleSelectButtonStateType.Start:
					// Theoretically I shouldn't actually be coming in here?
					throw new System.Exception("This actually should never be tweened to. Be safe about it.");
				case PlaystyleSelectButtonStateType.Displayed:
					targetTime = this.buttonTweenInTime;
					targetEase = this.buttonEaseInType;
					targetPosition = this.buttonDisplayPos;
					break;
				case PlaystyleSelectButtonStateType.Confirming:
					targetTime = this.buttonConfirmTweenInTime;
					targetEase = this.buttonConfirmEaseInType;
					targetPosition = this.buttonConfirmationPos;
					break;
				case PlaystyleSelectButtonStateType.Dismissed:
					targetTime = this.buttonTweenOutTime;
					targetEase = this.buttonEaseOutType;
					targetPosition = this.buttonDismissPos;
					break;
				default:
					throw new System.Exception("This should never be reached!");
			}

			// If not snapping, start up the tween.
			if (snapToPosition == false) {
				// Perform the tween.
				this.mainPivotRectTransform.DOAnchorPos(
						endValue: targetPosition,
						duration: targetTime, 
						snapping: true)
					.SetEase(ease: targetEase);
			} else {
				// If snapping, well... do that.
				this.mainPivotRectTransform.anchoredPosition = targetPosition;
			}
			
			

		}
		#endregion

		#region HIGHLIGHTING
		/// <summary>
		/// Sets the graphics on this button to be highlighted.
		/// </summary>
		private void Highlight() {
			this.buttonBackingFrontImage.color = Color.red;
		}
		/// <summary>
		/// Sets the graphics on this button to be dehighlighted.
		/// </summary>
		private void Dehghlight() {
			this.buttonBackingFrontImage.color = Color.white;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - EVENT SYSTEM
		public void OnSelect(BaseEventData eventData) {
			this.Highlight();
		}
		public void OnDeselect(BaseEventData eventData) {
			this.Dehghlight();
		}
		public void OnSubmit(BaseEventData eventData) {
			
			// Upon submitting this button, open the tutorial describing how this playstyle works.
			TutorialScreenController.OpenTutorial(
				tutorialTemplate: this.CurrentPlaystyleTemplate.PlaystyleTutorial,
				objectToReselectOnClose: null, 
				actionOnClose: () => {
					// On closing the tutorial, open up a chat to ask the player if they wish to use the playstyle.
					ChatControllerDX.GlobalOpen(
						textAsset: PlaystyleSelectScreenController.Instance.SelectionConfirmationChatScript, 
						chatClosedCallback: ((str, num, toggle) => {
							if (toggle == true) {
								// If they picked yes, transition out.
								Debug.Log("PICKED YES");
								throw new NotImplementedException("Transition out from this screen and also perhaps set the mode in gamevariables!");
							} else {
								Debug.Log("PICKED NO");
								EventSystem.current.SetSelectedGameObject(this.ButtonSelectable.gameObject);
							}
						}));
					
				});
		}
		public void OnCancel(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		#endregion
		
		#region ENUM DEFINITIONS
		/// <summary>
		/// The different states these kinds of buttons can be in.
		/// </summary>
		public enum PlaystyleSelectButtonStateType {
			Start		= 0,
			Displayed	= 1,
			Confirming	= 2,
			Dismissed	= 3,
		}
		#endregion
		
	}
}