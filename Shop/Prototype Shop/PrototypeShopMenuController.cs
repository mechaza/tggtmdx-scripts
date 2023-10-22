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
using Grawly.Menus;
using Grawly.Shop.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Shop.Prototype {
	
	/// <summary>
	/// A really quick way to buy things.
	/// Mainly to be used until I finish the proper shop menu.
	/// </summary>
	public class PrototypeShopMenuController : MonoBehaviour {

		public static PrototypeShopMenuController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The current params in use for this shop.
		/// </summary>
		private PrototypeShopParams CurrentShopMenuParams { get; set; }
		/// <summary>
		/// The callback to run when the shop is closed.
		/// </summary>
		private Action CurrentCloseCallback { get; set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all the other objects as children.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The list that actually displays the items available for purchase.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private PrototypeShopInventoryList prototypeInventoryList;
		/// <summary>
		/// Used to describe whatever item is currently highlighted in the inventory list.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		private PrototypeShopItemDescription prototypeItemDescription;
		/// <summary>
		/// The UI element that displays the amount of money on hand.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Scene References")]
		public PrototypeMoneyCounter MoneyCounter;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
			
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely reset the state of this menu.
		/// </summary>
		private void ResetState() {
			
			// Turn everything off.
			this.allObjects.SetActive(false);
			
			// Clear the menu list.
			this.prototypeInventoryList.ClearMenuList();
			
			// Null out the close callback and the params.
			this.CurrentCloseCallback = null;
			this.CurrentShopMenuParams = null;
			
		}
		#endregion
		
		#region OPENING/CLOSING
		/// <summary>
		/// Opens the prototype shop menu with the parameters specified.
		/// </summary>
		/// <param name="shopMenuParams">Contains the information on what the shop needs to use.</param>
		/// <param name="onCloseCallback">The callback to run when the shop is closed.</param>
		public void Open(PrototypeShopParams shopMenuParams, Action onCloseCallback) {
			
			// Reset the state of the prototype shop.
			this.ResetState();
			
			// Svae both the params and the close callback.
			this.CurrentShopMenuParams = shopMenuParams;
			this.CurrentCloseCallback = onCloseCallback;
			
			// Turn the objects on.
			this.allObjects.SetActive(true);
			
			// Update the money counter.
			this.MoneyCounter.UpdateCounter(gameVariables: shopMenuParams.CurrentGameVariables);
			
			// Build the inventory list.
			this.prototypeInventoryList.PrepareMenuList(
				allMenuables: shopMenuParams.CurrentShopItems.Cast<IMenuable>().ToList(),
				startIndex: 0);
			
			// Select the first item.
			this.prototypeInventoryList.SelectFirstMenuListItem();
			
		}
		/// <summary>
		/// Close the prototype shop menu.
		/// </summary>
		public void Close() {
			
			// Deselect whatever GameObject is currently selected by the EventSystem.
			EventSystem.current.SetSelectedGameObject(null);
			
			// Invoke the chat closed callback.
			this.CurrentCloseCallback?.Invoke();
			
			// Reset the state. I don't know if this is needed but hey
			this.ResetState();
			
		}
		#endregion

		#region PROTOTYPING - FIELDS
		[SerializeField, TabGroup("Controller", "Prototyping")]
		private PrototypeShopStoreTemplate prototypeStoreTemplate;
		#endregion
		
		#region PROTOTYPING - ODIN HELPERS
		/// <summary>
		/// Builds a prototype menu with debug info.
		/// </summary>
		[Button, TabGroup("Controller", "Prototyping"), HideInEditorMode]
		private void PrototypeBuildMenu() {
			
			// Create a set of parameters to use in the prototype shop.
			PrototypeShopParams prototypeShopParams = new PrototypeShopParams(
				prototypeStoreTemplate: this.prototypeStoreTemplate,
				gameVariables: GameController.Instance.Variables
			);
			
			this.Open(
				shopMenuParams: prototypeShopParams,
				onCloseCallback: () => {
					Debug.Log("Prototype Menu Closed.");
				});
			
		}
		#endregion
		
	}
}