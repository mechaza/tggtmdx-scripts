using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.UI.MenuLists {

	/// <summary>
	/// Contains the framework for how to manage a list of menu items.
	/// </summary>
	public abstract class MenuList : MonoBehaviour {
		
		#region FIELDS - STATE
		/// <summary>
		/// The current value of the "start index," which is basically "how many items am I skipping before grabbing the IMenuables I need?"
		/// IT IS NOT THE CURRENTLY SELECTED ITEM. IT IS THE OFFSET FOR WHICH TO START BUILDING THE MENU LIST.
		/// I am having it public in case I need to rebuild any menu list but obviously only this class should actually be setting it.
		/// </summary>
		public int currentStartIndex { get; private set; } = 0;
		/// <summary>
		/// A list of all the menuables that are being read at any given time.
		/// I need this because ScrollUp() and ScrollDown() are a lot easier to write
		/// with it.
		/// </summary>
		private List<IMenuable> allMenuables = new List<IMenuable>();
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The list of menu list items this gameobject manages.
		/// </summary>
		[SerializeField, TabGroup("Menu List","Scene References")]
		protected List<MenuItem> menuListItems = new List<MenuItem>();
		/// <summary>
		/// The item that should be activated if its possible to scroll up the menu. 
		/// When its selected, it automatically gets deselected in favor of menuListItems[0]
		/// and the list scrolls up.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Scene References")]
		private GameObject topInvisibleItem;
		/// <summary>
		/// The item that should be activated if its possible to scroll down the menu. 
		/// When its selected, it automatically gets deselected in favor of menuListItems.Last()
		/// and the list scrolls down.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Scene References")]
		private GameObject bottomInvisibleItem;
		/// <summary>
		/// The GameObject that houses the scrollbar.
		/// Gets turned off when its not needed.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Scene References")]
		private GameObject scrollBarObject;
		/// <summary>
		/// The mask that is used for the scrollbar. Needed for its height.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Scene References")]
		private Image scrollBarMask;
		/// <summary>
		/// The actual image that is used for the scroll bar's cursor.
		/// </summary>
		[SerializeField, TabGroup("Menu List", "Scene References")]
		private Image scrollbarCursor;
		/// <summary>
		/// The actual image that is used for the scroll bar's cursor.
		/// </summary>
		protected Image ScrollBarCursor {
			get {
				return this.scrollbarCursor;
			}
		}
		#endregion

		#region PREPARING THE LIST
		/// <summary>
		/// Prepares the menu list with all the menuables it will need.
		/// Primarily making this one public as to not expose the implementation to anything else.
		/// </summary>
		/// <param name="allMenuables">All of the menuables that should be part of this menu list.</param>
		/// <param name="startIndex">The index for which the menuables should be rebuilt.</param>
		public virtual void PrepareMenuList(List<IMenuable> allMenuables, int startIndex) {
			// Override the current start index. This will give me problems otherwise.
			this.currentStartIndex = startIndex;
			// Remember the menuables.
			this.allMenuables = allMenuables;
			// Rebuild the list, starting at the beginning.
			// this.RebuildMenuList(menuables: this.allMenuables, startIndex: 0);
			if (startIndex != 0) {
				Debug.LogError("I was having some issues with the start index so hopefully this gets my attention.");
				Debug.Break();
			}
			this.RebuildMenuList(menuables: this.allMenuables, startIndex: startIndex);
		}
		/// <summary>
		/// Prepares the menu list with all the menuables it will need.
		/// This also infers the index to start on from the startmenuable passed in.
		/// </summary>
		/// <param name="allMenuables"></param>
		/// <param name="startMenuable"></param>
		public virtual void PrepareMenuList(List<IMenuable> allMenuables, IMenuable startMenuable) {
			throw new NotImplementedException("Add this!");
		}
		#endregion

		#region MANIPULATING THE LIST
		/// <summary>
		/// Completely and totally cleans out the menu list to look blank.
		/// </summary>
		public virtual void ClearMenuList() {
			Debug.Log("Clearing out the menu list.");
			// Reset the state on the menuables/start index.
			this.currentStartIndex = -1;
			this.allMenuables = new List<IMenuable>();
			// Turn off the scrollbar as well
			this.scrollBarObject.SetActive(false);
			// Turn off the menu list items.
			this.menuListItems.ForEach(i => i.gameObject.SetActive(false));
		}
		/// <summary>
		/// Adds an IMenuable to the list of menuables.
		/// </summary>
		/// <param name="menuable">The menuable to add.</param>
		/// <param name="focusOnAdd">Should this item be focused on when added to?</param>
		public virtual void Append(IMenuable menuable, bool focusOnAdd = false) {
			
			// Don't add the item if it already exists in the list.
			if (this.ContainsMenuable(menuable: menuable) == true) {
				Debug.LogError("Menuable already exists in list! Aborting.");
			}
			
			// Add the item.
			this.allMenuables.Add(item: menuable);
			foreach (IMenuable m in this.allMenuables) {
				Debug.Log(m.PrimaryString);
			}
			
			// If instructed to focus on the new item...
			if (focusOnAdd == true) {
				// Rebuild with that item in mind.
				this.Focus(menuables: this.allMenuables, focusMenuable: menuable);
			} else {
				// If not focusing, rebuild from the current index.
				this.RebuildMenuList(menuables: this.allMenuables, startIndex: this.currentStartIndex);
			}
			
		}
		#endregion
		
		#region BUILDING THE LIST
		/// <summary>
		/// A publically accessible way for rebuilding the menu list. Obviously, it will fail if PrepareMenuList was not previously called.
		/// </summary>
		public void RebuildMenuList() {
			this.RebuildMenuList(menuables: this.allMenuables, startIndex: this.currentStartIndex);
		}
		/// <summary>
		/// Builds out the menu list with contents starting at the specified index.
		/// </summary>
		/// <param name="menuables">ALL of the items that should be as part of this menu list.</param>
		/// <param name="startIndex">The index at which to start reading out the items.</param>
		private void RebuildMenuList(List<IMenuable> menuables, int startIndex) {

			// Update the current start index.
			this.currentStartIndex = startIndex;
			
			// Check if the top invisible button needs to be enabled. This happens if it's not the first index.
			this.topInvisibleItem.SetActive(value: (startIndex > 0));
			
			// Do the same for the bottom button. 
			this.bottomInvisibleItem.SetActive(value: (startIndex + this.menuListItems.Count) < menuables.Count);

			// Rebuild the scroll bar.
			this.RebuildScrollbar(menuables: menuables, startIndex: startIndex);

			// Skip until reaching the start index, then take as many as can be held in the menu list.
			menuables = menuables.Skip(count: startIndex).Take(count: this.menuListItems.Count).ToList();
			// Go through each menuable and pass it over to the MenuListItem to build out.
			for (int i = 0; i < menuables.Count; i++) {
				this.menuListItems[i].gameObject.SetActive(true);
				this.menuListItems[i].BuildMenuItem(item: menuables[i]);
			}
			// If there are any remaining menuListItems that were not used, turn them off.
			for (int i = menuables.Count; i < this.menuListItems.Count; i++) {
				this.menuListItems[i].gameObject.SetActive(false);
			}
		}
		/// <summary>
		/// Rebuilds the scrollbar based on the passed in information.
		/// </summary>
		/// <param name="menuables">The menuables that are on display next to the scrollbar.</param>
		/// <param name="startIndex">The index that the menuables are being read from.</param>
		private void RebuildScrollbar(List<IMenuable> menuables, int startIndex) {
			// Check if the scrollbar is actually needed or not.
			if (menuables.Count <= this.menuListItems.Count) {
				// If it's not, just get out.
				this.scrollBarObject.SetActive(false);
				return;
			}
			// If the scroll bar is needed, turn it on, but also adjust the cursor to the appropriate spot.
			this.scrollBarObject.SetActive(true);
			float rangeOfMovement = this.scrollBarMask.rectTransform.sizeDelta.y - this.scrollbarCursor.rectTransform.sizeDelta.y;
			float offsetIncrements = rangeOfMovement / (menuables.Count - this.menuListItems.Count);
			// Determine how much to offset the scroll bar by.
			float offsetPos = offsetIncrements * startIndex * -1f;
			// Relocate the scrollbar.
			this.scrollbarCursor.rectTransform.anchoredPosition = new Vector2(x: this.scrollbarCursor.rectTransform.anchoredPosition.x, y: offsetPos);
		}
		#endregion

		#region FOCUSING
		/// <summary>
		/// Updates the MenuList's start index, rebuilds, and focuses on the specified menuable.
		/// </summary>
		/// <param name="focusMenuable">The menuable that should be at the top of the list.</param>
		public void Focus(IMenuable focusMenuable) {
			// Cal the version of this function that just uses the menuables on hand.
			this.Focus(menuables: this.allMenuables, focusMenuable: focusMenuable);
		}
		/// <summary>
		/// Updates the MenuList's start index, rebuilds, and focuses on the specified menuable.
		/// </summary>
		/// <param name="menuables">The menuables that should be used for building.</param>
		/// <param name="focusMenuable">The menuable that should be at the top of the list.</param>
		private void Focus(List<IMenuable> menuables, IMenuable focusMenuable) {
			// This won't work if the start menuable does not actually exist.
			if (this.ContainsMenuable(menuables: menuables, menuable: focusMenuable) == false) {
				throw new System.Exception("The menu list does not contain the provided menuable! " 
				                           + "Did you remember to update the menuables list by calling Prepare again?");
			}
			// Find the index of the specified menuable, but make sure to clamp it.
			int menuableIndex = menuables.IndexOf(focusMenuable);
			int maxStartIndex = this.GetMaximumStartIndex(menuables: menuables, menuItems: this.menuListItems);
			int clampedIndex = Mathf.Min(a: maxStartIndex, b: menuableIndex);
			
			// Use this to rebuild on this index.
			this.RebuildMenuList(menuables: menuables, startIndex: clampedIndex);
		}
		#endregion
		
		#region SELECTION
		/// <summary>
		/// A quick helper function to just select the first menu list item.
		/// </summary>
		public void SelectFirstMenuListItem() {
			Debug.Log("Selecting the first menu on the MenuList.");
			EventSystem.current.SetSelectedGameObject(this.menuListItems[0].gameObject);
		}
		#endregion

		#region SCROLL EVENTS - WITH INVISIBLES
		/// <summary>
		/// Scrols the menu up. Gets called from the invisible top button as an event trigger.
		/// </summary>
		public void ScrollUp() {
			// Re-select the top button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.menuListItems.First().gameObject));
			// Adjust the current slot.
			this.currentStartIndex -= 1;
			// Build out the list.
			this.RebuildMenuList(menuables: this.allMenuables, startIndex: this.currentStartIndex);
			// Highlight the first item. This helps prevent the "blink" problem I've been having.
			this.menuListItems.First().Highlight(item: this.allMenuables.Skip(count: this.currentStartIndex).Take(count: this.menuListItems.Count).First());
		}
		/// <summary>
		/// Scrols the menu down. Gets called from the invisible bottom button as an event trigger.
		/// </summary>
		public void ScrollDown() {
			// Re-select the last button. That's sort of the point for this.
			this.StartCoroutine(this.WaitAndReselect(gameObjectToSelect: this.menuListItems.Last().gameObject));
			// Adjust the current slot.
			this.currentStartIndex += 1;
			// Build out the list.
			this.RebuildMenuList(menuables: this.allMenuables, startIndex: this.currentStartIndex);
			// Highlight the first item. This helps prevent the "blink" problem I've been having.
			this.menuListItems.Last().Highlight(item: this.allMenuables.Skip(count: this.currentStartIndex).Take(count: this.menuListItems.Count).Last());
		}
		/// <summary>
		/// A helper method to just wait a frame and then re-select the specified menu list item.
		/// </summary>
		/// <param name="gameObjectToSelect"></param>
		/// <returns></returns>
		private IEnumerator WaitAndReselect(GameObject gameObjectToSelect) {
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
		}
		#endregion

		#region GETTERS - STATE
		/// <summary>
		/// Does this MenuList contain the specified Menuable?
		/// </summary>
		/// <param name="menuable">The Menuable to check for.</param>
		/// <returns>Whether or not this list is managing the provided menuable.</returns>
		public bool ContainsMenuable(IMenuable menuable) {
			// Use the regular ass version of this function.
			return this.ContainsMenuable(menuables: this.allMenuables, menuable: menuable);
		}
		/// <summary>
		/// Does this MenuList contain the specified Menuable?
		/// </summary>
		/// <param name="menuables">The list of menuables to check against.</param>
		/// <param name="menuable">The Menuable to check for.</param>
		/// <returns>Whether or not this list is managing the provided menuable.</returns>
		private bool ContainsMenuable(List<IMenuable> menuables, IMenuable menuable) {
			// Just return if the menuables list contains the menuable passed in.
			return menuables.Contains(menuable);
		}
		/// <summary>
		/// Gets the maximum allowed number that can be used as a start index.
		/// </summary>
		/// <param name="menuables">The menuables in use.</param>
		/// <param name="menuItems">The MenuItems that are available for building on screen.</param>
		/// <returns>The largest number the start index can be.</returns>
		private int GetMaximumStartIndex(List<IMenuable> menuables, List<MenuItem> menuItems) {
			// Subtract the number of available list items from the number of menuables being managed.
			int maxValue = menuables.Count - menuItems.Count;
			// Clamp it back to zero if it went past it. The index cant be in the negatives!
			int clampedMaxValue = Mathf.Max(a: 0, b: maxValue);
			// Return it.
			return clampedMaxValue;
		}
		#endregion
		
	}


}