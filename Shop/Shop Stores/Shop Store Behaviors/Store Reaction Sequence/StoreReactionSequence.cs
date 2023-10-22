using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Toggles;
using Sirenix.OdinInspector;
using Grawly.Chat;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Grawly.UI.MenuLists;

namespace Grawly.Shop.Behaviors {
	
	
	/// <summary>
	/// A callback that can be returned when iterating through StoreBehaviors in a Store.
	/// Similar to BattleReactions.
	/// </summary>
	/// <param name="storeReactionSequence"></param>
	public delegate void StoreReaction(StoreReactionSequence storeReactionSequence);

	/// <summary>
	/// Encapsulates a "sequence" of store behaviors so they can be executed sequentially.
	/// Similar to BattleReactionSequences.
	/// </summary>
	public class StoreReactionSequence {
		
		#region FIELDS - STATE
		/// <summary>
		/// A queue for holding the StoreReactions to be evaluated.
		/// </summary>
		private Queue<StoreReaction> storeReactionQueue = new Queue<StoreReaction>();
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
		/// Adds a list of StoreReactions to the sequence.
		/// </summary>
		/// <param name="storeReactions">The StoreReactions to add.</param>
		public void AddToSequence(List<StoreReaction> storeReactions) {
			storeReactions.ForEach(sbr => this.storeReactionQueue.Enqueue(item: sbr));
		}
		/// <summary>
		/// Appends a StoreReaction to the sequence.
		/// </summary>
		/// <param name="storeReaction">The StoreReaction to append to the sequence.</param>
		public void AddToSequence(StoreReaction storeReaction) {
			this.storeReactionQueue.Enqueue(item: storeReaction);
		}
		#endregion

		#region PLAYBACK
		/// <summary>
		/// Executes the next reaction in the sequence.
		/// </summary>
		public void ExecuteNextReaction() {
			// If there are still reactions to execute, do so.
			if (this.storeReactionQueue.Count > 0) {
				// Dequeue it and pass the reaction sequence as a parameter.
				this.storeReactionQueue.Dequeue()(storeReactionSequence: this);
			} else {
				// If there are no reactions left, finish up.
				this.Finish();
			}
		}
		/// <summary>
		/// Finishes up the sequence.
		/// </summary>
		private void Finish() {
			Debug.Log("StoreReactionSequence finished.");
			// Run the finish callback.
			this.finishCallback();
			// Null it out just in case.
			this.finishCallback = null;
		}
		#endregion
	}
}