using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.UI;
using System.Linq;
using Grawly.Battle;
using Grawly.UI.MenuLists;

namespace Grawly.Toggles {

	/// <summary>
	/// Contains a saved toggle as well as the functionality to figure out how to make it work.
	/// </summary>
	[System.Serializable, HideReferenceObjectPicker]
	public abstract class GameToggleDX : IMenuable {
		
		#region FIELDS - METADATA
		/// <summary>
		/// The name of the toggle.
		/// Usually will map to the primary string.
		/// </summary>
		public abstract string ToggleName { get; }
		/// <summary>
		/// The toggle string.
		/// Will usually map to the description string.
		/// </summary>
		public abstract string ToggleDescription { get; }
		/// <summary>
		/// The category this toggle belongs to.
		/// Helpful for things like organization.
		/// </summary>
		public abstract GameToggleCategoryType CategoryType { get; }
		/// <summary>
		/// The context in which this toggle will be used in.
		/// Distinction between GameSave and SaveCollection.
		/// </summary>
		public abstract GameToggleDXContextType ContextType { get; }
		#endregion

		#region OPERATIONS - INDEXING
		/// <summary>
		/// Computes an index when moving the specified number of indicies.
		/// </summary>
		/// <param name="moveAmount">The number of indicies to move.</param>
		/// <param name="currentIndex">The current index.</param>
		/// <param name="optionCount">The number of options there are.</param>
		/// <returns>The new index.</returns>
		protected int ComputeIndexShift(int moveAmount, int currentIndex, int optionCount) {
			if (moveAmount != -1 && moveAmount != 1) {
				throw new System.NotImplementedException("Fix this function so it accepts values other than -1 or 1.");
			}

			// Add that to the current index.
			int newIndex = currentIndex + moveAmount;
			// this.currentIndex += moveAmount;

			// If it's out of range, loop it back.
			if (newIndex == optionCount) {
				// Reaching the length of the param values loops it back to zero.
				newIndex = 0;
			} else if (newIndex < 0) {
				// Going into the negatives should loop back to the end of the list.
				newIndex = (optionCount - 1);
			}

			return newIndex;
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Returns a clone of GameToggleDX in a state where its in default configuration.
		/// </summary>
		/// <returns>A clone of this GameToggleDX in default mode.</returns>
		public abstract GameToggleDX GetDefaultConfiguration();
		/// <summary>
		/// Clones this GameToggleDX. Good for copying from a template.
		/// Is virtual because some subclasses may need to do a deeper configuration.
		/// </summary>
		/// <returns>A clone of this GameToggleDX.</returns>
		public virtual GameToggleDX Clone() {
			return (GameToggleDX)this.MemberwiseClone();
		}
		#endregion

		#region STATIC GETTERS
		/// <summary>
		/// A shortcut to getting a game toggle of a given type.
		/// Contacts the ToggleController for this.
		/// </summary>
		/// <typeparam name="T">The kind of toggle to get.</typeparam>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static T GetToggle<T>() where T : GameToggleDX {
			return ToggleController.GetToggle<T>();
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public abstract string PrimaryString { get; }
		public abstract string QuantityString { get; }
		public abstract string DescriptionString { get; }
		public abstract Sprite Icon { get; }
		#endregion

	}


}