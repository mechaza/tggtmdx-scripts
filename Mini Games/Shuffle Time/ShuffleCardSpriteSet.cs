using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// A behavior that provides access to the card sprites in the scene, because I'm getting tired of managing them on the individual level.
	/// </summary>
	public class ShuffleCardSpriteSet : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES : CARDS
		/// <summary>
		/// A list to the shuffle card sprites currently in the scene.
		/// </summary>
		[Title("Shuffle Card Sprites")]
		[TabGroup("Shuffle Controller", "Scene References"), SerializeField, ListDrawerSettings(Expanded = true)]
		private List<ShuffleCardSprite> shuffleCardSprites = new List<ShuffleCardSprite>();
		/// <summary>
		/// ALL of the shuffle card sprites. Primarily for debugging.
		/// </summary>
		private List<ShuffleCardSprite> AllCardSprites {
			get {
				return this.shuffleCardSprites;
			}
		}
		#endregion

		#region PROPERTIES
		/// <summary>
		/// The number of cards in this card sprite set.
		/// </summary>
		public int CardSpriteCount {
			get {
				return this.AllCardSprites.Count;
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Resets the state on all of the cards.
		/// </summary>
		public void ResetCards() {
			this.AllCardSprites.ForEach(c => c.ResetState());
		}
		#endregion

		#region CARD SPRITE MANAGEMENT
		/// <summary>
		/// Preps up a set of Shuffle Card Sprites with the shuffle cards given.
		/// </summary>
		/// <param name="shuffleCards">The shuffle cards that need their srpites.</param>
		/// <param name="table">The table these sprite cards will have.</param>
		/// <returns>A list of prepared ShuffleCardSprites all set to go.</returns>
		public List<ShuffleCardSprite> PrepareCardSprites(List<ShuffleCard> shuffleCards, ShuffleTimeTable table) {
			// Create an empty list.
			List<ShuffleCardSprite> preparedSprites = new List<ShuffleCardSprite>();
			// Go through each shuffle card...
			for (int i = 0; i < shuffleCards.Count; i++) {
				// ...prepare the sprite with its corresponding shuffle card...
				ShuffleCardSprite preparedSprite = this.AllCardSprites[i].Prepare(shuffleCard: shuffleCards[i], table: table);
				// ...and add it to the list.
				preparedSprites.Add(preparedSprite);
			}
			return preparedSprites;
		}
		/// <summary>
		/// Sets whether the specified all of shuffle cards in this set can be selected by Unity's event system or not.
		/// </summary>
		/// <param name="status">Whether the selectables on these cards should be on or off.</param>
		public void ActivateAllSpriteSelectables(bool status) {
			// Yep, just pass it all the cards.
			this.ActivateSpriteSelectables(cardSprites: this.AllCardSprites, status: status);
		}
		/// <summary>
		/// Sets whether the specified shuffle cards can be selected by Unity's event system or not.
		/// </summary>
		/// <param name="cardSprites">The shuffle cards that need their status set.</param>
		/// <param name="status">Whether the selectables on these cards should be on or off.</param>
		public void ActivateSpriteSelectables(List<ShuffleCardSprite> cardSprites, bool status) {
			// Go through each card sprite and whether its selectable or not.
			cardSprites.ForEach(s => s.ActivateSpriteSelectable(status: status));
		}
		#endregion

		#region HELPERS
		/*/// <summary>
		/// Returns the number of cards that can be used with this sprite set with the given deck.
		/// I.e., if there are too few cards in the deck, I can't use all the sprites, and
		/// if there are too few sprites in the set, I can't use all the cards in the deck.
		/// </summary>
		/// <param name="shuffleCardDeck">The card deck worth checking.</param>
		/// <param name="cardSpriteSet">The sprites that are are considered for use.</param>
		/// <returns>The number of cards allowed at maximum.</returns>
		private int GetMaximumAllowedCardCount(ShuffleCardDeck shuffleCardDeck) {
			// Ironically I need to use Min for this.
			return this.GetMaximumAllowedCardCount(shuffleCardDeck: shuffleCardDeck, cardSpriteSet: this);
		}*/

		#endregion

	}


}