using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace Grawly.UI {
	/// <summary>
	/// Controls the notifications that display on the top of the screen.
	/// </summary>
	public class NotificationController : MonoBehaviour {
		public static NotificationController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// A queue of the available notification items.
		/// </summary>
		private Queue<NotificationItem> ReadyItemQueue = new Queue<NotificationItem>();
		/// <summary>
		/// The list of items that are currently active.
		/// </summary>
		private List<NotificationItem> CurrentActiveItems = new List<NotificationItem>();
		/// <summary>
		/// The coroutine that is currenlty animating. I need this so I can stop it if snapping state.
		/// </summary>
		private Coroutine CurrentAnimationRoutine;
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// The number of items that are currently active.
		/// </summary>
		private int CurrentItemCount {
			get { return this.CurrentActiveItems.Count; }
		}
		#endregion

		#region FIELDS - TOGGLES : POSITIONS
		/// <summary>
		/// The Y-coordinate of the navigation bar when it is displayed.
		/// </summary>
		[Title("Positions")]
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float navigationBarDisplayY;
		/// <summary>
		/// The Y-coordinate of the navigatoin bar when it is hidden.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float navigationBarHideY;
		/// <summary>
		/// The base position notification items should target to.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 baseItemPosition;
		/// <summary>
		/// The position the item should be in when it's hiding.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 itemHidingPosition;
		/// <summary>
		/// The position the broadcast item will be when "holding'
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 broadcastItemHoldPosition1;
		/// <summary>
		/// The position the broadcast item will be when "holding'
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 broadcastItemHoldPosition2;
		/// <summary>
		/// The position the broadcast item will be when "holding'
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 broadcastItemFinalPosition;
		/// <summary>
		/// The position for the group of items when it's on display.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 baseRowPosition;
		#endregion

		#region FIELDS - TOGGLES : OFFSETS
		/// <summary>
		/// The offset to have each notification item at as more spawn in.
		/// </summary>
		[Title("Offsets")]
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 itemPositionOffset;
		/// <summary>
		/// The offset to apply when transitioning back and forth between the top and bottom rows.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Vector2 rowPositionOffset;
		#endregion

		#region FIELDS - TIMING
		/// <summary>
		/// The amount of time to tween the item in.
		/// </summary>
		[Title("Timing")]
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float itemTweenTime = 3f;
		/// <summary>
		/// The amount of time to take when tweening the broadcast item in.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float broadcastItemTweenInTime = 3f;
		/// <summary>
		/// The amount of time to take when tweening the broadcast item in.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float broadcastItemHoldTime = 3f;
		/// <summary>
		/// The amount of time to take when tweening the broadcast item out.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float broadcastItemTweenOutTime = 3f;
		/// <summary>
		/// The amount of time to take when tweening the top/bottom rows.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private float rowTweenTime = 0.5f;
		#endregion

		#region FIELDS - EASING
		/// <summary>
		/// The type of easing to use when tweening an item in.
		/// </summary>
		[Title("Easing")]
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Ease itemEaseType = Ease.Flash;
		/// <summary>
		/// The easing to use when tweening the broadcast item in.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Ease broadcastItemEaseIn = Ease.Flash;
		/// <summary>
		/// The easing to use when displaying the broadcast item.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Ease broadcastItemEaseHold = Ease.Flash;
		/// <summary>
		/// The easing to use when tweening the broadcast item out.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Ease broadcastItemEaseOut = Ease.Flash;
		/// <summary>
		/// The easing to use when tweening the top/bottom rows.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Toggles")]
		private Ease rowTweenEase = Ease.Flash;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label showing what the current area is.
		/// </summary>
		[Title("Info Item")]
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private SuperTextMesh topLeftLabel;
		/// <summary>
		/// The image for the top left.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private Image topLeftImage;
		/// <summary>
		/// The rect transform for the entire bar.
		/// </summary>
		[Title("RectTransforms")]
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private RectTransform navigationBarRectTransform;
		/// <summary>
		/// The rect transform that contains all of the active notificatoin items.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private RectTransform topItemRow;
		/// <summary>
		/// The notification item that rests on the top row and is used for signaling events.
		/// </summary>
		[Title("Notification Items")]
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private NotificationItem broadcastNotificationItem;
		/// <summary>
		/// The notification items that this controller should manage.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Scene References")]
		private List<NotificationItem> notificationItems = new List<NotificationItem>();
		#endregion

		#region FIELDS - DEBUGGING
		/// <summary>
		/// A list of debug parameters to use for the controller.
		/// </summary>
		[SerializeField, TabGroup("Notifications", "Debug")]
		private List<NotificationTemplate> debugTemplates = new List<NotificationTemplate>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Kills all of the tweens currently running on the controller's elements.
		/// Also snaps positions of the elements.
		/// </summary>
		private void SnapAnimations() {
			// Stop the coroutine.
			if (this.CurrentAnimationRoutine != null) {
				this.StopCoroutine(this.CurrentAnimationRoutine);
				this.CurrentAnimationRoutine = null;
			}

			// Kill the tweens on the top/bottom rows and reset their positions.
			this.topItemRow.DOKill(complete: true);
			this.topItemRow.anchoredPosition = this.baseRowPosition + this.rowPositionOffset;

			// Go through each item, kill its tween, and set its position appropriately.
			for (int i = 0; i < this.CurrentActiveItems.Count; i++) {
				Vector2 targetPos = this.baseItemPosition + (i * this.itemPositionOffset);
				this.notificationItems[i].GetComponent<RectTransform>().DOKill(complete: true);
				this.notificationItems[i].GetComponent<RectTransform>().anchoredPosition = targetPos;
			}

			// Also kill the tweens on the broadcast item.
			this.broadcastNotificationItem.GetComponent<RectTransform>().DOKill(complete: true);
			this.broadcastNotificationItem.GetComponent<RectTransform>().anchoredPosition = this.itemHidingPosition;
			this.broadcastNotificationItem.ResetState();
		}
		/// <summary>
		/// Totally resets the state of the notification controller.
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		private void ResetState() {
			// Kill all current tweens.
			this.SnapAnimations();

			// Remove all of the active items and recreate the queue.
			this.CurrentActiveItems.Clear();
			this.ReadyItemQueue = new Queue<NotificationItem>(this.notificationItems);

			// Go through each item in the notifications...
			foreach (var item in this.notificationItems) {
				// ... reset its position and state.
				item.GetComponent<RectTransform>().anchoredPosition = this.itemHidingPosition;
				item.ResetState();
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Presents the navigation bar.
		/// </summary>
		public void Present() {
			this.navigationBarRectTransform.DOKill(complete: true);
			this.navigationBarRectTransform.DOAnchorPosY(endValue: this.navigationBarDisplayY, duration: 0.5f, snapping: true).SetEase(Ease.Linear);
		}
		/// <summary>
		/// Dismisses the navigation bar.
		/// </summary>
		public void Dismiss() {
			this.navigationBarRectTransform.DOKill(complete: true);
			this.navigationBarRectTransform.DOAnchorPosY(endValue: this.navigationBarHideY, duration: 0.5f, snapping: true).SetEase(Ease.Linear);
		}
		/// <summary>
		/// Sets the top left visuals with the text and color provided.
		/// </summary>
		/// <param name="text"></param>
		public void RebuildAreaLabel(string text) {
			this.RebuildAreaLabel(text, GrawlyColors.colorDict[GrawlyColorTypes.Red]);
		}
		/// <summary>
		/// Sets the top left visuals with the text and color provided.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		public void RebuildAreaLabel(string text, Color color) {
			this.topLeftLabel.Text = text;
			this.topLeftImage.color = color;
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// The animation that should play when adding a new item.
		/// </summary>
		/// <param name="currentItem">The item that is currently being animated.</param>
		/// <returns></returns>
		private IEnumerator AddItemAnimationRoutine(NotificationItem currentItem, Vector2 itemTargetPos) {
			// Tween the rows.
			this.topItemRow.DOAnchorPos(endValue: this.baseRowPosition, duration: this.rowTweenTime, snapping: true).SetEase(this.rowTweenEase);

			// Wait for the rows to finish.
			yield return new WaitForSeconds(this.rowTweenTime);

			// Display the broadcast item and make sure it looks smooth.
			this.broadcastNotificationItem.GetComponent<RectTransform>().DOAnchorPos(endValue: this.broadcastItemHoldPosition1, duration: this.broadcastItemTweenInTime, snapping: true).SetEase(this.broadcastItemEaseIn);
			yield return new WaitForSeconds(this.broadcastItemTweenInTime);

			// The next part can vary depending on if there is secondary text.
			if (currentItem.CurrentParams.HasSecondaryText == true) {
				// Tween it in, but take twice as long.
				this.broadcastNotificationItem.GetComponent<RectTransform>().DOAnchorPos(endValue: this.broadcastItemHoldPosition2, duration: this.broadcastItemHoldTime * 2f, snapping: true).SetEase(this.broadcastItemEaseHold);

				// Wait the standard amount.
				yield return new WaitForSeconds(this.broadcastItemHoldTime);

				// Tween the secondary text.
				this.broadcastNotificationItem.TweenSecondaryLabel();

				// Wait the standard amount.
				yield return new WaitForSeconds(this.broadcastItemHoldTime);
			} else {
				// If there is no secondary text, just tween as normal.
				this.broadcastNotificationItem.GetComponent<RectTransform>().DOAnchorPos(endValue: this.broadcastItemHoldPosition2, duration: this.broadcastItemHoldTime, snapping: true).SetEase(this.broadcastItemEaseHold);
				yield return new WaitForSeconds(this.broadcastItemHoldTime);
			}

			this.broadcastNotificationItem.GetComponent<RectTransform>().DOAnchorPos(endValue: this.broadcastItemFinalPosition, duration: this.broadcastItemTweenOutTime, snapping: true).SetEase(this.broadcastItemEaseOut);
			yield return new WaitForSeconds(this.broadcastItemTweenOutTime);

			// Tween the rows back.
			this.topItemRow.DOAnchorPos(endValue: this.baseRowPosition + this.rowPositionOffset, duration: this.rowTweenTime, snapping: true).SetEase(this.rowTweenEase);

			// Wait for the rows to finish.
			yield return new WaitForSeconds(this.rowTweenTime);

			// If the item is persistent, tween it.
			if (currentItem.CurrentParams.isPersistent == true) {
				// Tween the item in.
				currentItem.GetComponent<RectTransform>().DOAnchorPos(endValue: itemTargetPos, duration: this.itemTweenTime, snapping: true).SetEase(this.itemEaseType);
			}
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Returns the next available notification item that hasn't been used.
		/// </summary>
		/// <returns></returns>
		private NotificationItem GetNextItem(bool isPersistent) {
			// Only dequeue if this is persistent.
			if (isPersistent == true) {
				return this.ReadyItemQueue.Dequeue();
			} else {
				return this.ReadyItemQueue.Peek();
			}

			/*// Dequeue the next item.
			NotificationItem item = this.ReadyItemQueue.Dequeue();
			// Return it.
			return item;*/
		}
		#endregion

		#region DEBUGGERS
		[Button, HideInEditorMode]
		public void RemoveItems() {
			this.ResetState();
		}
		[Button, HideInEditorMode]
		public void AddItem() {
			// Kill all current tweens.
			this.SnapAnimations();

			// Calculate the target positions for the item itself + the row.
			Vector2 itemTargetPos = this.baseItemPosition + (this.CurrentItemCount * this.itemPositionOffset);

			// Also get the notification params.
			NotificationParams notificationParams = this.debugTemplates[this.CurrentItemCount].ToParams();

			// Grab the next item and build it.
			NotificationItem item = this.GetNextItem(isPersistent: notificationParams.isPersistent);
			item.Build(notificationParams);
			this.broadcastNotificationItem.Build(notificationParams);

			// If this is a persistent notification, add it.
			if (item.CurrentParams.isPersistent == true) {
				this.CurrentActiveItems.Add(item);
			}

			// Start the animation routine and set its reference.
			this.CurrentAnimationRoutine = this.StartCoroutine(this.AddItemAnimationRoutine(currentItem: item, itemTargetPos: itemTargetPos));
		}
		#endregion
	}
}