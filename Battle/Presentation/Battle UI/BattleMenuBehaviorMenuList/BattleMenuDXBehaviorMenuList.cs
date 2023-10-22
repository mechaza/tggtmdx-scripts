using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using Sirenix.OdinInspector;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// A list of battle behaviors for use inside of the battle.
	/// </summary>
	public class BattleMenuDXBehaviorMenuList : MenuList {

		public static BattleMenuDXBehaviorMenuList instance;

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the elements for the BattleBehavior's description.
		/// </summary>
		[SerializeField, TabGroup("BattleMenuDXBehaviorMenuList", "Scene References")]
		private GameObject infoBoxGameObject;
		/// <summary>
		/// The SuperTextMesh that is used for the info box.
		/// </summary>
		[SerializeField, TabGroup("BattleMenuDXBehaviorMenuList", "Scene References")]
		private SuperTextMesh infoBoxTextLabel;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Does an extra step where it adds the "no selections available!" thing if nothing is available.
		/// </summary>
		/// <param name="allMenuables">All of the menuables that should be part of this menu list.</param>
		/// <param name="startIndex">The index for which the menuables should be rebuilt.</param>
		public override void PrepareMenuList(List<IMenuable> allMenuables, int startIndex) {
			
			// Prep the menuables as normal.
			base.PrepareMenuList(allMenuables, startIndex);
			
			// Set the color of the scrollbar based on what behaviors were passed in.
			this.ScrollBarCursor.color = (allMenuables[0] as BattleBehavior).behaviorType == BehaviorType.Item 
				? GrawlyColors.colorDict[GrawlyColorTypes.Yellow] 
				: GrawlyColors.colorDict[GrawlyColorTypes.Blue];
			
		}
		#endregion

		#region INFO BOX
		/// <summary>
		/// Sets whether or not the infobox's game object should be active or not.
		/// </summary>
		/// <param name="status"></param>
		public void SetInfoBoxActive(bool status) {
			this.infoBoxGameObject.SetActive(status);
		}
		/// <summary>
		/// Sets the text on the InfoBox.
		/// </summary>
		/// <param name="text">The text to show on the InfoBox.</param>
		public void SetInfoBoxText(string text) {
			this.infoBoxTextLabel.Text = text;
		}
		#endregion

	}


}