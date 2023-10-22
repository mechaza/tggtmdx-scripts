using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Serialization;

namespace Grawly.Friends {
	
	/// <summary>
	/// Encapsulates the data for saving out friends.
	/// </summary>
	[System.Serializable]
	public class FriendDataSet {

		#region FIELDS - FRIENDS
		/// <summary>
		/// The list containing data on all the friends.
		/// </summary>
		[OdinSerialize]
		private List<FriendData> allFriends = new List<FriendData>();
		#endregion

		#region PROPERTIES - FRIENDS
		/// <summary>
		/// A list of friends that have a rank above zero.
		/// </summary>
		public List<FriendData> AvailableFriends {
			get {
				return this.allFriends
					.Where(fd => fd.currentRank > 0)
					.ToList();
			}
		}
		/// <summary>
		/// The number of available friends in this set.
		/// </summary>
		public int FriendCount {
			get {
				return this.AvailableFriends.Count;
			}
		}
		#endregion
		
		#region CONSTRUCTORS
		/// <summary>
		/// Generates a friend data set with default friend data.
		/// </summary>
		public FriendDataSet() {
			
			// Just generate the default friends.
			this.allFriends = this.GenerateDefaultFriends();
			
		}
		/// <summary>
		/// Clones a FriendDataSet.
		/// </summary>
		/// <param name="friendDataSet">The friend data set to use in construction.</param>
		public FriendDataSet(FriendDataSet friendDataSet) {
			
			// Build a new list from the list passed in.
			this.allFriends = new List<FriendData>(friendDataSet.allFriends);
			
		}
		/// <summary>
		/// Generates a friend data set with saved data also input.
		/// </summary>
		/// <param name="savedFriends"></param>
		public FriendDataSet(List<FriendData> savedFriends) {
			
			// Generate the default friends.
			this.allFriends = this.GenerateDefaultFriends();
			
			// Iterate through the saved data and use that.
			foreach (FriendData friendData in savedFriends) {
				this.OverwriteFriendData(
					friendList: this.allFriends, 
					friendData: friendData);
			}
			
		}
		/// <summary>
		/// Generates a friend data set from a list of templates.
		/// </summary>
		/// <param name="friendTemplates">The templates containing the friends.</param>
		public FriendDataSet(List<FriendTemplate> friendTemplates) {
			
			// Generate the default friends.
			this.allFriends = this.GenerateDefaultFriends();
			
			// Iterate through the templates and use their information.
			foreach (FriendTemplate template in friendTemplates) {
				this.OverwriteFriendData(
					friendList: this.allFriends, 
					arcanaType: template.arcanaType, 
					rank: template.defaultRank);
			}
			
		}
		#endregion

		#region FRIEND DATA MANIPULATION
		/// <summary>
		/// Generates the default list of friends for this set.
		/// </summary>
		/// <returns>A list of friend data, all at rank zero.</returns>
		private List<FriendData> GenerateDefaultFriends() {
			
			// Create a new list of friend data.
			List<FriendData> friendDataList = new List<FriendData>();
			
			// Iterate through each arcana type and add a new friend to the list.
			foreach (ArcanaType at in System.Enum.GetValues(typeof(ArcanaType))) {
				friendDataList.Add(new FriendData(at, 0));
			}

			// Return it.
			return friendDataList;
			
		}
		/// <summary>
		/// Overwrites the friend data for the given friend.
		/// Arcana is implicit.
		/// </summary>
		/// <param name="friendData"></param>
		private void OverwriteFriendData(List<FriendData> friendList, FriendData friendData) {
			// Cal the version of this function that takes basic information.
			this.OverwriteFriendData(
				friendList: friendList,
				arcanaType: friendData.arcanaType,
				rank: friendData.currentRank);
		}
		/// <summary>
		/// Overwrites the given arcana type with the specified rank.
		/// </summary>
		/// <param name="arcanaType"></param>
		/// <param name="rank"></param>
		private void OverwriteFriendData(List<FriendData> friendList, ArcanaType arcanaType, int rank) {
			// Get the index to overwrite.
			int index = this.GetFriendIndex(friendDataList: friendList, arcanaType: arcanaType);
			// Overwrite it.
			this.allFriends[index] = new FriendData(arcanaType, rank);
		}
		#endregion
		
		#region PRIVATE GETTERS
		/// <summary>
		/// Calculates the index of the friend data with the given arcana.
		/// </summary>
		/// <param name="friendDataList">The list to search.</param>
		/// <param name="arcanaType">The arcana to check for.</param>
		/// <returns>The index of the friend data with the given arcana.</returns>
		private int GetFriendIndex(List<FriendData> friendDataList, ArcanaType arcanaType) {
			return friendDataList.FindIndex(fd => fd.arcanaType == arcanaType);
		}
		#endregion
		
	}

	
}