using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grawly.UI;
using Grawly.UI.MenuLists;
using UnityEngine;

namespace Grawly.Friends {
	
	/// <summary>
	/// Contains the data required to serialize a friend.
	/// </summary>
	[System.Serializable]
	public struct FriendData : IMenuable {
		
		#region FIELDS - GENERAL
		/// <summary>
		/// The arcana for this friend. Used to retrieve their template.
		/// </summary>
		public ArcanaType arcanaType;
		/// <summary>
		/// The current rank for this friend.
		/// </summary>
		public int currentRank;
		#endregion

		#region PROPERTIES - GENERAL
		/// <summary>
		/// The template containing the data associated with this friend.
		/// </summary>
		public FriendTemplate FriendTemplate {
			get {
				// Just call from the data controller.
				return DataController.GetFriendTemplate(arcanaType: this.arcanaType);
			}
		}
		/// <summary>
		/// When building friend data in the menu, I need to obfuscate definitions that arent available.
		/// This returns a list of rank definitions with rank less or equal to the current rank.
		/// </summary>
		public List<FriendRankDefinition> MenuableRankDefinitions {
			get {
				/*FriendData tmpThis = this;
				return tmpThis.FriendTemplate.RankDefinitions
					.TakeWhile(rd => rd.Rank <= tmpThis.currentRank)
					.Take(1)
					.ToList();*/
				int rank = this.currentRank;
				return this.FriendTemplate.RankDefinitions.Where(rd => rd.Rank <= rank).ToList();
			}
		}
		#endregion

		#region CONSTRUCTORS
		public FriendData(ArcanaType arcanaType, int rank) {
			this.arcanaType = arcanaType;
			this.currentRank = rank;
		}
		#endregion
		
		#region INTERFACE IMPLEMENTATION
		public string PrimaryString {
			get {
				return this.FriendTemplate.friendName;
			}
		}
		public string QuantityString  {
			get {
				return this.currentRank + "";
			}
		}
		public string DescriptionString  {
			get {
				return this.FriendTemplate.friendTagline;
			}
		}
		public Sprite Icon  {
			get {
				return this.FriendTemplate.bustUpSprite;
			}
		}
		#endregion

		
	}

}