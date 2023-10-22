using System.Collections;
using System.Collections.Generic;
using Grawly.UI;
using Grawly.UI.Legacy;
using Grawly.UI.SubItem;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.EventSystems;

namespace Grawly.Toggles.Proto {
	
	[System.Serializable, Title("Delete Save Data")]
	public class DeleteSaveData : StandardToggleDX, GTSubmitHandler {
		
		#region FIELDS - METADATA
		public override string ToggleName => "Delete Save Data";
		public override string ToggleDescription => "Deletes save data.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		public override GameToggleDX GetDefaultConfiguration() {
			// Clone this toggle.
			GameToggleDX clone = (GameToggleDX)this.Clone();
			// Return it.
			return clone;
		}
		public override string QuantityString { get; } = "???";
		public override SubItemParams CurrentSubItemParams {
			get {
				return new SubItemParams(
					subItemType: SubItemType.None,
					currentOption: "??");
			}
		}
		#endregion

		#region IMPLEMENTATION
		public void OnSubmit(BaseEventData eventData) {
			
			OptionPicker.instance.Display("Delete save data?", () => {
				SaveController.DeleteSaveCollection();
				// Select nothing.
				EventSystem.current.SetSelectedGameObject(null);
				AudioController.instance.StopMusic(track: 0, fade: 0.5f);
				// Force close the settings menu controller.
				SettingsMenuControllerDX.instance.ForceClose();
				SceneController.instance.BasicLoadSceneWithFade("TotalReset");
			});
		}
		#endregion
		
	}
}