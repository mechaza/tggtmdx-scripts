using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using Grawly.UI.SubItem;
using UnityEngine.EventSystems;

namespace Grawly.Toggles {
	
	#region BIG DADDY
	/// <summary>
	/// Things that attach onto GameToggles should extend from this.
	/// </summary>
	public interface GTComponent {

	}
	/// <summary>
	/// Interfaces extending from this should provide values passively.
	/// </summary>
	public interface GTValueHaver : GTComponent {
		
	}
	/// <summary>
	/// This describes something that a toggle can *do*.
	/// This is important from an event,
	/// which is what a toggle *does in response* to something.
	/// </summary>
	public interface GTOperator : GTComponent {

	}
	/// <summary>
	/// This describes something a toggle can do in response to something.
	/// </summary>
	public interface GTEventHandler : GTComponent {

	}
	#endregion



	#region VALUE HAVERS
	/// <summary>
	/// A GameToggleDX that implements this should provide some kind of enum.
	/// </summary>
	public interface GTEnumHaver<T> : GTValueHaver where T : System.Enum {
		/// <summary>
		/// Gets an enum constant that this toggle is currently assigned.
		/// </summary>
		/// <returns>An enum constant that this toggle is currently assigned.</returns>
		T GetToggleEnum();
	}

	/// <summary>
	/// A GameToggleDX that implements this should provide some kind of int value.
	/// </summary>
	public interface GTIntHaver : GTValueHaver {
		/// <summary>
		/// Gets an integer associated with this GameToggleDX.
		/// </summary>
		/// <returns>An integer associated with this GameToggle.</returns>
		int GetToggleInt();
	}

	/// <summary>
	/// A GameToggleDX that implements this should provide some kind of float value.
	/// </summary>
	public interface GTFloatHaver : GTValueHaver {
		/// <summary>
		/// Gets a float associated with this GameToggleDX.
		/// </summary>
		/// <returns>A float associated with this GameToggle.</returns>
		float GetToggleFloat();
	}

	/// <summary>
	/// A GameToggleDX that implements this should provide some kind of bool value.
	/// </summary>
	public interface GTBoolHaver : GTValueHaver {
		/// <summary>
		/// Gets a bool associated with this GameToggleDX.
		/// </summary>
		/// <returns>A bool associated with this GameToggle.</returns>
		bool GetToggleBool();
	}
	#endregion



	#region OPERATIONS
	/// <summary>
	/// 
	/// </summary>
	public interface GTIndexShifter : GTOperator {
		/// <summary>
		/// "Shifts" this GameToggle in response to a given direction.
		/// </summary>
		/// <param name="shiftType">The direction that was shifted.</param>
		void Shift(ToggleShiftType shiftType);
	}
	#endregion



	#region EVENTS
	/// <summary>
	/// Gets called when the game starts properly.
	/// Assumes the GameController exists.
	/// </summary>
	public interface GTGameSaveLoadHandler : GTEventHandler {
		/// <summary>
		/// Gets called when the game starts up properly.
		/// Assumes the GameController exists.
		/// </summary>
		void OnGameSaveLoad();
	}

	/// <summary>
	/// Gets called when the difficulty toggles are set.
	/// </summary>
	public interface GTDifficultyToggleHandler : GTEventHandler {
		void OnDifficultyToggleSet();
	}
	/// <summary>
	/// Gets called when the game boots up.
	/// </summary>
	public interface GTGameBootHandler : GTEventHandler {
		/// <summary>
		/// Gets called when the game boots up.
		/// Good for when I need to set the screen resolution or something.
		/// Assumes the GameController probably isn't in yet.
		/// </summary>
		void OnGameInitialize();
	}

	/// <summary>
	/// Gets called when the GameController initializes.
	/// I only do this once usually.
	/// BUT... ASSUME... IT MAY GET CALLED OTHER TIMES? LIKE WHEN LOADING?
	/// </summary>
	public interface GTGameControllerInitializeHandler : GTEventHandler {
		/// <summary>
		/// Gets called at the end of the start function on GameController.
		/// </summary>
		void OnGameControllerInitialize();
	}

	/// <summary>
	/// Handles an event when left/right is pressed on the menu.
	/// </summary>
	public interface GTHorizontalMoveHandler : GTEventHandler {
		/// <summary>
		/// Handles when left/right is given on a menu.
		/// </summary>
		/// <param name="moveDir">The direction that was moved in the menu.</param>
		void OnHorizontalMenuMove(HorizontalMoveDirType moveDir);
	}
	/// <summary>
	/// Toggles that should receive submission events implement this.
	/// </summary>
	public interface GTSubmitHandler : GTEventHandler {
		void OnSubmit(BaseEventData eventData);
	}
	#endregion

	#region OTHER
	/// <summary>
	/// Specifies that this toggle has a sub item.
	/// This is what receives left/right events, anything that uses Shift probably needs this.
	/// Not required though. If you wanna go ape shit be my guest. Me to myself. I'm so tired.
	/// </summary>
	public interface GTSubItem : GTComponent {
		/// <summary>
		/// SubItemParams that represent the state this toggle is currently in.
		/// </summary>
		SubItemParams CurrentSubItemParams { get; }
	}
	#endregion

}