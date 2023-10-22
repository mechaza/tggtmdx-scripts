using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uConsole;
using Grawly.Battle;
using Grawly.Dungeon;
using Grawly.Story;
using UnityEngine.EventSystems;
using Grawly;
using System.Linq;

namespace Grawly.Console {

	/// <summary>
	/// A very basic script that adds commands to the uConsole.
	/// </summary>
	public class StandardConsoleCommands : MonoBehaviour {

		/// <summary>
		/// A reference to the console interface in the event that I need access to it. (E.x, closing it when entering "finishbattle" or "startbattle")
		/// </summary>
		private ConsoleCommand mConsole;

		private void Start() {
			ConsoleCommand mConsole = FindObjectOfType<ConsoleCommand>();
			if (mConsole != null) {
				this.RegisterCommands(mConsole: mConsole);
			}
		}

		private void RegisterCommands(ConsoleCommand mConsole) {
			// Save the reference.
			this.mConsole = mConsole;

			mConsole.commandRegister.RegisterCommand("deletesavedata", this.DeleteSaveData, "Deletes all save data. Some crashes may be caused by changes to save data itself.");

			mConsole.commandRegister.RegisterCommand("startbattle", this.StartBattle, "Starts a battle from the specified template. startbattle [template name]");
			mConsole.commandRegister.RegisterCommand("finishbattle", this.FinishBattle, "Forces a battle in progress to end. Helpful if the battle soft locks.");
			
			mConsole.commandRegister.RegisterCommand("nextfloor", this.NextFloor, "Go to the next floor of the dungeon.");

			mConsole.commandRegister.RegisterCommand("additem", this.AddItem, "Adds the specified item to the inventory. additem [item name] [quantity #]");
			mConsole.commandRegister.RegisterCommand("addmove", this.AddMove, "Adds the move to the specified combatant. Only works with personas in the persona pool, but will accept the girls as well. addmove [combatant name] [move to add] [slot #](optional)");
			mConsole.commandRegister.RegisterCommand("setlevel", this.SetLevel, "Sets the level of the specified combatant. Can be used for both players (the girls) or personas in the persona pool. setlevel [combatant name] [level #]");
			mConsole.commandRegister.RegisterCommand("healparty", this.RestoreParty, "Restores the party's HP/MP/SP.");
			mConsole.commandRegister.RegisterCommand("setattribute", this.SetAttribute, "Sets an attribute on the given party member. setattribute [combatant name] [resource name] [resource value]");


			mConsole.commandRegister.RegisterCommand("setdebugfield", this.SetDebugField, "Assign a new value to the specified debug variable. setdebugfield [fieldname] [fieldvalue]");

			mConsole.commandRegister.RegisterCommand("loadscene", this.LoadScene, "Loads the specified scene. loadscene [scene name]");

			mConsole.commandRegister.RegisterCommand("grandmotherlode", this.GrandmotherLode, "Add $10000 to wallet.");
		}

		#region SAVE COMMANDS
		/// <summary>
		/// Deletes all save data.
		/// </summary>
		/// <param name="args"></param>
		private void DeleteSaveData(string[] args) {
			SaveController.DeleteSaveCollection();
		}
		#endregion
		
		#region BATTLE COMMANDS
		/// <summary>
		/// Starts a battle.
		/// </summary>
		/// <param name="args"></param>
		private void StartBattle(string[] args) {
			if (BattleController.Instance.IsBattling == true) {
				Debug.LogWarning("A battle is currently in progress. Please either finish the battle or use the finishbattle command.");
				return;
			}

			Debug.LogError("This command is broken. Please remind me to fix it. Thanks.");
			// Start up a battle with completely random enemies by asking the MissionData for a new Battle Template, populated from the pool of enemies in the MissionData.
			// DungeonController.Instance.PrepareBattle(battleTemplates: new List<BattleTemplate>() { LegacyStoryController.Instance.CurrentMission.GetBattleTemplate() });
			// Close out the console.
			// this.mConsole.SetVisibility(visible: false);
		}
		/// <summary>
		/// Forces the battle to end.
		/// </summary>
		/// <param name="args"></param>
		private void FinishBattle(string[] args) {
			// Call BackToDungeon, which should take care of it.
			if (BattleController.Instance.IsBattling == true) {
				BattleController.Instance.BattleComplete();
				// BattleController.Instance.BackToDungeon();
				// this.mConsole.SetVisibility(visible: false);
			} else {
				Debug.Log("There is currently no battle being played out.");
			}
		}

		#endregion

		#region BATTLE CALCULATIONS
		/// <summary>
		/// Passes a value to the debug field to set.
		/// </summary>
		/// <param name="args"></param>
		private void SetDebugField(string[] args) {
			// Initialize variables and parse them out.
			string fieldName; float fieldValue;
			fieldName = args[0];
			float.TryParse(args[1], out fieldValue);
			// Pass that info to the debug controller.
			throw new System.Exception("This was broken and I need to re-add it.");
			// DebugController.SetDebugField(fieldName: fieldName, fieldValue: fieldValue);
		}
		#endregion

		#region DUNGEON
		/// <summary>
		/// Tells the DungeonController to skip to the next floor.
		/// </summary>
		/// <param name="args"></param>
		private void NextFloor(string[] args) {
			if (BattleController.Instance.IsBattling == false) {
				throw new System.Exception("Deprecated this.");
				// DungeonGenerator.Instance.AdvanceFloor();
			} else {
				Debug.Log("You are in the middle of a battle. Please use 'finishbattle' to stop the battle and then try again.");
			}
			
		}
		#endregion

