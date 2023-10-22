using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Grawly.Gauntlet.Nodes {

	/// <summary>
	/// Probably is easier for me to just have this as monobehavior so I can make a prefab out of it.
	/// </summary>
	public class GauntletNodePathVisualization : GauntletNodeGameObject, IOnEnterNode, IOnExitNode, IOnCompleteNode {

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

	}


}