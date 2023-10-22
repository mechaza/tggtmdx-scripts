using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Sirenix.OdinInspector;
using Grawly.Battle.BattleMenu;
using Grawly.Battle.Equipment;

namespace Grawly.Battle {

	public class Player : Combatant {
		
		#region FIELDS - TEMPLATE
		/// <summary>
		/// The template used for this player. Mostly just so I can maintain references to sprites I may need.
		/// </summary>
		public PlayerTemplate playerTemplate { get; private set; }
		#endregion

		#region PROPERTIES - RESOURCES : MAX VALUES
		public override int MaxHP {
			get {
				return base.MaxHP;
				// return (this.Level + this.EN) * 6;
				// return this.GetAttributeValue((int)(this.Level * 0.5f), 120f, this.hpCeil);
			}
		}
		public override int MaxMP {
			get {
				return base.MaxMP;
				// return (this.Level + this.MA) * 3;
				// return this.GetAttributeValue((int)(this.Level * 0.5f), 40f, this.mpCeil);
			}
		}
		#endregion

		#region FIELDS - BATTLE : EQUIPMENT
		/// <summary>
		/// The weapon this player is currently using.
		/// </summary>
		public Weapon Weapon { get; private set; }
		#endregion
		
		#region FIELDS - BATTLE : PERSONA
		/// <summary>
		/// The Player's active persona.
		/// </summary>
		private Persona activePersona;
		/// <summary>
		/// The Player's active persona.
		/// </summary>
		public Persona ActivePersona {
			get {
				return this.activePersona;
			}
		}
		#endregion

		#region FIELDS - BATTLE : RESISTANCES AND BEHAVIORS
		/// <summary>
		/// Hide the base class resistances and get the ones which belong to the player's persona.
		/// </summary>
		public override Dictionary<ElementType, ResistanceType> Resistances {
			get {
				return activePersona.Resistances;
			}
			set {
				activePersona.Resistances = value;
			}
		}
		/// <summary>
		/// The Behaviors this player will be using.
		/// </summary>
		public override Dictionary<BehaviorType, List<BattleBehavior>> AllBehaviors {
			get {
				// This kinda sucks
				// playerBehaviors[BehaviorType.Special] = activePersona.AllBehaviors[BehaviorType.Special];

				// Concat the persona's behaviors with the behaviors available in the badge collection.
				playerBehaviors[BehaviorType.Special] = activePersona.AllBehaviors[BehaviorType.Special]
					// .Concat(this.BadgeCollection.ExtraBattleBehaviors.Where(bb=> bb.behaviorType == BehaviorType.Special))
					.ToList();
				return playerBehaviors;
			}
			set {
				activePersona.AllBehaviors = value;
			}
		}
		// Player behaviors belong to the player and not the persona.
		// E.x., tactics, attack, misc
		private Dictionary<BehaviorType, List<BattleBehavior>> playerBehaviors;
		/// <summary>
		/// How this player participates in tactics moves. I'll keep it for now.
		/// </summary>
		public TacticsParticipantType participantType { get {
				return this.playerTemplate.participantType;
			}
		}
		#endregion

