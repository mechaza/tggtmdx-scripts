using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

namespace Grawly.UI.MenuLists {
	
	/// <summary>
	/// Can be used in more complex MenuLists to dynamically build MenuItems without defining new classes.
	/// </summary>
	public class DynamicItemParams : IMenuable {

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString { get; }
		public string QuantityString { get; }
		public string DescriptionString { get; }
		public Sprite Icon { get; }
		#endregion
		
	}
}