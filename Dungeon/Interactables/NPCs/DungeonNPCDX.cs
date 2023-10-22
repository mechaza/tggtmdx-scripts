using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Chat;
using Sirenix.OdinInspector;

namespace Grawly.Dungeon {
	/// <summary>
	/// The new way in which NPCs should be placed into the scene.
	/// </summary>
	[RequireComponent(typeof(DungeonObjectPrompt))]
	public class DungeonNPCDX : MonoBehaviour, IDungeonPlayerInteractionHandler {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The template to use when constructing this NPC.
		/// </summary>
		[SerializeField, TabGroup("NPC", "Toggles")]
		private DungeonNPCTemplate npcTemplate;
		/// <summary>
		/// Should the player be freed when finished interacting with this NPC?
		/// </summary>
		[SerializeField, TabGroup("NPC", "Toggles")]
		private bool freePlayerOnChatClose = true;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The main sprite renderer which actually shows the NPC.
		/// </summary>
		[SerializeField, TabGroup("NPC", "Scene References")]
		private SpriteRenderer mainSpriteRenderer;
		/// <summary>
		/// The sprite renderer which is tasked with rendering the NPCs shadow.
		/// </summary>
		[SerializeField, TabGroup("NPC", "Scene References")]
		private SpriteRenderer shadowSpriteRenderer;
		/// <summary>
		/// The box collider that handles interactions from the player.
		/// </summary>
		[SerializeField, TabGroup("NPC", "Scene References")]
		private BoxCollider boxCollider;
		#endregion

		#region UNITY CALLS
		private void Start() {
			
			// Assign the NPC's sprites to the sprite renderer.
			this.mainSpriteRenderer.sprite = this.npcTemplate.NPCOverworldSprite;
			this.shadowSpriteRenderer.sprite = this.npcTemplate.NPCOverworldSprite;
			
			// Adjust the bounds on the box collider to match the size of the sprite being shown.
			// https://forum.unity.com/threads/changing-boxcollider2d-size-to-match-sprite-bounds-at-runtime.267964/
			Vector2 S = this.mainSpriteRenderer.sprite.bounds.size;
			this.boxCollider.size = S;
			this.boxCollider.center = new Vector2 (0, (S.y / 2));
			/*int spriteWidth = this.npcTemplate.NPCOverworldSprite.texture.width;
			int spriteHeight = this.npcTemplate.NPCOverworldSprite.texture.height;
			this.boxCollider.center = new Vector3(x: 0f, y: ((float)spriteHeight / 2f), z: 0f);
			this.boxCollider.size = new Vector3(
				x: (float)spriteWidth / 100f, 
				y: (float)spriteHeight / 100f, 
				z: 0.2f);*/
		}
		#endregion

		#region INTERFACE - IDUNGEONPLAYERINTERACTIONHANDLER
		/// <summary>
		/// Gets called when the player does try to interact with this object.
		/// </summary>
		public void OnPlayerInteract() {
			
			// Make the player wait upon interacting with the NPC.
			DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Wait);

			// Open the chat controller with the script the NPC is holding.
			ChatControllerDX.GlobalOpen(chatScript: this.npcTemplate.GetNPCChatScript(), chatClosedCallback: ((str, num, toggle) => {
				// Free the player if specified to do so.
				if (this.freePlayerOnChatClose == true) {
					DungeonPlayer.Instance.SetFSMState(DungeonPlayerStateType.Free);
				}
			}));
			
		}
		#endregion
	}
}