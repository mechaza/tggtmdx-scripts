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
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Shop.UI {
	
	/// <summary>
	/// Provides main controls for managing the shop UI.
	/// </summary>
	[RequireComponent(typeof(PlayMakerFSM))]
	public class ShopMenuController : SerializedMonoBehaviour {

		public static ShopMenuController Instance { get; private set; }
		
		#region FIELDS - STATE
		/// <summary>
		/// The parameters that are currently in use for this shop menu.
		/// </summary>
		public ShopMenuParams CurrentShopParams { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// The state machine managing a lot of this shit.
		/// </summary>
		private PlayMakerFSM FSM { get; set; }
		#endregion
		
		#region FIELDS - SCENE REFERENCES : MENU ELEMENTS
		/// <summary>
		/// The script that controls the bars at the top. Whoa.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public BorderBarController BorderBarController { get; private set; }
		/// <summary>
		/// The script controlling the shop keeper bust up.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopKeeperBustUp ShopKeeperBustUp { get; private set; }
		/// <summary>
		/// The script controlling the money available to the player.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopMoneyCounter MoneyCounter { get; private set; }
		/// <summary>
		/// The script that handles the visuals of the player's equipment summary.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopPlayerEquipmentSummary ShopPlayerEquipmentSummary { get; private set; }
		/// <summary>
		/// The script for the MenuList where a player can be selected in the shop.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopPlayerMenuList ShopPlayerMenuList { get; private set; }
		/// <summary>
		/// The script that handles the inventory list of available shop items.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopMenuInventoryList ShopMenuInventoryList { get; private set; }
		/// <summary>
		/// The script that handles the confirmation shit.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public ShopPurchaseConfirmationController ShopPurchaseConfirmationController { get; private set; }
		/// <summary>
		/// The top level buttons that provide access to the other parts of the shop.
		/// </summary>
		[OdinSerialize, TabGroup("Controller", "Scene References")]
		public List<ShopMenuTopLevelButton> TopLevelButtons { get; private set; } = new List<ShopMenuTopLevelButton>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
			this.FSM = this.GetComponent<PlayMakerFSM>();
		}
		private void Start() {
			// Upon start, reset the state.
			this.ResetState();
			this.BuildPrototypeMenu();
		}
		#endregion

		#region OPENING/CLOSING
		/// <summary>
		/// Opens the shop menu with the parameters specified.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		public void Open(ShopMenuParams shopMenuParams) {
			
			// Reset the state of like. Everything.
			this.ResetState();
			
			// Save the menu parameters that shuold be used.
			this.CurrentShopParams = shopMenuParams;
			
			// Prep the menu.
			this.Prepare(shopMenuParams: shopMenuParams);
			
			// Send an event to signal that preparation is complete.
			this.TriggerEvent(eventName: "Preparation Complete");
			
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the shop menu.
		/// </summary>
		private void ResetState() {
			
			// Null out the parameters.
			this.CurrentShopParams = null;
			
			// Reset like, basically everything.
			this.TopLevelButtons.ForEach(b => b.ResetState());
			this.ShopKeeperBustUp.ResetState();
			this.MoneyCounter.ResetState();
			this.ShopPlayerEquipmentSummary.ResetState();
			this.ShopPlayerMenuList.ResetState();
			this.ShopMenuInventoryList.ResetState();
			this.ShopPurchaseConfirmationController.ResetState();
			
		}
		/// <summary>
		/// Prepares this shop to be used with the provided store and theme templates.
		/// </summary>
		/// <param name="shopMenuParams"></param>
		private void Prepare(ShopMenuParams shopMenuParams) {
			
			// Prepare like. Fucking everything.
			// this.TopLevelButtons.ForEach(b => b.Prepare(shopMenuParams));
			this.ShopKeeperBustUp.Prepare(shopMenuParams);
			this.MoneyCounter.Prepare(shopMenuParams);
			this.ShopPlayerEquipmentSummary.Prepare(shopMenuParams);
			// this.ShopPlayerMenuList.Prepare(shopMenuParams);
			// this.ShopMenuInventoryList.Prepare(shopMenuParams);
			// this.ShopPurchaseConfirmationController.Prepare(shopMenuParams);
			
			
			
		}
		#endregion

		#region STATE MANIPULATION
		/// <summary>
		/// Triggers an event on the shop menu screen.
		/// </summary>
		/// <param name="eventName">The name of the event to trigger.</param>
		public void TriggerEvent(string eventName) {
			string str = "Triggering event with name " + eventName + " on the SHOP MENU controller.";
			Debug.Log(str);
			this.FSM.SendEvent(eventName: eventName);
		}
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// The theme template to use for prototyping purposes.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Prototyping")]
		private ShopThemeTemplate prototypeThemeTemplate;
		/// <summary>
		/// The template containing the information for what is available in the store.
		/// </summary>
		[SerializeField, TabGroup("Controller", "Prototyping")]
		private ShopStoreTemplate prototypeStoreTemplate;
		/// <summary>
		/// Builds the menu with the prototype theme provided.
		/// </summary>
		[Button, HideInEditorMode, TabGroup("Controller", "Prototyping")]
		private void BuildPrototypeMenu() {
			
			// Create some basic params.
			/*ShopMenuParams protoParams = new ShopMenuParams() {
				ShopStoreTemplate = this.prototypeStoreTemplate,
				ShopThemeTemplate = this.prototypeThemeTemplate
			};*/

			ShopMenuParams protoParams = new ShopMenuParams(
				storeTemplate: this.prototypeStoreTemplate, 
				themeTemplate: this.prototypeThemeTemplate, 
				gameVariables: GameController.Instance.Variables);
			
			// Prepare the different menu elements so they are ready to go.
			this.Open(shopMenuParams: protoParams);
			
		}
		#endregion

	}
}