		#region COMBATANTS
		/// <summary>
		/// Sets the level on the specified combatant.
		/// </summary>
		/// <param name="args"></param>
		private void SetLevel(string[] args) {
			try {
				string combatantName = args[0];
				int level; System.Int32.TryParse(args[1], out level);

				// Find the player who's name matches the name passed in, calculate the exp required to set to that level, and then do so.
				GameController.Instance.Variables.Players
					.Where(p => p.metaData.name == combatantName)
					.ToList()
					.ForEach(p => p.TotalEXP = p.ExpForLevel(level));

				// Do a similar thing here, but also check the personas.
				GameController.Instance.Variables.Personas
					.Where(p => p.metaData.name == combatantName)
					.ToList()
					.ForEach(p => {
						p.TotalEXP = p.ExpForLevel(level);
						Debug.LogWarning("Note that leveling the persona in this way may create unintended side effects if they still have moves that are learned upon level up.");
					});
			} catch (System.Exception e) {
				Debug.LogError("Error parsing SetLevel command.");
			}
		}
		/// <summary>
		/// Restores the entire party's HP/MP/SP.
		/// </summary>
		/// <param name="args"></param>
		private void RestoreParty(string[] args) {
			// I can set these to 9999 because I'm already clamping the HP/MP/SP upon being set.
			GameController.Instance.Variables.Players.ForEach(p => {
				p.HP = 9999;
				p.MP = 9999;
				p.SP = 9999;
			});
		}
		/// <summary>
		/// Sets a given attribute on a specified player.
		/// </summary>
		/// <param name="args"></param>
		private void SetAttribute(string[] args) {
			try {
				string partyMemberName = args[0];
				string resourceName = args[1];
				int newResourceValue; System.Int32.TryParse(args[2], out newResourceValue);

				switch (resourceName) {
					case "HP":
						GameController.Instance.Variables.Players.Find(p => p.metaData.name == partyMemberName).HP = newResourceValue;
						Debug.LogError("Remember to update the player status here.");
						// GameController.Instance.Variables.players.Find(p => p.metaData.name == partyMemberName).playerStatus.UpdateStatus();
						break;
					case "MP":
						GameController.Instance.Variables.Players.Find(p => p.metaData.name == partyMemberName).MP = newResourceValue;
						Debug.LogError("Remember to update the player status here.");
						// GameController.Instance.Variables.players.Find(p => p.metaData.name == partyMemberName).playerStatus.UpdateStatus();
						break;
					default:
						throw new System.Exception("Couldn't set attribute");
				}

			} catch (System.Exception e) {
				Debug.LogError("Error parsing SetAttribute command.");
			}
			
		}
		#endregion

		#region BEHAVIORS AND ITEMS
		/// <summary>
		/// Adds a set of items to the inventory.
		/// </summary>
		/// <param name="args"></param>
		private void AddItem(string[] args) {
			try {
				string itemName = args[0];
				int quantity; System.Int32.TryParse(args[1], out quantity);
				GameController.Instance.Variables.AddItemToInventory(itemName: itemName, quantity: quantity);
			} catch (System.Exception e) {
				Debug.LogError("Error parsing AddItem command.");
			}
		}
		/// <summary>
		/// Adds a battle behavior to the combatant in the specified slot.
		/// </summary>
		/// <param name="args"></param>
		private void AddMove(string[] args) {
			try {
				// Parse out the arguments that were provided.
				string combatantName = args[0];
				string behaviorName = args[1];
				int slot = -1; 
				// Check to see if the length is higher than two, because if one is not provided, the regular AddBehavior function should be used instead.
				if (args.Length > 2) {
					System.Int32.TryParse(args[2], out slot);
				}

				// Find the combatant. If they aren't found in the personas, find them in the players.
				Combatant combatant = GameController.Instance.Variables.Personas.Find(p => p.metaData.name == combatantName);
				if (combatant == null) { combatant = GameController.Instance.Variables.Players.Find(p => p.metaData.name == combatantName); }

				// Next, find the behavior that's going to be added.
				// BattleBehavior behavior = GameController.Instance.GetBattleBehavior(behaviorName: behaviorName);
				BattleBehavior behavior = DataController.Instance.GetBehavior(behaviorName: behaviorName);

				// Check to make sure the combatant is a player or a persona, because if it's a player, their persona should be used.
				if (combatant is Player) {
					Debug.LogWarning("The party themselves do not actually hold the moves they use. Will add to the persona they are using instead. (This may be what you were trying to do, anyway.)");
					combatant = ((Player)combatant).ActivePersona;
				}

				// If the slot is negative 1, that means nothing was passed in. Try adding the move.
				if (slot == -1) {
					combatant.AddBehavior(behavior: behavior);
				} else {
					// If the slot is not 1, that means the parse worked and I can add the behavior to the slot specified.
					combatant.AddBehavior(behavior: behavior, slot: slot);
				}

			} catch (System.Exception e) {
				Debug.LogError("Error parsing AddMove command.");
			}
		}
		#endregion

		#region MISSIONS AND SCENES
		/// <summary>
		/// Loads a scene.
		/// </summary>
		/// <param name="args"></param>
		private void LoadScene(string[] args) {
			try {
				SceneController.instance.LoadScene(sceneName: args[0]);
			} catch (System.Exception e) {
				Debug.LogError("Error parsing LoadScene command.");
			}
		}
		#endregion

		#region MISC
		private void GrandmotherLode(string[] args) {
			GameController.Instance.Variables.Money += 10000;
		}
		#endregion

		#region EVENTS
		/// <summary>
		/// Saves the GameObject that was last selected. Helpful for returning to the battle as I need to.
		/// </summary>
		public void OnOpen() {
			FindObjectOfType<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = true;
		}
		public void OnHide() {
			FindObjectOfType<Rewired.Integration.UnityUI.RewiredStandaloneInputModule>().allowMouseInput = false;
		}
		#endregion

		

	}


}