using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

namespace Grawly {

	/// <summary>
	/// Contains custom names for the different characters.
	/// </summary>
	[System.Serializable]
	public class CharacterIDMap {

		#region FIELDS
		/// <summary>
		/// Contains the mapping of a character ID to their specified name.
		/// </summary>
		[OdinSerialize]
		private Dictionary<CharacterIDType, string> characterIDNameDict = new Dictionary<CharacterIDType, string>();
		/// <summary>
		/// Contains the mapping of a character ID to their specified name.
		/// </summary>
		public Dictionary<CharacterIDType, string> CharacterIDNameDict {
			get {
				return this.characterIDNameDict;
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CharacterIDMap() {

		}
		/// <summary>
		/// Constructs an ID map out of a list of tuples containing an ID and a name.
		/// </summary>
		/// <param name="nameTuples">A list of tuples that each have an ID and a name.</param>
		public CharacterIDMap(List<(CharacterIDType idType, string name)> nameTuples) {
			nameTuples.ForEach(t => {
				this.characterIDNameDict.Add(key: t.idType, value: t.name);
			});
			/*foreach (CharacterIDType idType in System.Enum.GetValues(typeof(CharacterIDType))) {
				if (this.characterIDNameDict.ContainsKey(idType) == false) {
					throw new System.Exception("A new CharacterIDMap was made, but a name for ID of type " + idType.ToString() + " was never given!");
				}
			}*/
		}
		/// <summary>
		/// Constructs an ID map out of a dictionary passed in.
		/// </summary>
		/// <param name="dictionary"></param>
		public CharacterIDMap(Dictionary<CharacterIDType, string> dictionary) {
			Debug.Log("KEY COUNT IN MAP DICTIONARY: " + dictionary.Keys.Count);
			this.characterIDNameDict = new Dictionary<CharacterIDType, string>(dictionary);
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Sets the name of the specified character with the given ID type.
		/// </summary>
		/// <param name="characterIDType">The ID type of the character</param>
		/// <param name="characterName">The new name to set.</param>
		public void SetName(CharacterIDType characterIDType, string characterName) {
			if (this.characterIDNameDict.ContainsKey(key: characterIDType) == true) {
				this.characterIDNameDict[characterIDType] = characterName;
			} else {
				this.characterIDNameDict.Add(key: characterIDType, value: characterName);
			}
		}
		/// <summary>
		/// Gets the string associated with the given ID type.
		/// </summary>
		/// <param name="characterIDType">The ID type of the character requested.</param>
		/// <returns></returns>
		public string GetName(CharacterIDType characterIDType) {
			try {
				return this.characterIDNameDict[characterIDType];
			} catch (System.Exception e) {
				Debug.LogError("CharacterIDType of " + characterIDType.ToString() + " does not exist! Returning ERROR.");
				return "ERROR";
			}
		}
		/// <summary>
		/// A super handy way of just outright processing a string of text to replace it with the correct names.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string ProcessDialogue(string text) {
			StringBuilder sb = new StringBuilder(text);
			sb.Replace(oldValue: "{d}", newValue: this.characterIDNameDict[CharacterIDType.Dorothy]);
			sb.Replace(oldValue: "{r}", newValue: this.characterIDNameDict[CharacterIDType.Rose]);
			sb.Replace(oldValue: "{b}", newValue: this.characterIDNameDict[CharacterIDType.Blanche]);
			sb.Replace(oldValue: "{s}", newValue: this.characterIDNameDict[CharacterIDType.Sophia]);
			return sb.ToString();
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Clones this CharacterIDMap. 
		/// Relevant for when I'm copying from a template.
		/// </summary>
		/// <returns></returns>
		public CharacterIDMap Clone() {
			CharacterIDMap clone = new CharacterIDMap();
			this.characterIDNameDict
				.Keys
				.ToList()
				.ForEach(k =>
				clone.characterIDNameDict.Add(key: k, value: this.characterIDNameDict[k]));
			return clone;
		}
		#endregion

	}


}