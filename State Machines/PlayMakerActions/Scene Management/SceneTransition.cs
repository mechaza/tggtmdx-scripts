using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Grawly.Dungeon;

namespace Grawly.PlayMakerActions {

	/// <summary>
	/// Just tells the scene manager to load up a scene.
	/// </summary>
	[ActionCategory("Grawly - Scene"), Tooltip("Just tells the SceneController to load up a scene. This is MY scene controller btw.")]
	public class SceneTransition : FsmStateAction {

		#region FIELDS
		/// <summary>
		/// The name of the scene to load.
		/// </summary>
		[Tooltip("The name of the scene to load.")]
		public FsmString sceneName;
		[Tooltip("The ID of the spawn position to load up in. -1 means it goes unused."), ObjectType(typeof(DungeonSpawnType))]
		public FsmEnum spawnPositionType;
		#endregion

		#region EVENTS
		public override void OnEnter() {
			// Just tell the scene controller to load up the scene with that name.
			SceneController.instance.LoadScene(sceneName: this.sceneName.Value, spawnPositionType: (DungeonSpawnType)this.spawnPositionType.Value);
			base.Finish();
		}
		#endregion

	}


}