using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// A container class to hold a set of shuffle cards to be used in shuffle time.
	/// </summary>
	[System.Serializable]
	public class ShuffleCardDeck {

		#region FIELDS - CARDS
		/// <summary>
		/// The cards used for this deck.
		/// </summary>
		[OdinSerialize]
		public List<ShuffleCard> ShuffleCards { get; private set; } = new List<ShuffleCard>();
		#endregion

		#region CONSTRUCTORS
		public ShuffleCardDeck() {

		}
		public ShuffleCardDeck(List<ShuffleCard> shuffleCards) {
			/*Debug.LogWarning("Note that the cards being added for this constructor may or may not have been cloned. If there are issues, look here.");
			this.ShuffleCards.AddRange(shuffleCards);*/
			this.AddCards(shuffleCards);
		}
		public ShuffleCardDeck(List<ShuffleCardTemplate> cardTemplates) {
			// Transform the card templates into actual cards.
			this.AddCards(cardTemplates
				.Select(t => new ShuffleCard(cardTemplate: t))
				.ToList());
			/*this.ShuffleCards = cardTemplates
				.Select(t => new ShuffleCard(cardTemplate: t))
				.ToList();*/
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Add a single card to the deck.
		/// </summary>
		/// <param name="shuffleCard">The card to add to the deck.</param>
		/// <returns>This deck, but with the card added.</returns>
		public ShuffleCardDeck AddCard(ShuffleCard shuffleCard) {
			this.ShuffleCards.Add(shuffleCard);
			return this;
		}
		/// <summary>
		/// Add multiple cards to the deck.
		/// </summary>
		/// <param name="shuffleCards">The cards to add to the deck.</param>
		/// <returns>This deck, but with the cards added.</returns>
		public ShuffleCardDeck AddCards(List<ShuffleCard> shuffleCards) {
			this.ShuffleCards.AddRange(shuffleCards);
			return this;
		}
		#endregion

		#region RETRIEVAL
		/// <summary>
		/// Gets a random amount of cards from the deck.
		/// Will clamp the count if asking for too many.
		/// </summary>
		/// <param name="count">The number of cards to retrieve.</param>
		/// <returns>A set of random cards.</returns>
		public List<ShuffleCard> GetRandomCards(int count) {

			// Double check that the requested amount is valid.
			if (count > this.ShuffleCards.Count) {
				Debug.LogWarning("The number of cards requested is higher than the number in the deck! Returning all cards.");
			}

			// Clone the list of cards in this deck.
			List<ShuffleCard> clonedList = new List<ShuffleCard>(this.ShuffleCards);
			// Also create a placeholder list to add to.
			List<ShuffleCard> randomCards = new List<ShuffleCard>();
			// Figure out how many cards to grab.
			int min = Mathf.Min(a: count, b: clonedList.Count);
			
			// Get a random card one by one.
			for (int i = 0; i < min; i++) {
				randomCards.Add(clonedList.PullRandom());
			}

			return randomCards;
		}
		#endregion

		#region CLONING
		public ShuffleCardDeck Clone() {
			// Create anew variable.
			ShuffleCardDeck clone = new ShuffleCardDeck();
			// Go through each card in here and create a NEW card from it. This requires passing the template.
			clone.ShuffleCards = this.ShuffleCards.Select(c => new ShuffleCard(cardTemplate: c.Template)).ToList();
			// Return the new deck, which is full of new cards.
			return clone;
		}
		#endregion

	}


}