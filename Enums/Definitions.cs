using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Grawly.Battle;
using Grawly.Toggles;

namespace Grawly.Battle {

	// Keeps track of the name/description/etc of the combatant.
	[System.Serializable]
	public struct Metadata {
		public string name;
		public string description;
	}

	// This is mostly for enemies. Contains the value of EXP/money they can drop.
	[System.Serializable]
	public struct Drops {
		public int exp;
		public int money;
		public List<BattleBehavior> items;
	}
	
	/// <summary>
	/// At the start of the battle, the different types of possible advantages
	/// </summary>
	public enum BattleAdvantageType {
		Normal,
		PlayerAdvantage,
		EnemyAdvantage,
	}
	
	#region BATTLE BEHAVIOR RELATED


	/// <summary>
	/// Primarily used for keeping track of attack/defense/accuracy boosts.
	/// </summary>
	public enum PowerBoostType {
		// The integers are used to multiply the Multiplier in SetPowerBoost
		Attack = 0,
		Defense = 1,
		Accuracy = 2,
	}

	/// <summary>
	/// Ways of identifying how a power boost should be used.
	/// </summary>
	public enum PowerBoostIntentionType {
		Buff = 0,
		Debuff = 1,
	}


	/// <summary>
	/// Certain moves hit only once, though others hit multiple times. 
	/// </summary>
	public enum HitFrequencyType {
		Once = 0,
		Multiple = 1,
		Range = 2,
	}

	/// <summary>
	/// Different types for moves and whatnot.
	/// </summary>
	public enum ElementType {
		Phys = 0,
		Gun = 1,
		Fire = 2,
		Ice = 3,
		Elec = 4,
		Wind = 5,
		Psi = 6,
		Nuke = 7,
		Light = 8,
		Curse = 9,
		Gold = 10,
		Healing = 11,
		Assist = 12,
		Passive = 13,
		Ailment = 14,
		None = 15,
		Clown = 16,
	}

	/// <summary>
	/// The different kinds of attributes a combatant will have.
	/// I'm mostly using this for toggle accessability.
	/// </summary>
	public enum AttributeType {
		ST = 0,
		MA = 1,
		EN = 2,
		AG = 3,
		LU = 4,
	}

	/// <summary>
	/// A very basic data structure to help me with automatically setting up combatant templates.
	/// </summary>
	public enum AttributeRankType {
		VeryLow = 80,
		Low = 85,
		Medium = 90,
		High = 95,
		VeryHigh = 100,
	}
	
	/// <summary>
	/// How a combatant could be resistant to any given element.
	/// </summary>
	public enum ResistanceType {
		Nm = 0,
		Wk = 1,
		Str = 2,
		Nul = 3,
		Ref = 4,
		Abs = 5,
	}
	
	/// <summary>
	/// Helpful in calculations of behaviors and displayin the correct animations.
	/// </summary>
	public enum AccuracyType {
		Normal = 0,
		Miss = 1,
		Critical = 2,
	}
	
	/// <summary>
	/// The different ways a behavior can target combatants.
	/// </summary>
	public enum TargetType {
		OneAliveEnemy = 0,
		OneAliveAlly = 1,
		OneDeadAlly = 2,
		AllAliveEnemies = 3,
		AllAliveAllies = 4,
		AllDeadAllies = 5,
		AllAliveCombatants = 6,
		Self = 7,
	}

	/// <summary>
	/// Used for sorting out the menu mostly.
	/// </summary>
	public enum BehaviorType {
		Attack = 0,
		Special = 1,
		Tactics = 2,
		Item = 3,
		Misc = 4,
		Passive = 5,    // Passive is the only behavior type not given a category menu
	}

	/// <summary>
	/// Used to figure out the pool from wich to affect a target's resources.
	/// </summary>
	public enum BehaviorCostType {
		HP = 0,
		MP = 1,
		ST = 2,
		None = 3,
	}
	/// <summary>
	/// This is really only used for the tactics menu as a means to figure out who participates in the move.
	/// </summary>
	public enum TacticsParticipantType {
		None = 0,
		D = 1,      // Dorothy
		S = 2,      // Sophia
		B = 3,      // Blanche
		R = 4,      // Rose
	}

	/// <summary>
	/// Determines how a behavior's "amt" gets calculated.
	/// </summary>
	public enum BattleCalculationType {
		Dynamic = 0,
		Percentage = 1,
		Static = 2,
		None = 3,
	}

