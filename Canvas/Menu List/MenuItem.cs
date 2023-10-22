using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace Grawly.UI.MenuLists {

	/// <summary>
	/// Contains a framework for how items in a menu list should be represented.
	/// </summary>
	public abstract class MenuItem : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler, IMoveHandler {

		#region FIELDS - STATE
		/// <summary>
		/// Is this item available for use? I.e., if not, will play a buzzer sound or soemthing.
		/// </summary>
		protected abstract bool IsUsable { get; }
		#endregion

		#region BUILDING THE ITEM
		/// <summary>
		/// Builds the MenuListItem with the item specified.
		/// </summary>
		/// <param name="item">The item to build this menu with.</param>
		public abstract void BuildMenuItem(IMenuable item);
		#endregion

		#region VISUALS
		/// <summary>
		/// Adjusts the graphics on this item so it appears to be highlighted.
		/// </summary>
		/// <param name="item">The data from which to read from when building the graphics.</param>
		protected internal abstract void Highlight(IMenuable item);
		/// <summary>
		/// Adjusts the graphics on this item so it appears to be dehighlighted.
		/// </summary>
		/// <param name="item">The data from which to read from when building the graphics.</param>
		protected internal abstract void Dehighlight(IMenuable item);
		#endregion

		#region UI EVENT TRIGGERS
		public abstract void OnSelect(BaseEventData eventData);
		public abstract void OnDeselect(BaseEventData eventData);
		public abstract void OnSubmit(BaseEventData eventData);
		public abstract void OnCancel(BaseEventData eventData);
		public virtual void OnMove(AxisEventData eventData) {
			// If left/right was hit, call my custom Move function.
			switch (eventData.moveDir) {
				case MoveDirection.Left:
					this.OnHorizontalMove(moveDir: HorizontalMoveDirType.Left);
					break;
				case MoveDirection.Right:
					this.OnHorizontalMove(moveDir: HorizontalMoveDirType.Right);
					break;
				default:
					break;
			}
		}
		#endregion

		#region CUSTOM EVENT TRIGGERS
		/// <summary>
		/// A special function called in OnMove that captures horizontal events.
		/// </summary>
		/// <param name="moveDir">Whether left or right was pressed.</param>
		protected virtual void OnHorizontalMove(HorizontalMoveDirType moveDir) {
			// This is more or less blank in the event that I want to add implementation to the child classes.
			// It is not manditory, hence why it is not abstract.
		}
		#endregion

	}


}