using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using DG.Tweening;
using Grawly.Chat;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

namespace Grawly.MiniGames.ShuffleTime {

	/// <summary>
	/// 
	/// </summary>
	public class ShuffleTimeController : MonoBehaviour {
		
		public static ShuffleTimeController Instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The action to run when the shuffle time section is complete.
		/// </summary>
		private System.Action currentOnCompleteCallback;
		#endregion

		#region FIELDS - DEBUG
		/// <summary>
		/// A list of card templates to use in debug mode.
		/// </summary>
		[Title("Debug")]
		[TabGroup("Shuffle Controller", "Tables"), SerializeField]
		private List<ShuffleCardTemplate> debugCardTemplates = new List<ShuffleCardTemplate>();
		#endregion

		#region FIELDS - SCENE REFERENCES : TABLE BEHAVIORS
		/// <summary>
		/// The behavior for the standard table.
		/// </summary>
		[Title("Tables")]
		[TabGroup("Shuffle Controller", "Tables"), SerializeField, InlineButton(action: "TestTable", label: "Test Standard Table")]
		private StandardTable standardTable;
		#endregion

		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// All of the objects for the shuffle time controller.
		/// </summary>
		[Title("General")]
		[TabGroup("Shuffle Controller", "Scene References"), SerializeField]
		private GameObject allVisuals;
		#endregion

		#region FIELDS - SCENE REFERENCES : CAMERAS
		/// <summary>
		/// The camera brain for shuffle time.
		/// </summary>
		[Title("Cameras")]
		[TabGroup("Shuffle Controller", "Scene References"), SerializeField]
		private GameObject shuffleCameraBrain;
		/// <summary>
		/// The virtual camera for shuffle time.
		/// </summary>
		[TabGroup("Shuffle Controller", "Scene References"), SerializeField]
		private GameObject shuffleVirtualCamera;
		#endregion

		#region FIELDS - SHUFFLE CARDS
		/// <summary>
		/// The class that contains the shuffle card sprites that will be used for shuffle time.
		/// </summary>
		[Title("Cards")]
		[TabGroup("Shuffle Controller", "Scene References"), SerializeField]
		private ShuffleCardSpriteSet defaultCardSpriteSet;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(this.gameObject);
				// DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			this.ResetState();
			// If the game controller exists, it can be assumed I don't need the brain.
			// The brain is mostly here for debug purposes only anyway.
			if (GameController.Instance != null) {
				Destroy(this.shuffleCameraBrain.gameObject);
			} else {
				
			}
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Resets the state of the shuffle time controller.
		/// </summary>
		private void ResetState() {
			this.defaultCardSpriteSet.ResetCards();
			this.shuffleVirtualCamera.SetActive(false);
			this.allVisuals.SetActive(false);
		}
		/// <summary>
		/// Preps the shuffle time controller to be used with the specified deck.
		/// </summary>
		/// <param name="shuffleCardDeck">The deck to use in preparing.</param>
		/// <param name="onComplete">The callback to run when the table is totally finished.</param>
		/// <param name="cardSpriteSet">The sprite set that exists in the scene. This is how the ShuffleCards are visualized.</param>
		private void Prepare(ShuffleCardDeck shuffleCardDeck, System.Action onComplete, ShuffleCardSpriteSet cardSpriteSet) {

			// Turn on all the visuals.
			this.allVisuals.SetActive(true);
			this.shuffleVirtualCamera.SetActive(true);

			// Also remember the callback to run.
			this.currentOnCompleteCallback = onComplete;

			// Disable all the card sprites, first and foremost.
			cardSpriteSet.ResetCards();
			

			// Send the cards to the table.
			this.standardTable.RequestTable(deck: shuffleCardDeck, cardSpriteSet: cardSpriteSet);

		}
		/// <summary>
		/// Preps the shuffle time controller to be used with the specified deck.
		/// </summary>
		/// <param name="shuffleCardDeck">The deck to use in preparing.</param>
		/// <param name="onComplete">The callback to run when the table is totally finished.</param>
		public void Prepare(ShuffleCardDeck shuffleCardDeck, System.Action onComplete) {
			// May change in the future, but do one last thing and give the card sprite set that exists as a field.
			this.Prepare(
				shuffleCardDeck: shuffleCardDeck,
				onComplete: onComplete, 
				cardSpriteSet: this.defaultCardSpriteSet);
		}
		/// <summary>
		/// Preps the shuffle time controller to be used with the specified cards.
		/// </summary>
		/// <param name="shuffleCards">The cards to add to the deck.</param>
		public void Prepare(List<ShuffleCard> shuffleCards, System.Action onComplete) {
			// Make a new deck and pass it back to the prepare function.
			this.Prepare(shuffleCardDeck: new ShuffleCardDeck(shuffleCards), onComplete: onComplete);
		}
		/// <summary>
		/// Preps the shuffle time controller to be used with the card templates provided.
		/// </summary>
		/// <param name="shuffleCardTemplates">The card templates to use for this.</param>
		/// <param name="onComplete">The action to run on completion.</param>
		public void Prepare(List<ShuffleCardTemplate> shuffleCardTemplates, System.Action onComplete) {
			// Make a new deck and pass it to the appropriate function.
			this.Prepare(shuffleCardDeck: new ShuffleCardDeck(cardTemplates: shuffleCardTemplates), onComplete: onComplete);
		}
		/// <summary>
		/// Preps the shuffle time controller to be used with the information specified by the given battle template.
		/// </summary>
		/// <param name="battleTemplate">The battle template associated with this battle.</param>
		/// <param name="onComplete">The action to run when veryting is done.</param>
		public void Prepare(BattleTemplate battleTemplate, System.Action onComplete) {
			// Just grab the cards from the battle template.
			this.Prepare(shuffleCardDeck: battleTemplate.ShuffleCardDeck, onComplete: onComplete);	
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Dismisses the shuffle time controller.
		/// </summary>
		public void Dismiss() {
			this.ResetState();
			this.currentOnCompleteCallback.Invoke();
			// this.allObjects.SetActive(false);
		}
		#endregion

		#region DEBUGGING
		/// <summary>
		/// Tests a table with debug cards. Called from a button in the inspector.
		/// </summary>
		/// <param name="table">The table to use. There's a button in the attributes.</param>
		private void TestTable(StandardTable table) {
			if (Application.isPlaying == false) {
				Debug.LogError("Please enter play mode before testing.");
				return;
			}
			this.Prepare(shuffleCardTemplates: this.debugCardTemplates, onComplete: delegate {
				StartCoroutine(this.ReOpenTable());
			});
		}
		private IEnumerator ReOpenTable() {
			yield return new WaitForSeconds(3f);
			this.TestTable(table: this.standardTable);
		}
		#endregion

	}


}