using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using HutongGames.PlayMaker;
using Rewired.Utils.Classes.Data;

namespace Grawly.Chat {

	/// <summary>
	/// Gets passed into directive constructors when I need to use them in the context of a text file that's being parsed.
	/// </summary>
	public class ChatDirectiveParams {

		#region FIELDS
		/// <summary>
		/// Contains the key/value pairs that were associated with this line when it was being made.
		/// </summary>
		private List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
		#endregion
		
		#region FIELDS - COMPUTED
		/// <summary>
		/// The first label that was parsed from this line.
		/// </summary>
		public string FirstLabel {
			get {
				return this.keyValuePairs[0].Key;
			}
		}
		/// <summary>
		/// The type of ChatDirective this params probably is going to be constructing.
		/// </summary>
		public System.Type ChatDirectiveType {
			get {
				// Take a look at the first key and figure out what the intended directive is from there.
				switch (this.FirstLabel.ToLower()) {
					case "chat":
						return typeof(DialogueDirective);
					case "borders":
						return typeof(SlideBordersDirective);
					case "slide":
						return typeof(SlideChatBoxDirective);
					case "show":
						return typeof(ShowDirective);
					case "label":
						return typeof(LabelDirective);
					case "jump":
						return typeof(JumpDirective);
					case "define":
						return typeof(DefineDirective);
					case "option1":
						return typeof(OptionDirective);
					case "branch":
						return typeof(BranchDirective);
					case "dismiss":
						return typeof(DismissDirective);
					case "presentpicture":
						return typeof(PresentPictureDirective);
					case "dismisspicture":
						return typeof(DismissPictureDirective);
					case "toggle":
						return typeof(ToggleGameObjectDirective);
					case "setflag":
						return typeof(GlobalFlagDirective);
					case "setcamtag":
						return typeof(CameraTagDirective);
					case "camera":
						return typeof(CameraDirective);
					case "wait":
						return typeof(WaitDirective);
					case "load":	
						return typeof(LoadSceneDirective);
					case "fade":
						return typeof(FadeDirective);
					case "sfx":
						return typeof(SFXDirective);
					case "music":
						return typeof(MusicDirective);
					case "battle":
						return typeof(StartBattleDirective);
					case "close":
						return typeof(CloseDirective);
					default:
						return typeof(DialogueDirective);
				}
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Assembles a chat directive params based on the match collection passed in.
		/// </summary>
		/// <param name="matches"></param>
		/// <param name="parsedString"></param>
		public ChatDirectiveParams(MatchCollection matches, string parsedString) {

			// If there are no matches, it can be inferred that this is a dialogue string with no speaker. Add that as a key value pair.
			if (matches.Count == 0) {
				this.keyValuePairs.Add(new KeyValuePair<string, string>(key: "", value: parsedString));
				return;
			}

			// Go through each match and add its KVP.
			foreach (Match match in matches) {
				this.keyValuePairs.Add(item: new KeyValuePair<string, string>(
					key: RawDirectiveParser.GetLabel(match),
					value: RawDirectiveParser.GetProperty(match)));
			}
		}
		#endregion

		#region GETTERS - STATE
		/// <summary>
		/// Determines if the key provided is associated with a float.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool HasFloat(string key) {
			try {
				float value = float.Parse(s: this.GetValue(key: key));
				return true;
			} catch (System.Exception e) {
				return false;
			}
		}
		/// <summary>
		/// Determines if the key provided is defined in the type parameter.
		/// </summary>
		/// <param name="key"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool HasEnum<T>(string key) where T : System.Enum {
			try {
				// Try to get an enum for the specified key.
				T value = this.GetEnum<T>(key:key);
				// If it actually worked, it's valid.
				return true;
			} catch (System.Exception e) {
				// If this didn't work, it's false.
				return false;
			}
		}
		#endregion
		
		#region GETTERS - VALUE
		/// <summary>
		/// Gets the value stored for a certain key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetValue(string key) {
			// Find the first KVP where the key is equal to the passed param, then return its value.
			try {
				return this.keyValuePairs.First(kvp => kvp.Key.ToLower() == key.ToLower()).Value;
			} catch (System.Exception e) {
				return null;
			}
		}
		/// <summary>
		/// Gets the bool stored for a certain key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool GetBool(string key) {
			return this.GetValue(key: key) == "true" ? true : false;
		}
		/// <summary>
		/// Gets an int stored for a certain key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int GetInt(string key) {
			// Declare a variable.
			int value = -1;
			// Attempt to parse.
			Int32.TryParse(s: this.GetValue(key: key), result: out value);
			// Return the int. If it is -1, there was an error.
			return value;
		}
		/// <summary>
		/// Gets a float stored for a certain key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public float GetFloat(string key) {
			float value = 0f;
			float.TryParse(s: this.GetValue(key: key), result: out value);
			return value;
		}
		/// <summary>
		/// Gets an enum for the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetEnum<T>(string key) where T : System.Enum {
			try {
				
				// Get the actual VALUE stored for this key.
				string enumString = this.GetValue(key: key);
				// This value is going to be used, in turn, as its own KEY.
				return (T)Enum.Parse(enumType: typeof(T), value: enumString, ignoreCase: true);
				
			} catch (System.Exception e) {
				throw new System.Exception("Could not parse out the enum! Key: " + key + ", Type: " + typeof(T).FullName);
			}
		}
		/// <summary>
		/// Gets an enum for the specified key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetEnum<T>(int key) {
			try {
				return (T)Enum.ToObject(enumType: typeof(T), value: key);
			} catch (System.Exception e) {
				throw new System.Exception("Could not parse out the enum! What's the issue?");
			}
		}
		/// <summary>
		/// Gets the bust up type stored with the givne key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ChatBustUpType GetBustUpType(string key) {
			
			// Grab the value itself.
			string value = this.GetValue(key);

			// If nothing was found, just return none.
			if (value == null) {
				return ChatBustUpType.None;
			}
			
			// Switch case that.
			switch (value.ToLower()) {
				case "none":
					return ChatBustUpType.None;
				case "angry":
					return ChatBustUpType.Angry;
				case "hurt":
					return ChatBustUpType.Hurt;
				case "neutral":
					return ChatBustUpType.Neutral;
				case "neutralclosed":
				case "closed":
					return ChatBustUpType.NeutralClosed;
				case "smile":
					return ChatBustUpType.Smile;
				case "smirk":
					return ChatBustUpType.Smirk;
				case "surprise":
					return ChatBustUpType.Surprise;
				case "tense":
					return ChatBustUpType.Tense;
				case "sans":
					return ChatBustUpType.Sans;
				default:
					return ChatBustUpType.None;
			}
			
			/*None			= 0,
		
		Angry			= 1,
		Hurt			= 2,
		Neutral			= 3,
		NeutralClosed 	= 4,
		Smirk			= 5,
		Surprise		= 6,
		Tense			= 7,
		
		Sans			= 9,*/
		}
		#endregion


	}


}