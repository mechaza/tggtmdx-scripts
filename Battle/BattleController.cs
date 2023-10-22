using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grawly;
using Grawly.Dungeon;
using Grawly.Chat;
using DG.Tweening;
using System;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.BattleMenu.Legacy;
using Grawly.Battle.Functions;
using Grawly.Battle.Modifiers;
using Grawly.Battle.Navigator;
using Grawly.Toggles;
using Grawly.Toggles.Proto;
using Grawly.UI.Legacy;
using UnityEngine.SceneManagement;

namespace Grawly.Battle {
	
	public class BattleController : MonoBehaviour {

		public static BattleController Instance { get; private set; }

		#region FIELDS - STATE : FLAGS
		/// <summary>
		/// Is there a battle in progress?
		/// </summary>
		private bool isBattling = false;
		/// <summary>
		/// If the current combatant has exploited their enemy's weakness, this signals that they should be able to go again
		/// </summary>
		private bool oneMore = false;
		/// <summary>
		/// A flag for the baton touch. I think there's some set of rules I had for how this gets messed around with and I forgot it.
		/// </summary>
		private bool batonTouchUsed = false;
		#endregion

		#region FIELDS - STATE : TEMPLATES AND BEHAVIORS
		/// <summary>
		/// The current template to build up enemies from.
		/// </summary>
		public BattleTemplate CurrentBattleTemplate { get; private set; }
		/// <summary>
		/// The current behavior set that contains routines to run at specific points in the battle.
		/// </summary>
		public BattleParams CurrentBattleParams { get; private set; } 
		#endregion
		
		#region FIELDS - STATE
		/// <summary>
		/// Is there a battle in progress?
		/// This actually gets set in the DungeonController because things like the results screen might interfere.
		/// </summary>
		public bool IsBattling {
			get {
				return this.isBattling;
			}
			set {
				this.isBattling = value;
			}
		}
		/// <summary>
		/// The FSM used to control the flow of battle.
		/// This is primarily being used as a state machine.
		/// </summary>
		public PlayMakerFSM FSM { get; private set; }
		#endregion
		
		#region FIELDS - STATE : MODIFIERES AND REACTION SEQUENCES
		/// <summary>
		/// A dictionary that holds the different kinds of battle reaction sequences that get evaluated in response to different events.
		/// </summary>
		private Dictionary<BattleReactionSequenceType, BattleReactionSequence> battleReactionSequenceDict = new Dictionary<BattleReactionSequenceType, BattleReactionSequence>();
		#endregion

		#region FIELDS - STATE : TURN VARIABLES
		/// <summary>
		/// Check if all of the enemies are down or not. Helpful for All Out Attacks.
		/// </summary>
		public bool AllEnemiesDown {
			get {
				return this.Enemies.Count(e => e.IsDown == false) == 0;
			}
		}
		/// <summary>
		/// The data structure containing the turn order for the battle.
		/// </summary>
		private CombatantTurnQueue TurnQueue { get; set; } = new CombatantTurnQueue();
		/// <summary>
		/// The combatant who is currently up.
		/// </summary>
		public Combatant CurrentCombatant { get; private set; } = null;
		#endregion

		#region FIELDS - STATE : COMBATANT REFERENCES
		/// <summary>
		/// The Players currently involved in the battle.
		/// Calculated from the GameController's variables.
		/// </summary>
		public List<Player> Players {
			get {
				return GameController.Instance.Variables.Players;
			}
		}
		/// <summary>
		/// The enemies currently in the battle.
		/// Calculated from the BattleArenaControllerDX's active enemies.
		/// </summary>
		public List<Enemy> Enemies {
			get {
				return BattleArenaControllerDX.instance.ActiveWorldEnemyDXs.Select(we => we.Enemy).ToList();
			}
		}
		#endregion

