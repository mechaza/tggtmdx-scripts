using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Battle {

	/// <summary>
	/// Abstracts out what the combatant's mood is, and helps them to make decisions.
	/// </summary>
	[System.Serializable]
	public class CombatantMood {

		#region STATIC FIELDS - CACHE THAT I SHOULD ONLY USE HERE AND NOWHERE ELSE
		/// <summary>
		/// All of the mood types. Only using this for caching so I don't have to recompute it every time.
		/// </summary>
		private static List<CombatantMoodType> allMoodTypes;
		/// <summary>
		/// The range of allowable values for mood types. Only used for myself here.
		/// </summary>
		private static Vector2Int combatantMoodTypeRange = Vector2Int.zero;
		/// <summary>
		/// All of the mood types. Only using this for caching so I don't have to recompute it every time.
		/// </summary>
		private static List<CombatantMoodType> AllMoodTypes {
			get {
				// If the mood list is null, figure out what it is and sort it by its int value.
				if (allMoodTypes == null) {
					allMoodTypes = System.Enum.GetValues(typeof(CombatantMoodType)).OfType<CombatantMoodType>().ToList();
					allMoodTypes = allMoodTypes.OrderBy(m => (int)m).ToList();
				}
				return allMoodTypes;
			}
		}
		/// <summary>
		/// The range of allowable values for mood types. Only used for myself here.
		/// </summary>
		private static Vector2Int CombatantMoodTypeRange {
			get {
				// If the mood range is still at its default, create a new one by dynamically calculating the moods.
				if (combatantMoodTypeRange.x == 0 && combatantMoodTypeRange.y == 0) {
					combatantMoodTypeRange = new Vector2Int(
						x: AllMoodTypes.Min(m => (int)m),
						y: AllMoodTypes.Max(m => (int)m));
				}
				return combatantMoodTypeRange;
			}
		}
		#endregion

		#region FIELDS - STATE : MOOD DICT
		/// <summary>
		/// The dictionary that holds the moods. Gets initialized with basic information.
		/// </summary>
		[SerializeField]
		private Dictionary<CombatantMoodType, CombatantMoodSeverityType> moodDict = new Dictionary<CombatantMoodType, CombatantMoodSeverityType>();
		#endregion

		/*#region FIELDS - BASE MOODS
		/// <summary>
		/// How scared is this combatant?
		/// </summary>
		[PropertyRange(min: 0f, max:1f)]
		public float ScaredRatio {
			get {
				return this.moodDict[CombatantMoodType.Scared];
			}
			set {
				// Clamp the value between 0 and 1.
				this.moodDict[CombatantMoodType.Scared] = Mathf.Clamp01(value);
			}
		}
		/// <summary>
		/// How angry is this combatant?
		/// </summary>
		[PropertyRange(min: 0f, max: 1f)]
		public float AngryRatio {
			get {
				return this.moodDict[CombatantMoodType.Angry];
			}
			set {
				// Clamp the value between 0 and 1.
				this.moodDict[CombatantMoodType.Angry] = Mathf.Clamp01(value);
			}
		}
		/// <summary>
		/// How eager is this combatant?
		/// </summary>
		[PropertyRange(min: 0f, max: 1f)]
		public float EagerRatio {
			get {
				return this.moodDict[CombatantMoodType.Eager];
			}
			set {
				// Clamp the value between 0 and 1.
				this.moodDict[CombatantMoodType.Eager] = Mathf.Clamp01(value);
			}
		}
		/// <summary>
		/// How happy is this combatant?
		/// </summary>
		[PropertyRange(min: 0f, max: 1f)]
		public float HappyRatio {
			get {
				return this.moodDict[CombatantMoodType.Happy];
			}
			set {
				// Clamp the value between 0 and 1.
				this.moodDict[CombatantMoodType.Happy] = Mathf.Clamp01(value);
			}
		}
		#endregion*/

		#region MAIN CALLS
		public void IncrementMood(CombatantMoodType moodType) {

		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Increments the mood severity of the specified mood.
		/// </summary>
		/// <param name="moodType">The type of mood to increment.</param>
		/// <param name="incrementAmount">The amount by which to increment the mood.</param>
		/// <returns></returns>
		public CombatantMoodSeverityType IncrementMoodSeverity(CombatantMoodType moodType, int incrementAmount = 1) {

			// Determine the value amount of the severity to use.
			int valueOfRequestedSeverity = (int)this.GetMoodSeverity(moodType: moodType) + incrementAmount;
			// Determine what severity to use by clamping it.
			CombatantMoodSeverityType clampedSeverity = CombatantMood.ClampMoodSeverity(valueOfSeverity: valueOfRequestedSeverity);

			// Set it and return the severity that actually ended up being used.
			return this.SetMoodSeverity(
				moodType: moodType,
				moodSeverity: clampedSeverity);

		}
		/// <summary>
		/// Sets the mood severity on the specified mood type.
		/// </summary>
		/// <param name="moodType">The mood type to set.</param>
		/// <param name="moodSeverity">The severity to set the mood at.</param>
		/// <returns>The severity that was set. This makes sense when I use it in conjunction with Increment, I think.</returns>
		public CombatantMoodSeverityType SetMoodSeverity(CombatantMoodType moodType, CombatantMoodSeverityType moodSeverity) {
			if (this.moodDict.ContainsKey(moodType) == false) {
				Debug.LogWarning("This mood dict does not contain the key " + moodType.ToString() + "! Adding it now.");
				this.moodDict.Add(key: moodType, value: moodSeverity);
				return moodSeverity;
			} else {
				this.moodDict[moodType] = moodSeverity;
				return moodSeverity;
			}
		}
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the ratio associated with the specified mood.
		/// </summary>
		/// <param name="moodType"></param>
		/// <returns></returns>
		public float GetMoodRatio(CombatantMoodType moodType) {
			// Just call the version of this function that accepts the dict.
			return this.GetMoodRatio(moodDict: this.moodDict, moodType: moodType);
		}
		/// <summary>
		/// Gets the ratio associated with the specified mood.
		/// </summary>
		/// <param name="moodDict">The mood dict containing the Moods.</param>
		/// <param name="moodType">The type fo mood needed.</param>
		/// <returns></returns>
		private float GetMoodRatio(Dictionary<CombatantMoodType, CombatantMoodSeverityType> moodDict, CombatantMoodType moodType) {
			try {
				// Determine the numerator/denominator.
				float numerator = (float)moodDict[moodType];
				float denominator = (float)CombatantMood.CombatantMoodTypeRange.y;

				// Divide them.
				return numerator / denominator;

				// The idea here is that a mood type is from 0 to some maximum,
				// if I ever decide to change how many moods there are.

			} catch (System.Exception e) {
				Debug.LogError("Error retrieving mood ratio for " + moodType.ToString() + "! Returning 0");
				return 0f;
			}
		}
		/// <summary>
		/// Gets the severity of the specified mood type.
		/// </summary>
		/// <param name="moodType">The mood type to get the severity for.</param>
		/// <returns></returns>
		public CombatantMoodSeverityType GetMoodSeverity(CombatantMoodType moodType) {
			return this.moodDict[moodType];
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Clamps the value of severity so that it never exceeds the maximum allowed severity.
		/// </summary>
		/// <param name="valueOfSeverity">The value of severity that is potentially going to be used.</param>
		/// <returns>The clamped severity type.</returns>
		private static CombatantMoodSeverityType ClampMoodSeverity(int valueOfSeverity) {
			int clampedValueOfSeverity = Mathf.Clamp(
				value: valueOfSeverity, 
				min: CombatantMoodTypeRange.x, 
				max: CombatantMoodTypeRange.y);
			return (CombatantMoodSeverityType)clampedValueOfSeverity;
		}
		#endregion

		#region CLONING
		/// <summary>
		/// Clones this mood.
		/// </summary>
		/// <returns></returns>
		public CombatantMood Clone() {
			CombatantMood newMood = (CombatantMood)this.MemberwiseClone();
			newMood.moodDict = new Dictionary<CombatantMoodType, CombatantMoodSeverityType>(this.moodDict);
			return newMood;
		}
		#endregion

	}


}