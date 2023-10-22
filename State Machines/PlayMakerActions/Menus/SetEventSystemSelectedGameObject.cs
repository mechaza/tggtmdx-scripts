using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {

	[ActionCategory("Grawly - Menus"), Tooltip("Overrides the currently selected GameObject in the Event System. PlayMaker's integration only allows for referencing the desired object by name which is fucking insane.")]
	public class SetEventSystemSelectedGameObject : FsmStateAction {

		#region FIELDS
		[Tooltip("The GameObject to select.")]
		public FsmGameObject gameObjectToSelect;
		#endregion

		#region EVENTS
		public override void OnEnter() {

			if (this.gameObjectToSelect.Value != null) {
				UnityEngine.Debug.Log("Selecting game object " + this.gameObjectToSelect.Value.name);
				UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObjectToSelect.Value);
			} else {
				UnityEngine.Debug.LogWarning("GameObject to select is null. If this was intended, ignore this message.");
				UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			}
			
			base.Finish();
		}
		#endregion

	}


}