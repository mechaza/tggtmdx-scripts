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


namespace Grawly.Shop.UI {
	
	/// <summary>
	/// Contains a set of functions to help control the shop menu
	/// that I don't necessarily want in the main controller itself.
	/// </summary>
	[RequireComponent(typeof(ShopMenuController))]
	public class ShopMenuControllerHelper : MonoBehaviour {

		#region FSM ROUTINES - PRESENTATION CALLS
		/// <summary>
		/// Displays the top level menu screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentTopLevelMenuScreen() {
			
			// Tween the shop keeper bust up in.
			ShopMenuController.Instance.ShopKeeperBustUp.Present(
				shopMenuParams: ShopMenuController.Instance.CurrentShopParams);
			
			// Also tween the money counter in.
			ShopMenuController.Instance.MoneyCounter.Present(
				shopMenuParams: ShopMenuController.Instance.CurrentShopParams);
			
			// Go through all of the top level buttons and present them one by one.
			ShopMenuController.Instance.TopLevelButtons
				.ForEach(b => b.Present(shopMenuParams: ShopMenuController.Instance.CurrentShopParams));
			
			// Each button has a variable amount of time it takes before it tweens in. Find the highest.
			float longestButtonDelay = ShopMenuController.Instance.TopLevelButtons.Max(b => b.ButtonScaleInDelay);
			// Wait the max + a fraction of a second and then select the first button.
			GameController.Instance.WaitThenRun(
				timeToWait: longestButtonDelay + 0.01f, 
				action: () => {
					GameObject firstTopButton = ShopMenuController.Instance.TopLevelButtons.First().gameObject;
					EventSystem.current.SetSelectedGameObject(firstTopButton);
				});
			
		}
		/// <summary>
		/// Displays the player selection screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentPlayerSelectionScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Displays the weapon inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentWeaponInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Displays the armor inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentArmorInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Displays the accessory inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentAccessoryInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Displays the weapon inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentItemInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Displays the talk screen.
		/// Called from the FSM.
		/// </summary>
		public void PresentTalkScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
		#region FSM ROUTINES - DISMISSAL CALLS
		/// <summary>
		/// Dismisses the top level menu screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissTopLevelMenuScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the player selection screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissPlayerSelectionScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the weapon inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissWeaponInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the armor inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissArmorInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the accessory inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissAccessoryInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the weapon inventory screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissItemInventoryScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		/// <summary>
		/// Dismisses the talk screen.
		/// Called from the FSM.
		/// </summary>
		public void DismissTalkScreen() {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion

		#region FSM ROUTINES - MISC UI CALLS
		/// <summary>
		/// Moves the button associated with the specified category to the top of the screen to help communicate which menu is being used.
		/// Hides the other buttons.
		/// Called from the FSM.
		/// </summary>
		/// <param name="topLevelButtonType">The category of button that should be given priority.</param>
		public void FocusTopLevelCategoryButton(ShopMenuTopLevelButtonType topLevelButtonType) {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
	}
	
}