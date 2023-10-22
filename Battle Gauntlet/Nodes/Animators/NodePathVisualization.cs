using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Makes the paths visible with little arrows as part of the scene.
	/// </summary>
	public class NodePathVisualization : GauntletNodeAnimator, IOnEnterNode, IOnExitNode, IOnCompleteNode {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A list of GameObjects that 
		/// </summary>
		[SerializeField, TabGroup("Animation", "Scene References")]
		private List<GameObject> arrowObjects = new List<GameObject>();
		#endregion

		#region INTERFACE IMPLEMENTATION - IONENTERNODE
		public GauntletReaction OnEnterNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				this.arrowObjects[0].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnUp != null);
				this.arrowObjects[1].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnDown != null);
				this.arrowObjects[2].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnLeft != null);
				this.arrowObjects[3].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnRight != null);

				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONEXITNODE
		public GauntletReaction OnExitNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				this.arrowObjects.ForEach(go => go.SetActive(false));

				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IONCOMPLETENOTE
		public GauntletReaction OnCompleteNode(GauntletNodeEventParams eventParams) {
			return delegate (GauntletReactionSequence reactionSequence) {

				this.arrowObjects[0].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnUp != null);
				this.arrowObjects[1].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnDown != null);
				this.arrowObjects[2].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnLeft != null);
				this.arrowObjects[3].SetActive(eventParams.GauntletNode.GetComponent<Selectable>().navigation.selectOnRight != null);

				reactionSequence.ExecuteNextReaction();
			};
		}
		#endregion

		#region INSPECTOR JUNK
		private string inspectorDescription = "Makes the paths visible with little arrows as part of the scene.";
		protected override string InspectorDescription {
			get {
				return this.inspectorDescription;
			}
		}
		#endregion


	}


}