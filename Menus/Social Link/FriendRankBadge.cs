using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grawly.Menus.SocialLink {
	
	/// <summary>
	/// The badge that gets shown with the rank for the current friend.
	/// </summary>
	public class FriendRankBadge : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject containing all the visuals.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private GameObject allVisuals;
		/// <summary>
		/// The label for the rank number.
		/// </summary>
		[SerializeField]
		private SuperTextMesh rankNumberLabel;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Resets the state of the badge completely.
		/// </summary>
		public void ResetState() {
			this.allVisuals.SetActive(false);
		}
		/// <summary>
		/// The rank number to build this badge for.
		/// </summary>
		/// <param name="currentRank">The current rank of the friend.</param>
		public void Prepare(int currentRank) {
			this.allVisuals.SetActive(true);
			this.rankNumberLabel.Text = "" + currentRank;
		}
		#endregion
		
	}

	
}