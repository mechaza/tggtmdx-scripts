using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// A standard table. Mostly for debugging, since it doesn't really do much.
	/// </summary>
	public class StandardTable : ShuffleTimeTable {

		#region FIELDS - STATE : ANIMATION
		/// <summary>
		/// The sequence that shows the cards being displayed.
		/// Needed so I can possibly interrupt it.
		/// </summary>
		private Sequence tableDisplaySequence;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to delay by before playing the initial animation.
		/// </summary>
		[Title("Timing")]
		[TabGroup("Table", "Toggles"), SerializeField]
		private float animationDelayTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening a card.
		/// </summary>
		[TabGroup("Table", "Toggles"), SerializeField]
		private float cardTweenTime = 0.2f;
		/// <summary>
		/// The amount of time to take before the next card can get tweened to its anchor.
		/// </summary>
		[TabGroup("Table", "Toggles"), SerializeField]
		private float cardIntervalTime = 0.1f;
		/// <summary>
		/// The easing to use when tweening a card to its position.
		/// </summary>
		[Title("Easing")]
		[TabGroup("Table", "Toggles"), SerializeField]
		private Ease cardEaseType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The anchor where all the cards should start out when they get distributed.
		/// </summary>
		[Title("Anchors")]
		[TabGroup("Table","Scene References"), SerializeField]
		private Transform startingAnchor;
		/// <summary>
		/// A list of anchor points where the cards on display should be tweened to.
		/// Relevant when there are an odd number of cards.
		/// </summary>
		[TabGroup("Table", "Scene References"), SerializeField]
		private List<Transform> oddCardAnchors = new List<Transform>();
		/// <summary>
		/// A list of anchor points where the cards on display should be tweened to.
		/// Relevant when there are an even number of cards.
		/// </summary>
		[TabGroup("Table", "Scene References"), SerializeField]
		private List<Transform> evenCardAnchors = new List<Transform>();
		#endregion

		#region PRESENTATION - TABLE
		/// <summary>
		/// Preps the table to be used with the specified deck and animates them into place.
		/// </summary>
		/// <param name="deck">The deck to use for this table.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		/// <param name="count">The number of cards to display on the table.</param>
		protected override void DisplayTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet, int count) {

			// Get a list of card sprites to use from the set provided.
			List<ShuffleCardSprite> sprites = this.PrepareCardSprites(deck: deck, cardSpriteSet: cardSpriteSet, count: count);

			// Start up a new sequence.
			this.tableDisplaySequence = DOTween.Sequence();

			// Go through each sprite...
			foreach (ShuffleCardSprite sprite in sprites) {
				// ... and snap it to the starting anchor.
				sprite.transform.position = this.startingAnchor.position;

			}

			// Delay the sequence if needed.
			this.tableDisplaySequence.AppendInterval(this.animationDelayTime);

			// Tween each card to its appropriate anchor.
			for (int i = 0; i < sprites.Count; i++) {

				// Grab the card and its anchor.
				ShuffleCardSprite sprite = sprites[i];
				Transform targetAnchor = this.GetCardAnchor(sprites: sprites, cardIndex: i);

				// Append a callback to the sequence.
				this.tableDisplaySequence.AppendCallback(delegate {
					sprite.transform.DOMove(
						endValue: targetAnchor.transform.position,
						duration: this.cardTweenTime)
						.SetEase(ease: this.cardEaseType);
				});

				// Add a slight delay.
				this.tableDisplaySequence.AppendInterval(this.cardIntervalTime);
			}

			// When the sequence is complete, highlight the first sprite.
			this.tableDisplaySequence.onComplete = delegate {
				// Allow all the cards to be selectable.
				cardSpriteSet.ActivateAllSpriteSelectables(status: true);
				EventSystem.current.SetSelectedGameObject(sprites[0].Selectable.gameObject);
			};

			// Play that shit baby
			this.tableDisplaySequence.Play();

		}
		/// <summary>
		/// Dismisses the table from view.
		/// </summary>
		/// <param name="deck">The deck that was being used for this table.</param>
		/// <param name="cardSpriteSet">The sprite set that was being used for this table.</param>
		protected override void DismissTable(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet) {
			throw new System.NotImplementedException("uh");
		}
		#endregion

		#region PRESENTATION - CARDS
		/// <summary>
		/// Gets whether a card will show its face or not by default on preparation.
		/// </summary>
		/// <param name="shuffleCard">The shuffle card who may be shown face up or down.</param>
		/// <returns>Whether the card is going to be shown face up or down by default.</returns>
		public override bool GetDefaultCardFace(ShuffleCard shuffleCard) {
			// For now, just have them face down by default.
			return false;
		}
		/// <summary>
		/// Preps the card sprites to be used in the table.
		/// </summary>
		/// <param name="deck">The deck containing the cards to prepare.</param>
		/// <param name="cardSpriteSet">The set of sprites that can be used.</param>
		/// <param name="count">The number of card sprites to prepare.</param>
		/// <returns>The sprites that were passed in, but prepared.</returns>
		protected override List<ShuffleCardSprite> PrepareCardSprites(ShuffleCardDeck deck, ShuffleCardSpriteSet cardSpriteSet, int count) {

			// Grab some random cards from the deck. 
			List<ShuffleCard> randomCards = deck.GetRandomCards(count: count);

			// Prepare them all with the information provided by the table.
			return cardSpriteSet.PrepareCardSprites(shuffleCards: randomCards, table: this);

		}
		#endregion

		#region EVENTS FROM CARDS
		/// <summary>
		/// Gets called when a card sprite is picked.
		/// </summary>
		/// <param name="sprite">The sprite that was picked.</param>
		public override void CardPicked(ShuffleCardSprite sprite) {

			// Set all of the selectables to deactivated.
			this.CurrentCardSpriteSet.ActivateAllSpriteSelectables(status: false);

			// Flip this sprite to its front.
			sprite.FlipFront();

			Debug.Log("AHHHHHHHHH FLIP THE OTHER CARDS IDK");

			GameController.Instance?.WaitThenRun(timeToWait: 3f, action: delegate {
				ShuffleTimeController.Instance.Dismiss();
			});
		}
		/// <summary>
		/// Gets called when a cancel action is requested.
		/// </summary>
		/// <param name="sprite">The sprite that was highlighted when this was hit. If one was highlighted.</param>
		public override void CancelRequested(ShuffleCardSprite sprite = null) {
			Debug.Log("ADD CANCEL FUNCTIONALITY");
		}
		#endregion

		#region HELPER FUNCTIONS
		/// <summary>
		/// Gets the anchor that the card at the specified index needs to go to.
		/// </summary>
		/// <param name="sprites">All of the sprites in use.</param>
		/// <param name="cardIndex">The index of the card that needs its target anchor.</param>
		/// <returns>The anchor the card needs to move to.</returns>
		private Transform GetCardAnchor(List<ShuffleCardSprite> sprites, int cardIndex) {
			Debug.LogWarning("NEED TO FIX THIS SO IT TAKES CARD COUNT AND FIGURES IF TO USE ODD OR EVEN ANCHORS");
			return this.oddCardAnchors[cardIndex];
		}
		#endregion

	}


}

