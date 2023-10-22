using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Grawly.Dungeon;
using Grawly.UI.Legacy;
using Grawly.Chat;
using System;
using Sirenix.OdinInspector;
using Grawly.Story;

namespace Grawly.Battle {

	public class BattleResultsController : MonoBehaviour {

		// public static BattleResultsController instance;
		
		#region FIELDS - QUEUES
		/// <summary>
		/// If a player has leveled up, they will be stored here by the battlecontroller after a battle.
		/// </summary>
		public Queue<Combatant> leveledUpPlayers = new Queue<Combatant>();
		/// <summary>
		/// If a persona has leveled up, they will be stored here by the battlecontroller after a battle.
		/// </summary>
		public Queue<Combatant> leveledUpPersonas = new Queue<Combatant>();
		/// <summary>
		/// The persona that is currently being shown as leveling up.
		/// </summary>
		private Combatant currentLevelingPersona;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Holds basically everything in the canvas.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject canvasObject;
		/// <summary>
		/// Holds the visuals that aren't the backgroudn.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject visualsObject;
		/// <summary>
		/// Holds the portraits for when the girls level up.
		/// </summary>
		[TabGroup("Scene", "Objects"), SerializeField]
		private GameObject levelUpPortraitsObject;
		
		/// <summary>
		/// Shows how much experience was obtained after the battle.
		/// </summary>
		[TabGroup("Scene", "Text"), SerializeField]
		private SuperTextMesh experienceLabel;
		/// <summary>
		/// Shows how much money was obtained after the battle.
		/// </summary>
		[TabGroup("Scene", "Text"), SerializeField]
		private SuperTextMesh moneyLabel;
		/// <summary>
		/// Shows the items obtained after the battle.
		/// </summary>
		[TabGroup("Scene", "Text"), SerializeField]
		private SuperTextMesh itemsLabel;
		/// <summary>
		/// Shows how many of each item was obtained after the battle.
		/// </summary>
		[TabGroup("Scene", "Text"), SerializeField]
		private SuperTextMesh itemsQuantity;
		/// <summary>
		/// Results label for decoration only.
		/// </summary>
		[TabGroup("Scene", "Text"), SerializeField]
		private SuperTextMesh resultsLabel;
		
		/// <summary>
		/// Holds the graphics for the different players that may have leveled up.
		/// </summary>
		private Dictionary<TacticsParticipantType, Image> levelUpPortraitDict = new Dictionary<TacticsParticipantType, Image>();
		/// <summary>
		/// Maintains the origin positions of the different level up portraits.
		/// </summary>
		private Dictionary<TacticsParticipantType, Vector2> levelUpPortraitPositionDict = new Dictionary<TacticsParticipantType, Vector2>();
		/// <summary>
		/// The portrait for when Dorothy levels up.
		/// </summary>
		[TabGroup("Scene", "Graphics"), SerializeField]
		private Image dorothyLevelUpPortrait;
		/// <summary>
		/// The portrait for when Sophia levels up.
		/// </summary>
		[TabGroup("Scene", "Graphics"), SerializeField]
		private Image sophiaLevelUpPortrait;
		/// <summary>
		/// The portrait for when Blanche levels up.
		/// </summary>
		[TabGroup("Scene", "Graphics"), SerializeField]
		private Image blancheLevelUpPortrait;
		/// <summary>
		/// The portrait for when Rose levels up.
		/// </summary>
		[TabGroup("Scene", "Graphics"), SerializeField]
		private Image roseLevelUpPortrait;
		
		/// <summary>
		/// The items icon; which unlike exp/money, only shows up conditionally.
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private GameObject itemsIcon;
		/// <summary>
		/// This is the button to be pressed when the player should advance.
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private GameObject resultsButton;
		/// <summary>
		/// The white silhouette graphics
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private GameObject whiteSilhouettes;
		/// <summary>
		/// The rainbow silhouette graphics
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private GameObject rainbowSilhouettes;
		/// <summary>
		/// This is the position the silhouettes should move to.
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private Vector3 whiteSilhouettesPos;
		/// <summary>
		/// This is the position the silhouettes should move to.
		/// </summary>
		[TabGroup("Scene", "Misc"), SerializeField]
		private Vector3 rainbowSilhouettesPos;
		#endregion