	public enum BFXType {
		None				= 0,
		PlayerChargeUp		= 1001,
		DefaultAttack		= 1002,
		EnemyDeath			= 1003,
		FightCloud			= 1004,
		Buff				= 1005,
		Debuff				= 1006,
	}

	#endregion

	#region BATTLE RESULTS RELATED

	/// <summary>
	/// Contains info on when a persona learns a move.
	/// </summary>
	[Serializable]
	public struct LevelUpMove {
		public int level;
		public BattleBehavior behavior;
	}
	#endregion
}

namespace Grawly.Playstyle {
	/// <summary>
	/// An enum describing the playstyle of the game.
	/// </summary>
	public enum PlaystyleType {
		None		= 0,
		Standard	= 1,
		ThreeStrike	= 2,
	}
}

namespace Grawly {
	/// <summary>
	/// The difficulty for the game.
	/// </summary>
	public enum DifficultyType {
		VeryEasy		= 1,
		Easy			= 2,
		Normal			= 3,
		Hard			= 4,
		VeryHard		= 5,
	}
	
	
	/// <summary>
	/// A special kind of enum to be used with the difficulty toggles.
	/// </summary>
	public enum DifficultyOverrideType {
		Off				= 0,
		VeryEasy		= 1,
		Easy			= 2,
		Normal			= 3,
		Hard			= 4,
		VeryHard		= 5,
	}
	
	/// <summary>
	/// A basic class to help me organize my pallette if I ever need to change it.
	/// </summary>
	public static class GrawlyColors {

		#region COLOR SHORTCUTS
		public static Color Red => GrawlyColors.colorDict[GrawlyColorTypes.Red];
		public static Color Blue => GrawlyColors.colorDict[GrawlyColorTypes.Blue];
		public static Color Yellow => GrawlyColors.colorDict[GrawlyColorTypes.Yellow];
		public static Color Green => GrawlyColors.colorDict[GrawlyColorTypes.Green];
		public static Color Purple => GrawlyColors.colorDict[GrawlyColorTypes.Purple];
		public static Color White => GrawlyColors.colorDict[GrawlyColorTypes.White];
		public static Color Black => GrawlyColors.colorDict[GrawlyColorTypes.Black];
		public static Color Pink => GrawlyColors.colorDict[GrawlyColorTypes.Pink];
		#endregion
		
		public static Dictionary<GrawlyColorTypes, Color> colorDict = new Dictionary<GrawlyColorTypes, Color>();
		public static Dictionary<GrawlyColorTypes, Color32> color32Dict = new Dictionary<GrawlyColorTypes, Color32>();

		static GrawlyColors() {

			colorDict[GrawlyColorTypes.Red] = new Color(255.0f / 255.0f, 0.0f / 255.0f, 74.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.DarkRed] = new Color(77.0f / 255.0f, 0.0f / 255.0f, 22.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.Green] = new Color(0.0f / 255.0f, 255.0f / 255.0f, 23.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.DarkGreen] = new Color(0.0f / 255.0f, 75.0f / 255.0f, 7.0f / 255.0f, 1.0f);
			// colorDict[GrawlyColorTypes.Blue] = new Color(0.0f / 255.0f, 255.0f / 255.0f, 244.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.Blue] = new Color(0.0f / 255.0f, 157.0f / 255.0f, 255.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.Yellow] = new Color(255.0f / 255.0f, 164.0f / 255.0f, 0.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.Purple] = new Color(255.0f / 255.0f, 0.0f / 255.0f, 237.0f / 255.0f, 1.0f);
			colorDict[GrawlyColorTypes.White] = new Color(1f, 1f, 1f, 1.0f);
			colorDict[GrawlyColorTypes.Black] = new Color(0f, 0f, 0f, 1.0f);
			colorDict[GrawlyColorTypes.Pink] = new Color(255.0f / 255.0f, 66.0f / 255.0f, 118.0f / 255.0f, 1.0f);
			
			
			color32Dict[GrawlyColorTypes.Red] = new Color32(255, 0, 74, 1);
			color32Dict[GrawlyColorTypes.DarkRed] = new Color32(77, 0, 22, 1);
			color32Dict[GrawlyColorTypes.Green] = new Color32(0, 255, 23, 1);
			color32Dict[GrawlyColorTypes.DarkGreen] = new Color32(0, 75, 7, 1);
			// color32Dict[GrawlyColorTypes.Blue] = new Color32(0, 255, 244, 1);
			color32Dict[GrawlyColorTypes.Blue] = new Color32(0, 157, 255, 1);
			color32Dict[GrawlyColorTypes.Yellow] = new Color32(255, 164, 0, 1);
			color32Dict[GrawlyColorTypes.Purple] = new Color32(255, 0, 237, 1);
			color32Dict[GrawlyColorTypes.White] = new Color32(255, 255, 255, 1);
			color32Dict[GrawlyColorTypes.Black] = new Color32(0, 0, 0, 1);
			color32Dict[GrawlyColorTypes.Pink] = new Color32(255, 66,118, 1);

		}

