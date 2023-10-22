using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using Sirenix.OdinInspector;

namespace Grawly.UI {

	/// <summary>
	/// A menu list item to be used for building up an enemy spawn terminal's menu.
	/// </summary>
	public sealed class EnemySpawnTerminalMenuListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The battle template to associate with this menu list item.
		/// </summary>
		private BattleTemplate battleTemplate;
		#endregion

		#region FIELDS - MENULISTITEM IMPLEMENTATION
		protected override bool IsUsable {
			get {
				return true;
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The label to show the battle's name.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private SuperTextMesh battleNameLabel;
		/// <summary>
		/// The icon to show off the battle.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private Image battleIconImage;
		/// <summary>
		/// The object to use as a highlight.
		/// </summary>
		[SerializeField, TabGroup("Item", "Scene References")]
		private GameObject highlightObject;
		#endregion

		#region BUILDING
		public override void BuildMenuItem(IMenuable item) {
			this.battleTemplate = (BattleTemplate)item;
			this.Dehighlight(item: this.battleTemplate);
		}
		#endregion

		#region GRAPHICS
		protected internal override void Dehighlight(IMenuable item) {
			// this.battleIconImage.sprite = item.Icon;
			this.battleIconImage.overrideSprite = item.Icon;
			this.battleNameLabel.Text = "<c=black>" + item.PrimaryString;
			this.battleNameLabel.textMaterial = DataController.GetDefaultSTMMaterial("UIDefault");
			this.battleNameLabel.Rebuild();
			this.highlightObject.SetActive(false);
		}
		protected internal override void Highlight(IMenuable item) {
			// this.battleIconImage.sprite = item.Icon;
			this.battleIconImage.overrideSprite = item.Icon;
			this.battleNameLabel.Text = "<c=white>" + item.PrimaryString;
			this.battleNameLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
			this.battleNameLabel.Rebuild();
			this.highlightObject.SetActive(true);
		}
		#endregion

		#region EVENTS
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.battleTemplate);
		}
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.battleTemplate);
		}
		public override void OnSubmit(BaseEventData eventData) {
			EnemySpawnTerminalMenuList.instance.PickedBattleTemplate(battleTemplate: this.battleTemplate);
		}
		public override void OnCancel(BaseEventData eventData) {
			EnemySpawnTerminalMenuList.instance.Close();
		}
		#endregion
	}


}