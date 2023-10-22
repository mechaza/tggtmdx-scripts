using System.Collections;
using System.Collections.Generic;
using Grawly;
using Grawly.Shop.Prototype;
using HutongGames.PlayMaker;

namespace Grawly.PlayMakerActions {
	
	/// <summary>
	/// Opens the prototype shop menu.
	/// </summary>
	[ActionCategory("Grawly - Menus"), Tooltip("Opens up the prototype shop menu.")]
	public class OpenPrototypeShop : FsmStateAction {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The Store Template to load up.
		/// </summary>
		[Tooltip("The store template to load up."), ObjectType(typeof(PrototypeShopStoreTemplate))]
		public PrototypeShopStoreTemplate storeTemplate;
		#endregion
		
		#region PLAYMAKER EVENTS
		public override void OnEnter() {
			
			// Create a set of menu params.
			PrototypeShopParams prototypeShopParams = new PrototypeShopParams(
				prototypeStoreTemplate: this.storeTemplate, 
				gameVariables: GameController.Instance.Variables);
			
			// Use these params to open the prototype shop.
			PrototypeShopMenuController.Instance.Open(
				shopMenuParams: prototypeShopParams, 
				onCloseCallback: () => {
					// Upon closing the shop, tell the FSM that its finished.
					base.Finish();
				});
		}
		#endregion
		
	}
}