using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.UI {

	/// <summary>
	/// A basic enum for just saying Left or Right.
	/// Their int values are good for scrolling.
	/// </summary>
	public enum HorizontalMoveDirType {
		Left = -1,
		Right = 1,
	}

	/// <summary>
	/// A basic enum for just saying up or down.
	/// </summary>
	public enum VerticalMoveDirType {
		Up		= -1,
		Down	= 1,
	}

}