		#region FIELDS - COMBATANTS COMPUTED AS A RESULT OF THE STATE ABOVE
		/// <summary>
		/// All of the combatants in the battle. Or at least the combination of both Players and Enemies.
		/// </summary>
		public List<Combatant> AllCombatants {
			get {
				// Just returns all the combatants.
				return this.Players.Cast<Combatant>().Concat(this.Enemies.Cast<Combatant>()).ToList();
			}
		}
		/// <summary>
		/// Returns everyone who is still alive in battle.
		/// </summary>
		public List<Combatant> AllAliveCombatants {
			get {
				return this.AlivePlayers.Cast<Combatant>().Concat(this.AliveEnemies.Cast<Combatant>()).ToList();
			}
		}
		/// <summary>
		/// A list of players who are considered to be alive (more than zero HP)
		/// </summary>
		public List<Player> AlivePlayers {
			get {
				return this.Players.Where(p => p.IsDead == false).Cast<Player>().ToList();
			}
		}
		/// <summary>
		/// A list of players who are both alive but also able to participate in actions such as all out attacks or baton touches.
		/// </summary>
		public List<Player> HealthyAlivePlayers {
			get {
				return this.AlivePlayers.Where(p => p.IsDown == false && p.Affliction.Type == AfflictionType.None).ToList();
			}
		}
		/// <summary>
		/// A list of players who are currently considered to be dead (zero HP)
		/// </summary>
		public List<Player> DeadPlayers {
			get {
				return this.Players.Where(p => p.IsDead == true).Cast<Player>().ToList();
			}
		}
		/// <summary>
		///  All of the dead enemies.
		/// </summary>
		public List<Enemy> DeadEnemies {
			get {
				return this.Enemies.Where(e => e.IsDead == true).ToList();
			}
		}
		/// <summary>
		/// All of the enemies who are still alive.
		/// </summary>
		public List<Enemy> AliveEnemies {
			get {
				return this.Enemies.Where(e => e.IsDead == false).ToList();
			}
		}		
		#endregion

		#region FIELDS - STATE : COMBATANTS WHO HAVE SOMETHING GOING ON WITH THEM
		/// <summary>
		/// A list of the members who haven't touched the baton this turn.
		/// Gets reset every time the turn queue is dequeued. That's just how it be.
		/// </summary>
		private List<Combatant> potentialBatonTouchReadyCombatants = new List<Combatant>();
		/// <summary>
		/// A list of the members who haven't touched the baton this turn and are not in a condition where they can't.
		/// </summary>
		public List<Combatant> BatonTouchReadyCombatants {
			get {
				// Make sure those in the list are also in a state where they CAN touch the baton.
				return this.potentialBatonTouchReadyCombatants.Where(c => c.IsDown == false && c.IsDead == false).ToList();
			}
		}
		#endregion

		#region FIELDS - STATE : BATTLE BEHAVIOR METADATA
		// These three fields are set when a move is selected by an enemy/player
		// just before the FSM is called to begin evaluating them.
		// See PrepareBehaviorEvaluation in BattleMenuController for more details on
		// the player side.
		private BattleBehavior behavior;
		private List<Combatant> targets;
		private Combatant source;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				// DontDestroyOnLoad(this.gameObject);
				this.FSM = GetComponent<PlayMakerFSM>();
			}
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Resets the state of the battle controller.
		/// </summary>
		private void ResetState() {

			this.CurrentBattleTemplate = null;
			this.CurrentBattleParams = null;
			
			// Null out these variables.
			this.behavior = null;
			this.source = null;
			this.targets = null;
			
			// Add a sequence for every reaction sequence type.
			foreach (BattleReactionSequenceType reactionSequenceType in System.Enum.GetValues(typeof(BattleReactionSequenceType))) {
				this.battleReactionSequenceDict[reactionSequenceType] = new BattleReactionSequence();
			}
			
			// Reset the list of baton touch ready combatants.
			this.potentialBatonTouchReadyCombatants.Clear();

			// Reset the one more flag and baton touch flag.
			this.oneMore = false;
			this.batonTouchUsed = false;
			
			// Null out the current combatant.
			this.CurrentCombatant = null;
			
			// Clear the turn queue.
			this.TurnQueue.ResetState();
			
			this.isBattling = false;

		}
		#endregion
		
