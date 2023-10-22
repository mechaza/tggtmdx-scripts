using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

namespace Grawly.Gauntlet {

	/// <summary>
	/// A class that evaluates a queue of gauntlet reactions one by one and then triggers the next event when it is done.
	/// </summary>
	public class GauntletReactionSequence {

		#region FIELDS - STATE
		/// <summary>
		/// A queue for holding gauntlet reactions or whatever.
		/// </summary>
		private Queue<GauntletReaction> gauntletReactionQueue = new Queue<GauntletReaction>();
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
			// this.finishEvent = defaultFinishEvent;
			this.finishCallback = defaultFinishCallback;
		}
		/// <summary>
		/// Adds a list of reactions to the sequence.
		/// </summary>
		/// <param name="gauntletReactions"></param>
		public void AddToSequence(List<GauntletReaction> gauntletReactions) {
			// Goes through each reaction and adds it to the queue.
			gauntletReactions.ForEach(br => this.gauntletReactionQueue.Enqueue(item: br));
		}
		/// <summary>
		/// Appends a gauntlet reaction to the sequence.
		/// </summary>
		/// <param name="gauntletReaction">The reaction to evaluate.</param>
		public void AddToSequence(GauntletReaction gauntletReaction) {
			this.gauntletReactionQueue.Enqueue(item: gauntletReaction);
		}
		/// <summary>
		/// Clears out all the reactions in the sequence.
		/// </summary>
		public void ClearSequence() {
			this.gauntletReactionQueue.Clear();
		}
		#endregion

		#region SEQUENCE PLAYING	
		/// <summary>
		/// Executes the next reaction in the sequence.
		/// Will call the Finish() event if there is nothing left.
		/// </summary>
		public void ExecuteNextReaction() {
			// If there are still reactions to execute, do so.
			if (this.gauntletReactionQueue.Count > 0) {
				// Dequeue it and pass the reaction sequence as a param.
				this.gauntletReactionQueue.Dequeue()(gauntletReactionSequence: this);
			} else {
				this.Finish();
			}
		}
		/// <summary>
		/// Finishes up the sequence.
		/// </summary>
		public void Finish() {
			// Debug.Log("GAUNTLET: GauntletReactionSequence finished. Running finish event.");
			this.finishCallback();
			// Nullify the finish callback just in case.
			this.finishCallback = null;
		}
		#endregion

	}

	/// <summary>
	/// An enum type for handling the different kinds of situations for which a sequence would need to be evaluated.
	/// </summary>
	public enum GauntletReactionSequenceType {

		MarkerMove = 100,					// The sequence for when the marker moves a space.


	}


}