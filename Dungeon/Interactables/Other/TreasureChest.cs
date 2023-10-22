using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Grawly.Battle;
using Grawly.Chat;
using Grawly.Story;
using Grawly.Dungeon;
using Grawly.Dungeon.Generation;

namespace Grawly.Dungeon.Interactable {

	public class TreasureChest : SerializedMonoBehaviour, IPlayerInteractable {

		#region FIELDS - SCENE REFERENCES
		[SerializeField]
		private ParticleSystem itemShine;
		[SerializeField]
		private GameObject lidObject;
		#endregion

		#region FIELDS - OTHER
		private bool opened = false;
		[SerializeField]
		private TreasureChestType type;
		[ShowIf("IsItem")]
		[SerializeField]
		private List<BattleBehavior> items = new List<BattleBehavior>();
		#endregion

		private void Start() {
			/*if (LegacyStoryController.Instance.CurrentMission != null) {
				/*List<BattleBehavior> items = new List<BattleBehavior>();
				items.Add(StoryController.Instance.CurrentMission.GetTreasure(DungeonController.Instance.currentFloor));
				this.items = items;
		} else {
				Debug.LogWarning("Current Mission not available. Using default items.");
			}*/
		}

		#region PLAYER INTERACTABLE
		public void PlayerEnter() {
			DungeonPlayer.Instance.nodeLabel.ShowLabel(this);
		}
		public void PlayerExit() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
		}
		public void PlayerInteract() {
			Debug.Log("Interacting with treasure chest...");
			if (opened == false) {
				opened = true;
				Debug.Log("This is where the player status canvas used to get turned back on.");
				// CanvasController.Instance.SetPlayerStatusCanvasGroup(false);
				StartCoroutine(OpenChest());
			}
		}
		public string GetInteractableName() {
			return "Open";
		}
		#endregion

		#region CHATTABLE
		public void ChatOpened() {
			DungeonPlayer.Instance.nodeLabel.HideLabel();
			/*// If this has a flow machine, also send the Chat Opened event.
			if (this.GetComponent<Bolt.FlowMachine>() != null) {
				Bolt.CustomEvent.Trigger(target: this.gameObject, name: "Chat Opened");
			}*/
		}
		public void ChatClosed(string str, int num, bool toggle) {
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
			Debug.Log("This is where the player status canvas used to get turned back on.");
			// CanvasController.Instance.SetPlayerStatusCanvasGroup(true);
			itemShine.Stop(true);
			/*// If this has a flow machine, also send the Chat Closed event.
			if (this.GetComponent<Bolt.FlowMachine>() != null) {
				Bolt.CustomEvent.Trigger(target: this.gameObject, name: "Chat Closed");
			}*/
		}
		#endregion

		private IEnumerator OpenChest() {

			if (ProceduralDungeonController.instance == null) {
				throw new System.Exception("I am using legacy chests with the assumption the proc gen controller is still here.");
			}
			DungeonTreasureRange treasureRange = ProceduralDungeonController.instance.GetDungeonTreasureRange();

			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);
			lidObject.transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), 3f);
			yield return new WaitForSeconds(0.5f);
			itemShine.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			List<string> plainScript = new List<string>();
			switch (type) {
				case TreasureChestType.Item:
					plainScript.Add("Found <c=yellow>" + treasureRange.TreasureName + "</c>!");
					GameController.Instance.Variables.AddItemsToInventory(items: treasureRange.TreasureBattleBehaviors);
					/*foreach (BattleBehavior item in items) {
						plainScript.Add("Found <c=yellow>" + item.behaviorName + "</c>!");
						GameController.Instance.Variables.AddItemToInventory(item);
					}*/
					break;
				case TreasureChestType.Key:
					plainScript.Add("Found a <c=blue>key</c>!");
					throw new System.Exception("Don't add keys this way anymore!");
					// DungeonController.Instance.AddKey();
					break;
				default:
					Debug.LogError("Couldn't determine treasure type");
					plainScript.Add("<c=red>Couldn't determine treasure type!");
					break;
			}

			ChatControllerDX.GlobalOpen(
				chatScript: new PlainChatScript(rawTextList: plainScript), 
				chatOpenedCallback: this.ChatOpened, 
				chatClosedCallback: this.ChatClosed);

			/*Chat.Legacy.LegacyChatController.Open(
				script: new PlainChatScript(plainScript),
				chatOpenedCallback: ChatOpened,
				chatClosedCallback: ChatClosed);*/
		}


		#region ODIN FUNCTIONS
		private bool IsKey() {
			if (type == TreasureChestType.Key) {
				return true;
			} else {
				return false;
			}
		}
		private bool IsItem() {
			if (type == TreasureChestType.Item) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		public enum TreasureChestType {
			Item,
			Key,
		}
	}
}