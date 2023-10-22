using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// The item that should be picked in the battle menu.
	/// </summary>
	public sealed class BattleMenuDXBehaviorMenuListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The BattleBehavior that is being used for this menu list item.
		/// </summary>
		private BattleBehavior battleBehavior;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The STM used for this move's name.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private SuperTextMesh behaviorNameLabel;
		/// <summary>
		/// The cost for this behavior.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private SuperTextMesh behaviorCostLabel;
		/// <summary>
		/// The image used to represent the behavior's icon.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconImage;
		/// <summary>
		/// The highlight for the icon itself. Yes I have two highlights is there a fucking problem?
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconHighlightImage;
		/// <summary>
		/// The image that is used as a backing for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconBackingFrontImage;
		/// <summary>
		/// The image that is used as a backing's dropshadow for the behavior's elemental icon. Just decoration.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private Image behaviorElementalIconBackingDropshadowFrontImage;
		/// <summary>
		/// The GameObject that serves as a sort of Highlight.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private GameObject behaviorHighlightBarGameObject;
		/// <summary>
		/// The game object that should be turned on to show the player that an enemy is weak to this move.
		/// </summary>
		[SerializeField, TabGroup("Menu List Item", "Scene References")]
		private GameObject weakTagGameObject;
		#endregion

		#region FIELDS - COMPUTED PROPERTIES : ACTUAL COST
		/// <summary>
		/// There is a chance the combatant will have a modifier that adjusts the costs of the behavior.
		/// For convineince I'm gonna just get it here.
		/// </summary>
		private string ActualQuantityString {
			get {
				switch (this.battleBehavior.behaviorType) {
					case BehaviorType.Item:
						// If this is an item, just get the behavior's normal string.
						return this.battleBehavior.QuantityString;
					default:
						// Figure out if the "actual" cost is different.
						int actualCost = BattleController.Instance.CurrentCombatant.GetActualBattleBehaviorCost(behavior: this.battleBehavior);
						if (actualCost != this.battleBehavior.baseCost) {
							// If it is, I'll need to remake a new string.
							Debug.Log("Cost is different from base cost! Making a NEW string. Be careful if this causes issues!");
							return actualCost + ((this.battleBehavior.costType == BehaviorCostType.HP) ? "HP" : "MP");
						} else {
							// If they're the same, play it safe and return the string as it exists in the battle behavior.
							return this.battleBehavior.QuantityString;
						}
						
				}
			}
		}
		#endregion

		#region MENULISTITEM IMPLEMENTATION
		/// <summary>
		/// Can the current combatant use this behavior?
		/// </summary>
		protected override bool IsUsable {
			get {

				// Make sure that the combatant is able to expend the resources to use the behavior and that it has targets.
				return BattleController.Instance.CurrentCombatant.HasResourcesForBehavior(behavior: this.battleBehavior)
					&& BattleController.Instance.CurrentCombatant.TurnBehavior.GetTargets(combatant: BattleController.Instance.CurrentCombatant, behavior: this.battleBehavior).Count > 0;
				
			}
		}
		public override void BuildMenuItem(IMenuable item) {

			// First and foremost, save a reference to the item.
			this.battleBehavior = (BattleBehavior)item;

			// Call Dehighlight. This effectively builds the strings.
			this.Dehighlight(item: item);

			// Set the sprite on the elemental icon and its highlight
			this.behaviorElementalIconImage.overrideSprite = item.Icon;
			this.behaviorElementalIconHighlightImage.overrideSprite = item.Icon;

			// If there are *any* enemies where their resistance is weak to the behavior, set the weak tag on. Otherwise, itll be off.
			this.weakTagGameObject.SetActive(BattleController.Instance
				.AliveEnemies
				.Where(e => GameController.Instance.Variables.GetKnownEnemyResistance(enemy: e, behavior: (BattleBehavior)item).HasValue)
				.Count(e => e.CheckResistance(behavior: (BattleBehavior)item) == ResistanceType.Wk) > 0);

		}
		protected internal override void Dehighlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + item.PrimaryString;
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + this.ActualQuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(false);
			this.behaviorElementalIconBackingFrontImage.color = Color.black;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.clear;

			// Turn off the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(false);
		}
		protected internal override void Highlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + item.PrimaryString;
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + this.ActualQuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(true);
			this.behaviorElementalIconBackingFrontImage.color = (((BattleBehavior)item).behaviorType == BehaviorType.Special) ? Grawly.GrawlyColors.colorDict[GrawlyColorTypes.Blue] : Grawly.GrawlyColors.colorDict[GrawlyColorTypes.Yellow];
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.white;

			// Turn on the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(true);
			// Also assign the text on the info box.
			BattleMenuDXBehaviorMenuList.instance.SetInfoBoxText(text: item.DescriptionString);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - UNITY EVENTS
		public override void OnSubmit(BaseEventData eventData) {
			if (this.IsUsable == true) {
				AudioController.instance?.PlaySFX(SFXType.Select);
				// Set the Current Battle Behavior on the state graph to be the battle behavior assigned here.
				BattleMenuControllerDX.instance.SetCurrentBattleBehavior(behavior: this.battleBehavior);
				BattleMenuControllerDX.instance.TriggerEvent(eventName: "Behavior Selected");
			} else {
				AudioController.instance?.PlaySFX(SFXType.Invalid);
			}
			
		}
		public override void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			BattleMenuControllerDX.instance.TriggerEvent(eventName: "Back");
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.battleBehavior);
			AudioController.instance?.PlaySFX(SFXType.Hover);
		}
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.battleBehavior);
		}
		#endregion

	}


}