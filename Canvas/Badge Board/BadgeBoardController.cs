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

namespace Grawly.Menus.BadgeBoardScreen {
	
	/// <summary>
	/// Controls the menu in which a player can customize their weapon with badges. Whoa..!
	/// </summary>
	public class BadgeBoardController : SerializedMonoBehaviour {
		
		public static BadgeBoardController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The parameters that contain data regarding tha current board being edited.
		/// </summary>
		public BadgeBoardParams CurrentBoardParams { get; private set; } = new BadgeBoardParams();
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The state machine that handles events.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		private PlayMakerFSM FSM { get; set; }
		/// <summary>
		/// The slider that keeps track of which of the different menus are currently presented.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		private BadgeBoardMenuSlider BoardMenuSlider { get; set; }
		/// <summary>
		/// The component that allows picking from any of the available players.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public BadgeBoardPlayerSelection PlayerSelectionMenu { get; private set; }
		/// <summary>
		/// The menu in which a player can pick the weapon they currently want to edit.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public WeaponSelectionMenuList WeaponSelectionMenuList { get; private set; }
		/// <summary>
		/// The menu where a player can select what action they want to perform on the currently picked weapon.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public WeaponActionMenu WeaponActionMenu { get; private set; }
		/// <summary>
		/// The class that provides more specific actions to build the badge selection menu list.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public BadgeSelectionController BadgeSelectionController { get; private set; }
		/// <summary>
		/// The object that displays the summary of the weapon whos board is currently being edited.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		private WeaponSummary WeaponSummary { get; set; }
		/// <summary>
		/// The graphic displaying the headshot of the player who owns the weapon being edited.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public BadgeGridCurrentPlayerHeadshot PlayerHeadshot { get; private set; }
		/// <summary>
		/// The board that can be manipulated with pieces and whatnot.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		public BadgeBoard BadgeBoard { get; private set; }
		/// <summary>
		/// The borders that surround the sides of the screen.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		private ChatBorders ChatBorders { get; set; }
		/// <summary>
		/// The class that manages the background graphics for the board screen.
		/// </summary>
		[TabGroup("Grid", "Scene References"), OdinSerialize]
		private BadgeBoardBackgroundGraphic BoardBackgroundGraphic { get; set; }
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(obj: this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// Upon startup, reset.
			this.ResetState();
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Opens the BadgeBoardController.
		/// </summary>
		/// <param name="entryType">The entry type to use, which determines the first screen shown.</param>
		/// <param name="onComplete">The callback to run when complete.</param>
		public void Open(BadgeBoardEntryType entryType, System.Action onComplete) {
			
			// Create some new board params.
			BadgeBoardParams boardParams = new BadgeBoardParams() {
				CurrentEntryType = entryType,
				CurrentVariables = GameController.Instance.Variables,
				OnCloseCallback = onComplete,
			};
			
			// Send these down to the actaul open method.
			this.Open(boardParams: boardParams);
			
		}
		/// <summary>
		/// Opens the badge board controller 
		/// </summary>
		/// <param name="boardParams"></param>
		private void Open(BadgeBoardParams boardParams) {
			
			// Reset the state. Just in case.
			this.ResetState();
			
			// Keep a reference to the board params. Other objects may need access.
			this.CurrentBoardParams = boardParams;
			
			// Present the various elements.
			this.Present(boardParams: boardParams);

			// Trigger an event based on what entry type was called.
			switch (boardParams.CurrentEntryType) {
				case BadgeBoardEntryType.AllPlayers:
					this.TriggerEvent(eventName: "Edit All Player Weapons");
					break;
				case BadgeBoardEntryType.SinglePlayer:
					throw new NotImplementedException("ADD THIS");
					break;
				default:
					throw new System.Exception("This should not be reached!");
			}
			
		}
		/// <summary>
		/// Close out the badge board controller.
		/// </summary>
		public void Close() {
			
			// Dismiss everything.
			this.Dismiss(boardParams: this.CurrentBoardParams);
			
			GameController.Instance.WaitThenRun(timeToWait: 0.5f, action: () => {
				this.CurrentBoardParams.OnCloseCallback.Invoke();
			});
			
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally resets the state of the badge grid controller.
		/// </summary>
		private void ResetState() {
			
			// Just call everything that needs to be reset.
			this.BoardBackgroundGraphic.ResetState();
			
			this.BoardMenuSlider.ResetState();
			this.PlayerSelectionMenu.ResetState();
			this.WeaponSelectionMenuList.ResetState();
			this.WeaponActionMenu.ResetState();
			this.BadgeSelectionController.ResetState();
			this.BadgeBoard.ResetState();
			
			this.WeaponSummary.ResetState();
			this.PlayerHeadshot.ResetState();
			this.ChatBorders.ResetState();
			
		}
		#endregion
		
		#region STATE MANIPULATION
		/// <summary>
		/// Updates the board params to edit the weapons of the player with the given ID.
		/// </summary>
		/// <param name="player">The player who was just picked.</param>
		public void UpdateCurrentPlayer(Player player) {
			
			// Find the player in the variables who shares the same ID with the one provided.
			this.CurrentBoardParams.CurrentPlayer = player;

			this.TriggerEvent("Player Selected");

		}
		/// <summary>
		/// Updates the board params to remember what the currently selected weapon is.
		/// </summary>
		/// <param name="weapon"></param>
		public void UpdateCurrentWeapon(Weapon weapon) {
			this.CurrentBoardParams.CurrentWeapon = weapon;
		}
		/// <summary>
		/// Updates the board params to remember the action type that was picked.
		/// </summary>
		/// <param name="weaponActionType"></param>
		public void UpdateCurrentWeaponAction(WeaponActionItemType weaponActionType) {
			this.CurrentBoardParams.CurrentWeaponActionType = weaponActionType;
			BadgeBoardController.Instance.TriggerEvent("Action Selected");
		}
		/// <summary>
		/// Triggers an event on the board screen.
		/// </summary>
		/// <param name="eventName">The name of the event to trigger.</param>
		public void TriggerEvent(string eventName) {
			string str = "Triggering event with name " + eventName + " on the badge board controller.";
			Debug.Log(str);
			this.FSM.SendEvent(eventName: eventName);
		}
		#endregion
		
		#region PRESENTATION
		/// <summary>
		/// Presents the board controller using the data contained in the parameters specified.
		/// </summary>
		/// <param name="boardParams">The parameters containing the information on how this board should be.</param>
		private void Present(BadgeBoardParams boardParams) {
			
			// Just call everything that needs to be presented.
			this.BoardBackgroundGraphic.Present(boardParams: boardParams);
			
			this.BoardMenuSlider.Present(boardParams: boardParams);
			this.PlayerSelectionMenu.Present(boardParams: boardParams);
			this.WeaponSelectionMenuList.Present(boardParams: boardParams);
			this.WeaponActionMenu.Present(boardParams: boardParams);
			this.BadgeSelectionController.Present(boardParams: boardParams);
			this.BadgeBoard.Present(boardParams: boardParams);
			
			this.WeaponSummary.Present(boardParams: boardParams);
			// this.PlayerHeadshot.Present(boardParams: boardParams);
			this.ChatBorders.PresentBorders();
			
		}
		/// <summary>
		/// Dismisses the badge board and all elemenets associated from the screen.
		/// </summary>
		/// <param name="boardParams">The board params that were used to create this object.</param>
		private void Dismiss(BadgeBoardParams boardParams) {
			
			// Just call everything that needs to be dismissed.
			this.BoardBackgroundGraphic.Dismiss(boardParams: boardParams);
			
			this.BoardMenuSlider.Dismiss(boardParams: boardParams);
			this.PlayerSelectionMenu.Dismiss(boardParams: boardParams);
			this.WeaponSelectionMenuList.Dismiss(boardParams: boardParams);
			this.WeaponActionMenu.Dismiss(boardParams: boardParams);
			this.BadgeSelectionController.Dismiss(boardParams: boardParams);
			this.BadgeBoard.Dismiss(boardParams: boardParams);
			
			this.WeaponSummary.Dismiss(boardParams: boardParams);
			this.PlayerHeadshot.Dismiss(boardParams: boardParams);
			
			this.ChatBorders.DismissBorders();
			
		}
		#endregion

		
		
	}
}