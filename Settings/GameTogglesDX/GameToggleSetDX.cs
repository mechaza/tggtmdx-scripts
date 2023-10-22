using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace Grawly.Toggles {

	/// <summary>
	/// This is where I can store and retreieve GameToggleDX's at runtime.
	/// </summary>
	[System.Serializable, HideReferenceObjectPicker, InlineProperty, HideLabel]
	public class GameToggleSetDX {

		#region FIELDS - STATE
		/// <summary>
		/// The GameToggles currently being used inside of this set.
		/// </summary>
		[OdinSerialize, HideInEditorMode, ListDrawerSettings(Expanded = true)]
		public List<GameToggleDX> GameToggles { get; private set; } = new List<GameToggleDX>();
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a GameToggleSetDX from any number of toggle sets.
		/// This basically merges the lists so that I can refer to all of them from one set.
		/// </summary>
		/// <param name="sets">Any number of sets I want to merge together.</param>
		public GameToggleSetDX(params GameToggleSetDX[] sets) {
			
			this.GameToggles = sets.SelectMany(s => s.GameToggles).ToList();
		}
		/// <summary>
		/// Creates a GameToggleSetDX from a bunch of GameToggleDX's passed in.
		/// </summary>
		/// <param name="gameToggles">Any number of GameToggleDXs to pass in.</param>
		public GameToggleSetDX(List<GameToggleDX> gameToggles) {
			// I assume this will work.
			this.GameToggles = gameToggles;
		}
		#endregion

		#region OPERATIONS - GETTING TOGGLES
		/// <summary>
		/// Gets the toggle of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of GameToggle being retrieved.</typeparam>
		/// <returns>The GameToggle of the specified type.</returns>
		public T GetToggle<T>() where T : GameToggleDX {
			// It should be assumed that there is only going to be one toggle here.
			return (T)this.GameToggles.First(gt => gt is T);
		}
		/// <summary>
		/// Returns all of the GameToggles that implement the given component.
		/// </summary>
		/// <typeparam name="T">The kind of component that the toggle should implement.</typeparam>
		/// <returns>All the toggles that match the given type.</returns>
		public List<T> GetToggles<T>() where T : GTComponent {
			// Find all the toggles that implement whatever type was given and return them.
			return this.GameToggles
				.Where(gt => gt is T)
				.Cast<T>()
				.ToList();
		}
		/// <summary>
		/// Gets all of the GameToggleDXs that match the specified category.
		/// </summary>
		/// <param name="categoryType">The type of category to look for.</param>
		/// <returns>The toggles that match the specified category.</returns>
		public List<GameToggleDX> GetToggles(GameToggleCategoryType categoryType) {
			return this.GameToggles
				.Where(gt => gt.CategoryType == categoryType)
				.ToList();
		}
		#endregion

		#region OPERATIONS - ADDING TOGGLES 
		/// <summary>
		/// Adds a toggle to this set. Only does so if a toggle of the provided type does not already exist.
		/// Otherwise, it gets ignored.
		/// </summary>
		/// <param name="gameToggle">The toggle to add.</param>
		public void AddToggle(GameToggleDX gameToggle) {
			// Check if any of the toggles match the type of the toggle passed in.
			if (this.GameToggles.Any(t=> gameToggle.GetType().FullName == t.GetType().FullName)) {
				// Do nothing if there is a match.
				Debug.LogWarning("Toggle of type " + gameToggle.GetType().Name + " already exists. Skipping.");
			} else {
				// If no toggle of the same type was found, add it.
				this.GameToggles.Add(gameToggle.Clone());
			}
		}
		/// <summary>
		/// Adds the game toggles provided.
		/// Note that if there is already a toggle of the same type in this set,
		/// the one attempting to be added will be ignored.
		/// I'm mostly using this function for like, debug purposes.
		/// </summary>
		/// <param name="gameToggles">The GameToggles to add.</param>
		public void AddToggles(List<GameToggleDX> gameToggles) {
			foreach (GameToggleDX gt in gameToggles) {
				this.AddToggle(gameToggle: gt);
			}
		}
		#endregion

		#region OPERATIONS - CLONING
		/// <summary>
		/// Clones this GameToggleSetDX.
		/// Helpful for if I need to do shit in menus and need to revert changes.
		/// </summary>
		/// <returns>A clone of this toggle set.</returns>
		public GameToggleSetDX Clone() {
			GameToggleSetDX clone = (GameToggleSetDX)this.MemberwiseClone();
			clone.GameToggles = this.GameToggles.Select(gt => gt.Clone()).ToList();
			return clone;
		}
		/// <summary>
		/// Gets the default configurations for whatever toggles are in here and returns the set as a clone.
		/// </summary>
		/// <returns>A clone of this set but everything is default settings.</returns>
		public GameToggleSetDX GetDefaultSet() {
			GameToggleSetDX clone = (GameToggleSetDX)this.MemberwiseClone();
			clone.GameToggles = this.GameToggles.Select(gt => gt.GetDefaultConfiguration()).ToList();
			return clone;
		}
		#endregion
		
	}


}