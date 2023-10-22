using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;
using Grawly.Battle;
using Grawly.UI;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// This is what I'm using to keep track of the generated dungeon and where calls should be made to like. Remake it and shit.
	/// I'm tired. I hope this works.
	/// </summary>
	
	public class ProceduralDungeonController : MonoBehaviour {

		public static ProceduralDungeonController instance { get; private set; }

		#region FIELDS - STATE
		/// <summary>
		/// The number of the current floor.
		/// This is not saved long term.
		/// </summary>
		[SerializeField]
		private int currentFloorNumber = 1;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The DungeonTemplate currently in use.
		/// </summary>
		[SerializeField]
		private DungeonTemplate dungeonTemplate;
		#endregion

		#region FIELDS - SCENE REFERENCES : RUNTIME DUNGEON
		/// <summary>
		/// The RuntimeDungeon that generates the bullshit.
		/// </summary>
		// private RuntimeDungeon runtimeDungeon;
		/// <summary>
		/// The root transform for the dungeon.
		/// I need this *here* so I can build overrides properly.
		/// </summary>
		[SerializeField]
		private Transform dungeonRoot;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
			throw new System.NotImplementedException("There is no more DunGen.");
			// this.runtimeDungeon = this.GetComponent<RuntimeDungeon>();
		}
		private void Start() {
			NotificationController.Instance?.RebuildAreaLabel(text: this.dungeonTemplate.DungeonName + " " + this.currentFloorNumber + "F");
			this.RelocatePlayerToStartPosition();
		}
		#endregion

		#region DUNGEON GENERATION
		/// <summary>
		/// Relocates the player to the Start.
		/// </summary>
		private void RelocatePlayerToStartPosition() {
			DungeonPlayer.Instance.Relocate(pos: DungeonStartPoint.Instance.transform);
		}
		/// <summary>
		/// Gets called when the player interacts with a GeneratedDungeonStairs.
		/// </summary>
		/// <param name="floorIncrement">The number of floors to traverse.</param>
		[Button]
		public void ProceedThroughStairs(int floorIncrement = 1) {

	
			
			// Increment the floor count. Remember if this is the first time this floor has been encountered.
			bool arrivingToFloorFirstTime = this.IncrementFloorCount(floorIncrement: floorIncrement);

			// Set the player to wait.
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
			// Fade out.
			Grawly.UI.Legacy.Flasher.instance.FadeOut(color: Color.black, fadeTime: 0.5f);

			// Wait one second...
			GameController.Instance.WaitThenRun(timeToWait: 1f, action: delegate {
				
				// If there is an override for this floor, use that.
				if (this.dungeonTemplate.HasFloorOverride(floor: this.currentFloorNumber) == true) {
					throw new System.NotImplementedException("There is no more DunGen.");
					// this.runtimeDungeon.Generator.Clear(stopCoroutines: false);
					GameObject obj = Instantiate(original: this.dungeonTemplate.GetFloorOverride(this.currentFloorNumber), parent: this.dungeonRoot);
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localScale = Vector3.one;
					obj.transform.rotation = Quaternion.identity;

				} else {
					throw new System.NotImplementedException("There is no more DunGen.");
					// this.runtimeDungeon.Generate();
				}
			
				this.RelocatePlayerToStartPosition();
				// Wait an additional two seconds and then set the player free and fade back in.
				GameController.Instance.WaitThenRun(timeToWait: 2f, action: delegate {
					DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
					Grawly.UI.Legacy.Flasher.instance.FadeIn(fadeTime: 0.5f);
					// Signal that the floor has been arrived on.
					this.ArrivedOnFloorEvent(floorNumber: this.currentFloorNumber, firstTimeOnThisFloor: arrivingToFloorFirstTime);
				});
			});
		}
		#endregion

		#region ASSET GETTERS
		/// <summary>
		/// Gets a random battle template appropriate for the current floor.
		/// </summary>
		/// <returns>A battle template for the current floor.</returns>
		public BattleTemplate GetBattleTemplate() {
			return this.dungeonTemplate.GetRandomBattleTemplateForFloor(currentFloor: this.currentFloorNumber);
		}
		public DungeonTreasureRange GetDungeonTreasureRange() {
			return this.dungeonTemplate.GetRandomTreasureForFloor(currentFloor: this.currentFloorNumber);
		}
		/// <summary>
		/// Gets a random treasure set appropriate for the current floor.
		/// </summary>
		/// <returns>The treasure for the current floor.</returns>
		public List<BattleBehavior> GetTreasureBehaviors() {
			return this.GetDungeonTreasureRange().TreasureBattleBehaviors;
			// return this.dungeonTemplate.GetRandomTreasureForFloor(currentFloor: this.currentFloorNumber).TreasureBattleBehaviors;
		}
		#endregion

		#region DUNGEON EVENTS
		/// <summary>
		/// The event that gets called when the floor is arrived on.
		/// </summary>
		/// <param name="floorNumber">The current floor.</param>
		private void ArrivedOnFloorEvent(int floorNumber, bool firstTimeOnThisFloor) {

			NotificationController.Instance?.RebuildAreaLabel(text: this.dungeonTemplate.DungeonName + " " + this.currentFloorNumber + "F");
			
			// If this is the first time the player has been on this floor, run the FirstTime behaviors.
			if (firstTimeOnThisFloor == true) {
				this.dungeonTemplate
				.GetFloorBehaviors<IOnFloorFirstTime>(floor: floorNumber)
				.FirstOrDefault()
				?.OnFloorFirstTime();
			}
			
		}
		#endregion

		#region FLOOR MANAGEMENT
		/// <summary>
		/// Increments the local floor count by the specified amount.
		/// </summary>
		/// <param name="floorIncrement">The amount to increment the floor count by.</param>
		/// <returns>Whether or not this is the first time this floor is being reached or not.</returns>
		private bool IncrementFloorCount(int floorIncrement) {
			Debug.LogError("THERE IS AN ISSUE WITH MAKING SURE REVISITING THE HIGHEST FLOOR IS TAKEN INTO ACCOUNT. FIX IT.");
			this.currentFloorNumber += floorIncrement;
			// Signal to the variables that the floor was just changed. Return if it was the highest or not.
			return GameController.Instance.Variables.SignalDungeonFloorLevel(
				dungeonIDType: DungeonIDType.Castle,
				currentFloor: this.currentFloorNumber);
		}
		#endregion

	}


}