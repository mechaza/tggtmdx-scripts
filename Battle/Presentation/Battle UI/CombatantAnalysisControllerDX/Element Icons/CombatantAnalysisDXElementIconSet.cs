using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Encapsulates the number of elemental icons that are displayed
	/// in the combatant analysis screen.
	/// </summary>
	public class CombatantAnalysisDXElementIconSet : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The sequence currently being run to present the icons.
		/// </summary>
		private Sequence currentSequence;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The different elemental icons that this set should manage.
		/// </summary>
		[SerializeField, TabGroup("Icon Set","Scene References")]
		private List<CombatantAnalysisDXElementIcon> elementIcons = new List<CombatantAnalysisDXElementIcon>();
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			// Kill the current presentation sequence, if it exists.
			this.currentSequence?.Kill(complete: true);
			// Go through each icon and reset it.
			// This kills tweens but also resets its position.
			this.elementIcons.ForEach(i => i.ResetState());
		}
		/// <summary>
		/// Kills any tweens operating on this icon set.
		/// </summary>
		public void KillTweens() {
			// Go through each icon and kill its tweens.
			// This will not reset the position to hiding.
			this.elementIcons.ForEach(i => i.KillTweens());
			this.currentSequence?.Kill(complete: true);
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Animates the element into view.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for presentation.</param>
		public void Present(CombatantAnalysisParams analysisParams) {
			// Reset the state.
			this.ResetState();
			// Rebuild the icons themselves.
			this.Rebuild(analysisParams);
			
			// Create a new sequence.
			Sequence presentSequence = DOTween.Sequence();
			// Go through each icon in the list...
			for (int i = 0; i < this.elementIcons.Count; i++) {
				// Capture the index's value and then append a callback.
				int scopedIndex = i;
				presentSequence.AppendCallback(() => {
					this.elementIcons[scopedIndex].TweenIn();
				});
				// ...but also add an interval of a hot second.
				presentSequence.AppendInterval(interval: 0.05f);
			}
			
			// Save the sequence and play it.
			this.currentSequence = presentSequence;
			this.currentSequence.Play();
			
		}
		/// <summary>
		/// Dismisses this element from view.
		/// </summary>
		/// <param name="analysisParams">The parameters used to assemble this element.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			// Kill any tweens, just in case.
			this.KillTweens();
			
			// Create a new sequence.
			Sequence presentSequence = DOTween.Sequence();
			// Go through each icon in the list...
			for (int i = this.elementIcons.Count - 1; i >= 0; i--) {
				// Capture the index's value and then append a callback.
				int scopedIndex = i;
				presentSequence.AppendCallback(() => {
					this.elementIcons[scopedIndex].TweenOut();
				});
				// ...but also add an interval of a hot second.
				presentSequence.AppendInterval(interval: 0.05f);
			}
			
			// Save the sequence and play it.
			this.currentSequence = presentSequence;
			this.currentSequence.Play();
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Rebuild the element to reflect any changes in the parameters passed in.
		/// </summary>
		/// <param name="analysisParams">The parameters for this analysis screen. May contain changes to be shown.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			this.elementIcons.ForEach(i => i.Rebuild(analysisParams));
		}
		#endregion
		
	}
}