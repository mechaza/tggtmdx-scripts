using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Battle {

	/// <summary>
	/// Encapsulates the turn order of the battle.
	/// </summary>
	public class CombatantTurnQueue {

		#region FIELDS - STATE
		/// <summary>
		/// In the event of a player/enemy advantage, this queue is populated with combatants of the related team.
		/// On exhausting this queue, the regular raw queue is used.
		/// </summary>
		private Queue<Combatant> advantageTurnQueue { get; set; } = new Queue<Combatant>();
		/// <summary>
		/// The "raw" turn queue that actually holds the turn order.
		/// </summary>
		private Queue<Combatant> rawTurnQueue { get; set; } = new Queue<Combatant>();
		/// <summary>
		/// The number of actions that the current combatant has taken.
		/// </summary>
		private int currentTurnActionCount { get; set; } = 0;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally clears the state of the turn queue.
		/// </summary>
		public void ResetState() {
			this.advantageTurnQueue.Clear();
			this.rawTurnQueue.Clear();
		}
		/// <summary>
		/// Prepares the turn queue to be used with the combatants provided.
		/// </summary>
		/// <param name="combatants">The combatants to prepare this queue with.</param>
		/// <param name="advantageType">The advantage type to prep this queue with..</param>
		public void Prepare(List<Combatant> combatants, BattleAdvantageType advantageType) {
			
			// Depending on the type of advantage, populate the advantage queue with the appropriate combatants.
			if (advantageType == BattleAdvantageType.Normal) {
				// Just give it a blank queue for the normal advantage.
				this.advantageTurnQueue = new Queue<Combatant>();
			} else if (advantageType == BattleAdvantageType.PlayerAdvantage) {
				// For player advantage, grab the players from the combatants list.
				this.advantageTurnQueue = new Queue<Combatant>(combatants
					.Where(c => c is Player)
					.OrderBy(c => c.DynamicAG)
					.Reverse()
					.ToList()
				);
			} else if (advantageType == BattleAdvantageType.EnemyAdvantage) {
				// For enemy advantage, grab the enemies from the combatants list.
				this.advantageTurnQueue = new Queue<Combatant>(combatants
					.Where(c => c is Enemy)
					.OrderBy(c => c.DynamicAG)
					.Reverse()
					.ToList());
			} else {
				throw new System.Exception("This should never be reached!");
			}
			
			// Now grab the raw turn queue.
			this.rawTurnQueue = new Queue<Combatant>(combatants.OrderBy(c => c.DynamicAG).Reverse());
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Gets the combatant who should be up this turn.
		/// </summary>
		/// <param name="lastCombatant">The combatant who went last turn.</param>
		/// <returns>The combatant who should go this turn.</returns>
		public Combatant GetNextCombatant(Combatant lastCombatant) {
			
			// If the last combatant was null, just dequeue and move on.
			if (lastCombatant == null) {
				return this.CycleTurnQueue(
					advantageQueue: this.advantageTurnQueue, 
					turnQueue: this.rawTurnQueue);
				
			} else {
				// Call the version of this function that also acknowledges how many actions the combatant has taken.
				return this.GetNextCombatant(
					lastCombatant: lastCombatant, 
					turnActionCount: this.currentTurnActionCount);
			}

		}
		/// <summary>
		/// Gets the combatant who should be up this turn.
		/// </summary>
		/// <param name="lastCombatant">The combatant who moved last.</param>
		/// <param name="turnActionCount">The number of actions the combatant has taken so far.</param>
		/// <returns>The combatant who is up next.</returns>
		private Combatant GetNextCombatant(Combatant lastCombatant, int turnActionCount) {
			
			// Check if the combatant's turn behavior allows for more actions.
			bool hasMoreActions = lastCombatant.TurnBehavior.HasMoreTurnActions(turnActionCount: turnActionCount);
			
			// If they have more actions, increment 
			if (hasMoreActions == true) {
				this.currentTurnActionCount += 1;
				return lastCombatant;
			} else {
				return this.CycleTurnQueue(
					advantageQueue: this.advantageTurnQueue, 
					turnQueue: this.rawTurnQueue);
			}
				
		}
		/// <summary>
		/// Removes the specified combatants from the turn queue so they can no longer participate in battle.
		/// </summary>
		/// <param name="combatantsToRemove">The combatants to remove from the turn queue.</param>
		public void RemoveCombatants(List<Combatant> combatantsToRemove) {
			// This is a bit. bad. Make a list from the turn queue and remove dead enemies coming up.
			List<Combatant> turnList = new List<Combatant>(this.rawTurnQueue);
			
			foreach (Combatant combatant in combatantsToRemove) {
				bool removed = turnList.Remove(combatant);
				if (removed == false) {
					throw new Exception("Tried to remove a combatant that wasn't actually in the queue. This is a logical error.");
				}
			}
			
			this.rawTurnQueue = new Queue<Combatant>(turnList);
		}
		#endregion

		#region QUEUE MANAGEMENT
		/// <summary>
		/// "Cycles" the turn queue by returning the next in line while also enqueing them again.
		/// </summary>
		/// <param name="advantageQueue">The queue that may have advantage ready combatants..</param>
		/// <param name="turnQueue">The turn queue to cycle.</param>
		/// <returns>The combatant who was next up.</returns>
		private Combatant CycleTurnQueue(Queue<Combatant> advantageQueue, Queue<Combatant> turnQueue) {
			
			Debug.Log("Cycling the turn queue.");
			this.currentTurnActionCount = 1;
			
			// Make sure to exhaust the advantage queue before using the regular turn queue.
			if (advantageQueue.Count > 0) {
				// If there are still combatants in the advantage queue, dequeue the next one.
				return advantageQueue.Dequeue();
				
			} else {
				// If no advantage combatants are left, just use the regular queue.
				Combatant nextCombatant = turnQueue.Dequeue();
				turnQueue.Enqueue(nextCombatant);
				return nextCombatant;
			}
			
		}
		#endregion
		
	}
	
}