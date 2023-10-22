using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Grawly.UI;
using DG.Tweening;
using Grawly.Battle.Analysis;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.WorldEnemies;
using Grawly.Battle.BattleArena;
using Grawly.Battle.Modifiers;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// Provides access to the battle menu's tweens and animations as well as maintaining state.
	/// Replaces the old BattleMenuController.
	/// </summary>
	public class BattleMenuControllerDX : MonoBehaviour {
		
		public static BattleMenuControllerDX instance;
	
		#region FIELDS - STATE : SELECTIONS
		/// <summary>
		/// The battle behavior that is currently selected. 
		/// May get used to attack an enemy with.
		/// </summary>
		public BattleBehavior CurrentBattleBehavior { get; private set; }
		/// <summary>
		/// The top level selection that was picked for this turn.
		/// Mostly to just help me speed things up when figuring out how to proceed and how to build the next screen.
		/// </summary>
		public BattleMenuDXTopLevelSelectionType CurrentTopLevelSelectionType { get; private set; }
		/// <summary>
		/// The list of combatants who are targets of whatever move the player is about to use.
		/// </summary>
		public List<Combatant> CurrentTargetCombatants { get; private set; } = new List<Combatant>();
		/// <summary>
		/// A stack of game objects that remember the flow fo 
		/// </summary>
		private Stack<GameObject> currentSelectionStack = new Stack<GameObject>();
		#endregion

		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The amount to fade the vignette into when showing it on screen.
		/// </summary>
		private float VignetteShowAmount { get; set; } = 0.4f;
		/// <summary>
		/// The amount to fade the vignette into when hiding it from screen.
		/// </summary>
		private float VignetteHideAmount { get; set; } = 0f;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : GENERAL
		/// <summary>
		/// The GameObject that contains the player's bust up.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		private GameObject playerBustUpGameObject;
		/// <summary>
		/// The GameObject that contains the player's bust up.
		/// </summary>
		public GameObject PlayerBustUpGameObject {
			get {
				return this.playerBustUpGameObject;
			}
		}
		/// <summary>
		/// The image that shows the player's bust up.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		internal Image playerBustUpFrontImage;
		/// <summary>
		/// The image that acts as a dropshadow for the player's bustup.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		private Image playerBustUpDropshadowImage;
		/// <summary>
		/// The image that should be used for the "shine" effect.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "General")]
		internal Image playerBustUpShineEffectImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : TOP LEVEL SELECTIONS
		/// <summary>
		/// A list of the top level selections on the battle menu.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		private List<Selectable> topLevelSelections = new List<Selectable>();
		/// <summary>
		/// The top level selections as their buttons.
		/// </summary>
		private List<BattleMenuDXTopLevelSelectionButton> TopLevelSelectionButtons {
			get {
				return this.topLevelSelections.Select(s => s.GetComponent<BattleMenuDXTopLevelSelectionButton>()).ToList();
			}
		}
		/// <summary>
		/// The recttransform that has the highlight bar for top level selections.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		internal RectTransform TopLevelHighlightBarRectTransform;
		/// <summary>
		/// The script that allows me to show the description text associated with a top level button.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		private BattleMenuDXTopLevelDescription topLevelDescription;
		/// <summary>
		/// The GameObject used for the top level selections as a whole.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Top Level")]
		private GameObject topLevelSelectionsGameObject;
		/// <summary>
		/// The GameObject used for the top level selections as a whole.
		/// </summary>
		public GameObject TopLevelSelectionsGameObject {
			get {
				return this.topLevelSelectionsGameObject;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : SECOND LEVEL SELECTIONS
		/// <summary>
		/// The GameObject that contains all of the assets for the behavior selection as a whole.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Second Level")]
		private GameObject battleBehaviorMenuListGameObject;
		/// <summary>
		/// A reference to the BattleBehavior menu list that is used for building a list of... BattleBehaviors to select from.
		/// </summary>
		[SerializeField, TabGroup("Scene References", "Second Level")]
		private BattleMenuDXBehaviorMenuList battleBehaviorMenuList;
		#endregion
		
		#region FIELDS - DEBUG
		/// <summary>
		/// Should debug mode be turned on?
		/// </summary>
		[SerializeField, TabGroup("Debug", "Debug")]
		private bool debugMode = false;
		/// <summary>
		/// The time to take when tweening the behavior selection in/out.
		/// </summary>
		[SerializeField, TabGroup("Debug", "Debug")]
		private float behaviorSelectionTweenTime;
		/// <summary>
		/// The ease to use when twening the behavior selection in.
		/// </summary>
		[SerializeField, TabGroup("Debug", "Debug")]
		private Ease behaviorSelectionTweenEaseType;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region VARIABLE / STATE CHANGERS
		/// <summary>
		/// Keeps track of the BattleBehavior that the player may end up using.
		/// </summary>
		/// <param name="behavior"></param>
		public void SetCurrentBattleBehavior(BattleBehavior behavior) {
			Debug.Log("BATTLE MENU: Setting current battle behavior to " + behavior.behaviorName);
			this.CurrentBattleBehavior = behavior;
		}
		/// <summary>
		/// Keeps track of the last top level selection that was picked.
		/// </summary>
		/// <param name="battleMenuDXTopLevelSelectionType"></param>
		public void SetCurrentTopLevelSelectionType(BattleMenuDXTopLevelSelectionType battleMenuDXTopLevelSelectionType) {

			Debug.Log("BATTLE MENU: Setting current top level selection to " + battleMenuDXTopLevelSelectionType.ToString());
			this.CurrentTopLevelSelectionType = battleMenuDXTopLevelSelectionType;

			// I should probably move this out but, if Masque was picked, open the analysis canvas.
			if (battleMenuDXTopLevelSelectionType == BattleMenuDXTopLevelSelectionType.Masque) {
				Debug.Log("BATTLE: TOP LEVEL SELECTION WAS SET TO MASQUE. OPEN THE ANALYSIS CANVAS.");
				this.OpenCombatantAnalysis(
					combatants: GameController.Instance.Variables.Personas.Cast<Combatant>().ToList(), 
					contextType: UI.Legacy.CombatantAnalysisCanvas.ContextType.ChangePersona);

			} else if (battleMenuDXTopLevelSelectionType == BattleMenuDXTopLevelSelectionType.Analysis) {
				Debug.Log("BATTLE: TOP LEVEL SELECTION WAS SET TO ANALYSIS. OPEN THE ANALYSIS CANVAS.");
				this.OpenCombatantAnalysis(
					combatants: BattleController.Instance.Enemies.Cast<Combatant>().ToList(),
					contextType: UI.Legacy.CombatantAnalysisCanvas.ContextType.AnalyzeEnemy);

			} else if (battleMenuDXTopLevelSelectionType == BattleMenuDXTopLevelSelectionType.Escape) {
				Debug.Log("BATTLE: TOP LEVEL SELECTION WAS SET TO ESCAPE. RUNNING THE ESCAPE BATTLE BEHAVIOR FUNCTION.");
				this.SendSelectionsToBattleController();

			} else {
				// Send the appropriate event if literally any other selection was picked.
				this.TriggerEvent(topLevelSelectionType: battleMenuDXTopLevelSelectionType);
			}

		}
		/// <summary>
		/// Keeps track of the combatants who have been picked to be the targets for whatever move the player is about to use.
		/// </summary>
		/// <param name="combatants">The combatants to use.</param>
		public void SetCurrentTargetCombatants(List<Combatant> combatants) {

			// Remember the currently selected combatants.
			this.CurrentTargetCombatants = combatants;

			// If the top level selection was NOT analysis, tell the state machine as such.
			this.TriggerEvent(eventName: "Combatants Selected");

		}
		#endregion

		#region STATE MACHINE - EVENTS
		/// <summary>
		/// Triggers an event of the specified name on the state machine.
		/// </summary>
		/// <param name="eventName">The event to trigger.</param>
		public void TriggerEvent(string eventName) {
			Debug.Log("BATTLE: Triggering event with name " + eventName + " on the battle menu controller.");
			// Bolt.CustomEvent.Trigger(target: this.gameObject, name: eventName);
			Unity.VisualScripting.CustomEvent.Trigger(target: this.gameObject, name: eventName);
		}
		/// <summary>
		/// Triggers an event associated with the specified top level selection type.
		/// </summary>
		/// <param name="topLevelSelectionType">The selection that was just chosen.</param>
		private void TriggerEvent(BattleMenuDXTopLevelSelectionType topLevelSelectionType) {
			// Check which type it is and go from there.
			switch (topLevelSelectionType) {
				case BattleMenuDXTopLevelSelectionType.Attack:
				case BattleMenuDXTopLevelSelectionType.Analysis:
				case BattleMenuDXTopLevelSelectionType.BatonTouch:
					this.TriggerEvent(eventName: "Combatant Selection");
					break;
				case BattleMenuDXTopLevelSelectionType.Skill:
				case BattleMenuDXTopLevelSelectionType.Item:
					this.TriggerEvent(eventName: "Behavior Selection");
					break;
				case BattleMenuDXTopLevelSelectionType.Masque:
					this.TriggerEvent(eventName: "Persona Selection");
					break;
				case BattleMenuDXTopLevelSelectionType.Guard:
				case BattleMenuDXTopLevelSelectionType.Escape:
					Debug.Log("GUARD WAS PICKED. SENDING SELECTIONS TO BATTLE CONTROLLER.");
					this.SendSelectionsToBattleController();
					break;
				default:
					throw new System.Exception("Type is not defined to have a transition!");
					break;
			}
		}
		/// <summary>
		/// Resets mutable variables of the BattleMenuControllerDX and resets the state machine.
		/// </summary>
		public void ResetState() {
			Debug.Log("BATTLE MENU: Resetting menu controller state!");
			this.currentSelectionStack.Clear();
			this.CurrentBattleBehavior = null;
			this.CurrentTargetCombatants?.Clear();
			this.CurrentTopLevelSelectionType = BattleMenuDXTopLevelSelectionType.Attack;
			// TRYING TO TWEEN THE SQUARE OUT HERE I DONT THINK I SHOULD BUT MAY AS WELL TRY.
			RotatingMenuSquare.instance?.TweenSquareStatus(status: false);
		}
		#endregion

		#region STATE MACHINE - COMBATANT ANALYSIS
		/// <summary>
		/// Opens the combatant analysis canvas with the specified combatants and contxt.
		/// </summary>
		/// <param name="combatants">The combatants to populate the canvas with.</param>
		/// <param name="contextType">The context for which the canvas is being opened.</param>
		private void OpenCombatantAnalysis(List<Combatant> combatants, UI.Legacy.CombatantAnalysisCanvas.ContextType contextType) {
			// Reference the analysis canvas directly and pass over the information it needs.
			CombatantAnalysisControllerDX.Instance.TriggerEvent(eventName: "Analyze Enemies");
		}
		/// <summary>
		/// Closes the combatant analysis canvas. Gets called directly from the canvas usually.
		/// </summary>
		/// <param name="gameObjectToReselect">Was saved along with the canvas earlier. This is what should be reselected.</param>
		public void CloseCombatantAnalysis(GameObject gameObjectToReselect) {
			// Reselect the game object that the analysis canvas saved. This is usually the button that triggered the event in the first place.
			EventSystem.current.SetSelectedGameObject(gameObjectToReselect);
		}
		#endregion

		#region STATE MACHINE - TOP LEVEL SELECTION
		/// <summary>
		/// The method that should get called when entering the top level selection.
		/// </summary>
		public void EnterTopLevelSelectionState() {
			// Set the bust up's gameobject to active.
			this.playerBustUpGameObject.SetActive(true);

			// Turn on the top level selections GameObject.
			this.topLevelSelectionsGameObject.SetActive(true);

			// If there is something on the selection stack, it means I'm coming back from a cancelled selection. Pop the stack and reselect it.
			if (this.currentSelectionStack.Count > 0) {
				EventSystem.current.SetSelectedGameObject(this.currentSelectionStack.Pop());
			} else {

				// Go through each top level selection and disable it. Ones I can use will be re-enabled shortly.
				this.TopLevelSelectionButtons.ForEach(b => b.AllowSubmission = false);

				// I need to restrict other top level selections depending on how the battle is going.
				this.GetUsableTopLevelSelections(restrictorModifiers:                                                   // Call the method that returns a list of usable types by...
					BattleController.Instance.GetBattleModifiers<ITopLevelSelectionRestrictor>()                        // ...getting the restriction modifiers from the battle controller...
					.Concat(BattleController.Instance.CurrentCombatant.GetModifiers<ITopLevelSelectionRestrictor>())    // ...and the combatant.
					.ToList())
					.ForEach(t => {                                                                                     // Go through each usable type...
					this.TopLevelSelectionButtons
						.First(b => b.TopLevelSelectionType == t)
						.AllowSubmission = true;
					});

				// Additionally, if the player has no skills, turn the skill button off.
				if (BattleController.Instance.CurrentCombatant.UsableSpecialBehaviors.Count == 0) {
					this.TopLevelSelectionButtons
						.First(b => b.TopLevelSelectionType == BattleMenuDXTopLevelSelectionType.Skill)
						.AllowSubmission = false;
				}

				// Also turn off the items menu if the player has no items.
				if (GameController.Instance.Variables.Items.Keys.Any(b => b.showInBattleMenu == true) == false) {
					this.TopLevelSelectionButtons
						.First(b => b.TopLevelSelectionType == BattleMenuDXTopLevelSelectionType.Item)
						.AllowSubmission = false;
				}

				// Now, rebuild the buttons.
				this.TopLevelSelectionButtons
					.Where(b => b.TopLevelSelectionType != BattleMenuDXTopLevelSelectionType.Attack)		// There's some weird issue with rebuilding attack so just skip that one.
					.ToList()
					.ForEach(b => b.Rebuild());

				// Tell the event system to select the attack option.
				EventSystem.current.SetSelectedGameObject(this.topLevelSelections[2].gameObject);
				// Also tween the rotating square in and reset its color.
				RotatingMenuSquare.instance.TweenSquareStatus(status: true, resetColor: true);
			}

		}
		/// <summary>
		/// The method that should get called when exiting the top level selection.
		/// </summary>
		public void ExitTopLevelSelectionState() {

			// Push the current top selection onto the stack.
			this.currentSelectionStack.Push(EventSystem.current.currentSelectedGameObject);

			// Turn off the GameObject with the top level selections.
			this.topLevelSelectionsGameObject.SetActive(false);
		}
		#endregion

		#region STATE MACHINE - BATTLE BEHAVIOR SELECTION
		/// <summary>
		/// The method that should get called when entering the battle behavior selection.
		/// </summary>
		public void EnterBattleBehaviorSelectionState() {

			// Turn on the GameObject with the BattleBehavior menu list.
			this.battleBehaviorMenuListGameObject.SetActive(true);

			//
			// I'm like grabbing the behaviors but if the stack isnt equal to one im not doing anything with them.
			//

			// Create a variable for the battle behaviors list.
			List<IMenuable> battleBehaviors;
			// There are two possibilities in which the menu can be opened: skill and item. Determine which one is needed and grab the appropriate behaviors.
			switch (this.CurrentTopLevelSelectionType) {
				case BattleMenuDXTopLevelSelectionType.Skill:
					battleBehaviors = BattleController.Instance.CurrentCombatant.AllBehaviors[BehaviorType.Special].Where(b => b.showInBattleMenu == true).Cast<IMenuable>().ToList();
					break;
				case BattleMenuDXTopLevelSelectionType.Item:
					battleBehaviors = GameController.Instance.Variables.Items.Keys.Where(b => b.showInBattleMenu == true).OrderBy(el => el.priority).Cast<IMenuable>().ToList();
					break;
				default:
					throw new System.Exception("Invalid type for EnterBattleBehaviorSelectionState! Was it set appropriately?");
					break;
			}


			// If the stack is equal to one, it means that we are coming from the top level selection.
			if (this.currentSelectionStack.Count == 1) {
				// Set the vignette on the camera.
				// BattleCameraController.instance.SetVignette(amt: 5f, time: 0.5f);
				BattleCameraController.Instance.SetVignette(amt: this.VignetteShowAmount, time: 0.5f);
				// Now that the behaviors have been retrieved, prep the menu list.
				this.battleBehaviorMenuList.PrepareMenuList(allMenuables: battleBehaviors, startIndex: 0);
				// If there is only one selection on the stack (the top level) select the first menu list item.
				this.battleBehaviorMenuList.SelectFirstMenuListItem();

				// Do the Tween Animation.
				this.battleBehaviorMenuListGameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
				// Set the scale on the behavior list to be zero and then tween it to be Big.
				this.battleBehaviorMenuListGameObject.GetComponent<RectTransform>().DOScale(endValue: 1f, duration: this.behaviorSelectionTweenTime).SetEase(ease: this.behaviorSelectionTweenEaseType);

			} else {
				// Otherwise, just pop from the stack.
				EventSystem.current.SetSelectedGameObject(this.currentSelectionStack.Pop());
				// Set the vignette on the camera instantly.
				// BattleCameraController.instance.SetVignette(amt: 5f, time: 0f);
				BattleCameraController.Instance.SetVignette(amt: this.VignetteShowAmount, time: 0f);
			}

		}
		/// <summary>
		/// The method that should get called when exiting the battle behavior selection.
		/// </summary>
		public void ExitBattleBehaviorSelectionState() {

			// Turn off the vignette on the camera.
			// BattleCameraController.instance.SetVignette(amt: 0f, time: 0f);
			BattleCameraController.Instance.SetVignette(amt: this.VignetteHideAmount, time: 0f);
			
			// Push the current behavior selection onto the stack.
			this.currentSelectionStack.Push(EventSystem.current.currentSelectedGameObject);

			// Just turn the list off.
			this.battleBehaviorMenuListGameObject.SetActive(false);

		}
		#endregion

		#region STATE MACHINE - COMBATANT SELECTION
		/// <summary>
		/// The method that should get called when determining what targets to select. 
		/// </summary>
		public void EnterCombatantSelectionState() {

			// Show the final behavior visuals on the menu, but only if the choice is not attack.
			if (this.CurrentTopLevelSelectionType != BattleMenuDXTopLevelSelectionType.Attack) {
				FinalBehaviorSelectionController.instance.ShowFinalBehavior(finalBehaviorSelection: this.CurrentBattleBehavior);
			}

			// It is assumed that the top level type and battle behavior were assigned prior to this call.
			// Figure out how to proceed by calling the appropriate method.
			switch (this.CurrentBattleBehavior.targetType) {
				case TargetType.Self:
					this.EnterPlayerSelectionState(players: new List<Player>() { BattleController.Instance.CurrentCombatant as Player }, behaviorToUse: this.CurrentBattleBehavior);
					break;
				case TargetType.OneAliveAlly:
				case TargetType.AllAliveAllies:
					this.EnterPlayerSelectionState(players: BattleController.Instance.AlivePlayers, behaviorToUse: this.CurrentBattleBehavior);
					break;
				case TargetType.AllDeadAllies:
				case TargetType.OneDeadAlly:
					this.EnterPlayerSelectionState(players: BattleController.Instance.DeadPlayers, behaviorToUse: this.CurrentBattleBehavior);
					break;
				case TargetType.OneAliveEnemy:
				case TargetType.AllAliveEnemies:
					this.EnterEnemySelectionState(enemies: BattleController.Instance.AliveEnemies, behaviorToUse: this.CurrentBattleBehavior);
					break;
				case TargetType.AllAliveCombatants:
					throw new System.NotImplementedException("Need to add option to select all combatants.");
					break;
				default:
					throw new System.Exception("Couldn't figure out how to proceed with given target type!");
					break;
			}
		}
		/// <summary>
		/// Should get called when cancelling the combatant selection.
		/// This will effectively determine how to proceed based on what the top level selection was.
		/// </summary>
		public void CancelCombatantSelection() {

			// Hide the visuals for the Final Behavior since I'm not going to be using it. Sad!
			FinalBehaviorSelectionController.instance.HideFinalBehavior();

			// This may not be relevant for situations where an enemy was being selected but turn off the player selectables.
			this.ExitPlayerSelectionState();

			// There are two possible routes that depend on which top level selection was made.
			// One that shows the behavior selection, one that just goes back to the top level selections.
			switch (this.CurrentTopLevelSelectionType) {
				case BattleMenuDXTopLevelSelectionType.Analysis:
				case BattleMenuDXTopLevelSelectionType.Attack:
				case BattleMenuDXTopLevelSelectionType.BatonTouch:
					this.TriggerEvent("Back To Top Level Selection");
					break;
				case BattleMenuDXTopLevelSelectionType.Item:
				case BattleMenuDXTopLevelSelectionType.Skill:
					this.TriggerEvent("Back To Behavior Selection");
					break;
				default:
					break;
			}
		}
		#endregion

		#region STATE MACHINE - COMBATANT SELECTION : PLAYERS
		/// <summary>
		/// The method that should get called when entering the player selection state.
		/// </summary>
		/// <param name="players">The players that should be potential targets for whatever move is up for picks.</param>
		/// <param name="behaviorToUse">What is the behavior that is up for use?</param>
		private void EnterPlayerSelectionState(List<Player> players, BattleBehavior behaviorToUse) {
			// If the function to use is the baton touch function, only pass those who are ready.
			if (behaviorToUse.BattleFunction is Grawly.Battle.Functions.BatonTouchBattleBehaviorFunction) {
				Debug.Log("BATTLE: Forcefully overriding the selected players with those that are baton touch ready. Just remember that's a thing I do.");
				PlayerStatusDXController.instance.SetSelectablesOnPlayerTargets(players: BattleController.Instance.BatonTouchReadyCombatants.Cast<Player>().ToList(), currentBattleBehavior: behaviorToUse);
			} else {
				// Instruct the player status controller to build the statuses that can be used as buttons. It will also select the appropriate game object.
				PlayerStatusDXController.instance.SetSelectablesOnPlayerTargets(players: players, currentBattleBehavior: behaviorToUse);
			}
			
		}
		/// <summary>
		/// The method that should get called when exiting the player selection state.
		/// </summary>
		private void ExitPlayerSelectionState() {
			// Turn off the selectables on the player statuses.
			PlayerStatusDXController.instance.SetPlayerStatusSelectables(status: false);
		}
		#endregion

		#region STATE MACHINE - COMBATANT SELECTION : ENEMIES
		/// <summary>
		/// The method that should get called when entering the enemy selection state.
		/// </summary>
		/// <param name="worldEnemies">The world enemies that are potential targets for whatever move is up for picks.</param>
		/// <param name="behaviorToUse">What is the behavior that is up for use?</param>
		private void EnterEnemySelectionState(List<Enemy> enemies, BattleBehavior behaviorToUse) {
			/*EnemyCursorDXController.Instance.BuildEnemyCursorsWithSelectables(
				worldEnemies: BattleArenaControllerDX.Instance.ActiveWorldEnemyDXs		// Grab the world enemies from the battle arena controller,
					.Where(we => enemies.Contains(we.Enemy))						    // but only the ones whose enemies are inside the list passed in.
					.ToList(),	
				currentBehavior: behaviorToUse);										// The behavior being passed in determines whether selectables are enabled.*/

			EnemyCursorDXController.instance.BuildEnemyCursorsWithSelectables(
				worldEnemies: enemies.Select(e => e.WorldEnemyDX).ToList(),
				currentBehavior: behaviorToUse);                                        // The behavior being passed in determines whether selectables are enabled.
		}
		/// <summary>
		/// The method that should get called when exiting the enemy selection state.
		/// </summary>
		private void ExitEnemySelectionState() {
			// Turn off the gameobject that has all the enemy cursors in it.
			EnemyCursorDXController.instance.DisableEnemyCursors();
		}
		#endregion

		#region STATE MACHINE - FINALIZE
		/// <summary>
		/// Sends the selections that were made to the battle controller.
		/// </summary>
		public void SendSelectionsToBattleController() {

			// Hide the visuals for the Final Behavior since I'm sending it out. Sad!
			FinalBehaviorSelectionController.instance.HideFinalBehavior();

			// Tell the rotating cube to tween out.
			RotatingMenuSquare.instance.TweenSquareStatus(status: false);

			// Turn off the top level selections. Usually this would have been done anyway but.
			this.TopLevelSelectionsGameObject.SetActive(false);

			// Clear the selection stack because I don't need it anymore for this turn.
			this.currentSelectionStack.Clear();

			// Send over the behavior to the BattleController.
			BattleController.Instance.PrepareBehaviorEvaluation(
				source: BattleController.Instance.CurrentCombatant,
				targets: this.CurrentTargetCombatants,
				behavior: this.CurrentBattleBehavior);

		}
		#endregion

		#region HELPERS - TOP LEVEL SELECTIONS
		/// <summary>
		/// Goes through the list of restrictor modifiers and transform them into a list of usable top level selection types.
		/// </summary>
		/// <param name="restrictorModifiers">The modifiers that restrict usage of a top level selection.</param>
		/// <returns></returns>
		private List<BattleMenuDXTopLevelSelectionType> GetUsableTopLevelSelections(List<ITopLevelSelectionRestrictor> restrictorModifiers) {
			// As a fail safe, if the restrictor modifiers is empty, return all available top level selections.
			if (restrictorModifiers.Count == 0) {
				return new List<BattleMenuDXTopLevelSelectionType>() {
					BattleMenuDXTopLevelSelectionType.Analysis,
					BattleMenuDXTopLevelSelectionType.Attack,
					BattleMenuDXTopLevelSelectionType.BatonTouch,
					BattleMenuDXTopLevelSelectionType.Escape,
					BattleMenuDXTopLevelSelectionType.Guard,
					BattleMenuDXTopLevelSelectionType.Item,
					BattleMenuDXTopLevelSelectionType.Masque,
					BattleMenuDXTopLevelSelectionType.Skill,
				};
			} else {
				// If there WERE modifiers passed in, go through them manually.
				return restrictorModifiers                                                  // Get all the modifiers that restrict top level selections.
				.Select(i => i.RestrictTopLevelSelections())                                // Transform them into the list of lists of possible top level selections.
				.Aggregate<IEnumerable<BattleMenuDXTopLevelSelectionType>>(                 // Go through each list...
					(previousList, nextList) => previousList.Intersect(nextList))           // ... and get the intersect. By the end, it should be reduced down as much as possible.
				.ToList();
			}
		}
		#endregion

		#region HELPERS - BUST UP
		/// <summary>
		/// Sets the bust up sprite on screen to be that of the given player.
		/// </summary>
		/// <param name="player">The Player to show the bust up for.</param>
		public void SetPlayerBustUpSprite(Player player) {

			// Reset the light on the player bust up. This may be needed if I interrupted the shine effect earlier.
			this.playerBustUpShineEffectImage.GetComponent<_2dxFX_Shiny_Reflect>().Light = -0.5f;

			/*this.playerBustUpFrontImage.sprite = player.playerTemplate.bustUp;
			this.playerBustUpDropshadowImage.sprite = player.playerTemplate.bustUp;
			this.playerBustUpShineEffectImage.sprite = player.playerTemplate.bustUp;*/
			this.playerBustUpFrontImage.overrideSprite = player.playerTemplate.bustUp;
			this.playerBustUpDropshadowImage.overrideSprite = player.playerTemplate.bustUp;
			this.playerBustUpShineEffectImage.overrideSprite = player.playerTemplate.bustUp;

			// Set the material on the bust up to be whatever affliction the player currently has.
			if (player.Affliction.Type != AfflictionType.None) {
				this.playerBustUpFrontImage.material = DataController.GetAfflictionMaterial(afflictionType: player.Affliction.Type);
			} else {
				this.playerBustUpFrontImage.material = null;
			}
			

		}
		#endregion

		#region HELPERS - BATON TOUCH
		/// <summary>
		/// Sets whether the button representing the baton touch should be shown.
		/// </summary>
		/// <param name="status">Whether or not the baton touch button should be shown.</param>
		public void SetBatonTouchButton(bool status) {
			// ONLY set it active if there is more than one combatant to pick from.
			if (BattleController.Instance.BatonTouchReadyCombatants.Count > 0) {
				this.topLevelSelections.Last().gameObject.SetActive(status);
			} else {
				this.topLevelSelections.Last().gameObject.SetActive(false);
			}
			/*List<Combatant> batonTouchReady = BattleController.Instance.batonTouchReady;
			batonTouchReady = batonTouchReady.FindAll(c => c.isDown == false);
			batonTouchReady = batonTouchReady.FindAll(c => c.IsDead == false);
			if (batonTouchReady.Count == 0) {
				Debug.Log("Everybody has had the baton, or there are people who are downed/dead. Don't activate it.");
				batonTouchButton.SetActive(false);
			} else {
				batonTouchButton.SetActive(status);
			}*/
			
		}
		#endregion

		#region ANIMATIONS - SHINE
		/// <summary>
		/// Plays the animation that gets used for when the player's bust up needs to shine.
		/// </summary>
		/// <param name="endLightValue">The value that should be used as the end result of the light. Default makes light shine the entire way.</param>
		/// <param name="shineTime">The amount of time to take when shining the light. Default is one second.</param>
		internal void PlayerBustUpShineAnimation(float endLightValue = 1.5f, float shineTime = 1f) {
			// Get the reflect component from the shine effect image.
			_2dxFX_Shiny_Reflect reflectEffect = this.playerBustUpShineEffectImage.GetComponent<_2dxFX_Shiny_Reflect>();
			// Initialize its default values.
			reflectEffect.UseShinyCurve = false;
			reflectEffect.Light = -0.5f;
			// Use DOTween for that shit babey!!
			DOTween.To(
				getter: () => reflectEffect.Light,
				setter: x => reflectEffect.Light = x,
				endValue: endLightValue,
				duration: shineTime);
		}
		#endregion

	}


}