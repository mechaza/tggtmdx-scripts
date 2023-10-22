using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Battle.Equipment;
using UnityEngine.UI;
using Grawly.Battle.Equipment.Badges;
using Grawly.Chat;
using Grawly.UI;
using Grawly.UI.MenuLists;
using Sirenix.Serialization;

namespace Grawly.Menus.BattleTerminal {
	
	/// <summary>
	/// An item that can be selected in the Battle Terminal screen.
	/// </summary>
	public class BattleTerminalListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The template associated with this list item.
		/// </summary>
		private BattleTemplate CurrentBattleTemplate { get; set; }
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Is this item usable?
		/// </summary>
		protected override bool IsUsable { get; }
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The text mesh used for this battle's name.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References"), Title("Text")]
		private SuperTextMesh battleItemNameLabel;
		/// <summary>
		/// The image used for the backing on this list item.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References"), Title("Images")]
		private Image battleItemBackingFrontImage;
		#endregion
		
		#region PREPARATION
		public override void BuildMenuItem(IMenuable item) {
			
			// Save the BattleTemplate being referenced by this list item.
			this.CurrentBattleTemplate = (BattleTemplate)item;
			
			// Refresh the visual elements by dehighlighting it.
			this.Dehighlight(item: this.CurrentBattleTemplate);
			
			throw new NotImplementedException();
		}
		#endregion
		
		#region PRESENTATION
		protected internal override void Highlight(IMenuable item) {
			this.battleItemBackingFrontImage.color = GrawlyColors.Purple;
			this.battleItemNameLabel.Text = "<c=white>" + item.PrimaryString;
		}
		protected internal override void Dehighlight(IMenuable item) {
			this.battleItemBackingFrontImage.color = GrawlyColors.White;
			this.battleItemNameLabel.Text = "<c=black>" + item.PrimaryString;
		}
		#endregion
		
		#region EVENT SYSTEM
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.CurrentBattleTemplate);
		}
		public override void OnDeselect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(type: SFXType.Hover);
			this.Dehighlight(item: this.CurrentBattleTemplate);
		}
		public override void OnSubmit(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		public override void OnCancel(BaseEventData eventData) {
			throw new NotImplementedException();
		}
		#endregion
		
	}
}