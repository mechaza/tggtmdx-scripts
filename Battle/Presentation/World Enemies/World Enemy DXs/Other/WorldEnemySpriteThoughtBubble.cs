using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;

namespace Grawly.Battle.WorldEnemies {

	/// <summary>
	/// Gets used by WorldEnemySprites if they have anything interesting to say.
	/// </summary>
	public class WorldEnemySpriteThoughtBubble : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the visuals for the bubble.
		/// </summary>
		[SerializeField, TabGroup("Bubble", "Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The STM to use for the bubble.
		/// </summary>
		[SerializeField, TabGroup("Bubble", "Scene References")]
		private SuperTextMesh bubbleTextLabel;
		#endregion

		#region HELPERS
		/// <summary>
		/// Displays a thought bubble next to the enemy.
		/// </summary>
		/// <param name="text">The text to display.</param>
		/// <param name="time">The amount of time to keep it on screen.</param>
		public void DisplayThoughtBubble(string text, float time) {
			this.allVisuals.SetActive(true);
			this.bubbleTextLabel.Text = text;
			GameController.Instance.WaitThenRun(timeToWait: time, action: delegate {
				allVisuals.SetActive(false);
			});
		}
		#endregion

	}


}