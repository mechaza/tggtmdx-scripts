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
	public abstract class StandardBoolToggle : StandardToggleDX, GTHorizontalMoveHandler, GTBoolHaver, GTIndexShifter {

		#region FIELDS - STATE
		/// <summary>
		/// The current value stored by this toggle.
		/// </summary>
		[OdinSerialize, HideInInspector]
		private bool currentValue = false;
		/// <summary>
		/// The current value stored by this toggle.
		/// </summary>
		protected bool CurrentValue {
			get {
				return this.currentValue;
			}
			set {
				this.currentValue = value;
			}
		}
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The default value for this int toggle.
		/// Gets used when cloning mostly.
		/// </summary>
		[OdinSerialize]
		protected bool DefaultValue { get; private set; }
		#endregion

		#region OVERRIDES
		/// <summary>
		/// The default configuration is usually going to be
		/// replacing the current value with the default.
		/// </summary>
		/// <returns>A clone of this toggle with default configurations.</returns>
		public override GameToggleDX GetDefaultConfiguration() {
			// Clone this toggle.
			StandardBoolToggle clone = (StandardBoolToggle)this.Clone();
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
					currentOption: this.GetToggleBool().ToString());
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
		/// On a shift, I basically just flip it.
		/// Virtual in the event I want to override this functionality.
		/// </summary>
		/// <param name="shiftType">Shifting left or right.</param>
		public virtual void Shift(ToggleShiftType shiftType) {
			// Son of a bitch everything is real.
			this.CurrentValue = !this.CurrentValue;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - ITOGGLEINT
		/// <summary>
		/// Retrieves an integer this toggle currently manages.
		/// </summary>
		/// <returns></returns>
		public bool GetToggleBool() {
			// Just return the current value.
			return this.CurrentValue;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public override string QuantityString => this.GetToggleBool().ToString();
		#endregion

	}


}