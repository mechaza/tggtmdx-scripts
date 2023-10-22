using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Grawly.Special.PurpleCube {
	
	/// <summary>
	/// A simple script that can be used to debug the state of the purple cube world enemy.
	/// </summary>
	public class PurpleCubeStateBanner : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when tweening the state banner in
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private float tweenInTime = 0.5f;
		/// <summary>
		/// The amount of time to take when tweening the state banner out.
		/// </summary>
		[SerializeField]
		private float tweenOutTime = 0.5f;
		/// <summary>
		/// The amount of time to "hold" the state banner before tweening out.
		/// </summary>
		[SerializeField]
		private float holdDuration = 2f;
		/// <summary>
		/// The ease type to use when tweening the banner in.
		/// </summary>
		[SerializeField]
		private Ease easeInType = Ease.InOutCirc;
		/// <summary>
		/// The ease type to use when tweening the banner out.
		/// </summary>
		[SerializeField]
		private Ease easeOutType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label to use for the banner text.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private SuperTextMesh bannerLabel;
		/// <summary>
		/// The SpriteRenderer used for the banner's backing sprite.
		/// </summary>
		[SerializeField]
		private SpriteRenderer bannerSpriteRenderer;
		/// <summary>
		/// The "anchor" that should be tweened for movement.
		/// </summary>
		[SerializeField]
		private Transform bannerAnchor;
		#endregion

		#region PREPARATION
		private void ResetState() {
			this.bannerAnchor.DOKill(complete: true);
			this.bannerSpriteRenderer.DOKill(complete: true);
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Builds the banner with the specified text and color.
		/// </summary>
		/// <param name="text">The text to use for the banner.</param>
		/// <param name="colorType">The color to use for the backing.</param>
		public void Build(string text, GrawlyColorTypes colorType = GrawlyColorTypes.Red) {
			throw new System.NotImplementedException();
		}
		#endregion
		
	}
}