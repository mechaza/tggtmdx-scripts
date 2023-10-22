using System.Collections;
using System.Collections.Generic;
using Grawly.UI.MenuLists;
using UnityEngine;

namespace Grawly.UI.Special {

	/// <summary>
	/// A delegate that defines what the default index of a menuable should be when the entire list is bult.
	/// </summary>
	/// <param name="menuable">The menuable this delegate is attached to.</param>
	/// <returns></returns>
	public delegate int DefaultValueDelegate(IMenuable menuable);
	/// <summary>
	/// Restores the saved value
	/// </summary>
	/// <param name="menuable"></param>
	/// <returns></returns>
	public delegate int RestoreSavedValueDelegate(IMenuable menuable);
	/// <summary>
	/// A delegate that defines functionality to be run when moving horizontally along a menulistitem.
	/// </summary>
	/// <param name="menuable">The menuable this callback is attached to.</param>
	/// <param name="moveDir">The direction of movement that was received.</param>
	public delegate void HorizontalMoveDelegate(IMenuable menuable, HorizontalMoveDirType moveDir);
	/// <summary>
	/// Gets called when the value of this menuable's sub item or some other shit gets changed. 
	/// NOTE THAT THE CHANGE WAS ALREADY DONE IF THIS IS GETTING CALLED.
	/// DO NOT ADJUST ANY SUB ITEM INDICIES. THATS WHATS BEING PASSED IN.
	/// </summary>
	/// <param name="menuable">The menuable this callback is attached to.</param>
	/// <param name="previousIndex">The previous value.</param>
	/// <param name="newIndex">The new value.</param>
	public delegate void ChangeValueDelegate(IMenuable menuable, int previousIndex, int newIndex);
	/// <summary>
	/// Gets called when changes are applied. 
	/// Mostly relevant in the settings menu.
	/// </summary>
	/// <param name="menuable">The menuable this callback is attached to.</param>
	public delegate void CommitChangesDelegate(IMenuable menuable);
}