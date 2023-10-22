using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;
using System.Linq;

namespace Grawly.Battle.Analysis {
	
	/// <summary>
	/// Encapsulates the various attribute bars on the analysis screen.
	/// This is to easily provide ways of animating/building them.
	/// </summary>
	public class CombatantAnalysisDXAttributeBarSet : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The sequence currently being run to present the bars.
		/// </summary>
		private Sequence currentPresentationSequence;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The attribute bars this class should be managing.
		/// </summary>
		[SerializeField, TabGroup("Bar Set","Scene References")]
		private List<CombatantAnalysisDXAttributeBar> attributeBars = new List<CombatantAnalysisDXAttributeBar>();
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this element.
		/// </summary>
		public void ResetState() {
			this.KillTweens();
		}
		/// <summary>
		/// Kill any tweens currently operating on the bars in this set.
		/// </summary>
		private void KillTweens() {
			// Kill the presentation sequence, if it exists.
			this.currentPresentationSequence?.Kill(complete: true);
			// Reset the state on each of the bars.
			this.attributeBars.ForEach(a => a.ResetState());
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
			
			// TODO: Maybe add some flair here?
			
			// Animate the bars into view.
			this.AnimateBarFills(analysisParams: analysisParams);
		
		}
		/// <summary>
		/// Animates this element to close up.
		/// </summary>
		/// <param name="analysisParams">The parameters to use for this dismissal.</param>
		public void Dismiss(CombatantAnalysisParams analysisParams) {
			this.attributeBars.ForEach(a => a.Dismiss(analysisParams));
		}
		/// <summary>
		/// Performs the actual routine of filling in the attribute bars.
		/// </summary>
		/// <param name="analysisParams">The params containing the data required to fill the bars.</param>
		private void AnimateBarFills(CombatantAnalysisParams analysisParams) {
			
			// Create a new sequence.
			Sequence presentSequence = DOTween.Sequence();
			
			// Go through each bar in the list...
			for (int i = 0; i < this.attributeBars.Count; i++) {
				// Capture the index's value and then append a callback.
				int scopedIndex = i;
				presentSequence.AppendCallback(() => {
					this.attributeBars[scopedIndex].Present(analysisParams);
				});
				// ...but also add an interval of a hot second.
				presentSequence.AppendInterval(interval: 0.1f);
			}
			
			// Save the sequence and play it.
			this.currentPresentationSequence = presentSequence;
			this.currentPresentationSequence.Play();
			
		}
		#endregion

		#region BUILDING
		/// <summary>
		/// Rebuilds this element with the parameters specified.
		/// </summary>
		/// <param name="analysisParams">The parameters to use in building this element.</param>
		public void Rebuild(CombatantAnalysisParams analysisParams) {
			
			// Kill any tweens that might be in action.
			this.KillTweens();
			
			// Animate the bars into view.
			this.AnimateBarFills(analysisParams: analysisParams);
			
			/*// Create a new sequence.
			Sequence presentSequence = DOTween.Sequence();
			
			// Go through each bar in the list...
			for (int i = 0; i < this.attributeBars.Count; i++) {
				// Capture the index's value and then append a callback.
				int scopedIndex = i;
				presentSequence.AppendCallback(() => {
					this.attributeBars[scopedIndex].Present(analysisParams);
				});
				// ...but also add an interval of a hot second.
				presentSequence.AppendInterval(interval: 0.1f);
			}
			
			// Save the sequence and play it.
			this.currentPresentationSequence = presentSequence;
			this.currentPresentationSequence.Play();
			
			// this.attributeBars.ForEach(a => a.Present(analysisParams));*/
		}
		#endregion
		
	}
	
}