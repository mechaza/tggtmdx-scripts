using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;

namespace Grawly.Calendar.Behavior {

	/// <summary>
	/// A callback that can be returned when iterating through StoryBehaviors in a StoryBeat.
	/// Similar to BattleReactions.
	/// </summary>
	/// <param name="storyBeatReactionSequence"></param>
	public delegate void StoryBeatReaction(StoryBeatReactionSequence storyBeatReactionSequence);
	
	/// <summary>
	/// Encapsulates a "sequence" of story behaviors so they can be executed sequentially.
	/// Similar to BattleReactionSequences.
	/// </summary>
	public class StoryBeatReactionSequence {

		#region FIELDS - STATE
		/// <summary>
		/// A queue for holding the StoryBeatReactions to be evaluated.
		/// </summary>
		private Queue<StoryBeatReaction> storyBeatReactionQueue = new Queue<StoryBeatReaction>();
		/// <summary>
		/// The callback to run when finished.
		/// </summary>
		private Action finishCallback;
		#endregion


		#region PREPARATION
		/// <summary>
		/// Preps the sequence for use.
		/// </summary>
		/// <param name="defaultFinishCallback">The callback to run when this sequence is finished.</param>
		public void Prepare(Action defaultFinishCallback) {
			this.finishCallback = defaultFinishCallback;
		}
		/// <summary>
		/// Adds a list of StoryBeatReactions to the sequence.
		/// </summary>
		/// <param name="storyBeatReactions">The StoryBeatReactions to add.</param>
		public void AddToSequence(List<StoryBeatReaction> storyBeatReactions) {
			storyBeatReactions.ForEach(sbr => this.storyBeatReactionQueue.Enqueue(item: sbr));
		}
		/// <summary>
		/// Appends a StoryBeatReaction to the sequence.
		/// </summary>
		/// <param name="storyBeatReaction">The StoryBeatReaction to append to the sequence.</param>
		public void AddToSequence(StoryBeatReaction storyBeatReaction) {
			this.storyBeatReactionQueue.Enqueue(item: storyBeatReaction);
		}
		#endregion

		#region PLAYBACK
		/// <summary>
		/// Executes the next reaction in the sequence.
		/// </summary>
		public void ExecuteNextReaction() {
			// If there are still reactions to execute, do so.
			if (this.storyBeatReactionQueue.Count > 0) {
				// Dequeue it and pass the reaction sequence as a parameter.
				this.storyBeatReactionQueue.Dequeue()(storyBeatReactionSequence: this);
			} else {
				// If there are no reactions left, finish up.
				this.Finish();
			}
		}
		/// <summary>
		/// Finishes up the sequence.
		/// </summary>
		private void Finish() {
			Debug.Log("StoryBeatReactionSequence finished.");
			// Run the finish callback.
			this.finishCallback();
			// Null it out just in case.
			this.finishCallback = null;
		}
		#endregion
		
	}
}