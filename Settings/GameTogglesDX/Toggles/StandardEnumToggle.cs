using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI.SubItem;
using Grawly.UI;

namespace Grawly.Toggles {

	/// <summary>
	/// A lot of GameToggles I want to have strict typing on the kinds of values it can save.
	/// This is what should be extended from when I want to accomplish that.
	/// Good for things that have specific names.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public abstract class StandardEnumToggle<T> : StandardToggleDX, GTHorizontalMoveHandler, GTIndexShifter, GTEnumHaver<T> where T : System.Enum {

		#region FIELDS - STATE
		/// <summary>
		/// The current value stored by this Toggle.
		/// </summary>
		[OdinSerialize, HideInInspector]
		private T currentValue;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The default value for this enum toggle.
		/// Gets used when cloning to a default configuration.
		/// </summary>
		[OdinSerialize]
		protected T DefaultValue { get; private set; }
		#endregion

		#region OPERATIONS - INDEXING
		/// <summary>
		/// "Shifts" this GameToggle in the specified direction.
		/// </summary>
		/// <param name="shiftType">The direction to shift this toggle in.</param>
		public void Shift(ToggleShiftType shiftType) {
			// Just use the version of this function that takes an int.
			this.Shift(moveAmount: shiftType == ToggleShiftType.Left ? -1 : 1);
		}
		/// <summary>
		/// "Shifts" this GameToggleDX by the specified number of indicies.
		/// </summary>
		/// <param name="moveAmount"></param>
		private void Shift(int moveAmount) {
			// Figure out which enum "comes next" and assign it.
			this.currentValue = this.ComputeEnumShift(currentValue: this.currentValue, moveAmount: moveAmount);
		}
		/// <summary>
		/// Computes the enum constant when given the current enum value and the number of positions to move.
		/// </summary>
		/// <param name="currentValue">The current enum constant.</param>
		/// <param name="moveAmount">The amount of spaces to move.</param>
		/// <returns>Whatever enum should be given as a result of the shift.</returns>
		private T ComputeEnumShift(T currentValue, int moveAmount) {
			// Create a list of all the available values for this enum.
			List<T> enumValues = System.Enum.GetValues(typeof(T)).Cast<T>().ToList();
			// Determine the index of the current value inside of that list.
			int currentIndex = enumValues.IndexOf(item: currentValue);
			// Compute the new index for what I may want to retrieve from that list.
			int newIndex = this.ComputeIndexShift(moveAmount: moveAmount, currentIndex: currentIndex, optionCount: enumValues.Count);
			// Return the enum value at that new index location.
			return enumValues[newIndex];
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - GTSUBITEM
		/// <summary>
		/// The SubItemParameters that represent the state this toggle is currently in.
		/// </summary>
		public override SubItemParams CurrentSubItemParams {
			get {
				return new SubItemParams(
					subItemType: SubItemType.SimpleToggle,
					currentOption: this.GetToggleEnum().ToString());
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - GTHORIZONTALMOVEHANDLER
		/// <summary>
		/// Most of the time, moving means shifting.
		/// Made virtual just in case I want to do something different.
		/// </summary>
		/// <param name="moveDir">The direction being moved in.</param>
		public virtual void OnHorizontalMenuMove(HorizontalMoveDirType moveDir) {
			this.Shift(shiftType: moveDir == HorizontalMoveDirType.Left ? ToggleShiftType.Left : ToggleShiftType.Right);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITOGGLEENUM
		/// <summary>
		/// Returns the currently stored enum for this toggle.
		/// </summary>
		/// <returns>The currently stored enum constant for this toggle.</returns>
		public T GetToggleEnum() {
			return this.currentValue;
		}
		#endregion

		#region OPERATIONS - CLONING
		public override GameToggleDX GetDefaultConfiguration() {
			// Create a clone of this toggle.
			StandardEnumToggle<T> clone = (StandardEnumToggle<T>)this.Clone();
			// Assign the current value to be the default value.
			clone.currentValue = this.DefaultValue;
			// Return the clone.
			return clone;
		}
		#endregion

	}


}