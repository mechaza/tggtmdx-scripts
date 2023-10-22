using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.Battle.Modifiers;

namespace Grawly.Battle.Equipment.Badges.Behaviors {

	/// <summary>
	/// Adds a move to the combatant's moveset.
	/// </summary>
	[System.Serializable, InfoBox("Adds battle behaviors to a combatant's moveset. Does not have OnBadgeAdd/OnBadgeRemove events."), Title("Simple Behavior Adder"), HideReferenceObjectPicker]
	public class SimpleBehaviorAdder : BadgeBehavior, IBehaviorAdder, IBadgeFactHaver {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The battle behaviors that should be added to the combatant's moveset.
		/// </summary>
		[OdinSerialize, ListDrawerSettings(Expanded = true)]
		private List<BattleBehavior> BattleBehaviors { get; set; } = new List<BattleBehavior>();
		#endregion

		#region INTERFACE IMPLEMENTATION - IBADGEFACTHAVER
		/// <summary>
		/// The badge facts that should be used on display on the BadgeInfo.
		/// </summary>
		public List<BadgeFact> BadgeFacts {
			get {
				List<BadgeFact> badgeFacts = new List<BadgeFact>();
				string factText = this.BattleBehaviors.First().behaviorName;
				badgeFacts.Add(new BadgeFact() {
					FactText = factText
				});
				return badgeFacts;
			}
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IBEHAVIORINJECTOR
		/// <summary>
		/// Injects the specified behaviors into the list that was passed in.
		/// </summary>
		/// <param name="self">The combatant who is wearing this badge.</param>
		public List<BattleBehavior> GetBattleBehaviorsToAdd(Combatant self) {
			return this.BattleBehaviors;
		}
		#endregion

		
		
	}


}