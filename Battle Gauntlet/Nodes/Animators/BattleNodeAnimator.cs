using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Defines how a battle node should be animated in the scene.
	/// </summary>
	[System.Serializable]
	public class BattleNodeAnimator : GauntletNodeAnimator, IOnEnterNode, IOnExitNode, IOnGauntletStart {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The particle system that has the shine effect on this node.
		/// </summary>
		[SerializeField, TabGroup("Animator", "Scene References")]
		private ParticleSystem shineParticleSystem;
		#endregion

		#region INTERFACE IMPLEMENTATION - IONGAUNTLETSTART
		/// <summary>
		/// On the start of the gauntlet, turn off the particle system.
		/// </summary>
		public void OnGauntletStart() {
			this.shineParticleSystem.Stop();
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONENTERNODE
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				this.shineParticleSystem.Play();
				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONEXITNODE
		public GauntletReaction OnExitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {
				this.shineParticleSystem.Stop();
				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INSPECTOR JUNK
		private string inspectorDescription = "Some stuff...";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion


	}


}