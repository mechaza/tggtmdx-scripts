using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;

namespace Grawly.Dungeon.Legacy {

	public class DungeonEnemyBody : MonoBehaviour, IPlayerInteractable, IPlayerCollisionHandler {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The actual enemy this body is attached to.
		/// </summary>
		[SerializeField]
		private DungeonEnemy enemy;
		#endregion

		#region UNITY CALLS
		private void Update() {
			// Bob the enemy
			float x = Mathf.Cos(Time.time * enemy.bobAmplitude) * enemy.bobDistance;
			float y = Mathf.Sin(Time.time * enemy.bobAmplitude) * enemy.bobDistance;
			float z = Mathf.Sin(Time.time * enemy.bobAmplitude) * enemy.bobDistance;
			transform.localPosition = new Vector3(x, y, z);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IPLAYERCOLLISIONHANDLER
		/// <summary>
		/// Gets called when the DungeonPlayer enters a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionEnter(DungeonPlayer dungeonPlayer) {
			
			// If the player is free...
			if (dungeonPlayer.CurrentState == DungeonPlayerStateType.Free) {
				// ...grab the Battle Template from the enemy this body is attached to...
				BattleTemplate battleTemplate = this.GetDungeonEnemy().battleTemplate;
				// ...and initiate a battle.
				BattleController.Instance.StartBattle(battleTemplate: battleTemplate);
			}
			
		}
		/// <summary>
		/// Gets called when the DungeonPlayer exits a special collision.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionExit(DungeonPlayer dungeonPlayer) {
			
		}
		/// <summary>
		/// Gets called when the DungeonPlayer simply stays in an area.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who just collided.</param>
		public void OnPlayerCollisionStay(DungeonPlayer dungeonPlayer){
		
		}
		#endregion
		
		#region PLAYER INTERACTABLE IMPLEMENTATIONS
		public void PlayerEnter() {

		}
		public void PlayerExit() {

		}
		public void PlayerInteract() {
			/*Debug.Log("Interacted with enemy body");
			// If the enemy is not chasing the player when they interact with it (see: Attack) enter a PlayerAdvantage state.
			if (enemy.GetFSMState() == DungeonEnemyStateType.Docile) {
				BattleController.encounterState = BattleEncounterStateType.PlayerAdvantage;
				// BattleController.dungeonEnemy = GetDungeonEnemy();
				// DungeonController.Instance.fsm.SendEvent("Enemy Encounter");
				throw new System.Exception("Don't use dungeon enemy in battle controller anymore.");
				DungeonController.Instance.SetFSMState(DungeonControllerStateType.Battle);
			}*/
		}
		public string GetInteractableName() {
			return "";
		}
		#endregion

		#region OTHER
		/// <summary>
		/// Returns the DungeonEnemy assocaited with the body. This is called when the player collides with the body.
		/// </summary>
		/// <returns></returns>
		public DungeonEnemy GetDungeonEnemy() {
			return enemy;
		}
		#endregion
		
		
	}

}