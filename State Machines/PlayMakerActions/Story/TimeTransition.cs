using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Grawly.Calendar;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Just tells the scene manager to load up a scene.
	/// </summary>
	[ActionCategory("Grawly - Scene"), Tooltip("Transitions to the given scene via time time transition effect.")]
	public class TimeTransition : FsmStateAction {

		/*#region FIELDS
		/// <summary>
		/// The name of the scene to load.
		/// </summary>
		[Tooltip("The name of the scene to load.")]
		public FsmString sceneName;
		/// <summary>
		/// The time of day to transition to.
		/// </summary>
		[ObjectType(typeof(TimeOfDayType))]
		public FsmEnum timeToTransitionTo;
		#endregion*/

		#region EVENTS
		public override void OnEnter() {
			UnityEngine.Debug.Log("Going to next story beat.");
			CalendarController.Instance.GoToNextStoryBeat();
			/*// Just tell the scene controller to load up the scene with that name.
			// SceneController.Instance.LoadScene(sceneName: this.sceneName.Value);
			SceneController.Instance?.LoadScene(
				sceneName: this.sceneName.Value,
				startTime: GameController.Instance.Variables.CurrentTimeOfDay,
				endTime: (TimeOfDayType)this.timeToTransitionTo.Value);
			// Pass the new information over to the game controller.
			GameController.Instance.Variables.CurrentTimeOfDay = (TimeOfDayType)this.timeToTransitionTo.Value;*/
			base.Finish();
		}
		#endregion

	}


}