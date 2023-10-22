using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.Toggles;
using UnityEngine;

namespace Grawly.UI.MenuLists {

	/// <summary>
	/// Literally only to make it easier for me to define how things get passed around when building menu lists.
	/// Children of MenuList and MenuListItem will be catered to specific implementations of this interface
	/// but I need it for their abstract/parent methods.
	/// </summary>
	public interface IMenuable {
		
		/// <summary>
		/// The string that should typically be used for the main label.
		/// </summary>
		string PrimaryString { get; }
		/// <summary>
		/// The string that should typically be used for the "quantity."
		/// </summary>
		string QuantityString { get; }
		/// <summary>
		/// The string that should typically be used for the description.
		/// </summary>
		string DescriptionString { get; }
		/// <summary>
		/// The icon. May be null.
		/// </summary>
		Sprite Icon { get; }

	}

}