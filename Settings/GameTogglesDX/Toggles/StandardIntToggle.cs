using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI.SubItem;
using Grawly.UI;

namespace Grawly.Toggles {

	/// <summary>
	/// A GameToggleDX that is going to mostly provide some kind of integer.
	/// </summary>
	[System.Serializable]
	public abstract class StandardIntToggle : StandardToggleDX, GTIntHaver, GTIndexShifter, GTHorizontalMoveHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The current value stored by this toggle.
		/// </summary>
		[OdinSerialize, HideInInspector]
		private int currentValue = 0;
		/// <summary>
		/// The current value stored by this toggle.
		/// </summary>
		protected int CurrentValue {
			get {
				// NOTE: The current value isn't guaranteed to be within bounds
				// if I don't create default configurations at one point.
				return this.currentValue;
			}
			set {
				// If this is being clamped, use the clamp method.
				if (this.isClamped == true) {
					this.currentValue = Mathf.Clamp(value: value, min: this.min, max: this.max);
				} else {
					// If it's not being clamped just assign it.
					this.currentValue = value;
				}
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The default value for this int toggle.
		/// Gets used when cloning mostly.
		/// </summary>
		[OdinSerialize, ValidateInput(condition: "DefaultValueIsWithinRange", defaultMessage: "Default value must be within range!")]
		protected int DefaultValue { get; private set; }
		/// <summary>
		/// Does this toggle need to be clamped between two different values?
		/// </summary>
		[SerializeField]
		private bool isClamped = true;
		/// <summary>
		/// The minimum allowed value, if clamping is on.
		/// </summary>
		[OdinSerialize, ShowIf("isClamped")]
		private int min = 0;
		/// <summary>
		/// The maximum allowed value, if clamping is on.
		/// </summary>
		[OdinSerialize, ShowIf("isClamped")]
		private int max = 10;
		/// <summary>
		/// The amount that should be added/subtracted each "shift".
		/// </summary>
		[SerializeField]
		private int incrementAmount = 1;
		#endregion

		#region OVERRIDES
		/// <summary>
		/// The default configuration is usually going to be
		/// replacing the current value with the default.
		/// </summary>
		/// <returns>A clone of this toggle with default configurations.</returns>
		public override GameToggleDX GetDefaultConfiguration() {
			// Clone this toggle.
			StandardIntToggle clone = (StandardIntToggle)this.Clone();
			// Set its current value to the default.
			clone.CurrentValue = this.DefaultValue;
			// Return it.
			return clone;
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
					currentOption: this.GetToggleInt().ToString());
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

		#region INTERFACE IMPLEMENTATION - GTINDEXSHIFTER
		/// <summary>
		/// Increments/decrements the toggle based on the direction specified.
		/// Virtual in the event I want to override this functionality.
		/// </summary>
		/// <param name="shiftType">Shifting left or right.</param>
		public virtual void Shift(ToggleShiftType shiftType) {
			// Add/subtract the increment amount based on whether left or right was hit.
			this.CurrentValue += this.incrementAmount * (shiftType == ToggleShiftType.Left ? -1 : 1);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITOGGLEINT
		/// <summary>
		/// Retrieves an integer this toggle currently manages.
		/// </summary>
		/// <returns></returns>
		public int GetToggleInt() {
			// Just return the current value.
			return this.CurrentValue;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public override string QuantityString => this.GetToggleInt().ToString();
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// Verifies that the default value is clamped appropriately,
		/// if clamping is being used.
		/// </summary>
		/// <param name="defaultValue">The current default value.</param>
		/// <returns></returns>
		private bool DefaultValueIsWithinRange(int defaultValue) {
			if (this.isClamped == true) {
				return !(defaultValue < this.min || this.max < defaultValue);
			} else {
				return true;
			}
		}
		#endregion

	}


}