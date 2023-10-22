using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using Grawly.Battle;
using Invector.vCharacterController;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.AI;

namespace Grawly.Dungeon.Enemy {
	
	/// <summary>
	/// A simple enemy that can use a nav mesh agent.
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	public class NavAgentEnemy : DungeonEnemyDX {
		
		#region PROPERTIES - STATE
		/// <summary>
		/// The current state of this enemy.
		/// </summary>
		public override DungeonEnemyStateType CurrentState {
			get {
				throw new NotImplementedException("ADD THIS");
			}
		}
		/// <summary>
		/// Should the DungeonPlayer be locked upon invokation of this handler?
		/// The case here should be Yes because upon attacking, a battle should be initated.
		/// </summary>
		public override bool LockDungeonPlayer => true;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The NavMeshAgent controlling this enemy.
		/// </summary>
		private NavMeshAgent Agent { get; set; }
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.Agent = this.GetComponent<NavMeshAgent>();
		}
		/*private void Update() {
			this.Agent.SetDestination(target: DungeonPlayer.Instance.transform.position);
		}*/
		#endregion
		
		#region INTERFACE IMPLEMENTATION - IPLAYERATTACKHANDLER
		/// <summary>
		/// The function that should be called when a dungeon player attacks this enemy.
		/// </summary>
		/// <param name="dungeonPlayer">The DungeonPlayer who initated the attack.</param>
		public override void OnDungeonPlayerAttack(DungeonPlayer dungeonPlayer) {
			throw new NotImplementedException("ADD THIS");
		}
		#endregion
		
	}
}