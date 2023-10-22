using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using Sirenix.OdinInspector;
using System;

namespace Grawly {

	/// <summary>
	/// Contains values which can modify the way in which certain parts of the game operate (calculations especially.)
	/// </summary>
	public static class DebugController {

		#region FIELDS - PLAYER MULTIPLIERS
		public static float playerSTMultiplier = 1f;
		public static float playerMAMultiplier = 1f;
		public static float playerAGMultiplier = 1f;
		public static float playerLUMultiplier = 1f;
		public static float playerENMultiplier = 1f;
		#endregion

		#region FIELDS - ENEMY MULTIPLIERS
		public static float enemySTMultiplier = 1f;
		public static float enemyMAMultiplier = 1f;
		public static float enemyAGMultiplier = 1f;
		public static float enemyLUMultiplier = 1f;
		public static float enemyENMultiplier = 1f;
		#endregion

		#region FIELDS - PLAYER EXPONENTS
		public static float playerSTExponent = 1f;
		public static float playerMAExponent = 1f;
		public static float playerAGExponent = 1f;
		public static float playerLUExponent = 1f;
		public static float playerENExponent = 1f;
		#endregion

		#region FIELDS - ENEMY EXPONENTS
		public static float enemySTExponent = 1f;
		public static float enemyMAExponent = 1f;
		public static float enemyAGExponent = 1f;
		public static float enemyLUExponent = 1f;
		public static float enemyENExponent = 1f;
		#endregion

		#region FIELDS - BEHAVIOR MULTIPLIERS
		public static float behaviorPowerMultiplier = 1f;
		public static float behaviorPowerExponent = 1f;
		#endregion

		/*#region HELPERS
		/// <summary>
		/// Sets the value of a debug field.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="fieldValue"></param>
		public static void SetDebugField(string fieldName, float fieldValue) {

			throw new System.Exception("Do not use this anymore.");

			try {
				Type debugControllerType = typeof(DebugController);
				debugControllerType.GetField(fieldName).SetValue(null, fieldValue);
				Debug.Log("Setting " + fieldName + " to value of " + fieldValue);
			} catch (System.Exception e) {
				Debug.LogError("There was an error setting the debug field of: " + fieldName);
			}
		}
		/// <summary>
		/// Gets the value of a debug field.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static float GetDebugField(string fieldName) {

			throw new System.Exception("Do not use this anymore.");

			try {
				Type debugControllerType = typeof(DebugController);
				float debugValue = (float)debugControllerType.GetField(fieldName).GetValue(null);
				// Debug.Log("Returning value of " + debugValue + " for debug field of:" + fieldName);
				return debugValue;
			} catch (System.Exception e) {
				Debug.LogError("There was an error getting the debug field of: " + fieldName + ". Returning a value of 1.");
				return 1f;
			}
		}
		#endregion*/

	}


}