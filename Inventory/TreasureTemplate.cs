using System.Collections;
using System.Collections.Generic;
using Grawly.Battle;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Linq;
namespace Grawly {
	
	/// <summary>
	/// Contains information regarding what is stored in a treasure chest.
	/// This obviously can be repurposed for anything, but it's main utility is containing multiple kinds of objects.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Treasure")]
	public class TreasureTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The items that are contained within this treasure.
		/// </summary>
		[OdinSerialize]
		public List<BattleBehavior> AllItems { get; private set; } = new List<BattleBehavior>();
		#endregion

		#region PROPERTIES - DESCRIPTION
		/// <summary>
		/// The text that should be displayed in the chat window when obtaining this treasure.
		/// </summary>
		public string TreasureDescription {
			get {
				return string.Join(", ", this.AllItems.Select(i => i.behaviorName));
			}
		}
		#endregion
		
	}
}