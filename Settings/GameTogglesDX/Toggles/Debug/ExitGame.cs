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

	[System.Serializable, Title("Exit Game")]
	public class ExitGame : StandardToggleDX, GTSubmitHandler {

		#region FIELDS - METADATA
		public override string ToggleName => "Exit Game";
		public override string ToggleDescription => "Exits the game.";
		public override GameToggleCategoryType CategoryType => GameToggleCategoryType.Debug;
		public override GameToggleDXContextType ContextType => GameToggleDXContextType.SaveCollection;
		public override GameToggleDX GetDefaultConfiguration() {
			// Clone this toggle.
			GameToggleDX clone = (GameToggleDX)this.Clone();
			// Return it.
			return clone;
		}

		public override string QuantityString { get; } = "???";

		#endregion

		public override SubItemParams CurrentSubItemParams {
			get {
				return new SubItemParams(
					subItemType: SubItemType.None,
					currentOption: "??");
			}
		}
		public void OnSubmit(BaseEventData eventData) {
			
			OptionPicker.instance.Display("Exit?", () => {
				// Select nothing.
				EventSystem.current.SetSelectedGameObject(null);
				AudioController.instance.StopMusic(track: 0, fade: 0.5f);
				Flasher.FadeOut();
				GameController.Instance.WaitThenRun(1f, () => {
					Application.Quit();
				});
			});
			
			
		}
	}


}