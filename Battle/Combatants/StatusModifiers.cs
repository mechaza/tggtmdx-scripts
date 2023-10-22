using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Grawly.Battle {

	// Keeps track of where the combatant is powered up.
	[System.Serializable]
	public struct StatusModifiers {
		[SerializeField]
		private Dictionary<PowerBoostType, float> boostMultipliers;
		[SerializeField]
		private Dictionary<PowerBoostType, bool> debuffed;  // Debuffed dict just holds whether the boost type was a debuff.
		[SerializeField]
		private Dictionary<PowerBoostType, int> boostTimers;

		/// <summary>
		/// Primarily used for debug purposes to tell me what the current status is of the given boost type.
		/// </summary>
		/// <param name="boostType"></param>
		/// <returns></returns>
		public string GetBoostString(PowerBoostType boostType) {
			string str = "";
			if (boostTimers[boostType] == 0) {
				return str;
			}
			switch (boostType) {
				case PowerBoostType.Accuracy:
					str += "<c=bluey>ACC ";
					break;
				case PowerBoostType.Attack:
					str += "<c=redy>ATK ";
					break;
				case PowerBoostType.Defense:
					str += "<c=greeny>DEF ";
					break;
				default:
					break;
			}
			if (debuffed[boostType] == true) {
				str += "DN";
			} else {
				str += "UP";
			}
			return str;
		}


		/// <summary>
		/// A saimple function I'm using as a means to just, remove positive buffs.
		/// </summary>
		public void Dekaja() {
			throw new System.Exception("Don't call this ever again.");
			foreach (PowerBoostType boostType in System.Enum.GetValues(typeof(PowerBoostType))) {
				if (this.boostMultipliers[boostType] > 1f) {
					Debug.Log("Removed " + boostType.ToString() + " buff!");
					this.NegatePowerBoost(boostType: boostType);
				}
			}
		}
		/// <summary>
		/// Initialize the status modifiers struct.
		/// </summary>
		public void Reset() {
			boostMultipliers = new Dictionary<PowerBoostType, float>();
			boostTimers = new Dictionary<PowerBoostType, int>();
			debuffed = new Dictionary<PowerBoostType, bool>();
			foreach (PowerBoostType boostType in Enum.GetValues(typeof(PowerBoostType))) {
				boostMultipliers[boostType] = 1f;
				boostTimers[boostType] = 0;
				debuffed[boostType] = false;
			}
		}
		/// <summary>
		/// Resets the status of a given power boost. Good for healing moves or w/e.
		/// </summary>
		public void NegatePowerBoost(PowerBoostType boostType) {
			boostMultipliers[boostType] = 1f;
			boostTimers[boostType] = 0;
		}
		/// <summary>
		/// Ticks the timer down and return what boosts/afflictions were reverted. 
		/// </summary>
		/// <returns></returns>
		public TurnTickResults TurnTick() {

			TurnTickResults results = new TurnTickResults();
			results.powerBoostAlarms = new List<PowerBoostType>();

			foreach (PowerBoostType boostType in Enum.GetValues(typeof(PowerBoostType))) {
				// Go through each boost type higher than zero.
				if (boostTimers[boostType] > 0) {
					boostTimers[boostType] -= 1;
					// If the timer HITS zero, add it to the alarms and reset its modifier.
					if (boostTimers[boostType] == 0) {
						results.powerBoostAlarms.Add(boostType);
						boostMultipliers[boostType] = 1f;
					}
				}
			}
			return results;
		}
		public void SetPowerBoost(PowerBoostType boostType, bool debuff) {
			if (this.boostTimers[boostType] == 0) {
				// Multiplying by the enum will either add/subtract to the multiplier, depending  on if its a buff/debuff
				this.boostMultipliers[boostType] = 1.5f;
				this.boostTimers[boostType] = 3;
				this.debuffed[boostType] = debuff;
			} else {
				// Check if the intent to debuff aligns up with the current buff status
				// E.x., if these two don't match, could mean a debuffed player is attempting to "rebuff"
				if (debuff != this.debuffed[boostType]) {
					this.NegatePowerBoost(boostType);
				} else {
					// If it's higher than 0 and the intent is the same, clamp the boost so it doesn't exceed 6.
					this.boostTimers[boostType] = Mathf.Clamp(this.boostTimers[boostType] + 3, 4, 6);
				}
			}
		}
		/// <summary>
		/// Sets the power boost by either buffing or debuffing the specified power boost type.
		/// </summary>
		/// <param name="kvp">Key: Boost type to set. Value: Buff or debuff it?</param>
		public void SetPowerBoost(KeyValuePair<PowerBoostType, PowerBoostIntentionType> kvp) {
			// Haha just call the version of this that takes a bool I guess.
			this.SetPowerBoost(
				boostType: kvp.Key, 
				debuff: kvp.Value == PowerBoostIntentionType.Debuff);
		}
		public void SetPowerBoost(List<PowerBoostType> boostTypes, bool debuff) {
			foreach (PowerBoostType boostType in boostTypes) {
				SetPowerBoost(boostType, debuff);
			}
		}
		public float GetPowerBoost(PowerBoostType boostType) {
			if (boostTimers[boostType] == 0) {
				// If the timer is at zero for this boost type, just return 1.
				return 1f;
			} else {
				// If the boost is a "debuff", negate it.
				float modifier = (debuffed[boostType] == true) ? -.5f : .5f;
				return 1f + modifier;
			}
		}
		/// <summary>
		/// A list of buffs that are currently active. NOT debuffs.
		/// </summary>
		public List<PowerBoostType> ActivePowerBuffs {
			get {
				// This kinda sucks but I'm dealing with legacy software so:
				return this.debuffed					// Go through the debuffed dict.
					.Where(kvp => kvp.Value == false)	// Find power boosts where the value is false (i.e., this is a BUFF.)
					.Select(kvp => kvp.Key)				// Grab the buffs.
					.ToList();							// Get it as a list.
			}
		}
		/// <summary>
		/// A list of debuffs that are currently active.
		/// </summary>
		public List<PowerBoostType> ActivePowerDebuffs {
			get {
				// This kinda sucks but I'm dealing with legacy software so:
				return this.debuffed                    // Go through the debuffed dict.
					.Where(kvp => kvp.Value == true)   // Find power boosts where the value is false (i.e., this is a BUFF.)
					.Select(kvp => kvp.Key)             // Grab the buffs.
					.ToList();                          // Get it as a list.
			}
		}
	}

	/// <summary>
	/// After a combatant's turn tick, I need to alert the player if anything was updated.
	/// E.x., attack boost nullified, status ailment recovery.
	/// </summary>
	public struct TurnTickResults {
		public List<PowerBoostType> powerBoostAlarms;
	}

}