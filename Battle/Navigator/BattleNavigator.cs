using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Grawly.Battle.Modifiers;
using System.Linq;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// The actual interface for calling the battle navigator during runtime.
	/// </summary>
	[RequireComponent(typeof(BattleNavigatorCanvasController))]
	public class BattleNavigator : SerializedMonoBehaviour {

		public static BattleNavigator Instance { get; set; }

		#region FIELDS - STATE
		/// <summary>
		/// The behavior that this navigator is currently using.
		/// Will likely get hotswapped a lot.
		/// </summary>
		private NavigatorBehavior CurrentNavigatorBehavior { get; set; }
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The canvas controller for the battle navigator.
		/// The BattleNavigator and BattleNavigatorCanvasController operate closely with each other.
		/// </summary>
		private BattleNavigatorCanvasController NavigatorCanvas { get; set; }
		#endregion

		#region FIELDS - RESOURCES
		/// <summary>
		/// The voice I will be using for the navigator for the time being.
		/// </summary>
		[OdinSerialize]
		private BattleNavigatorVoice NavigatorVoice { get; set; }
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
				this.NavigatorCanvas = this.GetComponent<BattleNavigatorCanvasController>();
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			// Initialize the navigator so it can actually function.
			this.Initialize();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Initializes the navigator with whatever it needs to get going.
		/// </summary>
		public void Initialize() {
			// Tell the voice to initialize.
			this.NavigatorVoice.Initialize();
		}
		/// <summary>
		/// Preps the navigator for whatever context it needs when the specified battle is going to be run.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate that this navigator will be commentating on.</param>
		/// <param name="battleParams">The routine set involved in this battle.</param>
		public void Prepare(BattleTemplate battleTemplate, BattleParams battleParams) {
			
			// Clone the navigator behavior from the battle template.
			this.CurrentNavigatorBehavior = battleParams.NavigatorBehavior;
			
			// Prep it with the things it needs.
			this.CurrentNavigatorBehavior.Prepare(
				battleTemplate: battleTemplate, 
				navigatorVoice: this.NavigatorVoice, 
				navigatorCanvas: this.NavigatorCanvas);
			
		}
		#endregion

		#region BATTLE EVENTS
		/// <summary>
		/// Gets the modifiers of the navigator.
		/// This is so stupid.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> GetModifiers<T>() {
			if (CurrentNavigatorBehavior is T) {
				return new List<NavigatorBehavior>() { this.CurrentNavigatorBehavior }.Cast<T>().ToList();
			}
			return new List<T>();
		}
		#endregion

	}


}