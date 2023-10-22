using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Friends {
	
	/// <summary>
	/// A template to build a friend out of.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Friend/Friend Template")]
	public class FriendTemplate : SerializedScriptableObject {

		#region FIELDS - GENERAL
		/// <summary>
		/// The name of the friend.
		/// </summary>
		[BoxGroup("General")]
		public string friendName = "";
		/// <summary>
		/// The tagline to show on the friend summary screen.
		/// </summary>
		[BoxGroup("General")]
		public string friendTagline = "";
		/// <summary>
		/// The arcana associated with this friend.
		/// </summary>
		[BoxGroup("General")]
		public ArcanaType arcanaType = ArcanaType.None;
		/// <summary>
		/// The rank to use for this friend template by default.
		/// </summary>
		[BoxGroup("General")]
		public int defaultRank = 0;
		#endregion

		#region FIELDS - GRAPHICS
		/// <summary>
		/// The bust up sprite for this friend.
		/// </summary>
		[BoxGroup("Graphics")]
		public Sprite bustUpSprite;
		#endregion

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The rank definitions for this friend.
		/// </summary>
		[OdinSerialize, BoxGroup("Ranks")]
		private List<FriendRankDefinition> rankDefinitions = new List<FriendRankDefinition>();
		/// <summary>
		/// The rank definitions for this friend.
		/// </summary>
		public List<FriendRankDefinition> RankDefinitions {
			get {
				return this.rankDefinitions;
			}
		}
		#endregion
		
	}

	
	
}