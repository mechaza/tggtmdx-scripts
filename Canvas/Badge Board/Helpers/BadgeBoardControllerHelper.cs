using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;
using UnityEngine.SceneManagement;

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Provides implementation of common use cases for the BadgeBoardController.
	/// This exists so I don't clutter the controller itself with functions that rely on context.
	/// </summary>
	[RequireComponent(typeof(BadgeBoardController))]
	public class BadgeBoardControllerHelper : SerializedMonoBehaviour {
		
		public static BadgeBoardControllerHelper Instance { get; private set; }

		#region FIELDS - PROTOTYPING
		/// <summary>
		/// Should debug mode be activated?
		/// </summary>
		[OdinSerialize]
		private bool DebugMode { get; set; } = false;
		#endregion
		
		#region FIELDS - STATE
		/// <summary>
		/// A stack that keeps track of which objects were last selected in this menu.
		/// </summary>
		private Stack<GameObject> LastSelectedObjectStack { get; set; } = new Stack<GameObject>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
			}
		}
		private void Start() {
			// If debug mode is on, go ahead and build the controller.
			if (this.DebugMode == true) {
				GameController.Instance.WaitThenRun(1f, () => {
					BadgeBoardController.Instance.TriggerEvent("Edit All Player Weapons");
				});
			}
		}
		#endregion

		#region FSM ROUTINES - PRIMARY CALLS
		/// <summary>
		/// Displays the player select so that the weapons associated with the given character can be displayed.
		/// </summary>
		public void DisplayPlayerSelect() {
			BadgeBoardController.Instance.PlayerSelectionMenu.BuildPlayerItems(
				availablePlayers: BadgeBoardController.Instance.CurrentBoardParams.CurrentVariables.Players);
		}
		/// <summary>
		/// Displays the weapon select with available weapons for the provided character.
		/// </summary>
		/// <remarks>
		/// Called from the FSM.
		/// </remarks>
		public void DisplayWeaponSelect() {

			// Get the weapons that are available to the current player.
			List<Weapon> weaponsForCurrentPlayer = BadgeBoardController.Instance.CurrentBoardParams.CurrentVariables.WeaponCollectionSet.GetWeaponsForPlayer(
				playerIDType: BadgeBoardController.Instance.CurrentBoardParams.CurrentPlayer.playerTemplate.characterIDType);
			
			// Prepare the list of weapons to select and highlight the first one.
			BadgeBoardController.Instance.WeaponSelectionMenuList.PrepareMenuList(
				allMenuables:  weaponsForCurrentPlayer.Cast<IMenuable>().ToList(), 
				startIndex: 0);
		
		}
		/// <summary>
		/// Builds the action select menu with the info it needs.
		/// </summary>
		/// <remarks>
		/// Called from the FSM.
		/// </remarks>
		public void DisplayActionSelect() {
			// Build the weapon action menu.
			BadgeBoardController.Instance.WeaponActionMenu.BuildActionMenu(boardParams: BadgeBoardController.Instance.CurrentBoardParams);
		}
		/// <summary>
		/// Gets called on the FSM when Back is triggered on the Weapon Select.
		/// This is needed so I can either go back to the Player Select or close entirely depending on context.
		/// </summary>
		public void BackFromWeaponSelect() {
			// Trigger a different event based on what the entry type was.
			switch (BadgeBoardController.Instance.CurrentBoardParams.CurrentEntryType) {
				case BadgeBoardEntryType.AllPlayers:
					BadgeBoardController.Instance.TriggerEvent("Back To Player Selection");
					break;
				case BadgeBoardEntryType.SinglePlayer:
					BadgeBoardController.Instance.TriggerEvent("Exit Board Screen");
					break;
				default:
					throw new System.Exception("This should not be reached!");
			}
		}
		/// <summary>
		/// Dismisses the badge board. Bye bye!
		/// </summary>
		public void DismissBadgeBoardScreen() {
			throw new NotImplementedException("Finish this(?)");
		}
		#endregion

		#region FSM ROUTINES - BEST SELECTIONS
		/// <summary>
		/// Selects the first item on the player selection.
		/// </summary>
		public void SelectBestPlayerItem() {
			BadgeBoardController.Instance.PlayerSelectionMenu.SelectFirstPlayerItem();
		}
		/// <summary>
		/// Selects an item from the weapon selection list.
		/// If coming in for the first time, the top item will be selected.
		/// If coming back from a later menu, the item that was originally selected will be selected again.
		/// </summary>
		public void SelectBestWeaponItem() {
			BadgeBoardController.Instance.WeaponSelectionMenuList.SelectFirstMenuListItem();
		}
		/// <summary>
		/// Selects an item from the weapon action menu.
		/// If coming in for the first time, the first item will be selected.
		/// If coming back from a later menu, the item that was originally selected will be selected again.
		/// </summary>
		public void SelectBestActionItem() {
			// If no action has been previously selected (i.e., the screen was JUST opened), build the action menu.
			BadgeBoardController.Instance.WeaponActionMenu.BuildActionMenu(boardParams: BadgeBoardController.Instance.CurrentBoardParams);
			// Select the first available item.
			BadgeBoardController.Instance.WeaponActionMenu.SelectFirstActionItem();
		}
		/// <summary>
		/// Selects an item from the badge menu.
		/// If coming in for the first time, the first item will be selected.
		/// If coming back from a later menu, the item that was originally selected will be selected again.
		/// </summary>
		public void SelectBestBadgeItem() {
			// Select the first item.
			BadgeBoardController.Instance.BadgeSelectionController.SelectFirstBadgeItem();
		}
		#endregion

		#region FSM ROUTINES - WEAPON ACTIONS
		/// <summary>
		/// Figures out what to do based on the action item type that was chosen on the last screen.
		/// </summary>
		public void ProcessWeaponActionSelection() {

			// Grab the current action item type from the params.
			WeaponActionItemType actionItemType = BadgeBoardController.Instance.CurrentBoardParams.CurrentWeaponActionType;
			
			// Depending on what action was picked, a different list will be created.
			switch (actionItemType) {
				case WeaponActionItemType.Edit:
					// BadgeBoardController.Instance.BadgeSelectionController.BuildBadgeSelection(boardParams: BadgeBoardController.Instance.CurrentBoardParams);
					BadgeBoardController.Instance.TriggerEvent(eventName: "Edit Weapon Action Chosen");
					break;
				case WeaponActionItemType.Check:
					BadgeBoardController.Instance.TriggerEvent(eventName: "Check Weapon Action Chosen");
					break;
				case WeaponActionItemType.Clear:
					BadgeBoardController.Instance.TriggerEvent(eventName: "Clear Weapon Action Chosen");
					break;
				case WeaponActionItemType.Equip:
					BadgeBoardController.Instance.TriggerEvent(eventName: "Equip Weapon Action Chosen");
					break;
				default:
					throw new System.Exception("This shouldn't be reached!");
			}	
		}
		/// <summary>
		/// Gets called from the FSM when the edit weapon action was chosen.
		/// </summary>
		public void PickedEditWeaponAction() {
			
			// Get the current board params so I don't have long fucking lines of code.
			BadgeBoardParams boardParams = BadgeBoardController.Instance.CurrentBoardParams;
			
			// Build the badge selection with all of the available badges.
			BadgeBoardController.Instance.BadgeSelectionController.BuildBadgeSelection(
				boardParams: boardParams,
				badgesToDisplay: boardParams.CurrentVariables.BadgeCollectionSet.AllBadges);
			
		}
		/// <summary>
		/// Gets called from the FSM when the check weapon action was chosen.
		/// </summary>
		public void PickedCheckWeaponAction() {
			
			// Get the current board params so I don't have long fucking lines of code.
			BadgeBoardParams boardParams = BadgeBoardController.Instance.CurrentBoardParams;
			
			// Build the badge selection with all of the available badges.
			BadgeBoardController.Instance.BadgeSelectionController.BuildBadgeSelection(
				boardParams: boardParams,
				badgesToDisplay: boardParams.CurrentWeapon.BadgeGrid.CurrentBadges);
			
		}
		/// <summary>
		/// Gets called from the FSM when the clear weapon action was chosen.
		/// </summary>
		public void PickedClearWeaponAction() {
			
			// If the Clear option was picked, the grid needs to be cleared.
			BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid.RemoveAllBadges();
			
			// Tell the badge board to rebuild.
			BadgeBoardController.Instance.BadgeBoard.BuildBoard(
				badgeGrid: BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid);
			
		}
		/// <summary>
		/// Gets called from the FSM when the equip weapon action was chosen.
		/// </summary>
		public void PickedEquipWeaponAction() {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
		#region FSM ROUTINES - BADGE SELECTION LIST
		/// <summary>
		/// Refresh the badge selection list when coming back from the crane.
		/// Called from the FSM.
		/// </summary>
		public void RefreshBadgeSelectionList() {
			BadgeBoardController.Instance.BadgeSelectionController.RefreshBadgeSelection();
		}
		#endregion
		
		#region FSM ROUTINES - BADGE BOARD
		/// <summary>
		/// Refreshes the visuals on the badge board.
		/// Relevant when placing a new badge.
		/// Called from the FSM.
		/// </summary>
		public void RefreshBadgeBoard() {
			BadgeBoardController.Instance.BadgeBoard.BuildBoard(
				badgeGrid: BadgeBoardController.Instance.CurrentBoardParams.CurrentWeapon.BadgeGrid);
		}
		#endregion
		
		#region FSM ROUTINES - CRANE
		/// <summary>
		/// Selects the crane so that pieces may be manipulated on the badge board.
		/// </summary>
		public void PrepareBoardPieceCrane() {
			// Use this to activate the crane.
			BadgeBoardController.Instance.BadgeBoard.BoardPieceCrane.ActivateCrane(
				badgeToPlace: BadgeBoardController.Instance.CurrentBoardParams.CurrentSelectedBadge);
		}
		/// <summary>
		/// Cleans up different components of the crane before transitioning back to the badge selection list.
		/// </summary>
		public void DisassembleBoardPieceCrane() {
			// Just deactivate the crane.
			BadgeBoardController.Instance.BadgeBoard.BoardPieceCrane.DeactivateCrane();
		}
		#endregion
		
		#region FSM ROUTINES - EVENTSYSTEM MANAGEMENT
		/// <summary>
		/// Saves the GameObject that is currently selected into cache so it can be used later.
		/// </summary>
		public void PushCurrentSelectedGameObject() {
			// For right now, I'm just saving one at a time.
			Debug.Log("Saving current selected GameObject: " + EventSystem.current.currentSelectedGameObject);
			this.LastSelectedObjectStack.Push(EventSystem.current.currentSelectedGameObject);
		}
		/// <summary>
		/// Reselects the GameObject that was saved 
		/// </summary>
		public void PopLastSelectedGameObject() {
			
			// Abort if the last object is null.
			if (this.LastSelectedObjectStack.Count == 0) {
				throw new System.Exception("The stack count is zero! This shouldn't be called here!");
			}
			// Pop the last selected object.
			Debug.Log("Popping last selected GameObject: " + this.LastSelectedObjectStack.Peek());
			EventSystem.current.SetSelectedGameObject(this.LastSelectedObjectStack.Pop());
			
		}
		/// <summary>
		/// Deselects the GameObject that is currently selected.
		/// This is helpful when I need to transition between screens and don't want to accept events.
		/// </summary>
		private void DeselectCurrentSelectedGameObject() {
			
			// Just a little preemptive checking.
			if (this.LastSelectedObjectStack.Count == 0) {
				Debug.LogWarning("DeselectCurrentSelectedGameObject was called, " 
				                 + "but there is no cached object to return to. Be careful.");
			}
			
			// Just set it to null.
			EventSystem.current.SetSelectedGameObject(null);
			
		}
		#endregion
		
		#region FSM ROUTINES - VISUALS
		/// <summary>
		/// Tweens the player's headshot in after they have been selected.
		/// Called from the FSM.
		/// </summary>
		public void PresentPlayerHeadshot() {
			BadgeBoardController.Instance.PlayerHeadshot.Present(
				boardParams: BadgeBoardController.Instance.CurrentBoardParams);
		}
		/// <summary>
		/// Dismisses the player headshot when exiting from the weapon select.
		/// Called from the FSM.
		/// </summary>
		public void DismissPlayerHeadshot() {
			BadgeBoardController.Instance.PlayerHeadshot.Dismiss(
				boardParams: BadgeBoardController.Instance.CurrentBoardParams);
		}
		#endregion
		
	}
}