		#region FIELDS - BATTLE : COMBATANTS
		/// <summary>
		/// All of the available opponents a player can have.
		/// </summary>
		public override List<Combatant> Opponents {
			get {
				return BattleController.Instance.Enemies.Cast<Combatant>().ToList();
			}
		}
		/// <summary>
		/// A list of the available allies for this player.
		/// </summary>
		public override List<Combatant> Allies {
			get {
				return BattleController.Instance.Players.Cast<Combatant>().ToList();
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The PlayerStatusDX that represents this player on the screen.
		/// </summary>
		public PlayerStatusDX PlayerStatusDX { get; private set; }
		#endregion

		#region FIELDS - SCENE REFERENCES : COMBATANT ANIMATOR
		/// <summary>
		/// The animator this Player should use. Except. It's just their status.
		/// </summary>
		public override ICombatantAnimator CombatantAnimator {
			get {
				return this.PlayerStatusDX;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public Player(PlayerTemplate template, GameVariables gameVariables) : base(template, gameVariables) {

			// Get the "correct" name for this player and update it inside the metadata.
			this.metaData.name = gameVariables.CharacterIDMap.GetName(characterIDType: template.characterIDType);

			// Save a ref to the template.
			this.playerTemplate = template;

			this.playerBehaviors = new Dictionary<BehaviorType, List<BattleBehavior>>();
			foreach (BehaviorType type in System.Enum.GetValues(typeof(BehaviorType))) {
				playerBehaviors[type] = new List<BattleBehavior>();
			}
			// Make sure to add the behaviors, Yeah ?
			foreach (BattleBehavior behavior in template.battleBehaviors) {
				playerBehaviors[behavior.behaviorType].Add(behavior);
			}

			this.AssignPlayerPersona(new Persona(template: template.personaTemplate, gameVariables: gameVariables));

			// Ask the PlayerTemplate what the default WeaponID is, then use that ID to ask the collection set for the real weapon.
			this.Weapon = gameVariables.WeaponCollectionSet.GetWeapon(weaponID: template.weaponTemplate.WeaponID);
			
			// Set the HP/MP/SP. I normally do this in Combatant, but I am doing it AFTER the persona is set here.
			this.HP = MaxHP;
			this.MP = MaxMP;
			this.SP = MaxSP;

			gameVariables.Personas.Add(this.ActivePersona);

		}
		public Player(SerializablePlayer sp, GameSaveLoaderController scd, GameVariables gameVariables) : base(sp, scd, gameVariables) {

			// Find things like sprites from the playertemplates located in the save controller data
			PlayerTemplate template = scd.GetPlayerTemplate(sp);

			// Get the "correct" name for this player and update it inside the metadata.
			this.metaData.name = gameVariables.CharacterIDMap.GetName(characterIDType: template.characterIDType);

			// Save a ref to the template.
			this.playerTemplate = template;

			this.playerBehaviors = new Dictionary<BehaviorType, List<BattleBehavior>>();
			// Make a new list for each behavior type.
			foreach (BehaviorType type in System.Enum.GetValues(typeof(BehaviorType))) {
				this.playerBehaviors[type] = new List<BattleBehavior>();
			}

			// Set the turn behavior
			this.TurnBehavior = template.TurnBehavior;

			foreach (string str in sp.behaviors) {
				BattleBehavior behavior = scd.GetBattleBehavior(str);
				playerBehaviors[behavior.behaviorType].Add(behavior);
			}

			this.AssignPlayerPersona(new Persona(sp.persona, scd, gameVariables));
			
			// Ask the PlayerTemplate what the default WeaponID is, then use that ID to ask the collection set for the real weapon.
			this.Weapon = gameVariables.WeaponCollectionSet.GetWeapon(weaponID: sp.weaponID);
			
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Gets the modifiers associated with this Player.
		/// The Player version takes their equipment into account as well.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public override List<T> GetModifiers<T>() {
			
			// Get the modifiers from the base.
			List<T> allModifiers = base.GetModifiers<T>();
			
			// Add on the odifiers from the equipment set.
			allModifiers.AddRange(this.Weapon.GetModifiers<T>());
			
			// Return em baby.
			return allModifiers;

		}
		#endregion
		
		#region FIELD ASSIGNMENTS
		/// <summary>
		/// Sets the active Persona to the specified Persona.
		/// </summary>
		/// <param name="persona">The Persona to assign.</param>
		public void AssignPlayerPersona(Persona persona) {
			Debug.Log(this.metaData.name + " is updating their persona to " + persona.metaData.name);
			this.activePersona = persona;
		}
		/// <summary>
		/// Assigns this player a player status.
		/// </summary>
		/// <param name="playerStatusDX">The PlayerStatusDX to assign.</param>
		public void AssignPlayerStatus(PlayerStatusDX playerStatusDX) {
			this.PlayerStatusDX = playerStatusDX;
		}
		#endregion

		#region LEVELING UP
		public override void ParseBattleResults(LegacyBattleResultsData battleResults) {
			base.ParseBattleResults(battleResults);
			// Also allow the persona to parse the results.
			ActivePersona.ParseBattleResults(battleResults);
		}
		/// <summary>
		/// Parses the battle results as a result of the template and variables involved.
		/// </summary>
		/// <param name="battleTemplate">The template of the battle that was just completed.</param>
		/// <param name="gameVariables">The GameVariables associated with the current game.</param>
		public override void ParseBattleResults(BattleTemplate battleTemplate, GameVariables gameVariables) {
			base.ParseBattleResults(battleTemplate, gameVariables);
			// Also allow the persona to parse the results.
			ActivePersona.ParseBattleResults(battleTemplate: battleTemplate, gameVariables: gameVariables);
		}
		#endregion

		#region MENUABLE OVERRIDE
		/// <summary>
		/// For the player, their icon should be their player status icon.
		/// </summary>
		public override Sprite Icon {
			get {
				return this.playerTemplate.playerStatusIcon;
			}
		}
		#endregion

		#region SERIALIZABLES
		[System.Serializable]
		public class SerializablePlayer : SerializableCombatant {
			public CharacterIDType characterIDType;
			public TacticsParticipantType participantType;
			public Persona.SerializablePersona persona;
			public WeaponID weaponID;
			public SerializablePlayer(Player player) : base(player) {
				this.characterIDType = player.playerTemplate.characterIDType;
				this.participantType = player.participantType;
				this.persona = new Persona.SerializablePersona(player.ActivePersona);
				this.weaponID = player.Weapon.WeaponID;
			}
		}
		#endregion

	}


}