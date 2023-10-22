using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// A routine that can get called to tell Shuffle Time how to behave.
	/// These will be attached someplace in the scene probably? Who knows.
	/// </summary>
	public abstract class ShuffleTimeTable : MonoBehaviour {


		//
		//	I'm trying to keep this as stateless as possible but I do need to remember the set and the deck sometimes.
		//

		#region FIELDS - STATE
		/// <summary>
		/// The card sprite set that is currently being managed by this table.
		/// It's protected with private set because I don't want the tables to influence it.
		/// </summary>
		protected ShuffleCardSpriteSet CurrentCardSpriteSet { get; private set; }
		/// <summary>
		/// The deck that this table is currently managing.
		/// </summary>
		protected ShuffleCardDeck CurrentCardDeck { get; private set; }
		#endregion

		#region PRESENTATION - TABLE
		/// <summary>
		/// Requests the display of a table with the specified deck and sprite set.
		/// This version uses the maximum allowed of cards in the deck/sprites in the set.
		/// </summary>
		/// <param name="deck">The deck to use when displaying this table.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		public void RequestTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet) {
			// Get the maximum allowed number of cards.
			int maxCount = GetMaximumAllowedCardCount(shuffleCardDeck: deck, cardSpriteSet: cardSpriteSet);

			// Call the version of this function that takes the count. Its gonna do the max check again but whatever.
			this.RequestTable(deck: deck, cardSpriteSet: cardSpriteSet, count: maxCount);

		}
		/// <summary>
		/// Requests the display of a table with the specified deck and sprite set.
		/// </summary>
		/// <param name="deck">The deck to use when displaying this table.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		/// <param name="count">The number of cards to involve.</param>
		public void RequestTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet, int count) {

			// Remember the set and the deck.
			this.CurrentCardSpriteSet = cardSpriteSet;
			this.CurrentCardDeck = deck;

			// Figure out what the max count is.
			int maxCount = GetMaximumAllowedCardCount(shuffleCardDeck: deck, cardSpriteSet: cardSpriteSet);

			// If the requested count is too high, log it out and lower the count.
			if (count > maxCount) {
				Debug.LogError("Cannot request a table with count " + count + ". Will use max allowed of " + maxCount);
			}

			// Display the table now that I know everything has been verified.
			this.DisplayTable(deck: deck, cardSpriteSet: cardSpriteSet, count: count);
		}
		/// <summary>
		/// Preps the table for use with the specified deck.
		/// </summary>
		/// <param name="deck">The deck to use when displaying this table.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		/// <param name="count">The number of cards to use for this table.</param>
		protected abstract void DisplayTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet, int count);
		/// <summary>
		/// Dismisses the table from view.
		/// </summary>
		protected void DismissTable() {
			// Just call the version that takes the deck and sprite set that was remembered.
			this.DismissTable(deck: this.CurrentCardDeck, cardSpriteSet: this.CurrentCardSpriteSet);
		}
		/// <summary>
		/// Dismisses the table from view.
		/// </summary>
		/// <param name="deck">The deck that was being used for this table.</param>
		/// <param name="cardSpriteSet">The sprite set that was being used for this table.</param>
		protected abstract void DismissTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet);
		#endregion

		#region PRESENTATION - CARDS
		/// <summary>
		/// Prepares the card sprites with the information they need to be used in the table.
		/// </summary>
		/// <param name="deck">The deck containing the cards to use.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		/// <param name="count">The number of sprites to pass back.</param>
		/// <returns>The same list of sprites passed in, but prepared.</returns>
		protected abstract List<ShuffleCardSprite> PrepareCardSprites(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet, int count);
		/// <summary>
		/// Gets whether a card will show its face or not by default on preparation.
		/// </summary>
		/// <param name="shuffleCard">The shuffle card who may be shown face up or down.</param>
		/// <returns>Whether the card is going to be shown face up or down by default.</returns>
		public abstract bool GetDefaultCardFace(ShuffleCard shuffleCard);
		#endregion

		#region EVENTS FROM CARDS
		/// <summary>
		/// Gets called when a card sprite is picked.
		/// </summary>
		/// <param name="sprite">The sprite that was picked.</param>
		public abstract void CardPicked(ShuffleCardSprite sprite);
		/// <summary>
		/// Gets called when a cancel action is requested.
		/// </summary>
		/// <param name="sprite">The sprite that was highlighted when this was hit. If one was highlighted.</param>
		public abstract void CancelRequested(ShuffleCardSprite sprite = null);
		#endregion

		#region HELPERS
		/// <summary>
		/// Returns the number of cards that can be used with this sprite set with the given deck.
		/// I.e., if there are too few cards in the deck, I can't use all the sprites, and
		/// if there are too few sprites in the set, I can't use all the cards in the deck.
		/// </summary>
		/// <param name="shuffleCardDeck">The card deck worth checking.</param>
		/// <param name="cardSpriteSet">The sprites that are are considered for use.</param>
		/// <returns>The number of cards allowed at maximum.</returns>
		public static int GetMaximumAllowedCardCount(ShuffleCardDeck shuffleCardDeck, ShuffleCardSpriteSet cardSpriteSet) {
			// Ironically I need to use Min for this.
			return Mathf.Min(a: cardSpriteSet.CardSpriteCount, b: shuffleCardDeck.ShuffleCards.Count);
		}
		#endregion

	}


}