using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Grawly.Friends.Behaviors;
using Grawly.UI;
using Grawly.UI.MenuLists;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Friends {
	
	/// <summary>
	/// A template to store both the friend behavior and the actual rank required to use it.
	/// </summary>
	[System.Serializable]
	public class FriendRankDefinition : IMenuable {

		#region FIELDS - GENERAL
		/// <summary>
		/// The rank required to use the behavior in this template.
		/// </summary>
		[OdinSerialize]
		public int Rank { get; private set; } = 0;
		#endregion

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behavior for this rank.
		/// </summary>
		[OdinSerialize]
		private FriendBehavior friendBehavior;
		/// <summary>
		/// The behavior for this rank.
		/// </summary>
		public FriendBehavior FriendBehavior {
			get {
				return this.friendBehavior.Clone();
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString {
			get {
				return this.friendBehavior.BehaviorName;
			}
		}
		public string QuantityString  {
			get {
				return this.Rank + "";
			}
		}
		public string DescriptionString  {
			get {
				return this.friendBehavior.BehaviorDescription;
			}
		}
		public Sprite Icon  {
			get {
				return DataController.GetDefaultElementalIcon(ElementType.Assist);
			}
		}
		#endregion
		
	}

	
}