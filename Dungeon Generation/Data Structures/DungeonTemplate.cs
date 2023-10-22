using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.Serialization;
using Grawly.Battle;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// Used for construction of a generated dungeon. Or not generated.
	/// I just need something to design floors and whatnot.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Dungeon/Dungeon Template")]
	public class DungeonTemplate : SerializedScriptableObject {

		#region FIELDS - METADATA
		/// <summary>
		/// The name of this dungeon.
		/// </summary>
		[Title("General"), SerializeField]
		public string DungeonName = "";
		/// <summary>
		/// The ID for this dungeon.
		/// </summary>
		[SerializeField]
		public DungeonIDType dungeonIDType = DungeonIDType.Castle;
		/// <summary>
		/// The number of floors in this dungeon.
		/// </summary>
		[SerializeField]
		public int floorCount = 20;
		#endregion

		#region FIELDS - FLOOR OVERRIDES
		/// <summary>
		/// Contains prefabs that can get spawned when the given floor is reached.
		/// </summary>
		[Title("Overrides"), OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Floor", ValueLabel = "Prefab"), ListDrawerSettings(Expanded = true)]
		private Dictionary<int, GameObject> dungeonFloorOverrideDict = new Dictionary<int, GameObject>();
		#endregion

		#region FIELDS - FLOOR BEHAVIORS
		/// <summary>
		/// Contains the floor behaviors that should be dispatched on any given floor.
		/// </summary>
		[Title("Floor Behaviors"), OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Floor", ValueLabel = "Behaviors"), ListDrawerSettings(Expanded = true)]
		private Dictionary<int, List<DungeonFloorBehavior>> dungeonFloorBehaviorsDict = new Dictionary<int, List<DungeonFloorBehavior>>();
		#endregion

		#region FIELDS - BATTLES
		/// <summary>
		/// Should battle template ranges be used, or can battles just be picked from a list?
		/// </summary>
		[Title("Battle Templates"), SerializeField]
		private bool UseRangedBattleTemplates = true;
		/// <summary>
		/// A list of BattleTemplates and corresponding ranges that they can appear on.
		/// </summary>
		[OdinSerialize, ShowIf("UseRangedBattleTemplates")]
		private List<BattleTemplateRange> battleTemplateRanges = new List<BattleTemplateRange>();
		/// <summary>
		/// A list of battle templates to use if ranges are not in effect.
		/// </summary>
		/// <returns></returns>
		[OdinSerialize, HideIf("UseRangedBattleTemplates")]
		private List<BattleTemplate> battleTemplateList = new List<BattleTemplate>();
		#endregion

		#region FIELDS - TREASURE
		/// <summary>
		/// The treasures that can appear in this dungeon, and the range of floors they may appear on.
		/// </summary>
		[Title("Treasure"), OdinSerialize]
		private List<DungeonTreasureRange> dungeonTreasureRanges = new List<DungeonTreasureRange>();
		#endregion

		#region GETTERS - FLOORS
		/// <summary>
		/// Is there a floor override for the given floor?
		/// </summary>
		/// <param name="floor"></param>
		/// <returns></returns>
		public bool HasFloorOverride(int floor) {
			// Return true or false depending on if the key exists.
			return this.dungeonFloorOverrideDict.ContainsKey(floor);
		}
		/// <summary>
		/// Returns the prefab to build for the given floor.
		/// </summary>
		/// <param name="floor">THe floor number containing the override.</param>
		/// <returns>The prefab to build.</returns>
		public GameObject GetFloorOverride(int floor) {
			return this.dungeonFloorOverrideDict[floor];
		}
		#endregion
		
		#region GETTERS - BATTLES
		/// <summary>
		/// Gets the battle templates that can be used on this floor.
		/// </summary>
		/// <param name="currentFloor">The current floor the player is on.</param>
		/// <returns>A list of battle templates that can be picked from at random.</returns>
		public List<BattleTemplate> GetBattleTemplatesForFloor(int currentFloor) {

			// If not using ranges, just return the other list.
			if (this.UseRangedBattleTemplates == false) {
				return this.battleTemplateList;
			}
			
			List<BattleTemplate> templates = this.battleTemplateRanges											// Go through all the ranges...
				.Where(btr => btr.CheckInRange(currentFloor: currentFloor) == true)		// ... and check for ranges that contain this floor.
				.Select(btr => btr.BattleTemplate)										// Grab their templates.
				.ToList();
			Debug.Log("Possible templates: " + templates.Count);
			return templates;
		}
		/// <summary>
		/// Gets a random battle template for the given floor.
		/// </summary>
		/// <param name="currentFloor">The floor the player is currently on.</param>
		/// <returns>A BattleTemplate that is on this floor.</returns>
		public BattleTemplate GetRandomBattleTemplateForFloor(int currentFloor) {
			return this.GetBattleTemplatesForFloor(currentFloor).Random();
		}
		#endregion

		#region GETTERS - TREASURES
		/// <summary>
		/// Gets a treasure range for the specified floor.
		/// Note that a chest may have more than one item, which is why they're stored in the range.
		/// </summary>
		/// <param name="currentFloor">The floor the player is currently on.</param>
		/// <returns>A DungeonTreasureRange containing a list of items and what to tell the player it is.</returns>
		public DungeonTreasureRange GetRandomTreasureForFloor(int currentFloor) {
			return this.GetDungeonTreasureRanges(currentFloor: currentFloor).Random();
		}
		/// <summary>
		/// Gets the dungeon treasure ranges that are allowed for this given floor.
		/// </summary>
		/// <param name="currentFloor">The current floor the player is on.</param>
		/// <returns>A list of treasure ranges which may contain one or more BattleBehaviors in any individual range.</returns>
		public List<DungeonTreasureRange> GetDungeonTreasureRanges(int currentFloor) {
			return this.dungeonTreasureRanges
				.Where(dt => dt.CheckInRange(currentFloor: currentFloor) == true)
				.ToList();
		}
		#endregion

		#region BEHAVIOR GETTERS
		/// <summary>
		/// Get all the floor behaviors for the given floor of the givent ype.
		/// </summary>
		/// <typeparam name="T">The type of floor behavior to look for.</typeparam>
		/// <param name="floor">The current floor.</param>
		/// <returns>A list of floor behaviors that have the given interface implemented.</returns>
		public List<T> GetFloorBehaviors<T>(int floor) {
			// Check if the key exists, period.
			if (this.dungeonFloorBehaviorsDict.ContainsKey(key: floor)) {
				// If it does, find any behaviors that implement the given type and return the list.
				return this.dungeonFloorBehaviorsDict[floor].Where(b => b is T).Cast<T>().ToList();
			} else {
				// If the key does not exist, just return an empty list.
				return new List<T>();
			}
		}
		#endregion

	}


}