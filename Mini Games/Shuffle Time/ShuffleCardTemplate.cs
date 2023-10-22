using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;


namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// The template that gets used to create a card in Shuffle Time.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shuffle Time/Card Template")]
	public class ShuffleCardTemplate : SerializedScriptableObject {

		#region FIELDS - METADATA
		/// <summary>
		/// The name of this card.
		/// </summary>
		[OdinSerialize, Title("Metadata")]
		public string CardName { get; private set; } = "";
		/// <summary>
		/// The text that should be shown when this card is picked.
		/// </summary>
		[OdinSerialize]
		public string CardFlavorText { get; private set; } = "";
		/// <summary>
		/// The sprite that should be used to represent this card.
		/// </summary>
		[OdinSerialize]
		public Sprite CardSprite { get; private set; }
		#endregion

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behaviors that this card should use when created from this template.
		/// </summary>
		[OdinSerialize, Title("Behaviors"), ListDrawerSettings(Expanded = true), InlineProperty]
		private List<ShuffleCardBehavior> cardBehaviors = new List<ShuffleCardBehavior>();
		/// <summary>
		/// The behaviors that this card should use when created from this template.
		/// </summary>
		public List<ShuffleCardBehavior> CardBehaviors {
			get {
				// Clone the cards before sending them off.
				return this.cardBehaviors.Select(b => b.Clone()).ToList();
			}
		}
		#endregion

	}


}