		#region STARTUP
		/// <summary>
		/// Starts a battle with the given template.
		/// The behaviors stored in the template will be used for intros/parsers/etc.
		/// </summary>
		/// <param name="battleTemplate">The template being used to create this battle.</param>
		/// <param name="battleAdvantageType">The advantage type to start this battle with.</param>
		/// <param name="freePlayerOnReturn">Should the player be freed upon returning from the battle?</param>
		/// <param name="onBattleComplete">A callback to run upon completion of the battle.</param>
		/// <param name="onBattleReturn">A callback to run upon returning from the results screen.</param>
		public void StartBattle(BattleTemplate battleTemplate, BattleAdvantageType battleAdvantageType = BattleAdvantageType.Normal, bool freePlayerOnReturn = true, Action onBattleComplete = null, Action onBattleReturn = null) {
			
			Debug.Log("Starting battle: " + battleTemplate.BattleName + ".");

			// Generate the battle params. I need to do this because I also need to pass the params the advantage type.
			BattleParams generatedBattleParams = battleTemplate.GenerateBattleParams();
			generatedBattleParams.BattleAdvantageType = battleAdvantageType;
			generatedBattleParams.FreePlayerOnReturn = freePlayerOnReturn;
			generatedBattleParams.OnBattleComplete = onBattleComplete;
			generatedBattleParams.OnBattleReturn = onBattleReturn;
			
			// Get the routine set from the template and call the version of this function that accepts it.
			this.ExecuteBattle(
				battleTemplate: battleTemplate, 
				battleParams: generatedBattleParams);

		}
		/// <summary>
		/// Starts a battle with the given template.
		/// The behaviors stored in the template will be used for intros/parsers/etc.
		/// </summary>
		/// <param name="battleTemplate">The template being used to create this battle.</param>
		/// <param name="battleParams">The behavior set containing things like intros/arena setup/etc.</param>
		public void StartBattle(BattleTemplate battleTemplate, BattleParams battleParams) {
			
			Debug.Log("Starting battle: " + battleTemplate.BattleName + "."
			          + "\nPARAMETERS HAVE BEEN PASSED IN MANUALLY AND WILL NOT BE GENERATED FROM THE TEMPLATE.");
			
			// Call the function that actually executes the battle.
			this.ExecuteBattle(battleTemplate: battleTemplate, battleParams: battleParams);
			
		}
		/// <summary>
		/// The actual function that executes the battle.
		/// </summary>
		/// <param name="battleTemplate">The battle template asset used to create this battle.</param>
		/// <param name="battleParams">The battle params that were generated for this battle.</param>
		private void ExecuteBattle(BattleTemplate battleTemplate, BattleParams battleParams) {
			
			Debug.Log("Starting battle with template: " + battleTemplate.BattleName + ".");

			// Reset the state.
			this.ResetState();
			
			// Save the battle template being used.
			this.CurrentBattleTemplate = battleTemplate;
			this.CurrentBattleParams = battleParams;
			
			// If the battle navigator is in the scene, also prepare them with the template.
			BattleNavigator.Instance?.Prepare(battleTemplate: battleTemplate, battleParams: battleParams);
			
			// Is this where I call the intro?
			battleParams.BattleIntro.PlayIntro(template: battleTemplate, battleParams: battleParams);
			
			this.IsBattling = true;
			
		}
		/// <summary>
		/// Initializes the battle. Replaces what was previously the Start function.
		/// </summary>
		public void OnIntroCompleted() {

			// Prep the turn queue.
			this.TurnQueue.Prepare(
				combatants: this.AllCombatants, 
				advantageType: this.CurrentBattleParams.BattleAdvantageType);
			
			// Let the FSM know it's ready to proceed.
			FSM.SendEvent("Initialization Complete");
			return;
		}
		#endregion

		#region ONE MORE RELATED
		/// <summary>
		/// Sets up OneMore mode for when GetNextCombatant is called.
		/// </summary>
		public void EnableOneMore(bool status = true) {
			this.oneMore = status;
		}
		/// <summary>
		/// Passes the baton from the source to the target.
		/// Theoretically I could do ALL of this as a battle behavior, but I feel like
		/// it might be best here.
		/// </summary>
		public void BatonTouch(Combatant source, Combatant target) {
			// Reassign the current combatant
			this.CurrentCombatant = target;
			// Turn on the baton touch variable.
			this.batonTouchUsed = true;
		}
		#endregion

