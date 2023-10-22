using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	[ActionCategory("Grawly - Scene"), Tooltip("Destroys the GameController. This effectively puts the game into a state where it can be reset.")]
	public class DestroyGameController : FsmStateAction {

		public override void OnEnter() {
			UnityEngine.Debug.Log("DESTROYING GAMECONTROLLER");
			UnityEngine.GameObject.Destroy(GameController.Instance.gameObject);
			base.Finish();
		}

	}


}