using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {
	
	/// <summary>
	/// Displays an animation depending on if there is a player advantage or an enemy advantage.
	/// </summary>
	public class CombatantAdvantageAlertController : MonoBehaviour {

		public static CombatantAdvantageAlertController Instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// A placeholder alert for when there is a player advantage.
		/// </summary>
		[SerializeField]
		private GameObject prototypePlayerAdvantageAlert;
		/// <summary>
		/// A placeholder alert for when there is an enemy advantage.
		/// </summary>
		[SerializeField]
		private GameObject prototypeEnemyAdvantageAlert;
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			Instance = this;
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of this controller.
		/// </summary>
		private void ResetState() {
			this.prototypePlayerAdvantageAlert.SetActive(false);
			this.prototypeEnemyAdvantageAlert.SetActive(false);
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Displays an alert based on the advantage type set going into battle.
		/// </summary>
		/// <param name="advantageType">The advantage type that corresponds to the alert that needs to be shown.</param>
		public void DisplayAlert(BattleAdvantageType advantageType) {
			
			// Reset the alert totally.
			this.ResetState();
			
			// Switch based on what advantage this battle is.
			switch (advantageType) {
				case BattleAdvantageType.Normal:
					// Don't do anything for normal battles.
					break;
				case BattleAdvantageType.PlayerAdvantage:
					this.prototypePlayerAdvantageAlert.SetActive(true);
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: () => {
						this.prototypePlayerAdvantageAlert.SetActive(false);
					});
					break;
				case BattleAdvantageType.EnemyAdvantage:
					this.prototypeEnemyAdvantageAlert.SetActive(true);
					GameController.Instance.WaitThenRun(timeToWait: 3f, action: () => {
						this.prototypeEnemyAdvantageAlert.SetActive(false);
					});
					break;
				default:
					throw new System.Exception("This should never be reached!");
			}
		}
		#endregion
		
	}
}