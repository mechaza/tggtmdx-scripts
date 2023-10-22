using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.Shop.UI;
using Grawly.UI.MenuLists;
using UnityEngine.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// The class that helps control how buttons in the Shop UI operate on the top level.
	/// The diamond ones.
	/// </summary>
	public class ShopMenuTopLevelButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler {
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The category type associated with this button.
		/// </summary>
		[SerializeField, TabGroup("Button", "Toggles")]
		[Title("General")]
		private ShopMenuTopLevelButtonType topLevelButtonType = ShopMenuTopLevelButtonType.None;
		/// <summary>
		/// The string to use for this button's category label.
		/// </summary>
		[SerializeField, TabGroup("Button", "Toggles")]
		private string buttonCategoryString = "";
		#endregion

		#region FIELDS - TWEENING : ROTATION
		/// <summary>
		/// The rotation the diamond graphics (both front and dropshadow) should be at at the start.
		/// </summary>
		private Quaternion DiamondGraphicInitialRotation { get; set; }
		/// <summary>
		/// The speed which the diamond graphic should rotate at when its idling/dehighlighted.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		[Title("Rotation")]
		private float diamondGraphicIdleRotationSpeed = -10f;
		/// <summary>
		/// The speed which the diamond graphic should rotate at when its highlighted.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float diamondGraphicHighlightRotationSpeed = -15f;
		#endregion
		
		#region FIELDS - TWEENING : POSITION
		/// <summary>
		/// The position the button should be in when not in focus.
		/// I.e., the top level menu itself.
		/// </summary>
		[FormerlySerializedAs("buttonDisplayPos")]
		[SerializeField, TabGroup("Button", "Tweening")]
		[Title("Positions")]
		private Vector2 buttonUnfocusedPos = new Vector2();
		/// <summary>
		/// The position the button should be in when in "focus".
		/// I.e., when its picked to and needs to move to the top of the screen.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Vector2 buttonFocusedPos = new Vector2();
		/// <summary>
		/// The position the highlight bar should be in when dismissed.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Vector2 buttonHighlightBarDismissPos = new Vector2();
		/// <summary>
		/// The position the highlight bar should be in when presented.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Vector2 buttonHighlightBarPresentPos = new Vector2();
		#endregion

		#region FIELDS - TWEENING : TIMING
		/// <summary>
		/// The amount of time it should take for this button to scale in.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		[Title("Timing")]
		private float buttonScaleInTime = 0.5f;
		/// <summary>
		/// The amount of time it should take for this button to scale out.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonScaleOutTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the button to its hiding position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonHidingTweenTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the button to its display position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonDisplayTweenTime = 0.5f;
		/// <summary>
		/// The amount of time it should take for the highlight bar to be presented.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonHighlightBarPresentTime = 0.2f;
		/// <summary>
		/// The amount of time it should take for the highlight bar to be dismissed.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonHighlightBarDismissTime = 0.2f;
		/// <summary>
		/// When opening the shop, the amount of time to wait before actually scaling the button in.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonScaleInDelay = 0.1f;
		/// <summary>
		/// When closing the shop, the amount of time to wait before actually scaling the button out.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonScaleOutDelay = 0.1f;
		/// <summary>
		/// The amount of time to wait before tweening the button to its hiding position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonHidingTweenDelay = 0.1f;
		/// <summary>
		/// The amount of time to wait before tweening the button to its display position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private float buttonDisplayDelay = 0.1f;
		#endregion

		#region PROPERTIES - TWEENING : TIMING
		/// <summary>
		/// The amount of time it should take for this button to scale in.
		/// This property gets used when figuring out which of the buttons has the longest scale-in time.
		/// </summary>
		public float ButtonScaleInDelay => this.buttonScaleInDelay;
		#endregion
		
		#region FIELDS - TWEENING : EASE
		/// <summary>
		/// The easing to use when scaling the button in.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		[Title("Easing")]
		private Ease buttonScaleInEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when scaling the button out.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Ease buttonScaleOutEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening over to the hiding position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Ease buttonHidingEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening back to the display position.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Ease buttonDisplayEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the highlight bar in.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Ease buttonHighlightBarPresentEaseType = Ease.InOutCirc;
		/// <summary>
		/// The easing to use when tweening the highlight bar out.
		/// </summary>
		[SerializeField, TabGroup("Button", "Tweening")]
		private Ease buttonHighlightBarDismissEaseType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : ANCHORS
		/// <summary>
		/// All of the objects this top level button contains.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		[Title("Anchors")]
		private GameObject allObjects;
		/// <summary>
		/// The main "pivot" for the diamond graphic. Helpful for rotating the graphic itself.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private RectTransform diamondGraphicMainPivotRectTransform;
		/// <summary>
		/// The pivot to manipulate when tweening the highlight bar in and out.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private RectTransform buttonHighlightBarMainPivot;
		#endregion

		#region PROPERTIES - SCENE REFERENCES : ANCHORS
		/// <summary>
		/// The RectTransform for the "all objects" gameobject.
		/// Helpful for moving things around.
		/// </summary>
		private RectTransform AllObjectsRectTransform => this.allObjects.GetComponent<RectTransform>();
		/// <summary>
		/// The RectTransform for the front image of the diamond graphic.
		/// </summary>
		private RectTransform DiamondGraphicFrontRectTransform => this.diamondGraphicFrontImage.GetComponent<RectTransform>();
		/// <summary>
		/// The RectTransform for the dropshadow image of the diamond graphic.
		/// </summary>
		private RectTransform DiamondGraphicDropshadowRectTransform => this.diamondGraphicBackingImage.GetComponent<RectTransform>();
		#endregion
		
		#region FIELDS - SCENE REFERENCES : IMAGES
		/// <summary>
		/// The image that should represent the front of the diamond graphic
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		[Title("Images")]
		private Image diamondGraphicFrontImage;
		/// <summary>
		/// The image that should represent the back of the diamond graphic
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private Image diamondGraphicBackingImage;
		/// <summary>
		/// The image that should represent the front of the highlight bar.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private Image highlightBarFrontImage;
		/// <summary>
		/// The image that should represent the front of the highlight bar.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private Image highlightBarDropshadowImage;
		/// <summary>
		/// The image that should be used for the icon on this button.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		private Image buttonIconImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : LABELS
		/// <summary>
		/// The label that shows the category of this button to the player.
		/// </summary>
		[SerializeField, TabGroup("Button", "Scene References")]
		[Title("Labels")]
		private SuperTextMesh buttonCategoryLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Grab the local rotation of the diamond graphic's image.
			// (The dropshadow will have the same value so where I get it from does not matter.)
			this.DiamondGraphicInitialRotation = this.DiamondGraphicFrontRectTransform.localRotation;
		}
		private void Start() {
			// Reset state on start.
			// this.ResetState();
		}
		private void Update() {
			// Ok I actually really hate using Update() but i guess just... perform the rotation of the diamond graphic here. Maybe.
			this.DiamondGraphicFrontRectTransform.Rotate(xAngle: 0f, yAngle: 0f, zAngle: this.diamondGraphicIdleRotationSpeed * Time.deltaTime);
			this.DiamondGraphicDropshadowRectTransform.localEulerAngles = this.DiamondGraphicFrontRectTransform.localRotation.eulerAngles;
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the button.
		/// </summary>
		public void ResetState() {
			
			// Kill any tweens currently working on this button and its components.
			this.AllObjectsRectTransform.DOKill(complete: true);
			this.buttonHighlightBarMainPivot.DOKill(complete: true);
			
			// Completely and totally reset the positions/scales/rotations of anything that needs to be snapped back.
			this.AllObjectsRectTransform.anchoredPosition = this.buttonUnfocusedPos;
			this.DiamondGraphicFrontRectTransform.localRotation = this.DiamondGraphicInitialRotation;
			this.DiamondGraphicDropshadowRectTransform.localRotation = this.DiamondGraphicInitialRotation;
			this.buttonHighlightBarMainPivot.anchoredPosition = this.buttonHighlightBarDismissPos;
			this.AllObjectsRectTransform.localScale = Vector3.zero;
			
			// Turn all of the objects off.
			this.allObjects.SetActive(false);
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Presents this button in response to opening the shop menu.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Present(ShopMenuParams shopMenuParams) {
			
			// Turn all of the objects on.
			this.allObjects.SetActive(true);
			
			// Set the graphics by dehighlighting them.
			this.Dehighlight(shopMenuParams: shopMenuParams);
			
			// Each button has a slight difference in how long to wait before scaling in, to make it look More Fun.
			// Wait that amount of time, then begin scaling in.
			GameController.Instance.WaitThenRun(
				timeToWait: this.buttonScaleInDelay,
				action: () => {
					// Scale in the AllObjects rect.
					this.AllObjectsRectTransform.DOScale(
							endValue: 1f, 
							duration: this.buttonScaleInTime)
						.SetEase(this.buttonScaleInEaseType);
				});
			
		}
		/// <summary>
		/// Dismisses this top level button upon leaving the shop menu.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Dismiss(ShopMenuParams shopMenuParams) {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
		#region HIGHLIGHTING
		/// <summary>
		/// Sets this buttons graphics to be highlighted.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		private void Highlight(ShopMenuParams shopMenuParams) {
			
			// Kill any tweens operating on the highlight bar and snap it to its starting position.
			this.buttonHighlightBarMainPivot.DOKill(complete: true);
			this.buttonHighlightBarMainPivot.anchoredPosition = this.buttonHighlightBarDismissPos;
			// Start up a tween to move the highlight bar to its appropriate position.
			this.buttonHighlightBarMainPivot.DOAnchorPos(
				endValue: this.buttonHighlightBarPresentPos, 
				duration: this.buttonHighlightBarPresentTime,
				snapping: true)
				.SetEase(ease: this.buttonHighlightBarPresentEaseType);
			
			// Update the colors on anything that needs it.
			this.diamondGraphicFrontImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondFrontHighlightColor;
			this.highlightBarFrontImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondFrontHighlightColor;
			this.diamondGraphicBackingImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondDropshadowHighlightColor;
			this.highlightBarDropshadowImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondDropshadowHighlightColor;
			this.buttonCategoryLabel.textMaterial = shopMenuParams.ShopThemeTemplate.TopLevelLabelHighlightMaterial;

			// Set the label text.
			this.buttonCategoryLabel.Text = shopMenuParams.ShopThemeTemplate.TopLevelLabelHighlightPrefixString + this.buttonCategoryString;
			
		}
		/// <summary>
		/// Sets this buttons graphics to be dehighlighted.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		private void Dehighlight(ShopMenuParams shopMenuParams) {
			
			// Kill any tweens operating on the highlight bar and snap it to its presenting position.
			this.buttonHighlightBarMainPivot.DOKill(complete: true);
			this.buttonHighlightBarMainPivot.anchoredPosition = this.buttonHighlightBarPresentPos;
			// Start up a tween to move the highlight bar to its appropriate position.
			this.buttonHighlightBarMainPivot.DOAnchorPos(
					endValue: this.buttonHighlightBarDismissPos, 
					duration: this.buttonHighlightBarDismissTime,
					snapping: true)
				.SetEase(ease: this.buttonHighlightBarDismissEaseType);
			
			// Update the colors on anything that needs it.
			this.diamondGraphicFrontImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondFrontIdleColor;
			this.highlightBarFrontImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondFrontIdleColor;
			this.diamondGraphicBackingImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondDropshadowIdleColor;
			this.highlightBarDropshadowImage.color = shopMenuParams.ShopThemeTemplate.TopLevelDiamondDropshadowIdleColor;
			this.buttonCategoryLabel.textMaterial = shopMenuParams.ShopThemeTemplate.TopLevelLabelIdleMaterial;
			
			// Set the label text.
			this.buttonCategoryLabel.Text = shopMenuParams.ShopThemeTemplate.TopLevelLabelIdlePrefixString + this.buttonCategoryString;

		}
		#endregion
		
		#region EVENT SYSTEM IMPLEMENTATIONS
		public void OnCancel(BaseEventData eventData) {
			// If cancelling on a top level button, it should be assumed the player just wants to leave.
			ShopMenuController.Instance.TriggerEvent(eventName: "Selected Leave");
		}
		public void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(shopMenuParams: ShopMenuController.Instance.CurrentShopParams);
		}
		public void OnSelect(BaseEventData eventData) {
			this.Highlight(shopMenuParams: ShopMenuController.Instance.CurrentShopParams);
		}
		public void OnSubmit(BaseEventData eventData) {
			
			// Depending on the type of top level button, send the appropriate event to the FSM.
			switch (this.topLevelButtonType) {
				case ShopMenuTopLevelButtonType.Weapons:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Weapons Menu");
					break;
				case ShopMenuTopLevelButtonType.Armor:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Armor Menu");
					break;
				case ShopMenuTopLevelButtonType.Accessories:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Accessories Menu");
					break;
				case ShopMenuTopLevelButtonType.Items:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Items Menu");
					break;
				case ShopMenuTopLevelButtonType.Talk:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Talk");
					break;
				case ShopMenuTopLevelButtonType.Leave:
					ShopMenuController.Instance.TriggerEvent(eventName: "Selected Leave");
					break;
				default:
					throw new System.Exception("This button type isn't defined to do anything!");
			}
			
		}
		#endregion
		
	}
}