		#region BATTLE STATE - BEHAVIOR EVALUATION
		/// <summary>
		/// Preps the Battle Controller to get ready to evaluate the behavior.
		/// </summary>
		public void PrepareBehaviorEvaluation(Combatant source, List<Combatant> targets, BattleBehavior behavior) {
		
			this.targets = targets;
			this.behavior = behavior;
			this.source = source;

			// Remember to turn off the baton pass button
			BattleMenuControllerDX.instance.SetBatonTouchButton(status: false);

			FSM.SendEvent("Behavior Selected");
		}
		/// <summary>
		/// Gets called from the FSM and evaluates the battle behavior that was previously set
		/// </summary>
		public void EvaluateBehavior() {
			
			// Display the behavior on the battle notifier
			BattleNotifier.DisplayNotifier(behavior.behaviorName, 3f);
			// Evaluate the behavior.
			Debug.Log("Begin function execution for: " + behavior.behaviorName);
			behavior.BattleFunction.Execute(source: source, targets: targets, battleBehavior: behavior);
			// StartCoroutine(behavior.Function(source, targets, behavior));
			
			
		}
		#endregion

		#region BATTLE STATE - BATTLE EVALUATION
		/// <summary>
		/// Evaluates the battle state to determine whether to keep going or not.
		/// </summary>
		public void EvaluateBattleState() {

			if (this.Players.Count(p => p.IsDead == false) == 0) {
				// Check if all the players are dead.
				FSM.SendEvent("Game Over");
				// Check if all the enemies are death.
			} else if (this.AliveEnemies.Count == 0) {
				FSM.SendEvent("Battle Complete");
				// If all enemies are down and the last attack enabled a One More, ask for an all out attack.
			} else if (this.AllEnemiesDown == true && this.oneMore == true && this.HealthyAlivePlayers.Count > 1) {
				FSM.SendEvent("All Out Attack Prompt");
			} else {
				// If both conditions are false, continue the battle.
				FSM.SendEvent("Continue Battle");
			}
		}
		/// <summary>
		/// Battle is complete, Winner ! ! !
		/// </summary>
		public void BattleComplete() {

			// Turn off the active visuals.
			PlayerStatusDXController.instance.TurnOffActivePlayer();
			
			// Tell the battle menu controller to reset its state.
			BattleMenuControllerDX.instance.ResetState();

			// Turn off one more.
			this.EnableOneMore(false);
			// Also turn off the baton touch
			this.batonTouchUsed = false;

			// Clear the turn queue
			this.TurnQueue.ResetState();
			
			// Go through each of the players and clear out their modifiers.
			this.Players.ForEach(p => p.ClearModifiers());

			// Go through each dungeon enemy in the player's radius and destroy it.
			DungeonPlayer.Instance?.DungeonEnemiesInRadius.ForEach(de => Destroy(de.gameObject));

			// Reset the status modifiers and afflictions on each player.
			this.Players.ForEach(p => {
				p.statusModifiers.Reset();
				p.CureAffliction(animateCure: true);
				p.SetDownedStatus(isDown: false);
				p.PlayerStatusDX.QuickRebuild();
			});

			// Parse the battle drops and get the results that should be used in the Results Controller.
			BattleResultsSet battleResults = this.CurrentBattleParams.DropParser.ParseBattleDrops(
				battleTemplate: this.CurrentBattleTemplate, 
				battleParams: this.CurrentBattleParams,
				gameVariables: GameController.Instance.Variables);

			// For all the dead players, un-down them, give them one HP.
			this.DeadPlayers.ForEach(player => {
				player.HP = 1;
			});

			// Also nullify the Current Combatant reference so the page turn effect doesn't happen on the next battle.
			this.CurrentCombatant = null;
			
			// Invoke the BattleComplete callback, if one exists (usually used for destroying enemies from the scene)
			BattleController.Instance.CurrentBattleParams.OnBattleComplete?.Invoke();
			
			BattleController.Instance.CurrentBattleParams.BattleOutro.PlayOutro(
				template: BattleController.Instance.CurrentBattleTemplate, 
				battleParams: BattleController.Instance.CurrentBattleParams, 
				battleResultsSet: battleResults);

		}
		/// <summary>
		/// Game over... gets called from FSM.
		/// </summary>
		public void GameOver() {
			Debug.Log("GAME OVER");
			GameController.Instance.WaitThenRun(timeToWait: 3f, action: delegate {
				Grawly.UI.Legacy.Flasher.instance.FadeOut(color: Color.white, fadeTime: 1.5f);
				// Grawly.UI.Legacy.Flasher.Instance.Fade(color: Color.white, fadeOut: 2f, fadeIn: 1f, interlude: 7f, showLoadingText: false);
				AudioController.instance.StopMusic(track: 1, fade: 2f);
			});
			
			GameController.Instance.WaitThenRun(timeToWait: 5f, action: delegate {
				SceneManager.LoadScene("GameOver");
			});
		}
		#endregion

