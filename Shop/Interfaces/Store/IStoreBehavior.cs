using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.Shop.Behaviors {

	#region GENERAL
	/// <summary>
	/// The top of the tree of store behavior interfaces.
	/// Wig!
	/// </summary>
	public interface IStoreEvent {
		
	}
	/// <summary>
	/// Gets invoked when the store is opened.
	/// </summary>
	public interface IOnEnterStore : IStoreEvent {
		
	}
	#endregion
	
}