		#region UNITY CALLS
		private void Awake() {

			// I don't need this anymore.
			return;
			
			/*if (instance == null) {
				instance = this;
				// Save the positions that the silhouettes should move to
				whiteSilhouettesPos = whiteSilhouettes.transform.localPosition;
				rainbowSilhouettesPos = rainbowSilhouettes.transform.localPosition;

				levelUpPortraitDict.Add(TacticsParticipantType.D, dorothyLevelUpPortrait);
				levelUpPortraitDict.Add(TacticsParticipantType.B, blancheLevelUpPortrait);
				levelUpPortraitDict.Add(TacticsParticipantType.R, roseLevelUpPortrait);
				levelUpPortraitDict.Add(TacticsParticipantType.S, sophiaLevelUpPortrait);

				// Remember the original position of each level up portrait.
				foreach (TacticsParticipantType type in System.Enum.GetValues(typeof(TacticsParticipantType))) {
					if (levelUpPortraitDict.ContainsKey(type)) {
						levelUpPortraitPositionDict.Add(type, levelUpPortraitDict[type].GetComponent<RectTransform>().anchoredPosition);
					}
				}
			}*/
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Turn the battle results on or off.
		/// </summary>
		/// <param name="status"></param>
		public void SetActive(bool status) {
			// Also remember to set the visauls
			visualsObject.SetActive(status);
			canvasObject.SetActive(status);
			if (status == true) {
				// Only select the results button after a few seconds. This is to prevent errors.
				GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
					this.SetResultsButtonActive(true);
				});
				// Read the battle results from the Battle Controller
				throw new System.Exception("This has been deprecated as the results data no longer exists.");
				// ReadBattleResults(BattleController.Instance.BattleResultsData);
				
				// Start the tweening of the silhouettes
				StartCoroutine(TweenSilhouettes());
			} else {
				// Stop the coroutines
				StopAllCoroutines();
				whiteSilhouettes.transform.position = whiteSilhouettesPos;
				rainbowSilhouettes.transform.position = rainbowSilhouettesPos;
				// Reset the position of each level up portrait (some/all may have moved)
				foreach (TacticsParticipantType type in System.Enum.GetValues(typeof(TacticsParticipantType))) {
					if (levelUpPortraitDict.ContainsKey(type)) {
						levelUpPortraitDict[type].GetComponent<RectTransform>().anchoredPosition = levelUpPortraitPositionDict[type];
					}
				}
			}
		}
		/// <summary>
		/// Reads the battle results and fills the screen with the appropriate information.
		/// </summary>
		/// <param name="battleResults"></param>
		public void ReadBattleResults(LegacyBattleResultsData battleResults) {
			experienceLabel.Text = battleResults.exp.ToString();
			// experienceLabel.Text = battleResults.exp.ToString() + " x " + LegacyStoryController.Instance.expMultiplier.ToString() + "<size=36>EXP";
			moneyLabel.Text = battleResults.money.ToString() + "<size=36>DOL";
			// Reset these two items labels, because regardless of whether there are items or not, I'll need to.
			itemsLabel.Text = "";
			itemsQuantity.Text = "";

			// If there are no items, hide the items icon/labels
			if (battleResults.items.Count == 0) {
				itemsIcon.SetActive(false);
			} else {
				// If there ARE items, go through each item and create a list of each one and the number of its count
				itemsIcon.SetActive(true);
				Dictionary<BattleBehavior, int> itemsDict = new Dictionary<BattleBehavior, int>();
				foreach (BattleBehavior item in battleResults.items) {
					if (itemsDict.ContainsKey(item)) {
						itemsDict[item] += 1;
					} else {
						itemsDict.Add(item, 1);
					}
				}

				// Build up the label/quanitty texts
				foreach (BattleBehavior item in new List<BattleBehavior>(itemsDict.Keys)) {
					itemsLabel.Text += item.behaviorName + "\n";
					itemsQuantity.Text += "<size=50>x</size>" + itemsDict[item].ToString() + "\n";
				}
			}

		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tweens the silhouettes to create a nice animation.
		/// </summary>
		/// <returns></returns>
		private IEnumerator TweenSilhouettes() {
			// Snap the silhouettes
			whiteSilhouettes.transform.localPosition = new Vector3(1000f, 0f);
			rainbowSilhouettes.transform.localPosition = new Vector3(1000f, 0f);
			yield return new WaitForSeconds(.3f);
			// Tween them
			whiteSilhouettes.transform.DOLocalMoveX(whiteSilhouettesPos.x, .7f);
			rainbowSilhouettes.transform.DOLocalMoveX(rainbowSilhouettesPos.x, .8f);
		}
		#endregion

		#region BUTTON FUNCTIONS
		/// <summary>
		/// There is a secret button on the results screen. Manage what happens.
		/// </summary>
		public void Selected() {

			// Unsure if I need this?
			this.SetResultsButtonActive(false);

			// Turn off the level up portraits (this only is relevant if coming back from AnimatePlayerLevelUp
			levelUpPortraitsObject.SetActive(false);
			// Nullify the currently leveling persona.
			currentLevelingPersona = null;

			// Figure out where to go from here.
			if (leveledUpPlayers.Count != 0) {
				PlayerLevelUpEvent();
			} else if (leveledUpPersonas.Count != 0) {
				PersonaLevelUpEvent();
			} else {
				
				// If all of the leveld up combatants have been dealt with, just go back to the dungeon.
				Gauntlet.GauntletController.instance?.SetFSMState(Gauntlet.GauntletStateType.Free);
				BattleController.Instance.CurrentBattleParams.BattleOutro.ReturnToCaller(
					template: BattleController.Instance.CurrentBattleTemplate, 
					battleParams: BattleController.Instance.CurrentBattleParams);

			}
		}
		/// <summary>
		/// Enables/disables the button used for advancing thru the results screen.
		/// </summary>
		/// <param name="status"></param>
		private void SetResultsButtonActive(bool status) {
			Debug.Log("NOTE: ResultsButton set to " + status);
			resultsButton.SetActive(status);
			if (status == true) {
				EventSystem.current.SetSelectedGameObject(resultsButton);
			}
		}
		#endregion

		#region EVENT RESPONDERS
		/// <summary>
		/// A quick way to segment out the animation for when a Player levels up.
		/// </summary>
		private void PlayerLevelUpEvent() {
			// Turn the main visuals off.
			visualsObject.SetActive(false);
			// If there are players that leveled up, animate that.
			SetResultsButtonActive(false);
			// Turn on the level up portraits
			levelUpPortraitsObject.SetActive(true);
			List<string> script = new List<string>();
			while (leveledUpPlayers.Count > 0) {
				// For every leveled up player, create a tween that will move them to the center.
				Player player = (Player)leveledUpPlayers.Dequeue();
				levelUpPortraitDict[player.participantType].GetComponent<RectTransform>().DOAnchorPosY(0f, 0.2f);
				script.Add(player.metaData.name + " is now level " + player.Level + "!");
			}
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Basic effect
				AudioController.instance.PlaySFX(SFXType.PlayerExploit);
				Flasher.instance.Flash();
			}));
			seq.AppendInterval(0.5f);
			seq.AppendCallback(new TweenCallback(delegate {

				// Open up the chat controller and display it.
				
				ChatControllerDX.GlobalOpen(
					chatScript: new PlainChatScript(script),
					chatOpenedCallback: delegate {
						SetResultsButtonActive(false);
					}, chatClosedCallback: delegate {
						SetResultsButtonActive(true);
					});
				
				
			}));
		}
		/// <summary>
		/// A quick way to segment out the animation for when a Persona levels up.
		/// </summary>
		private void PersonaLevelUpEvent() {

			/*
			 
				This is some tower of babel shit but it's kinda functional so I'm keeping it. Here's what happens:
				1) seq will build the combatant analysis canvas and show the persona has leveled up.
				2) After running a script through the chatcontroller, use the ChatClosedCallback to check whether this persona
					has moves it can learn. If no, don't do anything.

			 */

			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(new TweenCallback(delegate {
				// Turn the main visuals off.
				visualsObject.SetActive(false);
				// Save the currently leveling persona.
				currentLevelingPersona = leveledUpPersonas.Dequeue();
				// Disable the results button for a second.
				SetResultsButtonActive(false);
				// Build populates the combatant analysis canvas and makes it so the thing flashes and shit.
				CombatantAnalysisCanvas.instance.Build(currentLevelingPersona, CombatantAnalysisCanvas.ContextType.PersonaLevelUp);
			}));
			seq.AppendInterval(1f);
			seq.AppendCallback(new TweenCallback(delegate {
				// After a second, open up a chat and say that this persona has leveled up.
				ChatControllerDX.GlobalOpen(
						chatScript: new PlainChatScript(currentLevelingPersona.metaData.name + " has leveled up!"),
						chatOpenedCallback: delegate {
							SetResultsButtonActive(false);
						}, chatClosedCallback: delegate {
							Debug.Log("TODO: FIX THE SEND MESSAGE CALLS HERE");
							//
							//	Check if Persona has additonl mvoes it can learn.
							//
							this.SendMessage("PersonaLevelUpMoveCheck", (Persona)currentLevelingPersona);
						});
				
			}));
			//
			//	Sequence has been assembled. Play it.
			//
			seq.Play();
		}
		#endregion

		#region PERSONA LEVEL UP UTILITIES
		/// <summary>
		/// A FUNCTION TO BE USED IN CONNECTION WITH PERSONALEVELUPEVENT!
		/// This will handle the functions of checking if a persona can learn a level up move,
		/// and making it happen if it can.
		/// </summary>
		/// <param name="persona"></param>
		private void PersonaLevelUpMoveCheck(Persona persona) {
			//
			//	Check for level up move.
			//
			if (persona.levelUpMoves.Count > 0) {
				//
				//	Persona has level up move. Check if it can learn it.
				//
				if (persona.levelUpMoves.Peek().level <= persona.Level) {
					BattleBehavior behavior = persona.levelUpMoves.Peek().behavior;
					CombatantAnalysisCanvas.instance.AddBehavior(
						persona: persona, 
						behavior: behavior, 
						startCallback: new TweenCallback(delegate {}), 
						finishCallback: new TweenCallback(delegate {
							Debug.Log("TODO: FIX THE SEND MESSAGE CALLS HERE");
							this.SendMessage("PersonaLevelUpMoveCheck", persona);
						}), 
						context: CombatantAnalysisCanvas.ContextType.PersonaLevelUp);
					
				} else {
					//
					//	Persona has moves, but cannot learn them.
					//
					SetResultsButtonActive(true);
				}
			//
			//	Persona does not have any level up moves.
			//
			} else {
				SetResultsButtonActive(true);
			}
		}
		/// <summary>
		/// Handles the adding of a persona move.
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="behavior"></param>
		public void PersonaAddLevelUpMove(Persona persona, int slot, BattleBehavior behavior) {
			throw new Exception("DO NOT CALL");
			/*Sequence seq2 = DOTween.Sequence();
			seq2.AppendCallback(new TweenCallback(delegate {
				persona.AddBehavior(behavior: behavior, slot: slot);
				persona.levelUpMoves.Dequeue();
				// persona.AddBehavior(persona.levelUpMoves.Dequeue().behavior);
				CombatantAnalysisCanvas.Instance.FlashNewMoveItem(persona, slot, behavior);
				CombatantAnalysisCanvas.Instance.RefreshLevelUpMoves(persona, CombatantAnalysisCanvas.ContextType.PersonaLevelUp);
			}));
			seq2.AppendInterval(1f);
			seq2.AppendCallback(new TweenCallback(delegate {
				ChatController.Open(
					script: new PlainChatScript(persona.metaData.name + " has learned " + behavior.behaviorName),
					chatOpenedCallback: delegate {}, 
					chatClosedCallback: delegate {
						// Recursive (not really) call to this method to check again for another move.
						this.SendMessage("PersonaLevelUpMoveCheck", persona);
					});
			}));
			seq2.Play();*/
		}
		#endregion

	}

}