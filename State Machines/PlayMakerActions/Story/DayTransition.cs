using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Grawly.Calendar;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Provies a means of transitioning to the next day.
	/// </summary>
	[ActionCategory("Grawly - Scene"), Tooltip("Provies a means of transitioning to the next day.")]
	public class DayTransition : FsmStateAction {

		/*#region FIELDS
		/// <summary>
		/// The name of the scene to load.
		/// </summary>
		[Tooltip("The name of the scene to load upon completion of the transition.")]
		public FsmString sceneName;
		/// <summary>
		/// The time of day to transition to.
		/// </summary>
		[Tooltip("The time of day to start up on upon transitioning the day."), ObjectType(typeof(TimeOfDayType))]
		public FsmEnum timeToTransitionTo;
		#endregion*/

		#region EVENTS
		public override void OnEnter() {
			UnityEngine.Debug.Log("Day transition.");
			CalendarController.Instance.GoToNextStoryBeat();
			base.Finish();
		}
		#endregion

	}


}