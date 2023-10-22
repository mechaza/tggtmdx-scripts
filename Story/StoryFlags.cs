using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly {

	/// <summary>
	/// A class that can be used to store flags that track progress throughout the game.
	/// </summary>
	[System.Serializable]
	public class StoryFlags {

		#region FIELDS
		/// <summary>
		/// Contains flags that track story progress. A flag without an entry is assumed to be false.
		/// The Label Text is set to Flag Overrides because this is what is shown in the inspector when
		/// edited from a GameVariablesTemplate.
		/// </summary>
		[SerializeField, DictionaryDrawerSettings(KeyLabel = "Flag Type", ValueLabel = "Override Value")]
		private Dictionary<StoryFlagType, int> flagsDict = new Dictionary<StoryFlagType, int>();
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Blank story flags constructor.
		/// </summary>
		public StoryFlags() { }
		/// <summary>
		/// StoryFlags constructor that gets used usually when loading up a game save.
		/// </summary>
		/// <param name="storyFlagKVPs"></param>
		public StoryFlags(List<StoryFlagKVP> storyFlagKVPs) {
			this.flagsDict = new Dictionary<StoryFlagType, int>();
			// Just go through each KVP and add its key/values to the dictionary.
			storyFlagKVPs.ForEach(kvp => this.flagsDict.Add(key: kvp.key, value: kvp.value));
		}
		#endregion

		#region GETTERS - COUNTS AND BOOLS
		/// <summary>
		/// Gets the count stored for the specified flag. Returns 0 if there is no entry.
		/// </summary>
		/// <param name="flagType">The flag to get the value for.</param>
		/// <returns></returns>
		public int GetCount(StoryFlagType flagType) {
			try {
				return this.flagsDict[flagType];
			} catch (System.Exception e) {
				Debug.LogWarning("FLAG: Flag of type " + flagType.ToString() + " not present in the flag dictionary. Returning zero.");
				return 0;
			}
		}
		/// <summary>
		/// Gets the "truth" value of the specified flag, which is basically if its higher than zero. Returns false if there is no entry.
		/// </summary>
		/// <param name="flagType"></param>
		/// <returns></returns>
		public bool GetFlag(StoryFlagType flagType) {
			try {
				return this.flagsDict[flagType] > 0;
			} catch (System.Exception e) {
				Debug.LogWarning("FLAG: Flag of type " + flagType.ToString() + " not present in the flag dictionary. Returning false.");
				return false;
			}
		}
		#endregion

		#region GETTERS - SWITCH SPECIFIC
		/// <summary>
		/// This one is a bit different from determining truth or not.
		/// This is good for getting flags that determine whether something is ready or not and, if it is, if its complete.
		/// This particular functio returns the hard notready/ready/complete status. 
		/// </summary>
		/// <param name="flagType">The type of flag to retrieve the switch status of.</param>
		/// <returns></returns>
		public FlagStatusType GetStatus(StoryFlagType flagType) {
			try {
				return (FlagStatusType)this.flagsDict[flagType];
			} catch (System.Exception e) {
				Debug.LogWarning("FLAG: Flag of type " + flagType.ToString() + " had an issue with getting the status. Returning NotReady by default.");
				return FlagStatusType.NotReady;
			}
		}
		/// <summary>
		/// This one is a bit different from determining truth or not.
		/// This is good for getting flags that determine whether something is ready or not and, if it is, if its complete.
		/// This particular function returns whether the switch is just ready or not.
		/// </summary>
		/// <param name="flagType">The type of flag to retrieve the switch status of.</param>
		/// <returns></returns>
		public bool GetReady(StoryFlagType flagType) {
			return this.GetStatus(flagType: flagType) == FlagStatusType.Ready;
		}
		/// <summary>
		/// This one is a bit different from determining truth or not.
		/// This is good for getting flags that determine whether something is ready or not and, if it is, if its complete.
		/// This particular function returns whether the switch is just ready or not.
		/// </summary>
		/// <param name="flagType">The type of flag to retrieve the switch status of.</param>
		/// <returns></returns>
		public bool GetComplete(StoryFlagType flagType) {
			return this.GetStatus(flagType: flagType) == FlagStatusType.Complete;
		}
		#endregion

		#region SETTERS - COUNTS AND BOOLS
		/// <summary>
		/// Sets the value of the specified flag type to the given number.
		/// WILL OVERRIDE EXISTING ENTRY. If you need to add, use AddCount.
		/// </summary>
		/// <param name="flagType"></param>
		/// <param name="count"></param>
		public void SetCount(StoryFlagType flagType, int count) {
			try {
				Debug.Log("FLAG: Setting flag of type " + flagType.ToString() + " to a value of " + count + ".");
				this.flagsDict[flagType] = count;
			} catch (System.Exception e) {
				Debug.LogWarning("FLAG: Flag of type " + flagType.ToString() + " not present in the flag dictionary. Adding entry with value of  " + count + ".");
				this.flagsDict.Add(flagType, count);
			}
		}
		/// <summary>
		/// Adds the value specified to the value stored in the flag.
		/// WILL NOT OVERRIDE EXISTING ENTRY. If you need to override, use SetCount.
		/// </summary>
		/// <param name="flagType"></param>
		/// <param name="count"></param>
		public void AddCount(StoryFlagType flagType, int count) {
			// Check if the key already exists. If it does, I can just add it to its current value.
			if (this.flagsDict.ContainsKey(flagType) == true) {
				this.SetCount(flagType: flagType, count: this.flagsDict[flagType] + count);
			} else {
				// If it doesn't already exist, I'm gonna just let SetCount take care of it. It'll work out anyway so whatever.
				this.SetCount(flagType: flagType, count: count);
			}
		}
		/// <summary>
		/// Sets the value of the specified flag to be 0 or 1, depending on if False or True is passed in.
		/// WILL OVERWRITE EXISTING ENTRY.
		/// </summary>
		/// <param name="flagType"></param>
		/// <param name="value"></param>
		public void SetFlag(StoryFlagType flagType, bool value) {
			// Set the new count to be 1 or 0, depending on if the value passed is True or False.
			int count = (value == true) ? 1 : 0;
			// Just use SetCount. It'll take care of it.
			this.SetCount(flagType: flagType, count: count);
		}
		#endregion

		#region SETTERS - STATUS SPECIFIC
		/// <summary>
		/// Marks a flag with a specific switch type.
		/// </summary>
		/// <param name="flagType">The flag to set a value for.</param>
		/// <param name="statusType">The mark to assign to this switch</param>
		private void SetStatus(StoryFlagType flagType, FlagStatusType statusType) {
			// Just call the SetCount method and pass it the switch as an int.
			this.SetCount(flagType: flagType, count: (int)statusType);
		}
		/// <summary>
		/// Sets the value of the specified flag to be ready if and only if it is in a Not Ready state.
		/// This will do nothing if the flag is marked as "complete"
		/// </summary>
		/// <param name="flagType">The type of switch to set as ready.</param>
		/// <returns>Whether the operation was successful. This happens if the switch was anything other than NotReady.</returns>
		public bool MarkReady(StoryFlagType flagType) {

			// Determine if the flag is already in the dict.
			if (this.flagsDict.ContainsKey(flagType) == true) {

				// Only continue if the switch is set to NotReady.
				if (this.GetStatus(flagType: flagType) == FlagStatusType.NotReady) {
					// If it is indeed not ready, set it to be as such.
					this.SetStatus(flagType: flagType, statusType: FlagStatusType.Ready);
					// Return true to signal that the operation was successful.
					return true;
		
				} else {
					// If the switch is set to anything else, do nothing and return false.
					Debug.Log(flagType.ToString() + " could not be set to ready. It is already set to something other than NotReady.");
					return false;
				}

			} else {
				// If the entry isn't present period, add it with the ready value.
				this.SetStatus(flagType: flagType, statusType: FlagStatusType.Ready);
				return true;
			}
		}
		/// <summary>
		/// Sets the value of the specified switch to be complete.
		/// This will succeed even if the switch is considered to be not ready.
		/// </summary>
		/// <param name="flagType">The type of switch to set as ready.</param>
		/// <returns>Whether the operation was successful. This happens if the switch was anything other than NotReady.</returns>
		public bool MarkComplete(StoryFlagType flagType) {
			// Check if this flag has been marked as ready.
			if (this.GetStatus(flagType: flagType) == FlagStatusType.Ready) {
				// If it has, mark it as complete and return true.
				this.SetStatus(flagType: flagType, statusType: FlagStatusType.Complete);
				return true;
			} else {
				Debug.Log(flagType.ToString() + " could not be set to complete. It is either undefined or set to a value other than Ready.");
				return false;
			}
		}
		#endregion

		#region SAVING
		/// <summary>
		/// Gets Key/Value pairs for the story flags. Helpful for saving.
		/// </summary>
		/// <returns></returns>
		public List<StoryFlagKVP> GetStoryFlagKVPs() {
			// Transform the keys into a list of KVPs.
			return this.flagsDict
				.Keys
				.Select(k => new StoryFlagKVP() {
					key = k, value = this.flagsDict[k] })
				.ToList();
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Clones this StoryFlags so I can separate it from places where its used in the inspector.
		/// </summary>
		/// <returns></returns>
		public StoryFlags Clone() {
			// Make a new Instance.
			StoryFlags clone = new StoryFlags();
			// Go through every key in THIS storyflags, and add its keys/values to the new one.
			this.flagsDict.Keys
				.ToList()
				.ForEach(k => clone.flagsDict.Add(key: k, value: this.flagsDict[k]));
			// Return the clone.
			return clone;
		}
		#endregion


	}

	/// <summary>
	/// A very simple way to save the KVP's.
	/// </summary>
	[System.Serializable]
	public struct StoryFlagKVP {
		public StoryFlagType key;
		public int value;
	}

	/// <summary>
	/// A form of the StoryFlags that allows it to be saved in a GameSave.
	/// </summary>
	[System.Serializable]
	public class SerializableStoryFlags {

		#region FIELDS
		/// <summary>
		/// A list of key/value pairs to help serialize the story flags for saving.
		/// </summary>
		public List<StoryFlagKVP> storyFlagKVPs = new List<StoryFlagKVP>();
		#endregion

		public SerializableStoryFlags(StoryFlags storyFlags) {
			// Use the functino provided by the StoryFlags to generate new KVPs.
			this.storyFlagKVPs = storyFlags.GetStoryFlagKVPs();
		}

	}

}