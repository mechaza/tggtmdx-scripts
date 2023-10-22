using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	[ActionCategory("Grawly - Scene"), Tooltip("Tells the AudioController to play the given SFX.")]
	public class PlaySFX : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The type of SFX to play.
		/// </summary>
		[ObjectType(typeof(SFXType))]
		public FsmEnum sfxType;
		/// <summary>
		/// How loud should it be?
		/// </summary>
		public FsmFloat volumeScale;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			AudioController.instance?.PlaySFX(type: (SFXType)this.sfxType.Value, scale: this.volumeScale.Value);
			base.Finish();
		}
		#endregion

	}


}