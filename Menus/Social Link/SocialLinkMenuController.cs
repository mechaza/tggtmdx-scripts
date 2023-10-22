using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.Friends;
using Grawly.Menus.NameEntry;
using Grawly.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;

namespace Grawly.Menus.SocialLink {
	
	/// <summary>
	/// The main controller for the social link menu.
	/// </summary>
	public class SocialLinkMenuController : MonoBehaviour {

		public static SocialLinkMenuController Instance { get; private set; }
		
		#region FIELDS - STATE
		/// <summary>
		/// The currently selected friend template.
		/// </summary>
		private int currentIndex =  0;
		/// <summary>
		/// The current set of friends.
		/// </summary>
		private FriendDataSet currentFriendSet;
		/// <summary>
		/// The callback to run when the menu is closed.
		/// </summary>
		private System.Action currentCloseCallback;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// The data of the friend currently being manipualted.
		/// </summary>
		public FriendData CurrentFriend {
			get {
				return this.currentFriendSet.AvailableFriends[this.currentIndex];
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The hiding position of the bust up.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Toggles"), Title("Tweening")]
		private Vector2 bustUpHidingPos = new Vector2();
		/// <summary>
		/// The display position of the bust up.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Toggles")]
		private Vector2 bustUpDisplayPos = new Vector2();
		/// <summary>
		/// The amount of time to take when tweening the bust up.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Toggles")]
		private float bustUpTweenTime = 0.2f;
		/// <summary>
		/// The easing to use when tweening the bust up.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Toggles")]
		private Ease bustUpEaseInType = Ease.InOutCirc;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The canvas group for all the visuals.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private CanvasGroup mainCanvasGroup;
		/// <summary>
		/// The navigation bar.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private NavigationTabBar navigationTabBar;
		/// <summary>
		/// The border bar controller, for controlling the border.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private BorderBarController borderBarController;
		/// <summary>
		/// The badge showing the current rank of the displayed friend.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private FriendRankBadge friendRankBadge;
		/// <summary>
		/// The menu list displaying the rank upgrades for this friend.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private RankUpgradeMenuList rankUpgradeMenuList;
		/// <summary>
		/// The label for the friends name.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private SuperTextMesh friendNameLabel;
		/// <summary>
		/// The label for the friends tagline.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private SuperTextMesh friendTaglineLabel;
		/// <summary>
		/// The rect transform for the bust up.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private RectTransform bustUpRectTransform;
		/// <summary>
		/// The image for the bust up sprite.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private Image bustUpFrontImage;
		/// <summary>
		/// The image for the bust up's dropshadow.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private Image bustUpDropshadowImage;
		#endregion

		#region FIELDS - TOGGLES

		#endregion
		
		#region FIELDS - DEBUG
		/// <summary>
		/// A list of debug templates to use to prepare this menu.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Prototyping")]
		private List<FriendTemplate> debugTemplates = new List<FriendTemplate>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			ResetController.AddToDontDestroy(this.gameObject);
		}
		private void Start() {
			this.mainCanvasGroup.alpha = 0f;
			this.mainCanvasGroup.interactable = false;
			this.mainCanvasGroup.gameObject.SetActive(false);
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Totally resets the state of the friend menu controller.
		/// </summary>
		private void SnapVisuals() {
			
			// Kill any tweens on the bust up and snap it back to its hiding position.
			this.bustUpRectTransform.DOKill(complete: true);
			this.bustUpRectTransform.anchoredPosition = this.bustUpHidingPos;

		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents the friend controller with the data in this save file.
		/// </summary>
		public void Present(System.Action closeCallback) {
			// Save the close callback.
			this.currentCloseCallback = closeCallback;
			// Grab the friend data set from the game variables.
			FriendDataSet friendDataSet = GameController.Instance.Variables.FriendDataSet;
			// Pass that in to the actual present function.
			this.Present(friendDataSet: friendDataSet);
		}
		/// <summary>
		/// Presents the friend controller with the friend data provided.
		/// </summary>
		/// <param name="friendDataSet">The friend data to use for this list.</param>
		private void Present(FriendDataSet friendDataSet) {
			
			// Save the friend data set.
			this.currentFriendSet = friendDataSet;
			
			// Enable all the objects.
			this.mainCanvasGroup.DOKill(complete: true);
			this.mainCanvasGroup.gameObject.SetActive(true);
			this.mainCanvasGroup.interactable = true;
			this.mainCanvasGroup.DOFade(endValue: 1f, duration: 0.5f).SetEase(Ease.Linear);
			
			// Turn the borders on.
			this.borderBarController.SetVisible(true);
			
			// Build.
			this.Build(friendDataSet: friendDataSet, index: 0);
			
		}
		/// <summary>
		/// Dismisses the canvas from view.
		/// </summary>
		public void Dismiss() {
			// Fade the canvas out.
			this.mainCanvasGroup.DOKill(complete: true);
			this.mainCanvasGroup.interactable = false;
			this.mainCanvasGroup.DOFade(endValue: 0f, duration: 0.5f).SetEase(Ease.Linear).OnComplete(delegate {
				this.currentCloseCallback.Invoke();
				this.currentCloseCallback = null;
				GameController.Instance.WaitThenRun(0.01f, delegate {
					this.mainCanvasGroup.gameObject.SetActive(false);
				});
			});
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the menu with the friend set and index.
		/// </summary>
		/// <param name="friendDataSet">The set to probe.</param>
		/// <param name="index">The index of the available friend.</param>
		private void Build(FriendDataSet friendDataSet, int index) {

			// Reset the visuals.
			this.SnapVisuals();
			
			// Update the current index.
			this.currentIndex = index;
			
			// Build the navigation bar to reflect the new index.
			this.navigationTabBar.RefreshNavigationBar(
				currentIndex: index, 
				totalScreens: friendDataSet.FriendCount);
			
			// Set the text on the labels.
			this.friendNameLabel.Text = this.CurrentFriend.FriendTemplate.friendName;
			this.friendTaglineLabel.Text = this.CurrentFriend.FriendTemplate.friendTagline;

			// Prepare the friend badge.
			this.friendRankBadge.Prepare(currentRank: this.CurrentFriend.currentRank);
			
			// Update the bust up's sprites.
			this.bustUpFrontImage.overrideSprite = this.CurrentFriend.FriendTemplate.bustUpSprite;
			this.bustUpDropshadowImage.overrideSprite = this.CurrentFriend.FriendTemplate.bustUpSprite;
			
			// Tween the whole thing in.
			this.bustUpRectTransform.DOAnchorPos(
				endValue: this.bustUpDisplayPos,
				duration: this.bustUpTweenTime, 
				snapping: true)
				.SetEase(this.bustUpEaseInType);
			
			// Prep the menu list to be used with the given rank defintiions.
			this.rankUpgradeMenuList.PrepareMenuList(
				allMenuables: this.CurrentFriend.MenuableRankDefinitions.Cast<IMenuable>().ToList(),
				startIndex: 0);
			
			
			// Select the first item.
			this.rankUpgradeMenuList.SelectFirstMenuListItem();
			
		}
		#endregion
		
		#region NAVIGATION EVENTS
		/// <summary>
		/// Gets called when a Select event is received from the left/right invisible buttons.
		/// </summary>
		/// <param name="indexIncrementValue"></param>
		public void Scroll(int indexIncrementValue) {

			// Calculate the new index.
			int newIndex = Mathf.Clamp(
				value: (this.currentIndex + indexIncrementValue), 
				min: 0,
				max: this.currentFriendSet.FriendCount - 1);
			
			// Build the new screen.
			this.Build(friendDataSet: this.currentFriendSet, index: newIndex);
			
		}
		#endregion

		#region PROTOTYPING
		[Button, HideInEditorMode]
		private void DebugStart() {
			
			this.Present(friendDataSet: new FriendDataSet(friendTemplates: this.debugTemplates));
		}
		#endregion
		
	}
	
}