		#region BATTLE STATE - MISC EVENTS
		/// <summary>
		/// Event that gets called at the very start of the battle.
		/// </summary>
		public void BattleStartEvent() {
			Debug.Log("BATTLE: Running Battle Start Event.");

			// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.BattleStart].Prepare(defaultFinishCallback: delegate { this.FSM.SendEvent("Pre Turn"); });

			// Grab the reactions from the battle modifiers as well as all the alive combatants.
			List<BattleReaction> battleModifierReactions = this.CurrentBattleParams.BattleModifiers.Where(bm => bm is IOnBattleStart).Cast<IOnBattleStart>().Select(i => i.OnBattleStart()).ToList();
			List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnBattleStart>()).Select(i => i.OnBattleStart()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.BattleStart].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());

			// Also add the navigator's reaction to the end of the sequence.
			this.battleReactionSequenceDict[BattleReactionSequenceType.BattleStart].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnBattleStart>().Select(i => i.OnBattleStart()).ToList());

			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.BattleStart].ExecuteNextReaction();

		}
		/// <summary>
		/// Gets called after the intro but before GetNextCombatant().
		/// By extension, gets called after TurnEndEvent(), but before GetNextCombatant().
		/// </summary>
		public void PreTurnEvent() {
			Debug.Log("BATTLE: Running Pre Turn Event.");
			
			
			// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].Prepare(defaultFinishCallback: delegate { this.FSM.SendEvent("Get Next Combatant"); });

			// Grab the reactions from the battle modifiers as well as all the alive combatants.
			List<BattleReaction> battleModifierReactions = this.CurrentBattleParams.BattleModifiers.Where(bm => bm is IOnPreTurn).Cast<IOnPreTurn>().Select(i => i.OnPreTurn()).ToList();
			List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnPreTurn>()).Select(i => i.OnPreTurn()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());

			// Also add the navigator's reaction to the end of the sequence.
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnPreTurn>().Select(i => i.OnPreTurn()).ToList());

			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.PreTurn].ExecuteNextReaction();

		}
		/// <summary>
		/// Get the next combatant and start up their turn.
		/// </summary>
		public void GetNextCombatant() {

			Debug.Log("Running GetNextCombatant");

			// Turn the page over. Don't do this on the first turn.
			if (this.CurrentCombatant != null) { BattleCameraController.Instance.PageTurnTransition(); }

			// Get the current combatant by dequeuing them, but don't proceed if the combatant is dead.
			// This only really would happen if its a player thats dead. I'm already removing the dead enemies
			// in BehaviorEvaluated.
			do {

				// If OneMore is enabled, turn it back off and use the currentcombatant again.
				if (this.oneMore == true) {

					// Start the OneMore Animation.
					OneMoreAnimationController.instance.PlayOneMoreAnimation();

					// Enable the baton touch button. It gets turned back off when a behavior is evaluated in PrepareBehaviorEvaluation.
					BattleMenuControllerDX.instance.SetBatonTouchButton(status: true);

					this.EnableOneMore(false);
					break;
				} else if (this.batonTouchUsed == true) {
					// batonTouch is true if it was SELECTED, not if it is "READY"
					this.batonTouchUsed = false;
					break;
				}

				// Reset the baton touch list. The ready players are basically everyone who is alive.
				this.potentialBatonTouchReadyCombatants = new List<Combatant>(AlivePlayers);

				// Get the next combatant. Note that the turn queue requires the combatant who went last turn in certain cases.
				this.CurrentCombatant = this.TurnQueue.GetNextCombatant(lastCombatant: this.CurrentCombatant);
				
				// As the new combatant is dequeued, set them back up.
				this.CurrentCombatant.SetDownedStatus(isDown: false);

			} while (this.CurrentCombatant.IsDead == true);



			// Remove the combatant from the baton touch list if they exist in it, so they can't be passed it again.
			if (this.potentialBatonTouchReadyCombatants.Contains(CurrentCombatant)) { this.potentialBatonTouchReadyCombatants.Remove(CurrentCombatant); }

			// Set the label telling the player who's up next. This is for DEBUG purposes and will not be in the final game.
			// this.nextCombatantLabel.Text = "Next: " + this.turnQueue.Peek().metaData.name;
			// BattleMenuController.Instance.nextCombatantLabel?.Text = "Next: " + this.turnQueue.Peek().metaData.name;

			Debug.Log("COMBATANT: " + this.CurrentCombatant.metaData.name);

			// Tell the FSM to head over to the Turn Start event.
			this.FSM.SendEvent("Turn Start");

		}
		/// <summary>
		/// Gets called after the combatant who is up next has been dequeued and other bullshit irt baton touch has been evaluated.
		/// </summary>
		public void TurnStartEvent() {
			Debug.Log("BATTLE: Running Turn Start Event.");

			// If the current combatant is a player, turn on their active visuals.
			if (this.CurrentCombatant is Player) {
				PlayerStatusDXController.instance.SetActivePlayer(player: CurrentCombatant as Player);
			}
			
			// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnStart].Prepare(defaultFinishCallback: delegate { this.FSM.SendEvent("Combatant Ready"); });

			// Grab the reactions from the battle modifiers as well as all the alive combatants.
			List<BattleReaction> battleModifierReactions = this.CurrentBattleParams.BattleModifiers.Where(bm => bm is IOnTurnStart).Cast<IOnTurnStart>().Select(i => i.OnTurnStart()).ToList();
			List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnTurnStart>()).Select(i => i.OnTurnStart()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnStart].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());

			// Also add the navigator's reaction to the end of the sequence.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnStart].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnTurnStart>().Select(i => i.OnTurnStart()).ToList());

			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnStart].ExecuteNextReaction();

		}
		/// <summary>
		/// Gets called after TurnStartEvent() has been run. Applies only to the combatant who is up.
		/// </summary>
		public void CombatantReadyEvent() {
			Debug.Log("BATTLE: Running Combatant Ready Event.");
			
			// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnReady].Prepare(defaultFinishCallback: delegate {
				// I actually need to run this via RunEndOfFrame because it can cause the reaction queue to become out of sync.
				// Fucked up.
				GameController.Instance.RunEndOfFrame((() => this.CurrentCombatant.TurnBehavior.ExecuteTurn()));
				// this.CurrentCombatant.TurnBehavior.ExecuteTurn();
			});

			// Grab the reactions from the battle modifiers as well as all the HUHHHHHHH not all alive combatants just the current one that is up.
			List<BattleReaction> battleModifierReactions = this.CurrentBattleParams.BattleModifiers.Where(bm => bm is IOnTurnReady).Cast<IOnTurnReady>().Select(i => i.OnTurnReady()).ToList();
			// List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnTurnReady>()).Select(i => i.OnTurnReady()).ToList();
			List<BattleReaction> combatantModifierReactions = this.CurrentCombatant.GetModifiers<IOnTurnReady>().Select(i => i.OnTurnReady()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnReady].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());

			// Also add the navigator's reaction to the end of the sequence.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnReady].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnTurnReady>().Select(i => i.OnTurnReady()).ToList());

			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnReady].ExecuteNextReaction();

		}
		/// <summary>
		/// Gets called from a behavior function when its complete
		/// </summary>
		public void BehaviorEvaluatedEvent() {
			Debug.Log("BATTLE: Running Behavior Evaluated Event.");
			
			// Turn off the active visuals.
			PlayerStatusDXController.instance.TurnOffActivePlayer();

			// Create an action to be run very very soon... very soon.. oh so soon...
			Action behaviorEvaluatedAction = delegate {
				// Update the player statuses so they reflect changes that have been made.
				foreach (Player player in this.Players) {
					// If the player is dead, set their down status to false and completely deplete thier SP.
					if (player.IsDead) {
						Debug.Log(player.metaData.name + " down to 0 HP! Depleting HP/Setting isDown to false");
						player.SetDownedStatus(false);
						player.SP = 0;
						player.SetAffliction(afflictionType: AfflictionType.None, animateAffliction: true);
					}
					player.PlayerStatusDX.QuickRebuild();
				}

				// Pick out the dead enemies from the turn queue.
				// this.turnQueue = RemoveDeadCombatantsFromQueue(queue: turnQueue, enemiesToRemove: DeadEnemies);
				this.TurnQueue.RemoveCombatants(combatantsToRemove: this.DeadEnemies.Cast<Combatant>().ToList());
				
				// Destroy any enemies that may have been killed.
				BattleArenaControllerDX.instance.RemoveWorldEnemies(
					enemiesToRemove: this.DeadEnemies, 
					battleParams: this.CurrentBattleParams);

				// Tell the FSM to continue.
				this.FSM.SendEvent("Behavior Evaluated");
			};



			// if there are any remaining enemies...
			// ... run the current combatant's reactions and use the behaviorEvaluatedAction as a finish callback
			// if there are no other reamining enemies...
			// ... just run the behaviorEvaluatedAction as is.


			//	if (this.RemainingEnemies.Where(e => e.HP > 0).Count() > 0) {
			if (this.AliveEnemies.Count > 0) {
				this.battleReactionSequenceDict[BattleReactionSequenceType.BehaviorEvaluated].Prepare(defaultFinishCallback: behaviorEvaluatedAction);
				this.battleReactionSequenceDict[BattleReactionSequenceType.BehaviorEvaluated].AddToSequence(battleReactions: this.CurrentCombatant.GetModifiers<IOnBehaviorEvaluated>().Select(m => m.OnBehaviorEvaluated()).ToList());
				this.battleReactionSequenceDict[BattleReactionSequenceType.BehaviorEvaluated].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnBehaviorEvaluated>().Select(i => i.OnBehaviorEvaluated()).ToList());
				this.battleReactionSequenceDict[BattleReactionSequenceType.BehaviorEvaluated].ExecuteNextReaction();
			} else {
				behaviorEvaluatedAction();
			}

		}
		/// <summary>
		/// A new state I'm placing in between "Continue Battle" and "Get Next Combatant" in the FSM.
		/// Basically plays the CurrentCombatant's TurnEndEvent sequence.
		/// It is assumed that the queue is ready to be popped at this point.
		/// </summary>
		public void TurnEndEvent() {

			Debug.Log("BATTLE: Running Turn End Event.");

			// Prep the sequence to get going by passing it the event it should run when complete.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnEnd].Prepare(defaultFinishCallback: delegate { this.FSM.SendEvent("Continue Battle"); });

			// Grab the reactions from the battle modifiers as well as all the alive combatants.
			List<BattleReaction> battleModifierReactions = this.CurrentBattleParams.BattleModifiers.Where(bm => bm is IOnTurnEnd).Cast<IOnTurnEnd>().Select(i => i.OnTurnEnd()).ToList();
			List<BattleReaction> combatantModifierReactions = this.AllAliveCombatants.SelectMany(c => c.GetModifiers<IOnTurnEnd>()).Select(i => i.OnTurnEnd()).ToList();

			// Pass them over to the sequence that needs them.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnEnd].AddToSequence(battleReactions: battleModifierReactions.Concat(combatantModifierReactions).ToList());

			// Also add the navigator's reaction to the end of the sequence.
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnEnd].AddToSequence(battleReactions: BattleNavigator.Instance.GetModifiers<IOnTurnEnd>().Select(i => i.OnTurnEnd()).ToList());

			// Execute the next reaction (which effectively means beginning a chain reaction)
			this.battleReactionSequenceDict[BattleReactionSequenceType.TurnEnd].ExecuteNextReaction();

		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Returns all modifiers of the specified type. Includes battle modifiers and modifiers from alive combatants.
		/// Adding this August 2018.
		/// </summary>
		/// <returns></returns>
		public List<T> GetAllModifiers<T>() {
			return this.CurrentBattleParams.BattleModifiers.Where(m => m is T).Cast<T>()					// Grab the battle modifiers that implement this interface.
				.Concat(this.AllAliveCombatants.SelectMany(c => c.GetModifiers<T>()))	// Also grab from the alive combatants.
				.ToList();
		}
		/// <summary>
		/// Returns all modifiers of the specified type. Only includes battle modifiers.
		/// Adding this August 2018.
		/// </summary>
		/// <returns></returns>
		public List<T> GetBattleModifiers<T>() {
			return this.CurrentBattleParams.BattleModifiers.Where(m => m is T).Cast<T>().ToList();
		}
		#endregion

		#region BATTLE REACTION SEQUENCES
		/// <summary>
		/// Adds a battle reaction of the specified type to the given BattleReactionSequence.
		/// Typically gets called from something external to the BattleController.
		/// </summary>
		/// <param name="reactionSequenceType">The type of battle reaction sequence to add to.</param>
		/// <param name="battleReaction">The reaction to be evaluated.</param>
		public void AddReaction(BattleReactionSequenceType reactionSequenceType, BattleReaction battleReaction) {
			this.battleReactionSequenceDict[reactionSequenceType].AddToSequence(battleReaction: battleReaction);
		}
		#endregion

		#region ALL OUT ATTACKS
		/// <summary>
		/// Checks if an all out attack is ready. 
		/// </summary>
		public void CheckForAllOutAttack() {
			
			// If the toggle does not allow for an all out attack, return.
			if (ToggleController.GetToggle<AllowAllOutAttack>().GetToggleBool() == false) {
				return;
			}

			// Check if all the enemies are down and there are at least two players who can participate in the attack.
			if (this.AllEnemiesDown == true && this.HealthyAlivePlayers.Count >= 2) {

				// Clear out the statuses to make room for the prompt visuals.
				PlayerStatusDXController.instance.TweenVisible(status: false, inBattle: true);

				// Displaying this prompt will show it, but also turn off the bust up.
				LegacyAllOutAttackDXAnimationController.instance.EnablePrompt(
					activePlayers: this.HealthyAlivePlayers,
					haveSeenTutorial: GameController.Instance.Variables.StoryFlags.GetFlag(flagType: StoryFlagType.SawAllOutAttackTutorial));

				// Also move the camera to the orbit position.
				BattleCameraController.Instance.ActivateVirtualCamera(BattleCameraController.BattleCameraType.OrbitEnemies);

			} else {
				// If not all the enemies are down, just keep going.
				FSM.SendEvent("All Out Attack Declined");
			}
		}
		/// <summary>
		/// After an all out attack, clean up things and re-evaluate the battle state.
		/// </summary>
		public void AllOutAttackComplete() {

			// Get each enemy back up.
			this.Enemies.ForEach(e => e.SetDownedStatus(isDown: false));

			// Turn off one more.
			EnableOneMore(status: false);

			// Tween the player statuses back in.
			PlayerStatusDXController.instance.TweenVisible(status: true, inBattle: true);

			// Send the behavior evaluated event.
			this.BehaviorEvaluatedEvent();
		}
		#endregion

	}

}


