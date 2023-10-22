using System.Collections;
using System.Collections.Generic;
using Grawly.Battle.BattleMenu;
using Grawly.Friends;
using UnityEngine;
using Grawly.UI;
using Grawly.UI.Legacy;
using Grawly.UI.MenuLists;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Grawly.Menus.SocialLink {
	
	public class RankUpgradeMenuListItem : MenuItem {

		#region FIELDS - STATE
		/// <summary>
		/// The current item being used.
		/// </summary>
		private FriendRankDefinition rankDefinition;
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
		#endregion
		
		#region PROPERTIES - STATE
		/// <summary>
		/// Is this item usable?
		/// </summary>
		protected override bool IsUsable {
			get {
				// If the friend currently on display has a saved rank equal to or greater than this definition, return true.
				return SocialLinkMenuController.Instance.CurrentFriend.currentRank >= this.rankDefinition.Rank;
			}
		}
		#endregion

		#region MENU LIST ITEM CALLS
		public override void BuildMenuItem(IMenuable item) {

			// First and foremost, save a reference to the item.
			this.rankDefinition = (FriendRankDefinition)item;
			
			// Call Dehighlight. This effectively builds the strings.
			this.Dehighlight(item: item);

			// Set the sprite on the elemental icon and its highlight
			this.behaviorElementalIconImage.overrideSprite = item.Icon;
			this.behaviorElementalIconHighlightImage.overrideSprite = item.Icon;

		}
		protected internal override void Dehighlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=black>" + item.PrimaryString : "<c=gray>" + "???????");
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=black>" : "<c=gray>") + item.QuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(false);
			this.behaviorElementalIconBackingFrontImage.color = Color.black;
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.clear;

			// Turn off the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(false);
		}
		protected internal override void Highlight(IMenuable item) {
			
			// Rebuild the labels with the appropriate strings.
			this.behaviorNameLabel.Text = (this.IsUsable == true ? "<c=white>" + item.PrimaryString : "<c=gray>" + "???????" ) ;
			this.behaviorCostLabel.Text = (this.IsUsable == true ? "<c=white>" : "<c=gray>") + item.QuantityString;

			// Also update the elemental icon images
			this.behaviorElementalIconHighlightImage.gameObject.SetActive(true);
			this.behaviorElementalIconBackingDropshadowFrontImage.color = Color.white;
			this.behaviorElementalIconBackingFrontImage.color = GrawlyColors.Red;
			
			// Turn on the highlight bar.
			this.behaviorHighlightBarGameObject.SetActive(true);
			
			// Also assign the text on the info box.
			RankUpgradeMenuList.Instance.SetInfoBoxText(text: item.DescriptionString);
			
		}

		protected override void OnHorizontalMove(HorizontalMoveDirType moveDir) {
			if (moveDir == HorizontalMoveDirType.Left) {
				SocialLinkMenuController.Instance.StartCoroutine(this.WaitThenScroll(-1));
				// SocialLinkMenuController.Instance.Scroll(-1);
			} else if (moveDir == HorizontalMoveDirType.Right) {
				SocialLinkMenuController.Instance.StartCoroutine(this.WaitThenScroll(1));
				// SocialLinkMenuController.Instance.Scroll(1);
			}
		}
		/// <summary>
		/// Seeing if this will help with visual erros.
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		private IEnumerator WaitThenScroll(int dir) {
			EventSystem.current.SetSelectedGameObject(null);
			yield return null;
			SocialLinkMenuController.Instance.Scroll(dir);
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - UNITY EVENTS
		public override void OnSubmit(BaseEventData eventData) {
			
		}
		public override void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			SocialLinkMenuController.Instance.Dismiss();
		}
		public override void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(item: this.rankDefinition);
			AudioController.instance?.PlaySFX(SFXType.Hover);
		}
		public override void OnSelect(BaseEventData eventData) {
			this.Highlight(item: this.rankDefinition);
		}
		#endregion
		
		
		
	}

	
}