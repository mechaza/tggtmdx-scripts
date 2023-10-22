using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

namespace Grawly.Battle {

	/// <summary>
	/// A class that evaluates a queue of battle reactions one by one and then triggers the next event when it is done.
	/// </summary>
	public class BattleReactionSequence {

		#region FIELDS - STATE
		/// <summary>
		/// A linked list for holding the battle reactions that get evaluated.
		/// </summary>
		private Queue<BattleReaction> battleReactionQueue = new Queue<BattleReaction>();
		/// <summary>
		/// The callback to run when the sequence is over.
		/// </summary>
		private Action finishCallback;
		#endregion

		#region SEQUENCE PREP
		/// <summary>
		/// Preps the sequence for use.
		/// </summary>
		/// <param name="defaultFinishCallback">The callback to run when the sequence is complete.</param>
		public void Prepare(Action defaultFinishCallback) {
			Debug.Assert(this.battleReactionQueue.Count == 0);
			Debug.Assert(this.finishCallback == null);
			// this.finishEvent = defaultFinishEvent;
			this.finishCallback = defaultFinishCallback;
		}
		/// <summary>
		/// Adds a list of reactions to the sequence.
		/// </summary>
		/// <param name="battleReactions"></param>
		public void AddToSequence(List<BattleReaction> battleReactions) {
			// Goes through each reaction and adds it to the queue.
			battleReactions.ForEach(br => this.battleReactionQueue.Enqueue(item: br));
		}
		/// <summary>
		/// Appends a battle reaction to the sequence.
		/// </summary>
		/// <param name="battleReaction">The reaction to evaluate.</param>
		public void AddToSequence(BattleReaction battleReaction) {
			this.battleReactionQueue.Enqueue(item: battleReaction);
		}
		/// <summary>
		/// Clears out all the reactions in the sequence.
		/// </summary>
		public void ClearSequence() {
			this.battleReactionQueue.Clear();
		}
		#endregion

		#region SEQUENCE PLAYING	
		/// <summary>
		/// Executes the next reaction in the sequence.
		/// Will call the Finish() event if there is nothing left.
		/// </summary>
		public void ExecuteNextReaction() {
			// If there are still reactions to execute, do so.
			if (this.battleReactionQueue.Count > 0) {
				// Dequeue it and pass the reaction sequence as a param.
				this.battleReactionQueue.Dequeue()(battleReactionSequence: this);
			} else {
				this.Finish();
			}
		}
		/// <summary>
		/// Finishes up the sequence.
		/// </summary>
		public void Finish() {
			Debug.Log("BATTLE: BattleReactionSequence finished. Running finish event.");
			Debug.Assert(this.finishCallback != null);
			this.finishCallback.Invoke();
			// this.finishCallback();
			// Nullify the finish callback just in case.
			this.finishCallback = null;
		}
		#endregion

	}

	/// <summary>
	/// The types of battle reaction sequences there are. 
	/// I mostly just use this for easily setting up a dictionary of BattleReactionSequences.
	/// </summary>
	public enum BattleReactionSequenceType {

		BattleStart 			= 0,
		PreTurn 				= 1,
		TurnStart				= 2,
		TurnReady 				= 3,
		BehaviorEvaluated		= 4,
		TurnEnd 				= 5,

		

	}


}