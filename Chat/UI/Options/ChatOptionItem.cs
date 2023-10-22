using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Grawly.Chat {

	/// <summary>
	/// The items that can be chosen from in a ChatOptionPicker during a chat.
	/// </summary>
	public abstract class ChatOptionItem : SerializedMonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The parameters that were passed into this option item when it was built.
		/// </summary>
		protected ChatOptionItemParams optionItemParams;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the visuals for the option item.
		/// </summary>
		[SerializeField, TabGroup("Option Item", "Scene References"), ShowInInspector, Title("All Visuals")]
		protected GameObject allVisuals;
		#endregion

		#region MAIN CALLS : STATE
		/// <summary>
		/// Totally resets the state of the option item.
		/// </summary>
		protected abstract void ResetState();
		/// <summary>
		/// Sets whether this chat option item should be active or not.
		/// (This is important for when I define many items in scene but only use a couple of them at a time)
		/// </summary>
		/// <param name="status">Whether it is active or not.</param>
		public void SetActive(bool status) {
			this.allVisuals.SetActive(status);
		}
		/// <summary>
		/// Gets called when this is the option that was picked.
		/// Will fail if it was not built, but if this is being called, it's fair to assume it was.
		/// </summary>
		public void PickedOption() {
			this.PickedOption(optionItemParams: this.optionItemParams);
		}
		/// <summary>
		/// Gets called when this is the option that was picked.
		/// </summary>
		/// <param name="optionItemParams">The parameters that were used when constructing this option item.</param>
		protected abstract void PickedOption(ChatOptionItemParams optionItemParams);
		#endregion

		#region MAIN CALLS : ANIMATIONS AND SHIT
		/// <summary>
		/// Builds and prepares the option item with the parameters passed in.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		public abstract void Build(ChatOptionItemParams optionItemParams);
		/// <summary>
		/// Dismisses this specific option.
		/// Will fail if it has not been built, but... if it's being dismissed it should have been built anyway.
		/// </summary>
		public void Dismiss() {
			this.Dismiss(optionItemParams: optionItemParams);
		}
		/// <summary>
		/// Dismisses this specific option.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected abstract void Dismiss(ChatOptionItemParams optionItemParams);
		#endregion

		#region GRAPHICS
		/// <summary>
		/// Sets the graphics on the item to look like it has been highlighted.
		/// Assumes it has already been built. Likely will fail otherwise.
		/// </summary>
		public void Highlight() {
			this.Highlight(optionItemParams: this.optionItemParams);
		}
		/// <summary>
		/// Sets the graphics on the item to look like it has been dehighlighted.
		/// Assumes it has already been built. Likely will fail otherwise.
		/// </summary>
		public void Dehighlight() {
			this.Dehighlight(optionItemParams: this.optionItemParams);
		}
		/// <summary>
		/// Sets the graphics on the item to look like it has been highlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected abstract void Highlight(ChatOptionItemParams optionItemParams);
		/// <summary>
		/// Sets the graphics on the item to look like it has been dehighlighted.
		/// </summary>
		/// <param name="optionItemParams">The parameters that describe how the item should be built.</param>
		protected abstract void Dehighlight(ChatOptionItemParams optionItemParams);
		#endregion

	}


}