		/// <summary>
		/// Gets the hex value from a color.
		/// http://answers.unity3d.com/questions/1102232/how-to-get-the-color-code-in-rgb-hex-from-rgba-uni.html
		/// </summary>
		public static string ToHexFromRGB(Color c) {
			return string.Format("{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
			// return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
		}
		/// <summary>
		/// Returns a string to be used in a SuperTextMesh to adjust the color.
		/// </summary>
		/// <param name="c">The color to change a text mesh.</param>
		/// <returns>The prefix required to make this color work.</returns>
		public static string STMPrefix(Color c) {
			return "<c=" + ToHexFromRGB(c) + ">";
		}
		private static byte ToByte(float f) {
			f = Mathf.Clamp01(f);
			return (byte)(f * 255);
		}
		/// <summary>
		/// Gets a color from the speciifed category type.
		/// </summary>
		/// <param name="categoryType"></param>
		/// <returns></returns>
		public static Color GetColorFromToggleCategory(GameToggleCategoryType categoryType) {
			switch (categoryType) {
				case GameToggleCategoryType.Display:
					return GrawlyColors.colorDict[GrawlyColorTypes.Yellow];
				case GameToggleCategoryType.Audio:
					return GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				case GameToggleCategoryType.Gameplay:
					return GrawlyColors.colorDict[GrawlyColorTypes.Green];
				case GameToggleCategoryType.Debug:
					return GrawlyColors.colorDict[GrawlyColorTypes.Red];
					
				default:
					// Debug.LogWarning("There is no color associated with this category!");
					return Color.black;

			}
		}
		/// <summary>
		/// Gets a color associated with the given attribute type.
		/// </summary>
		/// <param name="attributeType">The attribute type associated with the desired color.</param>
		/// <returns>The color associated with the provided attribute type.</returns>
		public static Color GetColorFromAttributeType(AttributeType attributeType) {
			switch (attributeType) {
				case AttributeType.ST:
					return GrawlyColors.Red;
				case AttributeType.MA:
					return GrawlyColors.Blue;
				case AttributeType.EN:
					return GrawlyColors.Purple;
				case AttributeType.AG:
					return GrawlyColors.Yellow;
				case AttributeType.LU:
					return GrawlyColors.Green;
				default:
					throw new NotImplementedException("Could not determine color to return from attribute " + attributeType.ToString());
			}
		}
	}

	public enum GrawlyColorTypes {
		Red = 0,
		Green = 1,
		Blue = 2,
		Yellow = 3,
		Purple = 4,
		DarkRed = 5,
		DarkGreen = 6,
		DarkBlue = 7,
		White = 8,
		Black = 9,
		Pink = 10,
	}

	/// <summary>
	/// Just for helping out with the DataController in managing what assets I'm loading.
	/// </summary>
	public enum ResourceType {
		Video,
		Music,
		SFX,
		BattleFX,
		GameObject,
		Other,
	}

	/// <summary>
	/// Used in the AudioController.
	/// I don't want SFXs to be dependent on their filenames in case I have to change them.
	/// This is only really used to see if I have a "default" sfx for this type. I will not be
	/// using it for EVERY SFX.
	/// </summary>
	public enum SFXType {


		None = 0,

		Hover = 1000,
		Select = 1001,
		Close = 1002,
		Invalid = 1003,
		PromptOpen = 1004,
		Pause = 1050,

		EnemyNotice = 1500,         // When the enemy makes the grunt sound.
		EnemyEncounter = 1501,          // When the player touches an enemy on the field.
		EnemySpawn = 1502,          // When the enemy goes POOF onto the field.

		PlayerAttack = 2001,            // This is the SFX that gets played on the flash animation before a player actually attacks.
		PlayerExploit = 2002,           // This is the SFX for when a player exploits a weakness. 
		PlayerBattleMenu = 2003,            // This is the SFX that plays when a player begins their turn.
		AllOutAttack = 2004,            // The all out attack sfx.
		DefaultAttack = 2005,          // This is what should be played when no sfx has been found for a given move.
		PAttackInterrupted = 2006,          // This is what should be played when a player's attack is interrupted.

