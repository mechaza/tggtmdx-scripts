using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// Contains the functionality of a Shuffle Time card.
	/// This is mostly the data representation of the card, as they will likely persist after Shuffle Time is over.
	/// </summary>
	[System.Serializable]
	public class ShuffleCard {

		#region FIELDS - STATE
		/// <summary>
		/// The template that was used to create this card.
		/// </summary>
		[OdinSerialize]
		public ShuffleCardTemplate Template { get; private set; }
		/// <summary>
		/// The list of behaviors this card is maintaining.
		/// Gets cloned from the template when its created.
		/// </summary>
		[OdinSerialize]
		public List<ShuffleCardBehavior> Behaviors { get; private set; }
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Creates a shuffle card to be used from the template passed in.
		/// </summary>
		/// <param name="cardTemplate"></param>
		public ShuffleCard(ShuffleCardTemplate cardTemplate) {
			this.Template = cardTemplate;
			// Note that this makes a clone of the behaviors from the template.
			this.Behaviors = cardTemplate.CardBehaviors;
		}
		#endregion

	}


}