		PhysAttack1 = 3001,
		GunAttack1 = 3051,
		FireAttack1 = 3101,
		IceAttack1 = 3151,
		ElecAttack1 = 3201,
		WindAttack1 = 3251,
		PsyAttack1 = 3301,
		NukeAttack1 = 3351,
		LightAttack1 = 3401,
		CureAttack1 = 3451,
		GoldAttack1 = 3501,
		HealingMove1 = 3551,
		AssistMove1 = 3601,
		DebuffMove1 = 3602,
		PassiveMove1 = 3651,
		AilmentAttack1 = 3701,
		
		ShuffleTime = 5001,
		
		CastleDoorOpen = 10001,

	}

	/// <summary>
	/// Similar to SFXType.
	/// </summary>
	public enum MusicType {
		None			= 0,
		Dungeon			= 1,
		Battle			= 2,
		BattleResults	= 3,
		ShuffleTime		= 4,
		
		Reasoning		= 101,
		
	}

	/// <summary>
	/// The different states that the audio controller can set a lock to which ignores music playback requests.
	/// </summary>
	public enum MusicPlaybackLockType {
		Locked		= 0,
		Unlocked	= 1,
	}
	
	/// <summary>
	/// A list of different possible directions that are relevant in game.
	/// </summary>
	public enum DirectionType {
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
	}

}
namespace Grawly.Overworld {

	/// <summary>
	/// Anything that implements ICursorInteractable is a thing that the OverworldCursor can interact with.
	/// </summary>
	public interface ICursorInteractable {
		/// <summary>
		/// The name to be used to refer to this interactable.
		/// </summary>
		/// <returns></returns>
		string GetInteractableName();
		void OnCursorEnter();
		void OnCursorExit();
		void OnCursorSubmit();
		Vector2 GetGravitationPoint();
	}

}
namespace Grawly.Dungeon {

	/// <summary>
	/// Something that implements IPlayerInteractable can be interacted with inside of a dungeon.
	/// Think chests, puzzle switches, etc. Something that might display a prompt.
	/// </summary>
	public interface IPlayerInteractable {
		string GetInteractableName();   // The name to be shown (may be blank in some cases)
		void PlayerEnter();             // When the object enters the player trigger
		void PlayerExit();              // When the object exits the player trigger
		void PlayerInteract();          // When the player hits the "submit" button to interact with the object
	}

	
	
	/// <summary>
	/// Something that implements this should respond to the player entering inside it.
	/// This is different from the player's front trigger.
	/// Things that need to be "interacted" with should implement IPlayerInteractable.
	/// </summary>
	public interface IPlayerCollisionHandler {
	
		/// <summary>
		/// Gets called when the DungeonPlayer enters a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		void OnPlayerCollisionEnter(DungeonPlayer dungeonPlayer);
		/// <summary>
		/// Gets called when the DungeonPlayer exits a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		void OnPlayerCollisionExit(DungeonPlayer dungeonPlayer);
		/// <summary>
		/// Gets called when the DungeonPlayer simply stays in an area.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		void OnPlayerCollisionStay(DungeonPlayer dungeonPlayer);
	}

	/// <summary>
	/// The state of the DungeonController's FSM
	/// </summary>
	public enum DungeonControllerStateType {
		Dungeon,
		Battle,
		Results,
		Pause,
		ERROR,
	}
	
	/// <summary>
	/// DungeonPlayer has a method to convert its FSM state into this enum.
	/// </summary>
	public enum DungeonPlayerStateType {
		Free = 0,
		Wait = 1,
		ERROR = 2,          // This isn't actually a type in the FSM. It's for GetFSMState()
		CameraOnly = 3,     // Lock input, but allow for camera movement.
	}

	public enum DungeonEnemyStateType {
		Docile,
		Alert,
		ERROR,          // This isn't actually a type in the FSM. It's for GetFSMState()
	}

}
namespace Grawly.Mission {

	/// <summary>
	/// The kind of mission.
	/// </summary>
	public enum MissionType {
		Dungeon,
		Minigame,
		Other,
	}

	/// <summary>
	/// The different kinds of difficulty.
	/// </summary>
	public enum MissionDifficultyType {
		Normal = 0,
		Hard = 1,
		Golden = 2,
	}

}
namespace Grawly.Story {
	/// <summary>
	/// The different types of day.
	/// </summary>
	public enum LegacyTimeOfDayType {
		Morning,
		Daytime,
		Afternoon,
		Evening,
	}

}